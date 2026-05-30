# Дизайн-система Windows-клиента

Обновлено: 2026-05-30
Статус: source-of-truth для визуального языка, раскладки и дизайн-токенов Windows-клиента.

## Назначение

Windows-клиент APSApp должен быть нативным desktop messenger, но визуально принадлежать тому же продукту, что и Android-приложение. Этот документ фиксирует:

- desktop-раскладку по логике Telegram-like messenger, без копирования Telegram;
- уникальную IA APSApp: chats, calls, settings, privacy/status context и structured chats;
- палитру из Android-приложения;
- правила мутного стекла, cloudy/frosted surfaces и Windows 10/11 fallback;
- визуальные QA-референсы, которые нужно использовать как эталон ощущения интерфейса.

Подробный screen-level UX остается в [UI_UX_SPEC.md](UI_UX_SPEC.md), Windows-native поведение - в [WINDOWS_NATIVE_SPEC.md](WINDOWS_NATIVE_SPEC.md), release gates - в [QA_STRATEGY.md](QA_STRATEGY.md).

## Источники

Каноничные Android-источники:

- `APSApp_android_app/core/ui/src/main/kotlin/ru/apsdev/lichka/core/ui/theme/Color.kt`
- `APSApp_android_app/core/ui/src/main/kotlin/ru/apsdev/lichka/core/ui/theme/ApsAppTheme.kt`
- `APSApp_android_app/core/ui/src/main/kotlin/ru/apsdev/lichka/core/ui/theme/ApsDesignSystem.kt`
- `APSApp_android_app/core/ui/src/main/kotlin/ru/apsdev/lichka/core/ui/theme/Shape.kt`
- `APSApp_android_app/core/ui/src/main/kotlin/ru/apsdev/lichka/core/ui/theme/Type.kt`
- `APSApp_android_app/feature/messenger/src/main/kotlin/ru/apsdev/lichka/feature/messenger/ui/theme/MessengerThemeTokens.kt`
- `APSApp_android_app/docs/APP_FEATURE_INVENTORY.md`
- `APSApp_android_app/docs/module_messenger/*`

Визуальные QA-референсы, извлеченные из текущих Android QA-прогонов:

| Референс | Evidence path | Что фиксирует |
|---|---|---|
| Темный список чатов | `F:\APSApp_project\QA\tmp\qa\app-performance-benchmark-2026-05-26-run01\phone-current-screen.png` | глубокий темный фон, frosted header/search, chips, выделенный нижний dock, teal/mint glow, видимый cloudy glass |
| Темные настройки | `F:\APSApp_project\QA\tmp\qa\appearance-optimization-visual-2026-05-26-run03\5554-settings-home.png` | крупные стеклянные панели, rows с icon capsules, bottom dock blur поверх реального контента |
| Светлая appearance-поверхность | `F:\APSApp_project\QA\tmp\qa\appearance-optimization-visual-2026-05-26-run03\5554-appearance-light.png` | pearl/mint light theme, translucent panels, selected teal segmented control |
| Темный direct chat | `F:\APSApp_project\APSApp_android_app\QA\tmp\reply-author-dark-2026-05-19\direct-chat.png` | glass header, incoming dark bubbles, outgoing teal gradient bubbles, composer dock |
| Темные comments/media | `F:\APSApp_project\APSApp_android_app\QA\tmp\post-comments-reply-2026-05-19-run01\24-after-data-fix-comments-screen.png` | media card, reply preview, comments timeline, composer и bubble hierarchy |

Важное QA-решение из `APP_FEATURE_INVENTORY.md`: после `app-performance-benchmark-2026-05-26-run01` cloudy glass был восстановлен как видимый продуктовый слой. Messenger и Calls снова включают foreground list/content в source layer, overlay chrome находится отдельно; Settings использует long scroll content как source для blur, а header/bottom dock остаются снаружи и блюрят реальные rows. Nested live blur избегается, live radius capped at 34, static frosted fill применяется внутри source.

## Дизайн-принципы

1. APS в первую очередь, Telegram только как ergonomic reference.
   Раскладка может использовать знакомую desktop-модель: navigation rail, список диалогов, conversation stage, optional info pane. Нельзя копировать Telegram цвета, иконографику, размеры, row styling, header layout или точные композиции.

2. Нативный desktop, не растянутый phone app.
   Windows использует persistent panes, hover, context menus, keyboard navigation, drag-and-drop, taskbar badge, native notifications, Snap Layouts и multi-monitor behavior.

3. APS Aqua Glass как продуктовая идентичность.
   Мутное стекло, frosted fill, мягкие teal/mint акценты и deep ink surfaces являются частью APSApp. Их нельзя заменять плоским dark slate UI без owner decision.

4. Плотность без шума.
   Desktop должен быть сканируемым: компактные строки, четкая иерархия, контролируемые radii, понятные hover/focus states. Крупные glass panels допустимы для shell, settings и composer, но повторяемые list rows не должны становиться oversized.

5. Privacy и security states видимы.
   E2EE, local lock, notification privacy, upload progress, failed send и ACL states должны иметь читаемые состояния и не передаваться только цветом.

6. Уникальная IA APSApp.
   В отличие от Telegram-like копии, APSApp выделяет calls, security/privacy, structured chats, channel comments и storage/media states как самостоятельные desktop-поверхности.

## Desktop-раскладка

### Breakpoints

Все размеры заданы в DIP.

| Ширина окна | Раскладка | Правила |
|---|---|---|
| 360-699 | Compact | один pane, route stack; rail сворачивается в top/back navigation; composer остается usable |
| 700-1023 | Narrow desktop | список и detail меняются как master-detail; info pane открывается как overlay/flyout |
| 1024-1365 | Normal | rail 56-64, list 320-360, conversation flexible; info pane overlay |
| 1366-1919 | Wide | rail 64-72, list 340-380, conversation stage, optional info pane 300-360 |
| 1920+ | Ultra-wide | conversation content lane ограничен 860-920, info pane включается без растягивания текста |

Минимальное окно не должно приводить к overlap между header, timeline и composer. Snap left/right должен оставлять доступными navigation, chat list и composer.

### Основная сетка

```text
+----------+----------------+---------------------------+----------------+
| APS rail | Chat/call list | Conversation/call/settings | Context pane   |
| 64-72    | 340-380        | flexible content lane      | 300-360        |
+----------+----------------+---------------------------+----------------+
```

Context pane не обязателен постоянно. Он открывается для profile, media, members, channel info, search results, call details и security/session diagnostics. На narrow desktop context pane должен быть modal sheet или inline route, но не должен сжимать timeline до нечитаемого состояния.

## Уникальная раскладка APSApp

### APS rail

Левая rail отвечает за top-level product zones, а не за список папок Telegram.

Состав:

- account avatar/status capsule сверху;
- Chats;
- Calls;
- Settings;
- optional workspace/folder shortcuts после P1;
- нижний блок: lock, diagnostics/about, network state.

Правила:

- selected state использует `BrandPrimary`/`BrandSoft`, но не превращает всю rail в яркую полосу;
- icon-only buttons имеют tooltip и accessible name;
- unread badge не раскрывает содержимое сообщений;
- rail width стабильна и не меняется от labels/badges.

### Панель списка чатов

Верхняя часть:

- compact account/search glass header;
- search input;
- filter chips: All, Direct, Groups, Channels, Unread;
- optional Contact requests row;
- create command как icon button или compact split button.

Строки:

- avatar 40-44;
- title, last message preview, status/time, unread/mute/pin;
- selected row: тонкая teal edge или мягкий filled track, не full-saturation rectangle;
- unread: compact badge с teal/mint gradient;
- hover: subtle frosted lift, без layout shift;
- context menu: pin, mute, profile, delete/hide, add/contact actions where allowed.

Desktop list rows должны быть плотнее Android-карточек. Повторяемые rows используют radius 6-8, glass shell вокруг списка может быть 16-24.

### Область диалога

Шапка:

- glass header с avatar, title, status/typing/online/channel state;
- right commands: search, call, video, info, overflow;
- compact back button только в narrow/compact.

Context strip под header:

- pinned message;
- E2EE/local lock/security status;
- reconnect/offline banner;
- channel post context;
- active call banner;
- upload/sync warning.

Лента:

- dark theme: `ApsInkDark`/`ApsNightSurface` depth;
- light theme: `ApsPearl`/`LightSurfaceVariant` depth;
- content lane centered, max 860-920 на ultra-wide;
- outgoing bubbles справа, incoming слева;
- media cards не выходят за lane;
- date separators и unread separators не переносят layout.

Поле ввода:

- bottom glass dock;
- attach, text input, emoji/sticker future, voice, send;
- reply/edit banner внутри dock над input row;
- draft attachment strip над input row;
- drag/drop overlay только поверх composer/timeline lane;
- composer не должен перекрывать последнее сообщение без scroll padding.

### Calls

Calls используют тот же shell, но не имитируют chat list один в один.

- list pane: history, missed, callable contacts;
- detail stage: selected call, active call setup или empty state;
- active call может получить dedicated stage с video tiles и compact controls;
- incoming call notification/window не раскрывает лишний chat content.

### Settings

Settings наследует Android glass feel, но на desktop остается плотным.

- left или center list sections: Account, Appearance, Security, Notifications, Network/Data, About;
- rows используют icon capsules и static frosted fill;
- header/bottom dock могут блюрить scroll content;
- nested live blur запрещен, если source уже является glass layer.

## Стекло и Windows materials

### Правило source/overlay

Cloudy/frosted glass в APSApp требует видимого source content под стеклом:

- source layer содержит реальные list rows, timeline, media или settings rows;
- overlay chrome, header и bottom dock находятся отдельно и блюрят source;
- glass должен быть заметен при scroll, но не должен портить читаемость;
- nested live blur избегается;
- если элемент находится внутри blur source, использовать static frosted fill вместо нового live blur;
- live blur radius cap: 34 DIP;
- fallback radius для плотных элементов: 12-20 DIP.

### Windows 11

- App backdrop: Mica или Mica Alt для long-lived shell.
- Glass panels: WinUI Acrylic/SystemBackdrop/BackdropBrush там, где есть readable source.
- Header/composer/list shell: translucent brush поверх real content source.
- Flyouts/dialogs: WinUI theme resources, без custom hacks.

### Windows 10

- Mica отсутствует. Использовать Acrylic там, где он доступен и стабилен.
- Если transparency отключена или API недоступен, использовать static frosted fill:
  - dark: `ApsNightGlass`/`ApsNightGlassElevated` поверх `ApsNightSurface`;
  - light: `ApsPearlGlass`/`ApsWhiteGlass` поверх `ApsPearl`;
  - border из `ApsStrokeDark`/`ApsStrokeLight`.
- Core readability не зависит от transparency.

### High Contrast и Transparency off

- Перейти на system brushes и high contrast tokens.
- Убрать decorative translucency.
- Сохранить layout, focus states, unread badges и status text.
- Не скрывать control boundaries, если glass выключен.

## Палитра Android

### Core APS palette

| Token | Hex | Назначение Windows |
|---|---|---|
| `ApsBrandPrimary` | `#14B9B9` | главный teal accent, selected states, badges, primary buttons |
| `ApsBrandSoft` | `#35C8C5` | mint accent, gradients, hover highlights, outgoing bubble start |
| `ApsBrandMuted` | `#A8D2D0` | muted strokes, secondary text accents, soft borders |
| `ApsBrandDeep` | `#075E66` | deep teal, dark gradient end, high emphasis accent |
| `ApsInkDark` | `#061315` | dark app background, deepest timeline surface |
| `ApsInk` | `#10282B` | primary dark text/surface tone |
| `ApsNightSurface` | `#0B1E22` | dark surface |
| `ApsNightSurfaceElevated` | `#102A2F` | elevated dark panels, incoming bubbles |
| `ApsPearl` | `#F6FCFB` | light background |
| `ApsPearlGlass` | `#CCEAF4F3` | light frosted panel |
| `ApsWhiteGlass` | `#CCFFFFFF` | bright glass overlay |
| `ApsNightGlass` | `#B30B1E22` | dark frosted panel |
| `ApsNightGlassElevated` | `#A6102A2F` | elevated dark glass |
| `ApsStrokeLight` | `#99A8D2D0` | light theme glass stroke |
| `ApsStrokeDark` | `#6624565A` | dark theme glass stroke |
| `LightError` / `DarkError` | `#D85C72` | destructive/error states |

### Light scheme

| Role | Hex |
|---|---|
| Primary | `#14B9B9` |
| OnPrimary | `#061315` |
| PrimaryContainer | `#E3F3F1` |
| OnPrimaryContainer | `#075E66` |
| Secondary | `#35C8C5` |
| SecondaryContainer | `#A8D2D0` |
| Tertiary | `#075E66` |
| Background | `#F6FCFB` |
| OnBackground | `#10282B` |
| Surface | `#FFFFFF` |
| SurfaceVariant | `#E3F3F1` |
| OnSurfaceVariant | `#60797B` |
| Outline | `#A8D2D0` |
| ErrorContainer | `#F8D9DF` |

### Dark scheme

| Role | Hex |
|---|---|
| Primary | `#35C8C5` |
| OnPrimary | `#061315` |
| PrimaryContainer | `#075E66` |
| Secondary | `#14B9B9` |
| SecondaryContainer | `#102A2F` |
| Tertiary | `#A8D2D0` |
| TertiaryContainer | `#002840` |
| Background | `#061315` |
| Surface | `#0B1E22` |
| SurfaceVariant | `#102A2F` |
| OnSurfaceVariant | `#9CB8B8` |
| Outline | `#24565A` |
| ErrorContainer | `#5C1F2B` |

### Messenger tokens

Dark:

- outgoing bubble gradient: `#35C8C5` -> `#14B9B9` -> `#075E66`;
- outgoing text: `#073236`;
- incoming surface: `#102A2F` with alpha around 0.64;
- incoming border: `#29A8D2D0`;
- incoming text: `#EFF9F7`;
- incoming meta: `#94B1AE`;
- sender label: `#35C8C5`;
- avatar gradient: `#35C8C5` -> `#148F86`;
- input dock: `#102A2F` alpha 0.62 -> `#061315` alpha 0.50;
- input stroke: `#14B9B9` alpha 0.20;
- input field: `#0B1E22` alpha 0.40.

Light:

- outgoing bubble gradient: `#E9FFFB` -> `#D3FAF4` -> `#C2F2EA`;
- outgoing text: `#103333`;
- incoming surface: `#FFFFFF` alpha 0.72;
- incoming border: `#2E075E66`;
- list background: `#F6FCFB` -> `#EAF4F3` -> `#E3F3F1`;
- input dock: `#FFFFFF` alpha 0.72 -> `#E3F3F1` alpha 0.56.

## Имена Windows resources

Целевые resource names:

```text
ApsColor.Brand.Primary
ApsColor.Brand.Soft
ApsColor.Brand.Muted
ApsColor.Brand.Deep
ApsColor.Ink.Dark
ApsColor.Ink.Base
ApsColor.Surface.Night
ApsColor.Surface.NightElevated
ApsColor.Surface.Pearl
ApsColor.Stroke.Light
ApsColor.Stroke.Dark
ApsBrush.Glass.Dark
ApsBrush.Glass.DarkElevated
ApsBrush.Glass.Light
ApsBrush.Glass.White
ApsBrush.Messenger.BubbleOutgoing
ApsBrush.Messenger.BubbleIncoming
ApsBrush.Messenger.InputDock
```

Hardcoded colors вне token/resource layer запрещены, кроме временных diagnostic overlays.

## Форма, плотность и отступы

Android использует крупные shapes 12/18/24/30/36 dp. Windows адаптирует их к desktop density:

| Поверхность | Radius | Заметки |
|---|---|---|
| Repeated list rows/cards | 6-8 | плотное сканирование, hover без layout shift |
| Buttons/input fields/chips | 8-12 | если control не должен быть pill |
| Chat bubbles | 14-20 | близко к Android, но с desktop line length |
| Composer dock | 18-24 | glass dock, держит reply/edit/drafts |
| Main glass shell panels | 16-24 | chat list shell, settings group, side pane |
| Hero/profile/settings large panels | 24-30 | только там, где нужна Android continuity |
| Live blur radius | <=34 | QA cap из Android visual correction |

Отступы:

- rail icon hit target: 40-44;
- list row height: 60-76;
- chat header height: 56-64;
- context strip: 36-48;
- composer collapsed height: 56-64;
- message vertical gap: 4-8;
- bubble max width: 64% stage width, max 620 на normal, max 720 на ultra-wide;
- media card max width: 520-640.

## Типографика

Windows использует Segoe UI или системный WinUI font stack. Android typography не переносится буквально.

Правила:

- negative letter spacing на Windows не использовать;
- display-scale type только для route title/profile/empty states, не для list rows;
- chat list title: 14-15, Semibold;
- preview/meta: 12-13, Regular;
- message body: 14-15, Regular, line height 1.35-1.45;
- composer input: 14-15;
- section title: 12-13, Semibold, muted;
- status/error text дополняется icon/label, не только цветом.

## Иконки и controls

- Использовать WinUI system icons, Fluent symbols или approved icon pack.
- Для Android parity-иконок источник проекта по умолчанию остается IconScout Unicons line icons, но Windows может использовать Fluent equivalent, если он лучше нативно читается.
- Icon-only controls имеют tooltip и accessible name.
- Текстовые кнопки использовать для явных команд: Save, Cancel, Retry, Accept, Decline.
- Меню actions должны быть `MenuFlyout`/context menu, а не custom phone sheet.

## Анимация

- Route transition subtle, 120-180 ms.
- Hover/focus transitions 80-120 ms.
- Message insert может использовать короткий fade/slide, если reduce motion выключен.
- Reduce motion отключает decorative movement и заменяет transitions на instant/crossfade.
- Нельзя использовать постоянные floating/background animations в shell.

## Состояния

Обязательные visual states:

- loading with skeleton/placeholder, без flash чужого route content;
- offline cached;
- reconnecting;
- sending/uploading;
- failed send/retry;
- edited;
- reply target;
- pinned/context strip;
- unread separator;
- muted/pinned/archived;
- permission denied/read-only;
- local lock/session blocked;
- high contrast;
- transparency off.

## QA-гейты для визуального слоя

Перед первым UI skeleton merge:

- все palette tokens заведены в resource layer;
- dark/light shell screenshots визуально сверены с Android QA references;
- glass fallback описан и включен при transparency off;
- High Contrast не теряет controls/status;
- list/detail layout проверен на 1024, 1366, 1920 и Snap left/right;
- compact route stack не теряет composer/draft state;
- нет Telegram copy: colors, icons, row composition и exact spacing не совпадают с Telegram.

Перед beta:

- screenshot evidence для Windows 11 dark/light, Windows 10 fallback dark/light;
- pixel/visual smoke: header и composer реально показывают frosted source underlay при scroll;
- no nested live blur crash/perf regression;
- chat list, direct chat, comments/media, settings и calls проходят visual QA;
- Russian и English text не переполняет buttons/rows;
- DPI 100/125/150/200 не ломает layout;
- Narrator и keyboard-only smoke не зависят от visual material.

## Запреты

- Не копировать Telegram 1:1.
- Не заменять APS palette generic dark blue/slate palette.
- Не делать glass декоративным фоном без readable content source.
- Не рисовать отдельные decorative orbs/blob backgrounds. Допустим только мягкий inherited glow как часть Android APS background language и только если он не выглядит самостоятельным шаром.
- Не использовать raw IDs, debug status codes или backend names как user-facing labels.
- Не раскрывать plaintext в notifications, badges, lock screen или logs.
- Не делать Windows 10 менее безопасным ради визуального parity.
- Не использовать Mica/Acrylic как единственный способ показать state или boundary.
