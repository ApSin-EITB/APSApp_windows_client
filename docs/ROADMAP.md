# Roadmap

Обновлено: 2026-05-30

## Phase 0 - Spec и repository bootstrap

Цель: создать репозиторий Windows-клиента и source-of-truth specifications.

Exit criteria:

- README и AGENTS существуют.
- Product spec существует.
- Architecture, API, security, UX, data/sync, QA и roadmap docs существуют.
- ADR фиксирует C# primary stack, C/C++ native islands и Qt decision.
- Windows-native spec фиксирует Windows 11 primary tier и Windows 10 22H2 compatibility tier.

## Phase 1 - Solution skeleton

Цель: пустое, но структурированное WinUI-приложение компилируется.

Deliverables:

- `APSApp.Windows.sln`.
- WinUI app project.
- Core/infrastructure/messenger/calls test projects.
- DI и logging bootstrap.
- App shell с placeholder top-level routes.
- Windows platform adapters skeleton: activation, windowing, notifications, protected secrets, file interaction и accessibility diagnostics.
- CI build/test workflow.

Exit criteria:

- restore/build/test проходят локально и в CI.
- single-instance launch и basic AppWindow shell проходят manual smoke.
- в repo нет secrets.

## Phase 2 - Auth и protected session

Цель: login и restore secure session.

Deliverables:

- typed core auth client;
- token storage через Credential Locker/DPAPI;
- refresh handling;
- login/logout UI;
- 2FA challenge UI;
- local lock design skeleton.

Exit criteria:

- dev contour login/refresh/me работает.
- tokens отсутствуют в logs и URLs.
- restart restore работает.

## Phase 3 - Messenger bootstrap

Цель: подключиться к chat runtime и загрузить chat list.

Deliverables:

- WebSocket auth frame;
- global sync catch-up;
- local DB schema;
- chat list UI;
- contact dirty-signal handling;
- route state и reconnect policy.

Exit criteria:

- chat list грузится из dev contour.
- reconnect не duplicate-ит rows.
- no previous-route flash в UI automation.

## Phase 4 - Direct E2EE text parity

Цель: Android-compatible direct encrypted text messaging.

Deliverables:

- direct crypto engine decision и implementation;
- key publish/fetch/backup basics;
- direct send/receive/decrypt;
- outbox replay;
- read/delivery states;
- Android interoperability tests.

Exit criteria:

- Windows <-> Android direct text проходит на dev contour.
- plaintext отсутствует на server/push/logs.

## Phase 5 - Media и Storage v1

Цель: attachment parity для beta media kinds.

Deliverables:

- file picker/drag/drop/paste;
- upload preparation;
- storage init/chunks/complete/bind;
- grant/stream/descriptor open;
- media cache;
- image/gif/video/audio/voice/pdf/file UI.

Exit criteria:

- full media matrix проходит между Windows и Android на dev contour.

## Phase 6 - Groups, channels и comments

Цель: structured chat parity.

Deliverables:

- group/channel creation;
- info/settings;
- member/subscriber management;
- comments;
- ACL-gated UI.

Exit criteria:

- role matrix покрыта или явно partial с owner-approved scope.

## Phase 7 - Calls

Цель: direct audio/video calls.

Deliverables:

- calls REST/signaling;
- LiveKit/WebRTC bridge;
- incoming call notifications;
- call controls/history.

Exit criteria:

- Windows <-> Android audio/video matrix проходит.

## Phase 8 - Release hardening

Цель: beta-quality Windows release.

Deliverables:

- MSIX packaging/signing;
- update policy;
- diagnostics/feedback;
- Windows-native QA: DPI, multi-monitor, High Contrast, Narrator, notification activation, taskbar badge;
- performance profile;
- security review;
- full QA matrix.

Exit criteria:

- release package installs/upgrades on Windows 10/11.
- QA evidence complete.
- нет critical security/privacy gaps.
