# Android to Windows Feature Parity

Updated: 2026-05-30

Source: `APSApp_android_app/docs/APP_FEATURE_INVENTORY.md` plus messenger/network/security docs.

Status labels:

- `P0` - required for first Windows beta.
- `P1` - required before broad beta or public release.
- `P2` - later enhancement.
- `Windows-specific` - replace Android behavior with desktop equivalent.
- `Out` - intentionally not in Windows scope.

## 1. App Shell and System Behavior

| Android runtime surface | Windows decision |
|---|---|
| Splash/bootstrap | P0: native startup/bootstrap state |
| Authenticated shell with Chats, Calls, Settings | P0: same top-level IA |
| Horizontal swipe between tabs | Windows-specific: navigation rail/list keyboard, no swipe dependency |
| Bottom navigation visibility by route | Windows-specific: adaptive panes and nav rail |
| Theme system/light/dark | P0 |
| Reduce motion | P0 |
| FLAG_SECURE screenshot privacy | P1: implement best Windows equivalent; document limits |
| Auto-lock after background | P0: lock after idle/background policy |
| PIN unlock | P0 |
| Biometric unlock | P1: Windows Hello |
| Notification permission | Windows-specific: Windows notification availability/settings |
| Battery/autostart guidance | Windows-specific/P1 only if real Windows background setting is needed |
| FCM runtime/token sync | Windows-specific: no FCM; WNS or reconnect/sync, metadata-only |
| OTA worker | Out for Windows; replaced by MSIX/update channel |

## 2. Auth and Onboarding

| Android runtime surface | Windows decision |
|---|---|
| Identifier/password login | P0 |
| Privacy policy link | P0 |
| Create account | P1 unless owner requires P0 |
| Email registration | P1 |
| Password recovery | P0 entry, P1 full live flow if backend contract needs desktop adjustments |
| 2FA challenge | P0 |
| TOTP code | P0/P1 depending owner manual scope |
| Backup code | P0 |
| OAuth providers Google/Yandex | P1 after desktop provider flow decision |

## 3. Messenger Activation

| Android runtime surface | Windows decision |
|---|---|
| First activation screen | P0 |
| Nickname entry | P0 |
| Activation loading/error/blocking states | P0 |
| Runtime connection footer | P1 or diagnostics-only |
| Auto-transition to chat list | P0 |

## 4. Chat List, Discovery and Navigation

| Android runtime surface | Windows decision |
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
| Pin/mute/rename/add/profile/delete | P0/P1 by backend support |
| Custom tabs management | P1 |
| Forward recipient picker | P0 |
| Direct-source forward warnings/privacy switch | P0 |

## 5. Dialog, Timeline, Composer, Message Actions

| Android runtime surface | Windows decision |
|---|---|
| Direct/group/channel/comment dialogs | P0 direct/group/channel, P1 comments if not ready in P0 |
| Read-only public channel | P0 |
| Header navigation/info/status/typing | P0 |
| Header call actions | P0 for direct calls |
| Notification presets | P1 |
| In-dialog search | P1 |
| Timeline virtualization | P0 |
| Empty/loading states | P0 |
| Text/reply/edited/status rendering | P0 |
| Image/GIF/video/audio/voice/PDF/file rendering | P0, voice waveform can be P1 if backend/media data incomplete |
| Comments entry | P1 |
| Composer text/reply/edit/cancel | P0 |
| Single attachment | P0 |
| Multi-attachment up to 10 drafts | P0 |
| Mixed multi-attach | P0/P1 depending transport verification |
| Send as original | P0 |
| Message actions copy/forward/delete | P0 basic, P1 full matrix |
| Reactions | Out/P2 until Android marks as release gate |

## 6. Groups, Channels, Topics, Members, Settings

| Android runtime surface | Windows decision |
|---|---|
| Create group | P0 |
| Create channel | P0 |
| Group/channel info | P0 |
| Topics/forums | Out unless backend/product scope revives |
| Manage members/subscribers | P1 |
| Promote/restrict/activate/remove/ban | P1 |
| Structured chat settings | P1 |
| Per-chat user prefs pin/mute | P0/P1: P0 local prefs, P1 server-backed matrix |
| Public/private channel visibility | P1 |
| Comments policy | P1 |

## 7. Peer Profile and Contact Relations

| Android runtime surface | Windows decision |
|---|---|
| Peer profile | P0 basic |
| Contact add/delete/alias | P0 add/search, P1 full alias/delete |
| Media grid | P2 |
| Fullscreen peer media | P2 |
| Report/block | P1 |

## 8. Calls

| Android runtime surface | Windows decision |
|---|---|
| Calls tab/history | P0 |
| Contacts/callable list | P0 |
| Audio call | P0 |
| Video call | P0 |
| Accept/decline/cancel/hangup | P0 |
| Mute/camera controls | P0 |
| Device selection | P1 |
| Missed/declined/cancelled history | P0 |
| Notification actions | P0/P1 depending Windows packaging state |
| Connection quality labels/errors | P1 |

## 9. Settings and Account

| Android runtime surface | Windows decision |
|---|---|
| Settings home/routes | P0 |
| Account screen | P0 |
| My profile display/about/avatar | P0/P1 avatar editor can be simpler in P0 |
| Sign-in methods | P1 |
| Change password | P1 unless owner requires P0 |
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
| Active session | P0 current device only |
| About/privacy policy | P0 |
| Feedback with screenshots/logs | P1 |

## 10. Updates

| Android runtime surface | Windows decision |
|---|---|
| User OTA screen | Out; replaced by Windows update channel |
| Developer update screen | Out for P0; maybe dev diagnostics later |
| Store installer gates | Windows-specific: MSIX/store channel policy |

## 11. Partial / Placeholder / Non-Gate

| Android runtime surface | Windows decision |
|---|---|
| Reactions not active release gate | Out/P2 |
| Chat preferences placeholder | Out until product contract exists |
| Admin route no access | Out for consumer Windows client P0 |
| VPN stubs | Out |
| Android removed browser/notes modules | Out |

## Parity Rule

Windows may improve layout and desktop ergonomics, but it must not weaken:

- auth security;
- direct E2EE;
- notification privacy;
- storage v1 media contract;
- group/channel ACL behavior;
- no-raw-technical-id display policy;
- QA evidence discipline.

