# RockRecommender

RockRecommender is a small, from-scratch music recommendation system for rock songs, built entirely in .NET (no Python anywhere). It recommends the next song to a listener the same way big streaming services do: content-based suggestions for brand-new users (based on the bands they say they like at signup, solving the cold-start problem), and a collaborative-filtering model, trained with ML.NET, once the user has given at least one like or dislike.

It is the companion code for a 5-part blog series that teaches how to evaluate a recommendation model, step by step, for readers who have never trained one before:

1. [Recommendation systems: how Spotify knows what you want to hear](https://devfullstack.net/blog/how-recommendation-systems-work). Content-based vs. collaborative recommendation, and the cold-start problem.
2. [How to evaluate a recommendation model](https://devfullstack.net/blog/how-to-evaluate-a-recommendation-model). Precision@K, Recall@K and NDCG, explained with numeric examples.
3. [Building the dataset and training the recommender with ML.NET](https://devfullstack.net/blog/training-a-rock-recommender-with-mlnet). The rock catalog and training a `MatrixFactorizationTrainer` model.
4. [Evaluating the model in practice](https://devfullstack.net/blog/evaluating-the-rock-recommender). Applying the metrics from article 2 with a leave-one-out evaluation.
5. [Building the rock recommender API](https://devfullstack.net/blog/building-the-rock-recommender-api). Wiring the trained model into a real ASP.NET Core API backed by MongoDB.

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
RockRecommender.Training        console app: load the catalog, generate synthetic interactions, train,
                                 evaluate, save the model
RockRecommender.Tests           xUnit tests for the domain, the value objects and RecommendationService
```

A controller only ever talks to a service, never straight to a repository or an entity, and the `Api` project has no project reference to `Domain` at all, so that rule is enforced at compile time, not just by convention.

## Prerequisites

- .NET SDK 10.0 or later (`dotnet --version`)
- Docker (to run MongoDB locally), or any MongoDB 6+/7+ instance reachable from your machine

## 1. Train the model and see the evaluation metrics

The training console app is fully standalone: the song catalog and the synthetic user interactions are generated in the process itself, so it does **not** need MongoDB to run.

```bash
cd RockRecommender.Training
dotnet run
```

This prints the catalog summary, the generated synthetic interactions, the leave-one-out Precision@5 / Recall@5 / NDCG@5 metrics, and finally saves the trained model to `RockRecommender.Training/rock-recommender.zip`.

Every number that shapes the training run (how many synthetic users, the evaluation `K`, the model path, the like/dislike probabilities used to generate synthetic taste) is configurable via `RockRecommender.Training/appsettings.json`, or overridden on the fly, for example:

```bash
dotnet run -- --Training:SyntheticUserCount=200
```

### Making the model available to the API

The Api project loads the model from the path configured under `"Model": { "Path": "..." }` in `RockRecommender.Api/appsettings.json` (relative to the working directory the API is run from). The simplest setup, and the one used in this repo, is to copy the freshly trained file next to the Api project:

```bash
cp RockRecommender.Training/rock-recommender.zip RockRecommender.Api/rock-recommender.zip
```

A copy of `rock-recommender.zip` (trained on the seed dataset) is already committed next to the Api project so the API works out of the box. Retrain and re-copy whenever you change the dataset or the training settings. If the file is missing, the API still starts and serves cold-start recommendations normally, it only fails the `next-song` request for a user who already has feedback, with a clear 503 error asking you to train the model first.

## 2. Start MongoDB

A single-service `docker-compose.yml` is included at the repo root:

```bash
docker compose up -d
```

This starts MongoDB on the default port `27017` with a named volume for persistence. The connection string and database name are configured in `RockRecommender.Api/appsettings.json` under `"Mongo"` and default to `mongodb://localhost:27017` / `rockrecommender`.

The API seeds the `songs` collection from the same catalog used by the Training project automatically on startup, the first time it connects to an empty database.

## 3. Run the API

```bash
cd RockRecommender.Api
dotnet run
```

By default it listens on `http://localhost:5110` (see `Properties/launchSettings.json`). The exact URL is also printed in the console on startup. Swagger is wired in and served at the root (`http://localhost:5110/`), so opening it in a browser is the fastest way to explore and try the three endpoints.

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
