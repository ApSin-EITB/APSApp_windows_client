# Карта parity Android -> Windows

Обновлено: 2026-05-30

Источник: `APSApp_android_app/docs/APP_FEATURE_INVENTORY.md` плюс messenger/network/security docs.

Статусы:

- `P0` - обязательно для первой Windows beta.
- `P1` - обязательно до широкой beta или public release.
- `P2` - позднее улучшение.
- `Windows-specific` - Android-поведение заменяется desktop-эквивалентом.
- `Out` - сознательно вне Windows scope.

## 1. App shell и системное поведение

| Android runtime surface | Решение для Windows |
|---|---|
| Splash/bootstrap | P0: нативное startup/bootstrap state |
| Authenticated shell with Chats, Calls, Settings | P0: та же top-level IA |
| Horizontal swipe between tabs | Windows-specific: navigation rail/list keyboard, без зависимости от swipe |
| Bottom navigation visibility by route | Windows-specific: adaptive panes и nav rail |
| Theme system/light/dark | P0 |
| Reduce motion | P0 |
| FLAG_SECURE screenshot privacy | P1: реализовать лучший Windows equivalent и описать ограничения |
| Auto-lock after background | P0: lock по idle/background policy |
| PIN unlock | P0 |
| Biometric unlock | P1: Windows Hello |
| Notification permission | Windows-specific: Windows notification availability/settings |
| Battery/autostart guidance | Windows-specific/P1 только если нужна реальная Windows background setting |
| FCM runtime/token sync | Windows-specific: без FCM; WNS или reconnect/sync, metadata-only |
| OTA worker | Out для Windows; заменяется MSIX/update channel |

## 2. Auth и onboarding

| Android runtime surface | Решение для Windows |
|---|---|
| Identifier/password login | P0 |
| Privacy policy link | P0 |
| Create account | P1, если владелец не требует P0 |
| Email registration | P1 |
| Password recovery | P0 entry, P1 полный live flow, если backend contract требует desktop adjustments |
| 2FA challenge | P0 |
| TOTP code | P0/P1 в зависимости от owner manual scope |
| Backup code | P0 |
| OAuth providers Google/Yandex | P1 после решения по desktop provider flow |

## 3. Messenger activation

| Android runtime surface | Решение для Windows |
|---|---|
| First activation screen | P0 |
| Nickname entry | P0 |
| Activation loading/error/blocking states | P0 |
| Runtime connection footer | P1 или diagnostics-only |
| Auto-transition to chat list | P0 |

## 4. Chat list, discovery и navigation

| Android runtime surface | Решение для Windows |
|---|---|
| Main chat list | P0 |
| Open chat | P0 |
| Own profile from list | P1 |
| Create sheet/FAB | Windows-specific: command button/menu |
| Create group/channel | P0/P1: P0 basic, P1 full settings |
| Contacts/All/custom tabs | P0 contacts/all, P1 custom tabs |
| Row metadata: typing/unread/mute/pin/preview | P0 |
| Pending contact requests | P0 |
| Search all/people/channels | P0 |
| Highlight matches | P1 |
| Result open | P0 |
| Chat row context menu | P0 |
| Pin/mute/rename/add/profile/delete | P0/P1 по backend support |
| Custom tabs management | P1 |
| Forward recipient picker | P0 |
| Direct-source forward warnings/privacy switch | P0 |

## 5. Dialog, timeline, composer, message actions

| Android runtime surface | Решение для Windows |
|---|---|
| Direct/group/channel/comment dialogs | P0 direct/group/channel, P1 comments если не готовы в P0 |
| Read-only public channel | P0 |
| Header navigation/info/status/typing | P0 |
| Header call actions | P0 для direct calls |
| Notification presets | P1 |
| In-dialog search | P1 |
| Timeline virtualization | P0 |
| Empty/loading states | P0 |
| Text/reply/edited/status rendering | P0 |
| Image/GIF/video/audio/voice/PDF/file rendering | P0; voice waveform можно P1, если backend/media data неполные |
| Comments entry | P1 |
| Composer text/reply/edit/cancel | P0 |
| Single attachment | P0 |
| Multi-attachment up to 10 drafts | P0 |
| Mixed multi-attach | P0/P1 в зависимости от transport verification |
| Send as original | P0 |
| Message actions copy/forward/delete | P0 basic, P1 full matrix |
| Reactions | Out/P2 до тех пор, пока Android не сделает их release gate |

## 6. Groups, channels, topics, members, settings

| Android runtime surface | Решение для Windows |
|---|---|
| Create group | P0 |
| Create channel | P0 |
| Group/channel info | P0 |
| Topics/forums | Out, пока backend/product scope не вернет их |
| Manage members/subscribers | P1 |
| Promote/restrict/activate/remove/ban | P1 |
| Structured chat settings | P1 |
| Per-chat user prefs pin/mute | P0/P1: P0 local prefs, P1 server-backed matrix |
| Public/private channel visibility | P1 |
| Comments policy | P1 |

## 7. Peer profile и contact relations

| Android runtime surface | Решение для Windows |
|---|---|
| Peer profile | P0 basic |
| Contact add/delete/alias | P0 add/search, P1 full alias/delete |
| Media grid | P2 |
| Fullscreen peer media | P2 |
| Report/block | P1 |

## 8. Calls

| Android runtime surface | Решение для Windows |
|---|---|
| Calls tab/history | P0 |
| Contacts/callable list | P0 |
| Audio call | P0 |
| Video call | P0 |
| Accept/decline/cancel/hangup | P0 |
| Mute/camera controls | P0 |
| Device selection | P1 |
| Missed/declined/cancelled history | P0 |
| Notification actions | P0/P1 в зависимости от Windows packaging state |
| Connection quality labels/errors | P1 |

## 9. Settings и account

| Android runtime surface | Решение для Windows |
|---|---|
| Settings home/routes | P0 |
| Account screen | P0 |
| My profile display/about/avatar | P0/P1; avatar editor в P0 может быть проще |
| Sign-in methods | P1 |
| Change password | P1, если владелец не требует P0 |
| Password reset | P0 entry, P1 full flow |
| 2FA settings | P1 |
| Appearance language/theme/reduce motion | P0 |
| PIN/local lock | P0 |
| Windows Hello | P1 |
| Screenshot/privacy | P1 |
| Hide lock-screen message content | P0 |
| Cache stats/limit/clear | P0 |
| Notifications settings | P0 |
| Network/data media policy | P0 |
| Active session | P0: только current device |
| About/privacy policy | P0 |
| Feedback with screenshots/logs | P1 |

## 10. Updates

| Android runtime surface | Решение для Windows |
|---|---|
| User OTA screen | Out; заменяется Windows update channel |
| Developer update screen | Out для P0; возможны dev diagnostics later |
| Store installer gates | Windows-specific: MSIX/store channel policy |

## 11. Partial / placeholder / non-gate

| Android runtime surface | Решение для Windows |
|---|---|
| Reactions not active release gate | Out/P2 |
| Chat preferences placeholder | Out до появления product contract |
| Admin route no access | Out для consumer Windows client P0 |
| VPN stubs | Out |
| Android removed browser/notes modules | Out |

## Правило parity

Windows может улучшать layout и desktop ergonomics, но не должен ослаблять:

- auth security;
- direct E2EE;
- notification privacy;
- storage v1 media contract;
- group/channel ACL behavior;
- no-raw-technical-id display policy;
- QA evidence discipline.
