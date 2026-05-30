# APSApp Windows Client

Нативный Windows-клиент для APSApp / «Личка».

Статус: репозиторий в режиме `spec-first`, создан 2026-05-30. Сейчас это источник истины для будущего Windows-клиента: здесь лежат продуктовые, архитектурные, security, API, UX, data/sync, QA и roadmap-спецификации.

## Канон

- Основной стек: C# + WinUI 3 / Windows App SDK.
- Нативные острова: C/C++ только там, где есть реальная польза: media/WebRTC, crypto bindings, performance hot paths и platform interop.
- Qt: описанная альтернатива, но не основной стек для Windows-native клиента.
- Продуктовый источник: текущий Android-клиент и backend-контракты.
- Язык документации: русский. Английский оставляем для названий стеков, API, команд, файлов, классов, протокольных событий и устоявшихся терминов.

## Документы

| Документ | Назначение |
|---|---|
| [docs/SPEC.md](docs/SPEC.md) | Полная продуктовая и инженерная спецификация |
| [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) | Архитектура Windows-клиента, модули, threading, native boundaries |
| [docs/WINDOWS_NATIVE_SPEC.md](docs/WINDOWS_NATIVE_SPEC.md) | Специфика Windows 10/11: lifecycle, окна, notifications, taskbar, DPI, accessibility и fallback-режимы |
| [docs/API_CONTRACTS.md](docs/API_CONTRACTS.md) | Контракты Core/chat/storage/calls runtime, которые потребляет Windows |
| [docs/SECURITY.md](docs/SECURITY.md) | Auth, token storage, E2EE, local data protection и notification privacy |
| [docs/UI_UX_SPEC.md](docs/UI_UX_SPEC.md) | Windows UX, IA, экраны, keyboard, accessibility |
| [docs/DATA_SYNC_STORAGE.md](docs/DATA_SYNC_STORAGE.md) | Local database, sync, cache, backups и media handling |
| [docs/FEATURE_PARITY.md](docs/FEATURE_PARITY.md) | Карта parity между Android и Windows |
| [docs/IMPLEMENTATION_PLAN.md](docs/IMPLEMENTATION_PLAN.md) | Порядок реализации с acceptance/verify |
| [docs/QA_STRATEGY.md](docs/QA_STRATEGY.md) | Manual и automated QA matrix |
| [docs/ROADMAP.md](docs/ROADMAP.md) | Milestones от spec-only до beta |
| [docs/decisions](docs/decisions) | ADR |

## Будущие команды

Команды становятся обязательными после появления первого solution skeleton.

```powershell
# Restore/build/test managed code
dotnet restore .\APSApp.Windows.sln
dotnet build .\APSApp.Windows.sln -c Debug
dotnet test .\APSApp.Windows.sln -c Debug
dotnet format .\APSApp.Windows.sln --verify-no-changes

# Native island, когда появится
cmake --preset windows-x64-debug
cmake --build --preset windows-x64-debug
ctest --preset windows-x64-debug --output-on-failure

# Packaging, когда появится
msbuild .\APSApp.Windows.sln /m /restore /p:Configuration=Release /p:Platform=x64
```

## Источники

Репозиторий bootstrapped из текущего APSApp workspace context:

- `APSApp_android_app/docs/APP_FEATURE_INVENTORY.md`
- `APSApp_android_app/docs/ARCHITECTURE.md`
- `APSApp_android_app/docs/NETWORK_AND_API.md`
- `APSApp_android_app/docs/module_messenger/*`
- `APSApp_app_backend/docs/API.md`
- `APSApp_app_backend/docs/ARCHITECTURE.md`
- `APSApp_messenger_storage_backend/docs/02_API.md`
- `APSApp_messenger_calls_api/docs/API_SIGNALING.md`
- `aps-rules.md`
