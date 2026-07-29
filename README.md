# RockRecommender

RockRecommender is a small, from-scratch music recommendation system for rock songs, built entirely in .NET (no Python anywhere). It recommends the next song to a listener the same way big streaming services do: content-based suggestions for brand-new users (based on the bands they say they like at signup, solving the cold-start problem), and a collaborative-filtering model, trained with ML.NET, once the user has given at least one like or dislike.

It is the companion code for two blog series:

- [Building a rock recommender with ML.NET](https://devfullstack.net/blog/series-rockrecommender), which teaches how to evaluate a recommendation model step by step, for readers who have never trained one before.
- [Closing the feedback loop in RockRecommender](https://devfullstack.net/blog/series-rockrecommender-feedback-loop), which finishes the training pipeline: deciding automatically between synthetic and real feedback, comparing a candidate model against production before promoting it, retraining on a schedule, and reloading the model live.

## Solution structure

```
RockRecommender.Domain          entities (Song, User, Feedback, ...), repository interfaces, value objects
                                 (RecommendationHistory, FeedbackHistory), ranking metrics, Result<T>, no
                                 external dependencies
RockRecommender.Application     UserService, FeedbackService, RecommendationService (the core business rule)
                                 and the request/response DTOs
RockRecommender.Infrastructure  MongoDB repositories, the rock catalog (embedded JSON), and the ML.NET
                                 model loading/scoring
RockRecommender.Api             ASP.NET Core Web API (controllers, Swagger, one shared exception-to-HTTP
                                 mapping base controller)
RockRecommender.Training        long-running host: on a schedule, reads real feedback from MongoDB (falling
                                 back to synthetic interactions while there isn't enough of it yet), trains
                                 a candidate model, compares it against the model currently in production,
                                 and only promotes it if it scores better
RockRecommender.Tests           xUnit tests for the domain, the value objects and RecommendationService
```

A controller only ever talks to a service, never straight to a repository or an entity, and the `Api` project has no project reference to `Domain` at all, so that rule is enforced at compile time, not just by convention.

## Prerequisites

- .NET SDK 10.0 or later (`dotnet --version`)
- Docker (to run MongoDB locally), or any MongoDB 6+/7+ instance reachable from your machine

## 1. Train the model and see the evaluation metrics

The training host reads real feedback from MongoDB, so it needs MongoDB running (see step 2 below) even before the API is started. On every run it counts how many real interactions exist in the `feedback` collection and compares that against how many synthetic interactions it would generate: if there is more real feedback than that, it trains on the real data, otherwise it falls back to the synthetic interactions generated in the process itself.

```bash
cd RockRecommender.Training
dotnet run
```

This is a long-running host, not a one-shot script: on startup, and then on every tick of the configured `RetrainInterval`, it prints the catalog summary, the interaction summary (and which source it came from), the leave-one-out Precision@5 / Recall@5 / NDCG@5 metrics for the newly trained candidate model, the same metrics for the model currently in production, and whether the candidate was promoted. A candidate is only promoted when its total score beats the active model's (or when there is no active model yet), so a retrain can never make production worse.

Every number that shapes a training run (how many synthetic users, the evaluation `K`, the retrain interval, the model paths, the like/dislike probabilities used to generate synthetic taste) is configurable via `RockRecommender.Training/appsettings.json`, or overridden on the fly, for example:

```bash
dotnet run -- --Training:SyntheticUserCount=200
```

### How the model reaches the API

Both `RockRecommender.Training/appsettings.json` and `RockRecommender.Api/appsettings.json` point their `Model`/`Training` path at the same file, `rock-recommender.zip` at the repo root, so there is nothing to copy by hand. When the training host promotes a new candidate, it overwrites that shared file, and the API notices the change on its own: it periodically checks the file's last-modified time (`Model:ReloadCheckInterval`) and reloads the model in memory without needing a restart. A copy trained on the seed dataset is already committed at the repo root so the API works out of the box. If the file is ever missing, the API still starts and serves cold-start recommendations normally, it only fails the `next-song` request for a user who already has feedback, with a clear 503 error asking you to train the model first.

## 2. Start MongoDB

A single-service `docker-compose.yml` is included at the repo root:

```bash
docker compose up -d
```

This starts MongoDB on the default port `27017` with a named volume for persistence. The connection string and database name are configured under `"Mongo"` in both `RockRecommender.Api/appsettings.json` and `RockRecommender.Training/appsettings.json`, and default to `mongodb://localhost:27017` / `rockrecommender` in both.

The API seeds the `songs` collection from the same catalog used by the Training project automatically on startup, the first time it connects to an empty database.

## 3. Run the API

```bash
cd RockRecommender.Api
dotnet run
```

By default it listens on `http://localhost:5110` (see `Properties/launchSettings.json`). The exact URL is also printed in the console on startup. Swagger is wired in and served at the root (`http://localhost:5110/`), so opening it in a browser is the fastest way to explore and try the four endpoints.

### List the available bands

```bash
curl http://localhost:5110/bands
```

Returns the full list of band names in the catalog, e.g. `["Metallica", "Iron Maiden", ...]`, useful for building a signup screen that lets a new user pick their favorite bands.

### Create a user (cold start)

```bash
curl -X POST http://localhost:5110/users \
  -H "Content-Type: application/json" \
  -d '{"likedBands": ["Metallica", "Iron Maiden"]}'
```

Returns `201 Created` with the new user, e.g.:

```json
{ "id": "007471e0-a1f3-434e-9469-fdc087d61f42", "likedBands": ["Metallica", "Iron Maiden"] }
```

### Get the next recommended song

```bash
curl http://localhost:5110/users/007471e0-a1f3-434e-9469-fdc087d61f42/next-song
```

Before any feedback exists for the user, this prioritizes songs from bands the user likes (content-based / cold start). After the user has given at least one like/dislike, it switches to the trained collaborative model and scores every song not yet shown to the user. Every song returned is logged so it will not be recommended again to that user until the whole catalog has been shown once.

### Send feedback for a song

```bash
curl -X POST http://localhost:5110/users/007471e0-a1f3-434e-9469-fdc087d61f42/feedback \
  -H "Content-Type: application/json" \
  -d '{"songId": "3f1c9b2a-6d4e-4a1a-9c3d-8e2f7a5b1c0d", "liked": true}'
```

Returns `204 No Content`. Song IDs are deterministic GUIDs derived from the band and song title (see `RockRecommender.Infrastructure/Catalog/RockCatalog.cs`), grab a real one from the catalog with `GET /users/{id}/next-song` first.

## Running the tests

```bash
dotnet test RockRecommender.Tests
```

## Dataset

The catalog (`RockRecommender.Infrastructure/Catalog/rock-catalog.json`) contains 43 well-known rock bands across 9 subgenres (classic rock, hard rock, heavy metal, thrash metal, grunge, punk rock, alternative rock, progressive rock, black metal), with real songs per band, totaling 280 songs. It is a plain data file, not code, so growing the catalog never means touching a line of C#. It is shared by both the Training app (as the seed dataset for synthetic interactions) and the Infrastructure project (to seed MongoDB's `songs` collection), so there is a single source of truth for the catalog.

## See also

[RockPlayerApi](https://github.com/rafaelarantes/RockPlayerApi) and [RockPlayerWeb](https://github.com/rafaelarantes/RockPlayerWeb) are a companion project that consumes this API and actually plays the recommended songs, with a blog series of its own: [RockPlayer: actually playing what RockRecommender recommends](https://devfullstack.net/blog/series-rockplayer).
