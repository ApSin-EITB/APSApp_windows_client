# Спецификация: APSApp Windows Client

Обновлено: 2026-05-30
Статус: начальная source-of-truth спецификация для bootstrap репозитория.

## Цель

Создать нативный Windows desktop client для APSApp / «Личка», который дает пользователю полноценный desktop companion к Android-приложению:

- вход в аккаунт, auth-adjacent flows, local lock, профиль и настройки;
- direct 1x1 encrypted messenger;
- группы, каналы, комментарии, контакты, поиск, forwarding и moderation-aware structured chat surfaces;
- media upload/download/playback через storage v1 contract;
- audio/video calls через calls runtime;
- Windows-native notifications, privacy controls, packaging, update policy, diagnostics и QA evidence.

Успех означает, что Windows user может использовать тот же APSApp account и chat/calls runtime, что и Android, без ослабления security, E2EE, media privacy, auth или backend contracts.

## Scope

### P0 Desktop Beta Scope

- Windows-native shell с тремя top-level surfaces: chats, calls, settings.
- Дизайн-система Windows: Telegram-like desktop ergonomics без копирования Telegram, Android palette, cloudy/frosted glass и визуальные QA-референсы.
- Вход по identifier/password, token refresh, logout и восстановление current-user session.
- Two-factor challenge support для login: TOTP и backup code entry.
- Desktop onboarding существующего аккаунта в messenger runtime.
- Direct chats: список, открытие, send/receive text, sync, edit, delete/hide там, где это supported by backend.
- Direct E2EE совместимый с Android current direct-chat contract.
- Contact discovery/search/add и contact request handling.
- Group/channel read/write surfaces с учетом current user permissions.
- Basic group/channel creation и info screens.
- Comments for channel posts там, где backend/runtime их поддерживает.
- Media attachments: image, gif, video, audio, voice, pdf, generic file.
- Storage v1 path: upload/bind/grant/stream.
- Calls: direct audio/video call, accept/decline/hangup, call history и incoming call notification.
- Settings: account, profile, appearance, security/privacy, notifications, network/data, active session, about и feedback.
- Local encrypted persistence, cache limits и restart/resume behavior.
- Russian и English UI.
- Windows 11 current supported releases как primary x64 target и Windows 10 22H2 как compatibility target.

### Windows 10/11 support tiers

Подробная platform-native спецификация живет в [WINDOWS_NATIVE_SPEC.md](WINDOWS_NATIVE_SPEC.md).

| Tier | OS | Scope |
|---|---|---|
| Tier A | Windows 11 current supported releases, x64 | Primary beta/release target: полный Windows-native UX, MSIX install/upgrade, notifications, taskbar badge, accessibility, DPI и multi-monitor QA. |
| Tier B | Windows 10 22H2 x64 | Compatibility target: core auth/messenger/media/calls работают, Windows 11-only visual/system features имеют fallback или `QA PARTIAL`. |
| Future | Windows 11 ARM64 | P2 после стабилизации x64. |

На дату 2026-05-30 Windows 10 Home/Pro уже вышла из обычного Microsoft support lifecycle 2025-10-14. Поэтому Windows 10 не является равным production baseline: поддерживаем ее осознанно для compatibility/owner QA, но Windows 11 остается основной нативной целью.

### P1 Scope

- Full group/channel role management parity: promote, restrict, activate, remove/ban там, где backend allows.
- Advanced media reliability: cache loss repair, descriptor refresh и re-grant/re-stream retry matrix.
- Rich notification actions для messages и calls.
- Multi-device direct E2EE restore parity и user-data backup restore UX.
- Performance instrumentation и desktop macro scenarios.
- MSIX installer channel и signed release pipeline.

### P2 Scope

- Store distribution, если владелец выбирает Microsoft Store.
- ARM64 build.
- Deep Windows Share Target / file association integration.
- Optional native C++ acceleration beyond required media/crypto islands.
- Qt reconsideration только если появится cross-platform desktop roadmap.

### Out of Scope для initial Windows beta

- Android-only battery optimization screens.
- Android OTA APK update worker behavior.
- Direct access to Prometheus, Loki, Alertmanager, Grafana или backend-internal tables.
- Query-string bearer tokens для WebSocket или HTTP.
- Cloud push payloads with plaintext message content.
- Qt primary UI shell.

## Tech stack

Primary:

- Language: C#.
- UI: WinUI 3 on Windows App SDK.
- Windows App SDK: только stable channel; Preview/Experimental APIs запрещены без ADR.
- Runtime: current .NET LTS.
- Architecture: MVVM + repository + service clients.
- Packaging: MSIX first; unpackaged dev run разрешен только для local development.
- Local persistence: SQLite для relational app state; SQLCipher или app-level encrypted payload strategy для sensitive chat data; DPAPI/Credential Locker для key material.
- HTTP/WS: `HttpClient` / `ClientWebSocket` с typed clients и strict auth interceptors.
- Serialization: `System.Text.Json` with source generation после стабилизации models.
- Logging: `Microsoft.Extensions.Logging` with local redaction.
- DI: `Microsoft.Extensions.DependencyInjection`.
- Test: xUnit или NUnit для unit/integration, WinUI UI automation для UI, native tests для C/C++ islands.

Native islands:

- C/C++ разрешены для media/WebRTC, crypto bindings, performance hot paths и Windows interop, где это реально безопаснее или надежнее managed-кода.
- Native APIs должны быть wrapped behind narrow C# interfaces и tested independently.
- Native code не должен владеть product policy или interpretation backend contracts.

Qt:

- Не primary stack.
- Можно вернуться новым ADR только если product goal изменится на cross-platform desktop.

## Commands

Обязательные команды после появления solution skeleton:

```powershell
dotnet restore .\APSApp.Windows.sln
dotnet build .\APSApp.Windows.sln -c Debug
dotnet test .\APSApp.Windows.sln -c Debug
dotnet format .\APSApp.Windows.sln --verify-no-changes
```

Native island commands после появления native modules:

```powershell
cmake --preset windows-x64-debug
cmake --build --preset windows-x64-debug
ctest --preset windows-x64-debug --output-on-failure
```

Release/package commands после появления packaging:

```powershell
msbuild .\APSApp.Windows.sln /m /restore /p:Configuration=Release /p:Platform=x64
```

## Project structure

Целевая структура:

```text
src/
  APSApp.Windows/              WinUI 3 app, shell, views, resources, app lifecycle
  APSApp.Client.Core/          Domain models, use cases, state machines, interfaces
  APSApp.Client.Infrastructure/HTTP, WebSocket, storage, sync, logging, OS adapters
  APSApp.Client.Messenger/     Chat domain, E2EE facade, media, outbox, repository
  APSApp.Client.Calls/         Calls UI/domain and LiveKit/WebRTC bridge facade
  APSApp.Client.Native/        Optional C/C++ native island boundary
tests/
  APSApp.Client.Core.Tests/
  APSApp.Client.Infrastructure.Tests/
  APSApp.Client.Messenger.Tests/
  APSApp.Windows.UiTests/
native/
  aps_native/                  Native C/C++ code, only after ADR-backed need
docs/
  *.md
  decisions/
tools/
  verify-docs.ps1
  qa/
```

## Code style

C#:

```csharp
public sealed partial class ChatListViewModel : ObservableObject
{
    private readonly IChatRepository _chatRepository;
    private readonly ILogger<ChatListViewModel> _logger;

    [ObservableProperty]
    private AsyncState<IReadOnlyList<ChatPreview>> chats = AsyncState<IReadOnlyList<ChatPreview>>.Idle();

    public ChatListViewModel(IChatRepository chatRepository, ILogger<ChatListViewModel> logger)
    {
        _chatRepository = chatRepository;
        _logger = logger;
    }

    public async Task RefreshAsync(CancellationToken cancellationToken)
    {
        Chats = AsyncState<IReadOnlyList<ChatPreview>>.Loading();

        try
        {
            var snapshot = await _chatRepository.GetChatSnapshotAsync(cancellationToken).ConfigureAwait(false);
            Chats = AsyncState<IReadOnlyList<ChatPreview>>.Ready(snapshot);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "Chat list refresh failed");
            Chats = AsyncState<IReadOnlyList<ChatPreview>>.Failed(UserFacingError.NetworkUnavailable);
        }
    }
}
```

Native C/C++:

```cpp
extern "C" __declspec(dllexport) int ApsNativeVersion() noexcept
{
    return 1;
}
```

Правила:

- Async methods заканчиваются на `Async`.
- Domain state явный: без magic strings для message status, chat type, media kind или auth state.
- User-facing text берется из resources.
- Network DTOs отделены от domain models.
- UI view models не parse-ят backend payloads напрямую.
- Native functions expose narrow, versioned, exception-free ABI boundaries.

## Product requirements

### App shell

- App стартует в splash/bootstrap state и восстанавливает prior authenticated session, когда это возможно.
- Authenticated shell показывает top-level navigation для Chats, Calls и Settings.
- App является single-instance: повторный launch, notification/protocol/file activation маршрутизируются в существующий process через Windows activation router.
- Desktop layout использует responsive master-detail model:
  - compact width: один pane;
- normal desktop: chat/call list + detail pane;
- wide desktop: list + conversation + info/details pane where relevant.
- Desktop layout следует [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md): APS rail, list pane, conversation/call/settings stage и optional context pane. Telegram допускается только как ergonomic reference, не как визуальный шаблон.
- Цвета, glass surfaces, radius, typography и visual states берутся из design token layer, основанного на Android `Color.kt` и `MessengerThemeTokens.kt`.
- Shell сохраняет route state across restart when safe.
- App поддерживает русский и английский, по умолчанию следует Windows theme.
- Windows 11 использует нативные visual materials where appropriate; Windows 10 получает documented fallback без потери core functionality.

### Auth

- Поддержать identifier/password login.
- Поддержать public auth completion/exchange только через HTTPS app-link compatible flows, без custom query token shortcuts.
- Поддержать 2FA login challenge через TOTP и backup code.
- Поддержать refresh token/session restore через secure local storage.
- Поддержать logout с cleanup локальных tokens/keys; local cache сохраняется только по явному выбору пользователя.
- Никогда не класть bearer tokens в URLs.

### Messenger activation

- Если account не активировал messenger, показать activation flow, эквивалентный Android nickname activation.
- Activation state должен подтверждаться authoritative server response.
- После success перейти к chat list и запустить chat transport bootstrap.

### Chat list и discovery

- Показывать direct chats, groups, channels и local custom organization states, которые поддержаны backend/client storage.
- Поддержать search across local chats и server-backed people/channel discovery.
- Поддержать unread, muted, pinned, typing и last-message preview indicators.
- Поддержать pending contact requests with accept/reject.
- Не показывать raw technical IDs как display names.

### Dialog / timeline

- Поддержать direct, group, channel и comments timelines.
- Messages должны render-ить text, replies, edited markers, status, media cards, failure states и upload/send progress.
- Timeline не должен flash-ить previous chat content при switching conversations.
- Empty states должны ждать route loading settle.
- Read и delivery states monotonic и следуют backend contract.

### Composer

- Text send, reply, edit и cancel reply/edit.
- До 10 attachment drafts per batch.
- Media send поддерживает Android kinds: image, gif, video, audio, voice, pdf, file.
- Image `send as original` maps to file semantics.
- Outbound message send разрешен только после того, как каждый attachment slot имеет persisted `binding_id` и descriptor-normalized media JSON.

### Forwarding

- Поддержать eligible-target forwarding в direct chats, groups и writable channels.
- Direct-source forwarding должен предупреждать, что участвует local decrypt/re-encrypt и что visibility может увеличиться в зависимости от target.
- Уважать user/server privacy policy для forwarding from direct chats.
- Preserve source markers согласно backend/runtime contract.

### Structured chats

- Create group и channel.
- Показывать info screen, members/subscribers, settings, avatar/title/about where permissions allow.
- Role-gated actions только когда current ACL says available.
- Unsupported admin actions должны быть absent или disabled с понятным non-technical state.

### Calls

- Calls runtime отделен от chat runtime.
- Поддержать incoming/outgoing direct audio/video calls.
- Поддержать accept, decline, cancel, hangup, mute microphone, camera toggle, speaker/device selection where Windows APIs allow.
- Поддержать call history, missed call states и notification actions.
- WebRTC/LiveKit integration может жить в native C/C++, если managed path insufficient.

### Settings

- Account: current user, profile, sign-in methods entry, password change/reset entry, 2FA entry, account deletion request и logout.
- Profile: display name, about, avatar change/upload/bind.
- Appearance: system/light/dark, language system/RU/EN, reduce motion.
- Security/privacy: local lock, lock timeout, Windows Hello if available, screenshot/privacy mode where Windows supports it, message preview privacy и cache limit/clear.
- Notifications: permission/status, Windows notification settings deep link where available и per-channel app settings.
- Network/data: media download policy и server connectivity check.
- Active session: current device/session only, если backend не добавит multi-session API.
- About/privacy: version/build и privacy policy.
- Feedback: text, optional screenshot attachment и sanitized logs.

### Updates

- Android OTA behavior напрямую не применяется.
- Windows beta first использует MSIX/manual update policy.
- Store/channel update behavior определен в ADR-002.

## Security requirements

- Token storage использует Windows Credential Locker или DPAPI-protected local secrets.
- Direct 1x1 messages используют compatible Signal/libsignal behavior; no server plaintext.
- Group/channel/private comments используют текущую trusted-server `server_encrypted_v1` / `enc=4` / `private_server` model.
- Cloud/push notification payloads должны быть metadata-only.
- Local logs redact-ят Authorization, cookies, tokens, device keys, backup payloads и message bodies.
- Local database protection обязательна before beta.
- Crash/feedback reports sanitize local paths и secrets.

## Testing strategy

- Unit tests для state machines, DTO mapping, auth refresh, sync, outbox, upload retry и privacy rules.
- Integration tests against dev contour для auth, chat, storage и calls contracts.
- UI automation для core routes и resize/adaptive layout.
- Native tests для C/C++ modules.
- Manual QA matrix покрывает fresh install, upgrade, restart, reconnect, cache loss, dark/light, RU/EN, Windows 10/11.
- Visual QA сверяет Windows dark/light shell, chat list, direct chat, comments/media, settings и calls с Android QA references из `DESIGN_SYSTEM.md`.
- Windows-native QA отдельно покрывает Windows 11 Tier A и Windows 10 22H2 Tier B, включая notifications, taskbar badge, DPI, High Contrast, Narrator, keyboard-only, Snap Layouts и multi-monitor.
- Security QA проверяет notification privacy и отсутствие bearer tokens in URL.

## Boundaries

Always:

- Следовать upstream backend contracts.
- Использовать typed DTOs и explicit state models.
- Держать tokens вне URLs и logs.
- Держать Android parity claims traceable to source docs or QA evidence.
- Писать ADR перед сменой primary stack, local crypto/storage model или packaging channel.

Ask first:

- Switching primary UI stack to Qt или C++/WinRT.
- Adding a new backend endpoint requirement.
- Changing E2EE behavior.
- Changing notification cloud provider или push payload semantics.
- Persisting additional sensitive data locally.
- Introducing broad native dependencies.

Never:

- Commit secrets.
- Не класть plaintext message content в push/cloud notification payloads.
- Не трактовать backend-internal DB tables как client contract.
- Не хранить bearer tokens в query strings.
- Не отключать TLS validation для упрощения dev.
- Не заменять direct E2EE на server plaintext.

## Success criteria

- Repository имеет source-of-truth specs и ADRs.
- First code skeleton можно создать без повторного решения stack, module boundaries, security model или QA gates.
- Windows 10/11 support tiers зафиксированы, а Windows-native behavior вынесен в отдельную спецификацию.
- Дизайн-система зафиксирована: layout, Android palette, glass rules и визуальные QA-референсы вынесены в `DESIGN_SYSTEM.md`.
- У каждой P0 feature есть owner doc section и test strategy.
- Каждый upstream contract имеет named source document.
- Qt/C++ рассмотрены явно, а не оставлены будущей ambiguity.

## Open questions

- Windows beta будет private sideload MSIX only или также Microsoft Store?
- Какой Windows push path предпочтителен для packaged builds: WNS, foreground/reconnect-only notifications или server-managed desktop push later?
- Нужно ли Windows поддерживать ARM64 в первой public beta или после стабилизации x64?
- Какие owner account/provider flows обязательны live в первой QA: Google/Yandex OAuth на desktop или сначала identifier/password plus 2FA?
