# StudyNotesPlatform

Платформа для публикации и поиска учебных конспектов (ASP.NET Core + Vue).

## Что в репозитории

- `StudyNotesPlatform.csproj` - backend API на ASP.NET Core 9
- `vue-frontend/` - frontend на Vue + Vite
- `tests/StudyNotesPlatform.SmokeTests/` - минимальные smoke-тесты API

## Требования

- .NET SDK 9
- PostgreSQL (доступный по строке подключения в `appsettings.json`)
- Node.js 20+ (для работы frontend)

## Быстрый старт

1. Проверьте строку подключения в `appsettings.json`.
2. Запустите backend:

```powershell
dotnet run --project StudyNotesPlatform.csproj
```

3. Запустите frontend:

```powershell
cd vue-frontend
npm install
npm run dev
```

## Минимальные автотесты (smoke)

Smoke-тесты автоматически поднимают backend и проверяют публичные эндпоинты:

- `GET /api/notes/statuses`
- `GET /api/lookup/all-universities`
- `GET /api/notes`

Запуск:

```powershell
powershell -ExecutionPolicy Bypass -File tests/run-smoke-tests.ps1
```

Запуск с параметрами:

```powershell
powershell -ExecutionPolicy Bypass -File tests/run-smoke-tests.ps1 -BaseUrl "http://127.0.0.1:5199" -StartupTimeoutSeconds 90
```

Логи тестового прогона сохраняются в `artifacts/test-logs/`.

## CI

Smoke-тесты также запускаются автоматически в GitHub Actions:

- `.github/workflows/smoke-tests.yml`

## Чистота репозитория

В проект добавлен `.gitignore` для временных логов (`*.out`, `*.err`) и служебных артефактов сборки, чтобы в репозиторий попадал только полезный код.

