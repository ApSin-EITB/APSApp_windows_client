# Архитектура Windows-клиента

Обновлено: 2026-05-30

## Обзор

Windows-клиент - нативное desktop-приложение с managed C# shell/domain layer и опциональными C/C++ native islands. Архитектура повторяет продуктовые границы Android-клиента, но использует Windows-native lifecycle, layout и interaction patterns.

```text
WinUI 3 shell
  Views, resources, adaptive layout, Windows lifecycle

ViewModels
  MVVM state, commands, route-scoped effects

Domain / use cases
  Auth, messenger, calls, settings, notifications, sync policies

Repositories
  ChatRepository, AuthRepository, StorageRepository, CallsRepository

Infrastructure
  HTTP, WebSocket, local DB, encrypted secrets, media cache, logging

Native island boundary
  WebRTC/media, libsignal/crypto bindings, hot paths, Windows interop
```

## Windows platform layer

Все OS-specific возможности живут за adapters в `APSApp.Client.Infrastructure` или `APSApp.Windows`, а product/domain policy остается в domain modules.

| Adapter | Ответственность |
|---|---|
| `IAppActivationRouter` | Launch, protocol, notification, file и future share activation после auth/local-lock gate |
| `IWindowingService` | Main window, optional secondary windows, placement restore, DPI, title bar и AppWindow interop |
| `INotificationService` | Local app notifications, activation args, unread badge и future WNS boundary |
| `IProtectedSecretStore` | Credential Locker/DPAPI-backed tokens, refresh secrets, device material |
| `IFileInteractionService` | File picker, save picker, drag/drop, clipboard paste и safe temp files |
| `IAccessibilityDiagnostics` | High Contrast, text scaling, reduce motion и UI automation smoke hooks |
| `IPackagingInfoProvider` | MSIX identity, version, channel и update/install diagnostics |

## Ownership модулей

| Модуль | Ответственность |
|---|---|
| `APSApp.Windows` | App startup, WinUI shell, navigation, resources, app lifecycle и notifications UI |
| `APSApp.Client.Core` | Domain models, state machines, interfaces и общие result/error types |
| `APSApp.Client.Infrastructure` | HTTP/WS clients, auth handlers, storage, DB, settings, logging и OS adapters |
| `APSApp.Client.Messenger` | Chat repository, E2EE facade, outbox, timeline, media descriptors и backup flows |
| `APSApp.Client.Calls` | Calls state, call history, LiveKit/WebRTC facade и device selection |
| `APSApp.Client.Native` | Узкий native C/C++ bridge, только после обоснованной необходимости |

## Почему не полностью C++

Полностью C++ вариант технически возможен через C++/WinRT и WinUI 3, но это не лучший default для этого приложения. Большая часть клиента - это UI state, auth, sync, persistence, DTO mapping, settings, QA-oriented instrumentation и error handling. C# снижает продуктовый риск, ускоряет итерации и дает более безопасные defaults для крупного desktop-приложения. C/C++ оставляем там, где он действительно является правильным инструментом.

## Граница Qt

Qt не является основным стеком, потому что продуктовая цель - Windows-native client, а не cross-platform desktop shell. Qt добавит дополнительную UI/runtime абстракцию и ослабит прямое соответствие MSIX, WinUI, Windows notifications, Credential Locker, DPAPI и Windows app lifecycle. Если Linux/macOS desktop client станет продуктовой целью, перед возвращением к Qt нужен новый ADR.

## Threading model

- UI state обновляется только на WinUI dispatcher thread.
- Network, storage, crypto и media работа выполняется вне UI thread.
- Долгие операции принимают `CancellationToken`.
- WebSocket reconnect и sync сериализуются через domain-level coordinators, а не через view code.
- Native calls должны быть non-blocking или явно выполняться off UI thread.

## Навигация

Desktop navigation не копирует phone nav graph. Используется adaptive layout:

- compact: один content pane;
- normal: list + detail;
- wide: список + conversation + info/details.

Route state должен быть явным и сериализуемым только там, где это безопасно. Sensitive media requests, decrypted payloads и keys нельзя класть в navigation state.

## App lifecycle и activation

WinUI/Windows App SDK apps по умолчанию могут запускать несколько instances, но APSApp должен быть single-instance приложением:

- второй запуск активирует уже запущенный main instance;
- activation payload проходит через `IAppActivationRouter`;
- route intent не выполняется до session restore и local lock validation;
- notification action для call/message не может раскрыть content до unlock;
- file/protocol/share activation, когда появятся, используют тот же gate.

`APSApp.Windows` отвечает за ранний `AppInstance` decision, а domain modules получают только sanitized route intents.

## Windowing model

- Main window содержит Chats, Calls и Settings.
- Active call может получить secondary window после P1 decision, если это улучшает UX.
- Media viewer secondary window - P2.
- `AppWindow` используется для title bar, icon, min size, placement и future presenter states.
- Placement restore проверяет, что bounds остаются внутри видимой work area.
- Multi-monitor и per-monitor DPI не должны менять domain state или ломать timeline virtualization.

Windows 11 получает Mica/Mica Alt и richer title bar там, где это уместно. Windows 10 получает Acrylic/solid fallback без product behavior divergence.

## Network layer

Network layer использует named typed clients:

- Core backend: auth, profile, directory, settings и feedback.
- Chat backend: WebSocket, keys, reports и chats metadata.
- Storage backend: media upload/bind/grant/stream.
- Calls backend: call join/invite/end/signaling bootstrap.
- Update backend: Windows update metadata только если ADR-002 разрешит такой канал.

Все authenticated requests передают `Authorization: Bearer <jwt>` и стабильный desktop device identifier header, если backend contract этого требует. Query token auth запрещен.

## Локальное хранение

Локальное хранение разделено по sensitivity:

- Credential Locker / DPAPI: tokens, local encryption keys и device secret material.
- Encrypted SQLite или encrypted payload columns: chat history, outbox, media descriptors, sync state.
- Plain app settings: non-sensitive UI preferences.
- App-private cache: media thumbnails, decrypted temp playback files и upload payloads.

## Windows shell integration

Shell integrations являются optional adapters, а не core domain:

- taskbar unread badge - P0;
- local app notifications - P0;
- notification actions - P1 после packaged app QA;
- Jump List - P1/P2;
- tray icon - только после ADR, если нужен background presence;
- file associations/share target - P2 после hardened activation router.

Ни одна shell integration не может передавать plaintext message content в cloud/push payload или обходить local lock.

## E2EE / crypto boundary

Direct 1x1 E2EE должен быть совместим с текущей Android-моделью Signal/libsignal. Так как основной app слой на C#, crypto implementation абстрагируется:

```text
IPrivateChatCrypto
  PublishDeviceKeysAsync
  EncryptDirectMessageAsync
  DecryptDirectMessageAsync
  ExportSameDeviceKeyBackupAsync
  RestoreSameDeviceKeyBackupAsync
```

Backing implementation может быть native, если доступный managed-вариант недостаточно production-grade. Product code не должен зависеть от native-specific types.

## Media boundary

Media handling разделен:

- managed: descriptor parsing, upload orchestration, cache policy и UI state;
- native или проверенная library: WebRTC, codec-heavy paths, accelerated transforms, если нужно.

Большие media должны идти через file/stream paths. Нельзя материализовать полный remote attachment в memory для open, preview, recovery или notification preview.

## Observability

Client logs являются local-first и redacted. Feedback reports могут загружать sanitized diagnostics через core backend feedback path. Windows-клиент не должен ходить напрямую в Prometheus, Loki, Grafana или backend-internal hosts.

## Error model

Backend и transport errors нормализуются в domain errors:

- auth expired / refresh failed;
- offline / DNS / TLS / timeout;
- server unavailable;
- permission denied;
- chat access lost;
- media binding invalid;
- decrypt failed;
- local storage unavailable.

User-facing errors должны быть локализованы и не быть техническими. Diagnostic details пишутся только в redacted logs.
