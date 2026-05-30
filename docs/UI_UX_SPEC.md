# UI / UX Specification

Updated: 2026-05-30

## Design Direction

The Windows client should feel like a serious native desktop messenger, not a stretched phone app. It should keep APSApp's product model while using desktop strengths: persistent panes, keyboard navigation, hover/context menus, drag-and-drop attachments, native notifications and resize-aware layout.

## Top-Level IA

Top-level surfaces:

- Chats
- Calls
- Settings

Desktop layout:

| Width | Layout |
|---|---|
| Compact | one pane, route stack |
| Normal | list pane + detail pane |
| Wide | list pane + conversation/call detail + info/settings side pane |

## App Bootstrap

States:

- cold start;
- loading local session;
- locked local session;
- unauthenticated;
- authenticated shell;
- fatal local storage/config error.

Rules:

- Do not flash authenticated content before local lock validation.
- Do not show stale route content before current account/session is confirmed.
- Startup errors use localized, non-technical text with diagnostics available in logs.

## Auth Screens

Required:

- login identifier/password;
- create-account entry if backend/owner wants desktop registration live;
- password recovery entry;
- 2FA challenge screen;
- backup-code mode switch;
- privacy policy link.

P0 can prioritize existing-account login plus 2FA if provider/OAuth desktop flows need separate owner validation.

## Chats

### Chat List

Required states:

- loading;
- empty;
- offline with cached data;
- authenticated but messenger inactive;
- loaded with unread/pin/mute/typing metadata;
- search mode;
- contact requests visible when present.

Actions:

- open chat;
- search chats/people/channels;
- create group/channel;
- accept/reject contact request;
- context menu: pin, mute presets, rename/local alias where allowed, add contact, profile, delete/hide, tab assignment where implemented.

### Search / Discovery

Tabs:

- All
- People
- Channels

Rules:

- Raw numeric IDs are not a user-facing add/search mechanism.
- Results show display name, nickname/handle where safe, avatar, relation state.
- Opening a result respects backend access.

### Conversation

Header:

- back/close in compact mode;
- avatar;
- title;
- status/typing/online/channel state;
- call actions when direct chat supports them;
- overflow for notifications, search, delete/hide, info.

Timeline:

- text;
- reply preview;
- edited marker;
- status;
- image;
- animated GIF inline;
- video preview card + fullscreen player;
- audio/voice inline player with progress/waveform where available;
- PDF/file card;
- upload/send progress;
- failed send/retry state;
- read-only channel/comment states.

Rules:

- Switching chats clears previous route rows before target data binds.
- Empty state appears only after route load settles.
- Timeline virtualization is mandatory.
- Context actions must respect message ownership, chat type and permissions.

### Composer

Required:

- text input;
- reply/edit banner;
- attach button;
- drag-and-drop file attachment;
- paste image/file support where safe;
- attachment draft strip;
- send/cancel actions;
- max 10 attachment drafts;
- send-as-original for still images.

Rules:

- Multi-attachment send waits for all slots to have storage `binding_id`.
- File preparation must not freeze UI.
- Large files show progress and can fail/retry clearly.

## Structured Chats

Screens:

- Create group
- Create channel
- Group/channel info
- Members/subscribers
- Chat settings
- Per-chat user preferences
- Post comments

Role-gated actions:

- show only actions current user can execute;
- avoid presenting backend-denied actions as normal buttons;
- when permissions change live, update UI after authoritative sync.

## Calls

Surfaces:

- calls list/history;
- contacts/callable people tab if contract supports it;
- incoming call window/notification;
- active call window;
- compact in-call controls;
- call detail/history row.

Controls:

- accept/decline/cancel/hangup;
- microphone mute;
- camera on/off;
- speaker/device selection;
- video layout with local/remote tiles;
- network/connection state.

Rules:

- Incoming call must be actionable even if main window is minimized.
- Media connect failures are recoverable when possible.
- Call history refreshes after call end/missed/declined state.

## Settings

Sections:

- Account
- My profile
- Sign-in methods
- Password and 2FA
- Appearance
- Security and privacy
- Notifications
- Network and data
- Active session
- About/privacy
- Feedback

Windows-only interpretation:

- Battery optimization screen is omitted or replaced by Windows startup/background permission diagnostics only if a real Windows setting exists.
- Screenshot privacy uses available Windows APIs/policies; if exact Android `FLAG_SECURE` parity is impossible, the UI must be honest and the security doc updated.
- Windows Hello can be used as local unlock helper when available.

## Notifications

Channels/categories:

- chats;
- calls;
- system;
- updates if Windows update channel exists.

Rules:

- Message content preview obeys user setting.
- Metadata-only cloud push boundary is mandatory.
- Notification click opens the correct route after session/local-lock handling.
- If locked, route intent is stored and resumed after unlock.

## Accessibility

- Full keyboard navigation for top-level surfaces and chat list.
- Visible focus states.
- Screen-reader names for icon buttons.
- High contrast and text scaling support.
- Reduce motion respected globally.
- No color-only status communication.

## Localization

Languages:

- Russian
- English
- System default mode

Rules:

- All user text in resources.
- No developer/debug raw codes in user-facing UI.
- Error copy is localized and non-technical.

