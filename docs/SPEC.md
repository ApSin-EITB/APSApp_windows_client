# Spec: APSApp Windows Client

Updated: 2026-05-30
Status: initial source-of-truth specification for repository bootstrap.

## Objective

Build a native Windows desktop client for APSApp / "Личка" that gives users a full desktop companion to the Android app:

- account login, registration-adjacent auth flows, local lock, profile and settings;
- direct 1x1 encrypted messenger;
- groups, channels, comments, contacts, search, forwarding, moderation-aware structured chat surfaces;
- media upload/download/playback using the storage v1 contract;
- audio/video calls through the calls runtime;
- Windows-native notifications, privacy controls, packaging, update policy, diagnostics and QA evidence.

Success means a Windows user can use the same APSApp account and chat/calls runtime as Android without weakening security, E2EE, media privacy, auth, or backend contracts.

## Scope

### P0 Desktop Beta Scope

- Windows-native shell with three top-level surfaces: chats, calls, settings.
- Login by identifier/password, token refresh, logout, current-user session restore.
- Two-factor challenge support for login: TOTP and backup code entry.
- Existing-account desktop onboarding into messenger runtime.
- Direct chats: list, open, send/receive text, sync, edit, delete/hide as supported by backend.
- Direct E2EE compatible with Android current direct-chat contract.
- Contact discovery/search/add and contact request handling.
- Group/channel read/write surfaces for current user permissions.
- Basic group/channel creation and info screens.
- Comments for channel posts where backend/runtime supports them.
- Media attachments: image, gif, video, audio, voice, pdf, generic file.
- Storage v1 upload/bind/grant/stream path.
- Calls: direct audio/video call accept/decline/hangup, call history, incoming call notification.
- Settings: account, profile, appearance, security/privacy, notifications, network/data, active session, about, feedback.
- Local encrypted persistence, cache limits, restart/resume behavior.
- Russian and English UI.
- Windows 10 22H2 and Windows 11 support on x64.

### P1 Scope

- Full group/channel role management parity: promote, restrict, activate, remove/ban where backend allows.
- Advanced media reliability: cache loss repair, descriptor refresh, re-grant/re-stream retry matrix.
- Rich notification actions for messages and calls.
- Multi-device direct E2EE restore parity and user-data backup restore UX.
- Performance instrumentation and desktop macro scenarios.
- MSIX installer channel and signed release pipeline.

### P2 Scope

- Store distribution if owner chooses Microsoft Store.
- ARM64 build.
- Deep Windows Share Target / file association integration.
- Optional native C++ acceleration beyond required media/crypto islands.
- Qt reconsideration only if a cross-platform desktop roadmap appears.

### Out of Scope for Initial Windows Beta

- Android-only battery optimization screens.
- Android OTA APK update worker behavior.
- Direct access to Prometheus, Loki, Alertmanager, Grafana, or backend-internal tables.
- Query-string bearer tokens for WebSocket or HTTP.
- Cloud push payloads with plaintext message content.
- Qt primary UI shell.

## Tech Stack

Primary:

- Language: C#.
- UI: WinUI 3 on Windows App SDK.
- Runtime: current .NET LTS.
- Architecture: MVVM + repository + service clients.
- Packaging: MSIX first, unpackaged dev run allowed only for local development.
- Local persistence: SQLite for relational app state; SQLCipher or app-level encrypted payload strategy for sensitive chat data; DPAPI/Credential Locker for key material.
- HTTP/WS: `HttpClient` / `ClientWebSocket` with typed clients and strict auth interceptors.
- Serialization: `System.Text.Json` with source generation once models stabilize.
- Logging: `Microsoft.Extensions.Logging` with local redaction.
- DI: `Microsoft.Extensions.DependencyInjection`.
- Test: xUnit or NUnit for unit/integration, WinUI UI automation for UI, native tests for C/C++ islands.

Native islands:

- C/C++ permitted for media/WebRTC, crypto bindings, performance hot paths, and Windows interop that is materially safer or more reliable outside managed code.
- Native APIs must be wrapped behind narrow C# interfaces and tested independently.
- Native code must not own product policy or backend contract interpretation.

Qt:

- Not primary stack.
- May be revisited by a new ADR only if the product goal changes to cross-platform desktop.

## Commands

Mandatory once the solution skeleton exists:

```powershell
dotnet restore .\APSApp.Windows.sln
dotnet build .\APSApp.Windows.sln -c Debug
dotnet test .\APSApp.Windows.sln -c Debug
dotnet format .\APSApp.Windows.sln --verify-no-changes
```

Native island commands once native modules exist:

```powershell
cmake --preset windows-x64-debug
cmake --build --preset windows-x64-debug
ctest --preset windows-x64-debug --output-on-failure
```

Release/package commands once packaging exists:

```powershell
msbuild .\APSApp.Windows.sln /m /restore /p:Configuration=Release /p:Platform=x64
```

## Project Structure

Target structure:

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

## Code Style

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

Rules:

- Async methods end with `Async`.
- Domain state is explicit: no magic strings for message status, chat type, media kind or auth state.
- User-facing text comes from resources.
- Network DTOs are separate from domain models.
- UI view models do not parse backend payloads directly.
- Native functions expose narrow, versioned, exception-free ABI boundaries.

## Product Requirements

### App Shell

- The app starts into a splash/bootstrap state and restores the prior authenticated session when possible.
- Authenticated shell exposes top-level navigation for Chats, Calls and Settings.
- Desktop layout uses a responsive master-detail model:
  - compact width: one pane at a time;
  - normal desktop: chat/call list + detail pane;
  - wide desktop: list + conversation + info/details pane when relevant.
- The shell preserves route state across restart when safe.
- The app supports Russian and English and follows Windows theme by default.

### Auth

- Support identifier/password login.
- Support public auth completion/exchange only through HTTPS app-link compatible flows, not custom query token shortcuts.
- Support 2FA login challenge by TOTP and backup code.
- Support refresh token/session restore using secure local storage.
- Support logout with local token/key cleanup while preserving explicit local cache only when the user chooses.
- Never put bearer tokens into URLs.

### Messenger Activation

- If the account has not activated messenger, show an activation flow equivalent to Android's nickname activation.
- Activation state must be backed by authoritative server response.
- After success, transition to chat list and start chat transport bootstrap.

### Chat List and Discovery

- Show direct chats, groups, channels and local custom organization states supported by backend/client storage.
- Support search across local chats and server-backed people/channel discovery.
- Support unread, muted, pinned, typing and last-message preview indicators.
- Support pending contact requests with accept/reject.
- Do not show raw technical IDs as display names.

### Dialog / Timeline

- Support direct, group, channel and comments timelines.
- Messages must render text, replies, edited markers, status, media cards, failure states, and upload/send progress.
- Timeline must not flash previous chat content while switching conversations.
- Empty states must wait for route loading to settle.
- Read and delivery states are monotonic and follow backend contract.

### Composer

- Text send, reply, edit, cancel reply/edit.
- Up to 10 attachment drafts per batch.
- Media send supports current Android kinds: image, gif, video, audio, voice, pdf, file.
- Image "send as original" maps to file semantics.
- Outbound message send is allowed only after every attachment slot has a persisted `binding_id` and descriptor-normalized media JSON.

### Forwarding

- Support eligible-target forwarding to direct chats, groups and writable channels.
- Direct-source forwarding must warn that local decrypt/re-encrypt is involved and may increase visibility depending on target.
- Respect user/server privacy policy for forwarding from direct chats.
- Preserve source markers according to backend/runtime contract.

### Structured Chats

- Create group and channel.
- Show info screen, members/subscribers, settings, avatar/title/about where permissions allow.
- Support role-gated actions only when current ACL says they are available.
- Unsupported admin actions must be absent or disabled with a clear non-technical state.

### Calls

- Calls runtime is separate from chat runtime.
- Support incoming/outgoing direct audio/video calls.
- Support accept, decline, cancel, hangup, mute microphone, camera toggle, speaker/device selection where Windows APIs allow.
- Support call history, missed call states and notification actions.
- WebRTC/LiveKit integration may live in native C/C++ if the managed path is insufficient.

### Settings

- Account: current user, profile, sign-in methods entry, password change/reset entry, 2FA entry, account deletion request, logout.
- Profile: display name, about, avatar change/upload/bind.
- Appearance: system/light/dark, language system/RU/EN, reduce motion.
- Security/privacy: local lock, lock timeout, Windows Hello if available, screenshot/privacy mode where Windows supports it, message preview privacy, cache limit/clear.
- Notifications: permission/status, Windows notification settings deep link where available, per-channel app settings.
- Network/data: media download policy, server connectivity check.
- Active session: current device/session only unless backend adds multi-session API.
- About/privacy: version/build, privacy policy.
- Feedback: text, optional screenshot attachment, sanitized logs.

### Updates

- Android OTA behavior does not apply directly.
- Windows beta uses MSIX/manual update policy first.
- Store/channel update behavior is defined in ADR-002.

## Security Requirements

- Token storage uses Windows Credential Locker or DPAPI-protected local secrets.
- Direct 1x1 messages use compatible Signal/libsignal behavior; no server plaintext.
- Group/channel/private comments use current trusted-server `server_encrypted_v1` / `enc=4` / `private_server` model.
- Cloud/push notification payloads must be metadata-only.
- Local logs redact Authorization, cookies, tokens, device keys, backup payloads and message bodies.
- Local database protection is mandatory before beta.
- Crash/feedback reports must sanitize local paths and secrets.

## Testing Strategy

- Unit tests for state machines, DTO mapping, auth refresh, sync, outbox, upload retry and privacy rules.
- Integration tests against dev contour for auth, chat, storage and calls contracts.
- UI automation for core routes and resize/adaptive layout.
- Native tests for C/C++ modules.
- Manual QA matrix must cover fresh install, upgrade, restart, reconnect, cache loss, dark/light, RU/EN, Windows 10/11.
- Security QA must verify notification privacy and no bearer tokens in URL.

## Boundaries

Always:

- Follow upstream backend contracts.
- Use typed DTOs and explicit state models.
- Keep tokens out of URLs and logs.
- Keep Android parity claims traceable to source docs or QA evidence.
- Write ADR before changing primary stack, local crypto/storage model, or packaging channel.

Ask first:

- Switching primary UI stack to Qt or C++/WinRT.
- Adding a new backend endpoint requirement.
- Changing E2EE behavior.
- Changing notification cloud provider or push payload semantics.
- Persisting additional sensitive data locally.
- Introducing broad native dependencies.

Never:

- Commit secrets.
- Put plaintext message content in push/cloud notification payloads.
- Treat backend-internal DB tables as client contract.
- Store bearer tokens in query strings.
- Disable TLS validation to make dev easier.
- Replace direct E2EE with server plaintext.

## Success Criteria

- Repository has source-of-truth specs and ADRs.
- First code skeleton can be created without re-deciding stack, module boundaries, security model or QA gates.
- Every P0 feature has an owner doc section and a test strategy.
- Every upstream contract has a named source document.
- Qt/C++ are addressed explicitly instead of being left as future ambiguity.

## Open Questions

- Will Windows beta be private sideload MSIX only, or also Microsoft Store?
- Which Windows push path is preferred for packaged builds: WNS, foreground/reconnect-only notifications, or server-managed desktop push later?
- Should Windows support ARM64 in the first public beta or after x64 stabilizes?
- Which owner account/provider flows are required live in first QA: Google/Yandex OAuth on desktop, or identifier/password plus 2FA first?

