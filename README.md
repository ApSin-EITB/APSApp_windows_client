# APSApp Windows Client

Native Windows desktop client for APSApp / "Личка".

Status: specification-first bootstrap, created 2026-05-30. The repository is the future Windows client source of truth and currently contains product, architecture, security, API, UX, data/sync, QA, and roadmap specifications.

## Canon

- Primary stack: C# with WinUI 3 / Windows App SDK.
- Native islands: C/C++ only where they buy real value: media/WebRTC, crypto bindings, performance hot paths, and platform interop.
- Qt: documented alternative, not the primary stack for this Windows-native client.
- Product source: current Android app and backend contracts.

## Documents

| Document | Purpose |
|---|---|
| [docs/SPEC.md](docs/SPEC.md) | Full product and engineering specification |
| [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) | Windows architecture, modules, threading, native boundaries |
| [docs/API_CONTRACTS.md](docs/API_CONTRACTS.md) | Core/chat/storage/calls runtime contracts consumed by Windows |
| [docs/SECURITY.md](docs/SECURITY.md) | Auth, token storage, E2EE, local data protection, notification privacy |
| [docs/UI_UX_SPEC.md](docs/UI_UX_SPEC.md) | Windows UX, IA, screens, keyboard, accessibility |
| [docs/DATA_SYNC_STORAGE.md](docs/DATA_SYNC_STORAGE.md) | Local database, sync, cache, backups, media handling |
| [docs/FEATURE_PARITY.md](docs/FEATURE_PARITY.md) | Android-to-Windows feature parity map |
| [docs/IMPLEMENTATION_PLAN.md](docs/IMPLEMENTATION_PLAN.md) | Ordered implementation tasks with acceptance checks |
| [docs/QA_STRATEGY.md](docs/QA_STRATEGY.md) | Manual and automated QA matrix |
| [docs/ROADMAP.md](docs/ROADMAP.md) | Milestones from spec-only to beta |
| [docs/decisions](docs/decisions) | ADRs |

## Future Commands

Commands become mandatory after the first solution skeleton lands.

```powershell
# Restore/build/test managed code
dotnet restore .\APSApp.Windows.sln
dotnet build .\APSApp.Windows.sln -c Debug
dotnet test .\APSApp.Windows.sln -c Debug
dotnet format .\APSApp.Windows.sln --verify-no-changes

# Native island, when introduced
cmake --preset windows-x64-debug
cmake --build --preset windows-x64-debug
ctest --preset windows-x64-debug --output-on-failure

# Packaging, when introduced
msbuild .\APSApp.Windows.sln /m /restore /p:Configuration=Release /p:Platform=x64
```

## Source References

This repo was bootstrapped from the current APSApp workspace context:

- `APSApp_android_app/docs/APP_FEATURE_INVENTORY.md`
- `APSApp_android_app/docs/ARCHITECTURE.md`
- `APSApp_android_app/docs/NETWORK_AND_API.md`
- `APSApp_android_app/docs/module_messenger/*`
- `APSApp_app_backend/docs/API.md`
- `APSApp_app_backend/docs/ARCHITECTURE.md`
- `APSApp_messenger_storage_backend/docs/02_API.md`
- `APSApp_messenger_calls_api/docs/API_SIGNALING.md`
- `aps-rules.md`
