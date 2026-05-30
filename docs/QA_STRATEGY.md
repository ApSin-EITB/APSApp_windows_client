# QA Strategy

Updated: 2026-05-30

## Principle

Windows QA follows the APSApp rule: no broad `QA PASS` after one representative happy path. A feature passes only after its matrix is covered or the uncovered areas are explicitly marked `QA PARTIAL`, `QA BLOCKED`, or `QA MANUAL`.

## Environments

Minimum beta matrix:

- Windows 10 22H2 x64, Russian locale.
- Windows 11 current x64, Russian locale.
- Windows 11 current x64, English locale.
- Fresh install.
- Upgrade install.
- Light theme.
- Dark theme.
- Network online/offline/reconnect.

Future:

- ARM64 after explicit roadmap decision.
- Microsoft Store packaged install if store channel is chosen.

## Automated Gates

Managed code:

```powershell
dotnet restore .\APSApp.Windows.sln
dotnet build .\APSApp.Windows.sln -c Debug
dotnet test .\APSApp.Windows.sln -c Debug
dotnet format .\APSApp.Windows.sln --verify-no-changes
```

Native code:

```powershell
cmake --preset windows-x64-debug
cmake --build --preset windows-x64-debug
ctest --preset windows-x64-debug --output-on-failure
```

Packaging:

```powershell
msbuild .\APSApp.Windows.sln /m /restore /p:Configuration=Release /p:Platform=x64
```

## Unit Test Areas

- Auth state and refresh.
- 2FA challenge state.
- Token redaction.
- DTO mapping.
- WebSocket auth/reconnect/sync protocol.
- Outbox dedupe/replay.
- Delivery/read status monotonicity.
- Contact dirty-signal handling.
- Media upload retry and terminal failures.
- Descriptor parsing.
- Local backup import filters.
- Notification privacy.
- Settings persistence.

## Integration Test Areas

Against dev contour:

- login/refresh/me;
- 2FA backup-code login path with disposable QA user;
- messenger activation;
- WebSocket auth and global sync;
- direct text send across two clients;
- Android-to-Windows and Windows-to-Android direct E2EE;
- storage upload/bind/grant/stream for each media kind;
- calls join/invite/end;
- feedback report upload with sanitized logs.

## Manual QA Matrix

### App Shell

- Fresh install opens unauthenticated state.
- Login restores shell.
- Restart restores route safely.
- Local lock prevents content flash.
- RU/EN switching.
- Light/dark switching.
- Resize compact/normal/wide.

### Auth

- Identifier/password success.
- Wrong password.
- Refresh after restart.
- Logout clears tokens.
- 2FA TOTP entry, owner/manual if required.
- 2FA backup code.
- Password recovery entry and backend-supported flow if in scope.

### Chats

- Chat list load.
- Search all/people/channels.
- Contact request accept/reject.
- Direct open.
- Group open.
- Channel open.
- Comments open.
- Back/forward navigation.
- No previous chat flash on switch.
- Empty state waits for load settle.

### Messaging

- Direct text send Windows -> Android.
- Direct text send Android -> Windows.
- Same-user multi-device sync.
- Edit propagation.
- Delete/hide behavior.
- Reply.
- Forward direct -> direct.
- Forward direct -> group/channel with warning.
- Unread/read status repair after reconnect.
- Killed/restarted app catches up without duplicates.

### Media

For image, gif, video, audio, voice, pdf, file:

- attach;
- upload progress;
- send;
- receive on Android;
- receive on Windows;
- cache loss open;
- restart open;
- failed upload retry/cancel.

### Structured Chats

- Create group.
- Create channel.
- Edit title/about/avatar.
- Add member/subscriber by allowed search contract.
- Raw numeric ID add rejected in UI.
- Promote/restrict/remove/ban where permission allows.
- Member device reflects access changes.
- Channel comments enable/disable where supported.

### Calls

- Audio call Windows -> Android.
- Audio call Android -> Windows.
- Video call Windows -> Android.
- Video call Android -> Windows.
- Decline.
- Cancel before answer.
- Missed call.
- Hangup.
- Microphone/camera toggles.
- Incoming notification opens call route.
- Reconnect/timeout states.

### Settings

- Profile edit and avatar upload.
- Appearance theme/language/reduce motion.
- Local lock and timeout.
- Windows Hello if hardware available.
- Hide notification message preview.
- Cache stats/limit/clear.
- Network check.
- Active session current-device info with no raw device ID.
- Feedback with screenshot and sanitized logs.

## Security QA

- Log scan for bearer tokens, refresh tokens, auth codes.
- Confirm no bearer token in WebSocket/HTTP URLs.
- Confirm notification payloads are metadata-only.
- Confirm direct message plaintext appears only after local decrypt.
- Confirm local DB/protected storage cannot be read as plaintext from ordinary files.
- Confirm feedback bundle redaction.
- Confirm native module inputs cannot crash the app with malformed media descriptors.

## Evidence Requirements

Each QA row needs:

- build/version;
- OS/version/locale;
- account/device topology;
- steps;
- expected/actual;
- screenshots or UI automation output when visual;
- logs with secrets redacted;
- backend evidence when behavior is server-authoritative;
- local DB/cache evidence when persistence is the claim.

