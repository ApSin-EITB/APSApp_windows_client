# План реализации

Обновлено: 2026-05-30

План превращает спецификации в ordered implementation slices. Каждая задача должна помещаться в один focused branch/PR.

## Phase 1 - Solution skeleton

- [ ] Задача: создать C# WinUI solution и проекты.
  - Приемка: `APSApp.Windows.sln` содержит app, core, infrastructure, messenger, calls и test projects.
  - Проверка: `dotnet restore .\APSApp.Windows.sln`; `dotnet build .\APSApp.Windows.sln -c Debug`.
  - Файлы: `src/*`, `tests/*`, solution/project files.

- [ ] Задача: добавить DI, logging и configuration bootstrap.
  - Приемка: app starts, пишет redacted startup line, shell view model резолвится через DI.
  - Проверка: unit test на service registration; manual local run.
  - Файлы: `src/APSApp.Windows`, `src/APSApp.Client.Infrastructure`.

- [ ] Задача: добавить placeholder shell routes.
  - Приемка: Chats, Calls и Settings routes существуют с adaptive empty states.
  - Проверка: UI smoke test открывает каждый route.
  - Файлы: `src/APSApp.Windows`.

- [ ] Задача: добавить CI workflow.
  - Приемка: restore/build/test выполняются на PR.
  - Проверка: GitHub Actions green.
  - Файлы: `.github/workflows/windows-ci.yml`.

## Phase 2 - Auth и session

- [ ] Задача: реализовать typed core auth API client.
  - Приемка: login/refresh/me DTOs map-ятся в domain state без UI dependency.
  - Проверка: DTO unit tests и dev-contour integration test с local-only credentials.
  - Файлы: `src/APSApp.Client.Infrastructure`, `tests/*`.

- [ ] Задача: реализовать secure token store.
  - Приемка: tokens хранятся через Credential Locker/DPAPI abstraction, не plaintext repo/local config.
  - Проверка: unit tests с fake store; manual filesystem inspection.
  - Файлы: `src/APSApp.Client.Infrastructure`.

- [ ] Задача: собрать login/logout UI.
  - Приемка: login success входит в shell; logout очищает session и возвращает на login.
  - Проверка: UI automation и dev-contour smoke.
  - Файлы: `src/APSApp.Windows`, `src/APSApp.Client.Core`.

- [ ] Задача: добавить 2FA challenge state.
  - Приемка: TOTP и backup-code challenge modes поддержаны.
  - Проверка: unit tests плюс disposable QA account smoke, когда доступно.
  - Файлы: auth view models and API models.

## Phase 3 - Local storage и sync foundation

- [ ] Задача: добавить local DB schema v1.
  - Приемка: chats, messages, outbox, sync cursors и media jobs persist.
  - Проверка: migration tests.
  - Файлы: `src/APSApp.Client.Infrastructure`.

- [ ] Задача: добавить protected local data strategy.
  - Приемка: sensitive chat/token/key data encrypted/protected согласно `SECURITY.md`.
  - Проверка: security unit tests и local file inspection.
  - Файлы: storage infrastructure.

- [ ] Задача: реализовать WebSocket client auth/reconnect.
  - Приемка: отправляет canonical auth frame, обрабатывает auth_ok/error, reconnects with backoff.
  - Проверка: protocol unit tests и dev-contour smoke.
  - Файлы: messenger infrastructure.

- [ ] Задача: реализовать global sync catch-up coordinator.
  - Приемка: startup/reconnect sync идет до old outbox replay.
  - Проверка: unit tests на ordering и replay suppression.
  - Файлы: messenger domain/infrastructure.

## Phase 4 - Chat list и direct text

- [ ] Задача: Chat list repository and UI.
  - Приемка: cached и live chat snapshots render with unread/pin/mute/typing/preview.
  - Проверка: repository tests и UI automation.
  - Файлы: messenger, Windows views.

- [ ] Задача: Direct text outbox.
  - Приемка: outgoing direct text persists locally, sends once, reconciles by `clientId`.
  - Проверка: unit tests и Windows -> Android dev smoke.
  - Файлы: messenger repository/outbox.

- [ ] Задача: Direct inbound text.
  - Приемка: incoming Android -> Windows message decrypts/renders after sync/live event.
  - Проверка: dev-contour two-device test.
  - Файлы: crypto facade, sync, UI.

- [ ] Задача: Read/delivery status.
  - Приемка: statuses monotonic и repair after reconnect.
  - Проверка: unit tests плюс restart/reconnect QA.
  - Файлы: messenger domain.

## Phase 5 - E2EE и backup hardening

- [ ] Задача: выбрать concrete libsignal binding.
  - Приемка: ADR или ADR update выбирает managed/native implementation с test plan.
  - Проверка: owner review.
  - Файлы: `docs/decisions/*`, crypto project.

- [ ] Задача: publish/fetch device keys.
  - Приемка: key directory endpoints работают на dev contour.
  - Проверка: integration tests.
  - Файлы: crypto/key service.

- [ ] Задача: same-device key backup restore.
  - Приемка: backup read/import/restore-complete работает, pure read не считается proof.
  - Проверка: restore QA with fresh install.
  - Файлы: backup manager.

- [ ] Задача: user-data backup import filters.
  - Приемка: poison placeholders и unresolved local rows skipped.
  - Проверка: unit tests reproducing Android incidents.
  - Файлы: backup/data import.

## Phase 6 - Media storage

- [ ] Задача: attachment picker, drag-drop и draft strip.
  - Приемка: up to 10 drafts, removable before send.
  - Проверка: UI automation.
  - Файлы: Windows views, messenger media domain.

- [ ] Задача: upload preparation и storage v1 upload.
  - Приемка: init/chunks/complete/bind with accepted chunk size and recovery rules.
  - Проверка: storage integration tests.
  - Файлы: media upload worker/service.

- [ ] Задача: descriptor-aware open/playback.
  - Приемка: image/gif/video/audio/voice/pdf/file open from grant/stream.
  - Проверка: media QA matrix.
  - Файлы: media resolver/cache/UI.

## Phase 7 - Structured chats

- [ ] Задача: group/channel create and info.
  - Приемка: create flows, info screens and avatar/title/about basics work.
  - Проверка: dev-contour QA with backend evidence.
  - Файлы: messenger structured chat.

- [ ] Задача: member/subscriber management.
  - Приемка: role-gated actions match server ACL.
  - Проверка: role matrix QA.
  - Файлы: structured chat UI/repository.

## Phase 8 - Calls

- [ ] Задача: calls API client and history.
  - Приемка: history loads; invite/end/join DTOs map correctly.
  - Проверка: unit and dev integration tests.
  - Файлы: calls module.

- [ ] Задача: WebRTC/LiveKit bridge.
  - Приемка: audio/video call media connects between Windows and Android.
  - Проверка: Windows <-> Android calls QA.
  - Файлы: calls module, possible native island.

- [ ] Задача: incoming call notifications.
  - Приемка: incoming call notification can accept/decline/open route.
  - Проверка: packaged app QA.
  - Файлы: Windows notification integration.

## Phase 9 - Packaging и release

- [ ] Задача: MSIX packaging.
  - Приемка: signed package installs and upgrades.
  - Проверка: Windows 10/11 install/upgrade QA.
  - Файлы: packaging project, CI.

- [ ] Задача: feedback/diagnostics.
  - Приемка: sanitized report with optional screenshot uploads through core backend.
  - Проверка: backend evidence и redaction scan.
  - Файлы: settings/feedback.

- [ ] Задача: full beta QA.
  - Приемка: `QA_STRATEGY.md` matrix has PASS/PARTIAL/BLOCKED evidence.
  - Проверка: manual evidence bundle.
  - Файлы: QA artifacts вне repo, если summaries не sanitized.
