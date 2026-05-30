# QA Strategy

Обновлено: 2026-05-30

## Принцип

Windows QA следует общему правилу APSApp: нельзя ставить широкий `QA PASS` после одного representative happy path. Функция считается пройденной только когда покрыта ее matrix или непокрытые области явно отмечены как `QA PARTIAL`, `QA BLOCKED` или `QA MANUAL`.

## Окружения

Минимальная beta matrix:

- Windows 11 current supported x64, русская локаль, Tier A.
- Windows 11 current supported x64, английская локаль, Tier A.
- Windows 10 22H2 x64, русская локаль, Tier B compatibility.
- Fresh install.
- Upgrade install.
- Light theme.
- Dark theme.
- Network online/offline/reconnect.

Future:

- ARM64 после явного roadmap decision.
- Microsoft Store packaged install, если выбран store channel.

## Windows-native QA matrix

### OS tiers

- Windows 11 Tier A: все P0 release gates должны иметь `QA PASS` или owner-approved `QA PARTIAL`.
- Windows 10 22H2 Tier B: core flows должны работать, visual/system gaps фиксируются как compatibility fallback или `QA PARTIAL`.
- Windows 10 pre-22H2 и Windows 7/8/8.1 не тестируются без нового owner decision.

### Shell/lifecycle

- Single-instance: повторный launch активирует existing window.
- Notification activation: click/action открывает correct route после local-lock gate.
- Protocol activation: safe route или login/onboarding без раскрытия content.
- File activation/open-with: если включено, не обходит auth/local-lock.
- Restart после crash не replay-ит unsafe pending activation.

### Windowing

- Snap left/right/top/bottom layouts.
- Min size не ломает navigation/composer.
- Restore placement на том же monitor.
- Restore placement после отключения monitor возвращает окно в visible work area.
- Multi-window call/media flows, если включены, не теряют focus и privacy state.

### DPI/theme/accessibility

- DPI scaling: 100%, 125%, 150%, 200%.
- High Contrast on/off.
- Transparency off.
- Text scaling.
- Reduce motion.
- Keyboard-only login/chat/send/settings.
- Narrator smoke для login, chat list, conversation, composer, calls и settings.

### Windows integration

- MSIX install.
- MSIX upgrade with local data migration.
- Uninstall/reinstall behavior documented.
- Taskbar unread badge set/clear.
- Jump List, если реализован.
- Tray icon, если реализован и approved ADR существует.
- File picker/save picker, drag/drop и clipboard paste.

## Матрица visual QA

Visual QA выполняется по [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md). Нельзя ставить `QA PASS` визуальному слою только потому, что controls видны: нужно проверить palette, density, layout, glass source/overlay и fallback.

### Android-референсы

- Темный список чатов: `F:\APSApp_project\QA\tmp\qa\app-performance-benchmark-2026-05-26-run01\phone-current-screen.png`
- Темные настройки: `F:\APSApp_project\QA\tmp\qa\appearance-optimization-visual-2026-05-26-run03\5554-settings-home.png`
- Светлая appearance-поверхность: `F:\APSApp_project\QA\tmp\qa\appearance-optimization-visual-2026-05-26-run03\5554-appearance-light.png`
- Темный direct chat: `F:\APSApp_project\APSApp_android_app\QA\tmp\reply-author-dark-2026-05-19\direct-chat.png`
- Темные comments/media: `F:\APSApp_project\APSApp_android_app\QA\tmp\post-comments-reply-2026-05-19-run01\24-after-data-fix-comments-screen.png`

### Обязательные visual passes

- Windows 11 dark: shell, chat list, direct chat, comments/media, settings, calls.
- Windows 11 light: shell, chat list, direct chat, settings.
- Windows 10 dark/light fallback: same IA без Mica dependency.
- Transparency off: boundaries, badges, composer, context strip и buttons остаются читаемыми.
- High Contrast: system colors, focus states, readable text, no color-only status.
- DPI 100%, 125%, 150%, 200%.
- Snap left/right и 1024/1366/1920 widths.
- RU/EN text fit для headers, buttons, rows, composer banners.

### Glass-specific checks

- Header и composer блюрят реальный scrolled content/source, а не пустой фон.
- Settings header/bottom dock блюрят real rows при scroll.
- Внутри blur source нет nested live blur; используются static frosted fills.
- Live blur radius не выше 34 DIP.
- При выключенной transparency используется static frosted fill из design tokens.
- Нет Telegram copy: цвета, row layout, header composition и icons не совпадают 1:1 с Telegram.

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
