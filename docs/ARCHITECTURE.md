# Windows Architecture

Updated: 2026-05-30

## Overview

The Windows client is a native desktop app built around a managed C# shell and domain layer, with optional C/C++ native islands. The architecture mirrors Android's product boundaries but uses Windows-native interaction and lifecycle patterns.

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

## Module Ownership

| Module | Responsibility |
|---|---|
| `APSApp.Windows` | App startup, WinUI shell, navigation, resources, app lifecycle, notifications UI |
| `APSApp.Client.Core` | Domain models, state machines, interfaces, shared result/error types |
| `APSApp.Client.Infrastructure` | HTTP/WS clients, auth handlers, storage, DB, settings, logging, OS adapters |
| `APSApp.Client.Messenger` | Chat repository, E2EE facade, outbox, timeline, media descriptors, backup flows |
| `APSApp.Client.Calls` | Calls state, call history, LiveKit/WebRTC facade, device selection |
| `APSApp.Client.Native` | Narrow native C/C++ bridge, only after a justified need |

## Why Not All C++

All-C++ would be technically possible with C++/WinRT and WinUI 3, but it is not the best default for this app. Most of the client is UI state, auth, sync, persistence, DTO mapping, settings, QA-oriented instrumentation and error handling. C# reduces product risk, improves iteration speed and gives safer defaults for a large desktop app. C/C++ is kept for domains where it is actually the right tool.

## Qt Boundary

Qt is not primary because the product goal is a Windows-native client, not a cross-platform desktop shell. Qt would add another UI/runtime abstraction while weakening direct alignment with MSIX, WinUI, Windows notifications, Credential Locker, DPAPI and Windows app lifecycle. If a Linux/macOS desktop client becomes a product goal, write a new ADR before revisiting Qt.

## Threading Model

- UI state is updated on the WinUI dispatcher thread only.
- Network, storage, crypto and media work runs off the UI thread.
- Long operations accept `CancellationToken`.
- WebSocket reconnect and sync are serialized through domain-level coordinators, not view code.
- Native calls must be non-blocking or explicitly offloaded.

## Navigation

Desktop navigation is not a phone nav graph copy. It uses adaptive layout:

- compact: single content pane;
- normal: list + detail;
- wide: list + conversation + info/details.

Route state must be explicit and serializable only when safe. Sensitive media requests, decrypted payloads and keys must not be put into navigation state.

## Network Layer

The network layer uses named typed clients:

- Core backend: auth, profile, directory, settings, feedback.
- Chat backend: WebSocket, keys, reports, chats metadata.
- Storage backend: media upload/bind/grant/stream.
- Calls backend: call join/invite/end/signaling bootstrap.
- Update backend: Windows update metadata only if ADR-002 enables it.

All authenticated requests include `Authorization: Bearer <jwt>` and a stable desktop device identifier header where backend contract requires it. Query token auth is forbidden.

## Local Storage

Local storage is split by sensitivity:

- Credential Locker / DPAPI: tokens, local encryption keys, device secret material.
- Encrypted SQLite or encrypted payload columns: chat history, outbox, media descriptors, sync state.
- Plain app settings: non-sensitive UI preferences.
- App-private cache: media thumbnails, decrypted temp playback files, upload payloads.

## E2EE / Crypto Boundary

Direct 1x1 E2EE must be compatible with Android's current Signal/libsignal model. Since the primary app is C#, the crypto implementation must be abstracted:

```text
IPrivateChatCrypto
  PublishDeviceKeysAsync
  EncryptDirectMessageAsync
  DecryptDirectMessageAsync
  ExportSameDeviceKeyBackupAsync
  RestoreSameDeviceKeyBackupAsync
```

The backing implementation may be native if the available managed option is not production-grade. Product code must not depend on native-specific types.

## Media Boundary

Media handling is split:

- managed: descriptor parsing, upload orchestration, cache policy, UI state;
- native or proven library: WebRTC, codec-heavy paths, accelerated transforms if required.

Large media must use file/stream paths. Do not materialize full remote attachments in memory for open, preview, recovery or notification preview.

## Observability

Client logs are local-first and redacted. Feedback reports can upload sanitized diagnostics through the core backend feedback path. The Windows client must not talk directly to Prometheus, Loki, Grafana or backend-internal hosts.

## Error Model

Backend and transport errors are normalized into domain errors:

- auth expired / refresh failed;
- offline / DNS / TLS / timeout;
- server unavailable;
- permission denied;
- chat access lost;
- media binding invalid;
- decrypt failed;
- local storage unavailable.

User-facing errors must be localized and non-technical. Diagnostic details go to redacted logs.

