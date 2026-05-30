# UI / UX спецификация

Обновлено: 2026-05-30

## Направление дизайна

Windows-клиент должен ощущаться как серьезный native desktop messenger, а не как растянутый phone app. Он сохраняет product model APSApp, но использует desktop strengths: persistent panes, keyboard navigation, hover/context menus, drag-and-drop attachments, native notifications и resize-aware layout.

Визуальный контракт, Android-палитра, glass rules и визуальные QA-референсы зафиксированы в [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md). Этот документ описывает поведение экранов; если есть конфликт по цветам, материалам, radius или desktop layout, source-of-truth для визуального слоя - `DESIGN_SYSTEM.md`.

Подробные Windows 10/11 правила: [WINDOWS_NATIVE_SPEC.md](WINDOWS_NATIVE_SPEC.md).

## Windows 10/11 native UX

Windows 11 является основной UX-целью. Windows 10 22H2 поддерживается как compatibility tier с fallback-ами.

| Area | Windows 11 | Windows 10 |
|---|---|---|
| Backdrop | Mica/Mica Alt для long-lived windows where readable | Acrylic или solid theme brush |
| Title bar | Integrated/custom title bar с system caption controls | Standard/custom title bar без Mica dependency |
| Layout | Snap-friendly min sizes, multi-pane desktop first | Та же IA, без Windows 11-only assumptions |
| Notifications | App notifications + badge + activation | То же, если packaged/app identity позволяет; иначе documented fallback |
| Accessibility | Narrator, High Contrast, text scaling, keyboard-only | Та же QA matrix |

Нельзя делать core functionality зависимой от Mica, rounded corners, Snap flyout или других Windows 11-only visual affordances.

## Top-level IA

Top-level surfaces:

- Chats
- Calls
- Settings

Desktop layout:

| Width | Layout |
|---|---|
| Compact | один pane, route stack |
| Normal | list pane + detail pane |
| Wide | list pane + conversation/call detail + info/settings side pane |

Базовая desktop-эргономика может быть знакомой пользователю Telegram-like messenger: rail, список, conversation stage и optional context pane. При этом APSApp не копирует Telegram визуально: rail, context strip, calls/settings surfaces, palette, glass и status/security zones следуют [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md).

## Визуальная идентичность

- Палитра берется из Android `Color.kt` и `MessengerThemeTokens.kt`.
- Dark theme строится на `#061315`, `#0B1E22`, `#102A2F`, `#14B9B9`, `#35C8C5`.
- Light theme строится на `#F6FCFB`, `#E3F3F1`, `#FFFFFF`, `#14B9B9`, `#075E66`.
- Мутное стекло/cloudy glass сохраняется как product layer: header, composer, list shell и settings panels должны иметь видимый frosted source underlay.
- При transparency off, High Contrast или Windows 10 fallback core state остается читаемым и не зависит от blur.

## App bootstrap

States:

- cold start;
- loading local session;
- locked local session;
- unauthenticated;
- authenticated shell;
- fatal error локального storage/config.

Правила:

- Не flash-ить authenticated content до local lock validation.
- Не показывать stale route content до подтверждения current account/session.
- Startup errors используют localized non-technical text; diagnostics доступны в logs.

## Auth screens

Обязательно:

- login identifier/password;
- create-account entry, если backend/owner хочет live desktop registration;
- password recovery entry;
- 2FA challenge screen;
- backup-code mode switch;
- privacy policy link.

P0 может сначала сфокусироваться на existing-account login + 2FA, если provider/OAuth desktop flows требуют отдельной owner validation.

## Chats

### Chat list

Required states:

- loading;
- empty;
- offline with cached data;
- authenticated, но messenger inactive;
- loaded state с unread/pin/mute/typing metadata;
- search mode;
- contact requests показываются, когда они есть.

Actions:

- open chat;
- search chats/people/channels;
- create group/channel;
- accept/reject contact request;
- context menu: pin, mute presets, rename/local alias where allowed, add contact, profile, delete/hide и tab assignment where implemented.
- taskbar unread badge отражает aggregate unread count, но не заменяет in-app unread state.

### Search / discovery

Tabs:

- All
- People
- Channels

Правила:

- Raw numeric IDs не являются user-facing add/search mechanism.
- Results показывают display name, nickname/handle where safe, avatar, relation state.
- Opening result должен respect backend access.

### Conversation

Header:

- back/close в compact mode;
- avatar;
- title;
- status/typing/online/channel state;
- call actions, если direct chat их поддерживает;
- overflow для notifications, search, delete/hide, info.

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

Правила:

- Switching chats очищает previous route rows до bind target data.
- Empty state появляется только после route load settle.
- Timeline virtualization обязательна.
- Context actions уважают message ownership, chat type и permissions.

### Composer

Обязательно:

- text input;
- reply/edit banner;
- attach button;
- drag-and-drop file attachment;
- paste image/file support where safe;
- attachment draft strip;
- send/cancel actions;
- max 10 attachment drafts;
- send-as-original для still images.
- native file picker/save picker для open/save сценариев; drag/drop и paste не должны freeze-ить UI.

Правила:

- Multi-attachment send ждет storage `binding_id` у всех slots.
- File preparation не должна freeze-ить UI.
- Large files показывают progress и понятный fail/retry.

## Structured chats

Screens:

- Create group
- Create channel
- Group/channel info
- Members/subscribers
- Chat settings
- Per-chat user preferences
- Post comments

Role-gated actions:

- показывать только actions, которые current user может выполнить;
- не показывать backend-denied actions как обычные buttons;
- при live permission change обновлять UI после authoritative sync.

## Calls

Surfaces:

- calls list/history;
- contacts/callable people tab, если contract это поддерживает;
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

Правила:

- Incoming call должен быть actionable, даже если main window minimized.
- Media connect failures должны быть recoverable where possible.
- Call history обновляется после call end/missed/declined state.

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

- Battery optimization screen omitted или заменен на Windows startup/background permission diagnostics, только если есть реальная Windows setting.
- Screenshot privacy использует доступные Windows APIs/policies; если точная Android `FLAG_SECURE` parity невозможна, UI честно сообщает ограничения, а security doc обновляется.
- Windows Hello можно использовать как local unlock helper, если доступен.
- Windows 10 получает честный fallback там, где Windows 11-only APIs/materials недоступны.

## Notifications

Channels/categories:

- chats;
- calls;
- system;
- updates, если появится Windows update channel.

Правила:

- Message content preview следует user setting.
- Metadata-only cloud push boundary обязательна.
- Notification click открывает правильный route после session/local-lock handling.
- Если app locked, route intent сохраняется и resume-ится после unlock.
- Notification actions не должны раскрывать текст сообщения до local unlock.

## Accessibility

- Full keyboard navigation для top-level surfaces и chat list.
- Visible focus states.
- Screen-reader names для icon buttons.
- High contrast и text scaling support.
- Reduce motion уважается globally.
- Нельзя передавать status только через color.
- `Ctrl+F`, `Esc`, `F5`, `F6`/`Shift+F6` и scoped create/search accelerators должны работать там, где пользователь Windows ожидает keyboard-first UX.
- Narrator smoke pass обязателен для login, chat list, conversation, composer, calls и settings.

## DPI, resize и multi-monitor

- Layout обязан работать при 100%, 125%, 150% и 200% scaling.
- Window placement restore не должен открывать окно за пределами видимой work area.
- Compact/normal/wide transitions не должны терять draft text, selected chat или call controls.
- Snap Layouts не должны приводить к overlap composer/timeline/header.

## Localization

Languages:

- Russian
- English
- System default mode

Правила:

- All user text in resources.
- Нельзя показывать developer/debug raw codes в user-facing UI.
- Error copy localized и non-technical.
