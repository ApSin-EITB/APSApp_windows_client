# Implementation Plan

Updated: 2026-05-30

This plan turns the specs into ordered implementation slices. Each task should be small enough for one focused branch/PR.

## Phase 1 - Solution Skeleton

- [ ] Task: Create C# WinUI solution and projects.
  - Acceptance: `APSApp.Windows.sln` contains app, core, infrastructure, messenger, calls and test projects.
  - Verify: `dotnet restore .\APSApp.Windows.sln`; `dotnet build .\APSApp.Windows.sln -c Debug`.
  - Files: `src/*`, `tests/*`, solution/project files.

- [ ] Task: Add DI, logging and configuration bootstrap.
  - Acceptance: app starts, logs redacted startup line, resolves shell view model through DI.
  - Verify: unit test for service registration; manual local run.
  - Files: `src/APSApp.Windows`, `src/APSApp.Client.Infrastructure`.

- [ ] Task: Add placeholder shell routes.
  - Acceptance: Chats, Calls and Settings routes exist with adaptive empty states.
  - Verify: UI smoke test opens each route.
  - Files: `src/APSApp.Windows`.

- [ ] Task: Add CI workflow.
  - Acceptance: restore/build/test run on PR.
  - Verify: GitHub Actions green.
  - Files: `.github/workflows/windows-ci.yml`.

## Phase 2 - Auth and Session

- [ ] Task: Implement typed core auth API client.
  - Acceptance: login/refresh/me DTOs map to domain state without UI dependency.
  - Verify: DTO unit tests and dev-contour integration test with local-only credentials.
  - Files: `src/APSApp.Client.Infrastructure`, `tests/*`.

- [ ] Task: Implement secure token store.
  - Acceptance: tokens stored via Credential Locker/DPAPI abstraction, never plaintext repo/local config.
  - Verify: unit tests with fake store; manual filesystem inspection.
  - Files: `src/APSApp.Client.Infrastructure`.

- [ ] Task: Build login/logout UI.
  - Acceptance: login success enters shell; logout clears session and returns to login.
  - Verify: UI automation and dev-contour smoke.
  - Files: `src/APSApp.Windows`, `src/APSApp.Client.Core`.

- [ ] Task: Add 2FA challenge state.
  - Acceptance: TOTP and backup-code challenge modes are supported.
  - Verify: unit tests plus disposable QA account smoke when available.
  - Files: auth view models and API models.

## Phase 3 - Local Storage and Sync Foundation

- [ ] Task: Add local DB schema v1.
  - Acceptance: chats, messages, outbox, sync cursors and media jobs persist.
  - Verify: migration tests.
  - Files: `src/APSApp.Client.Infrastructure`.

- [ ] Task: Add protected local data strategy.
  - Acceptance: sensitive chat/token/key data is encrypted/protected according to `SECURITY.md`.
  - Verify: security unit tests and local file inspection.
  - Files: storage infrastructure.

- [ ] Task: Implement WebSocket client auth/reconnect.
  - Acceptance: sends canonical auth frame, handles auth_ok/error, reconnects with backoff.
  - Verify: protocol unit tests and dev-contour smoke.
  - Files: messenger infrastructure.

- [ ] Task: Implement global sync catch-up coordinator.
  - Acceptance: startup/reconnect sync runs before old outbox replay.
  - Verify: unit tests for ordering and replay suppression.
  - Files: messenger domain/infrastructure.

## Phase 4 - Chat List and Direct Text

- [ ] Task: Chat list repository and UI.
  - Acceptance: cached and live chat snapshots render with unread/pin/mute/typing/preview.
  - Verify: repository tests and UI automation.
  - Files: messenger, Windows views.

- [ ] Task: Direct text outbox.
  - Acceptance: outgoing direct text persists locally, sends once, reconciles by `clientId`.
  - Verify: unit tests and Windows -> Android dev smoke.
  - Files: messenger repository/outbox.

- [ ] Task: Direct inbound text.
  - Acceptance: incoming Android -> Windows message decrypts/renders after sync/live event.
  - Verify: dev-contour two-device test.
  - Files: crypto facade, sync, UI.

- [ ] Task: Read/delivery status.
  - Acceptance: statuses are monotonic and repair after reconnect.
  - Verify: unit tests plus restart/reconnect QA.
  - Files: messenger domain.

## Phase 5 - E2EE and Backup Hardening

- [ ] Task: Decide concrete libsignal binding.
  - Acceptance: ADR or ADR update chooses managed/native implementation with test plan.
  - Verify: owner review.
  - Files: `docs/decisions/*`, crypto project.

- [ ] Task: Publish/fetch device keys.
  - Acceptance: key directory endpoints work on dev contour.
  - Verify: integration tests.
  - Files: crypto/key service.

- [ ] Task: Same-device key backup restore.
  - Acceptance: backup read/import/restore-complete works and pure read is not proof.
  - Verify: restore QA with fresh install.
  - Files: backup manager.

- [ ] Task: User-data backup import filters.
  - Acceptance: poison placeholders and unresolved local rows are skipped.
  - Verify: unit tests reproducing Android incidents.
  - Files: backup/data import.

## Phase 6 - Media Storage

- [ ] Task: Attachment picker, drag-drop and draft strip.
  - Acceptance: up to 10 drafts, removable before send.
  - Verify: UI automation.
  - Files: Windows views, messenger media domain.

- [ ] Task: Upload preparation and storage v1 upload.
  - Acceptance: init/chunks/complete/bind with accepted chunk size and recovery rules.
  - Verify: storage integration tests.
  - Files: media upload worker/service.

- [ ] Task: Descriptor-aware open/playback.
  - Acceptance: image/gif/video/audio/voice/pdf/file open from grant/stream.
  - Verify: media QA matrix.
  - Files: media resolver/cache/UI.

## Phase 7 - Structured Chats

- [ ] Task: Group/channel create and info.
  - Acceptance: create flows, info screens and avatar/title/about basics work.
  - Verify: dev-contour QA with backend evidence.
  - Files: messenger structured chat.

- [ ] Task: Member/subscriber management.
  - Acceptance: role-gated actions match server ACL.
  - Verify: role matrix QA.
  - Files: structured chat UI/repository.

## Phase 8 - Calls

- [ ] Task: Calls API client and history.
  - Acceptance: history loads; invite/end/join DTOs map correctly.
  - Verify: unit and dev integration tests.
  - Files: calls module.

- [ ] Task: WebRTC/LiveKit bridge.
  - Acceptance: audio/video call media connects between Windows and Android.
  - Verify: Windows <-> Android calls QA.
  - Files: calls module, possible native island.

- [ ] Task: Incoming call notifications.
  - Acceptance: incoming call notification can accept/decline/open route.
  - Verify: packaged app QA.
  - Files: Windows notification integration.

## Phase 9 - Packaging and Release

- [ ] Task: MSIX packaging.
  - Acceptance: signed package installs and upgrades.
  - Verify: Windows 10/11 install/upgrade QA.
  - Files: packaging project, CI.

- [ ] Task: Feedback/diagnostics.
  - Acceptance: sanitized report with optional screenshot uploads through core backend.
  - Verify: backend evidence and redaction scan.
  - Files: settings/feedback.

- [ ] Task: Full beta QA.
  - Acceptance: `QA_STRATEGY.md` matrix has PASS/PARTIAL/BLOCKED evidence.
  - Verify: manual evidence bundle.
  - Files: QA artifacts outside repo unless sanitized summaries are committed.

