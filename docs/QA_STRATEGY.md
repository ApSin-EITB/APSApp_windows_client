# QA Strategy

Обновлено: 2026-05-30

## Принцип

Windows QA следует общему правилу APSApp: нельзя ставить широкий `QA PASS` после одного representative happy path. Функция считается пройденной только когда покрыта ее matrix или непокрытые области явно отмечены как `QA PARTIAL`, `QA BLOCKED` или `QA MANUAL`.

## Окружения

Минимальная beta matrix:

- Windows 10 22H2 x64, русская локаль.
- Windows 11 current x64, русская локаль.
- Windows 11 current x64, английская локаль.
- Fresh install.
- Upgrade install.
- Light theme.
- Dark theme.
- Network online/offline/reconnect.

Future:

- ARM64 после явного roadmap decision.
- Microsoft Store packaged install, если выбран store channel.

## Automated gates

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

## Области unit tests

- Auth state и refresh.
- 2FA challenge state.
- Token redaction.
- DTO mapping.
- WebSocket auth/reconnect/sync protocol.
- Outbox dedupe/replay.
- Delivery/read status monotonicity.
- Contact dirty-signal handling.
- Media upload retry и terminal failures.
- Descriptor parsing.
- Local backup import filters.
- Notification privacy.
- Settings persistence.

## Области integration tests

На dev contour:

- login/refresh/me;
- 2FA backup-code login path с disposable QA user;
- messenger activation;
- WebSocket auth и global sync;
- direct text send между двумя clients;
- Android-to-Windows и Windows-to-Android direct E2EE;
- storage upload/bind/grant/stream для каждого media kind;
- calls join/invite/end;
- feedback report upload с sanitized logs.

## Manual QA matrix

### App shell

- Fresh install открывает unauthenticated state.
- Login восстанавливает shell.
- Restart безопасно восстанавливает route.
- Local lock не допускает content flash.
- RU/EN switching.
- Light/dark switching.
- Resize compact/normal/wide.

### Auth

- Identifier/password success.
- Wrong password.
- Refresh after restart.
- Logout clears tokens.
- 2FA TOTP entry, owner/manual если требуется.
- 2FA backup code.
- Password recovery entry и backend-supported flow, если в scope.

### Chats

- Chat list load.
- Search all/people/channels.
- Contact request accept/reject.
- Direct open.
- Group open.
- Channel open.
- Comments open.
- Back/forward navigation.
- Нет flash предыдущего чата при switch.
- Empty state ждет load settle.

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

Для image, gif, video, audio, voice, pdf, file:

- attach;
- upload progress;
- send;
- receive on Android;
- receive on Windows;
- cache loss open;
- restart open;
- failed upload retry/cancel.

### Structured chats

- Create group.
- Create channel.
- Edit title/about/avatar.
- Add member/subscriber по разрешенному search contract.
- Raw numeric ID add rejected in UI.
- Promote/restrict/remove/ban там, где permission allows.
- Member device reflects access changes.
- Channel comments enable/disable там, где supported.

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
- Active session current-device info без raw device ID.
- Feedback with screenshot and sanitized logs.

## Security QA

- Log scan на bearer tokens, refresh tokens, auth codes.
- Подтвердить, что bearer token не попадает в WebSocket/HTTP URLs.
- Подтвердить, что notification payloads являются metadata-only.
- Подтвердить, что direct message plaintext появляется только после local decrypt.
- Подтвердить, что local DB/protected storage не читаются как plaintext из ordinary files.
- Подтвердить feedback bundle redaction.
- Подтвердить, что native module inputs не crash-ат app на malformed media descriptors.

## Evidence requirements

Каждая QA row требует:

- build/version;
- OS/version/locale;
- account/device topology;
- steps;
- expected/actual;
- screenshots или UI automation output, если проверка visual;
- logs with secrets redacted;
- backend evidence, если behavior server-authoritative;
- local DB/cache evidence, если заявляется persistence.
