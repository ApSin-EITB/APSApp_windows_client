# Roadmap

Updated: 2026-05-30

## Phase 0 - Spec and Repository Bootstrap

Goal: create the Windows client repository and source-of-truth specifications.

Exit criteria:

- README and AGENTS exist.
- Product spec exists.
- Architecture, API, security, UX, data/sync, QA and roadmap docs exist.
- ADR captures C# primary stack, C/C++ native islands and Qt decision.

## Phase 1 - Solution Skeleton

Goal: compile an empty but structured WinUI app.

Deliverables:

- `APSApp.Windows.sln`.
- WinUI app project.
- Core/infrastructure/messenger/calls test projects.
- DI and logging bootstrap.
- App shell with placeholder top-level routes.
- CI build/test workflow.

Exit criteria:

- restore/build/test pass locally and in CI.
- no secrets in repo.

## Phase 2 - Auth and Protected Session

Goal: login and restore secure session.

Deliverables:

- typed core auth client;
- token storage via Credential Locker/DPAPI;
- refresh handling;
- login/logout UI;
- 2FA challenge UI;
- local lock design skeleton.

Exit criteria:

- dev contour login/refresh/me works.
- tokens absent from logs and URLs.
- restart restore works.

## Phase 3 - Messenger Bootstrap

Goal: connect to chat runtime and load chat list.

Deliverables:

- WebSocket auth frame;
- global sync catch-up;
- local DB schema;
- chat list UI;
- contact dirty-signal handling;
- route state and reconnect policy.

Exit criteria:

- chat list loads from dev contour.
- reconnect does not duplicate rows.
- no previous-route flash in UI automation.

## Phase 4 - Direct E2EE Text Parity

Goal: Android-compatible direct encrypted text messaging.

Deliverables:

- direct crypto engine decision and implementation;
- key publish/fetch/backup basics;
- direct send/receive/decrypt;
- outbox replay;
- read/delivery states;
- Android interoperability tests.

Exit criteria:

- Windows <-> Android direct text passes on dev contour.
- plaintext absent from server/push/logs.

## Phase 5 - Media and Storage v1

Goal: attachment parity for beta media kinds.

Deliverables:

- file picker/drag/drop/paste;
- upload preparation;
- storage init/chunks/complete/bind;
- grant/stream/descriptor open;
- media cache;
- image/gif/video/audio/voice/pdf/file UI.

Exit criteria:

- full media matrix passes across Windows and Android for dev contour.

## Phase 6 - Groups, Channels and Comments

Goal: structured chat parity.

Deliverables:

- group/channel creation;
- info/settings;
- member/subscriber management;
- comments;
- ACL-gated UI.

Exit criteria:

- role matrix is covered or explicitly partial with owner-approved scope.

## Phase 7 - Calls

Goal: direct audio/video calls.

Deliverables:

- calls REST/signaling;
- LiveKit/WebRTC bridge;
- incoming call notifications;
- call controls/history.

Exit criteria:

- Windows <-> Android audio/video matrix passes.

## Phase 8 - Release Hardening

Goal: beta-quality Windows release.

Deliverables:

- MSIX packaging/signing;
- update policy;
- diagnostics/feedback;
- performance profile;
- security review;
- full QA matrix.

Exit criteria:

- release package installed/upgraded on Windows 10/11.
- QA evidence complete.
- no critical security/privacy gaps.

