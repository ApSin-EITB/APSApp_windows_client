# Нативная спецификация Windows 10/11

Обновлено: 2026-05-30
Статус: source-of-truth спецификация Windows-native поведения.

## Назначение

Этот документ уточняет, как APSApp Windows Client должен использовать Windows 10/11 не как абстрактный desktop, а как нативную Windows-платформу: жизненный цикл приложения, окна, MSIX identity, уведомления, taskbar, защищенное хранилище, accessibility, DPI, файловую интеграцию и визуальные материалы.

Продуктовые контракты остаются в `SPEC.md`, архитектурные границы - в `ARCHITECTURE.md`, безопасность - в `SECURITY.md`, визуальная система - в `DESIGN_SYSTEM.md`, QA - в `QA_STRATEGY.md`.

## Источники Microsoft

Использовать только stable production APIs и сверять реализацию с текущими документами Microsoft Learn:

- Windows App SDK release channels: https://learn.microsoft.com/windows/apps/windows-app-sdk/preview-channel
- Windows App SDK stable channel: https://learn.microsoft.com/windows/apps/windows-app-sdk/stable-channel
- Windows App SDK downloads: https://learn.microsoft.com/windows/apps/windows-app-sdk/downloads
- Windows App SDK system requirements: https://learn.microsoft.com/windows/apps/windows-app-sdk/system-requirements
- Windows 10 Home and Pro lifecycle: https://learn.microsoft.com/lifecycle/products/windows-10-home-and-pro
- Windowing overview: https://learn.microsoft.com/windows/apps/develop/ui/windowing-overview
- Single-instanced WinUI app: https://learn.microsoft.com/windows/apps/windows-app-sdk/applifecycle/applifecycle-single-instance
- Mica material: https://learn.microsoft.com/windows/apps/design/style/mica
- System backdrops: https://learn.microsoft.com/windows/apps/develop/ui/system-backdrops
- App notifications: https://learn.microsoft.com/windows/apps/develop/notifications/app-notifications/
- File management and pickers: https://learn.microsoft.com/windows/apps/develop/files/
- Credential Locker: https://learn.microsoft.com/windows/apps/develop/security/credential-locker
- Windows Hello: https://learn.microsoft.com/windows/apps/develop/security/windows-hello
- Accessibility overview: https://learn.microsoft.com/windows/apps/design/accessibility/accessibility-overview
- Keyboard accelerators: https://learn.microsoft.com/windows/apps/develop/input/keyboard-accelerators

## Уровни поддержки

| Tier | OS | Статус для APSApp | Правило |
|---|---|---|---|
| Tier A | Windows 11 current supported releases, x64 | Primary target | Полная UX/QA цель, Windows 11 materials, AppWindow, notifications, accessibility и MSIX upgrade должны проходить release gates. |
| Tier B | Windows 10 22H2 x64 | Compatibility target | Core auth/messenger/media/calls должны работать, но Windows 11-only визуальные эффекты заменяются fallback-ами. |
| Future | Windows 11 ARM64 | P2 | Добавлять после x64 beta, отдельной задачей и QA matrix. |
| Unsupported | Windows 10 до 22H2, Windows 7/8/8.1, Windows Server как desktop-клиент | Out | Не являются release target без нового owner decision. |

На дату 2026-05-30 Windows 10 Home/Pro уже вне обычного support lifecycle Microsoft с 2025-10-14. Поэтому Windows 10 22H2 остается compatibility tier для ESU/LTSC/owner-controlled окружений, но не равен Windows 11 как primary production baseline.

Compatibility tier не должен блокировать security fixes и Windows 11-native улучшения. Если Windows 10 fallback невозможен без риска для безопасности, функция получает `QA PARTIAL/BLOCKED` на Windows 10 и требует решения владельца.

## Windows App SDK и WinUI

- Использовать Windows App SDK stable channel. Preview/Experimental APIs запрещены для P0/P1 без ADR и owner approval.
- Версию `Microsoft.WindowsAppSDK` pin-ить в solution skeleton и обновлять отдельным PR с release notes и smoke QA.
- UI shell строить на WinUI 3 XAML. WPF/WinForms не использовать для основного shell.
- C/C++ interop допустим только через narrow adapter interfaces из `ARCHITECTURE.md`.
- Release baseline - packaged MSIX. Unpackaged run разрешен только для local development и diagnostics.

## Жизненный цикл приложения и activation

APSApp должен быть single-instance приложением с route activation:

- второй запуск активирует существующий main instance;
- protocol activation, notification activation, file activation и future share activation маршрутизируются в существующий процесс;
- activation payload не должен обходить auth restore, local lock или account check;
- если app locked, intent сохраняется как pending activation и выполняется после unlock;
- если пользователь не авторизован, activation приводит к login/onboarding, а не к раскрытию контента.

Activation router обязан нормализовать источники:

```text
Launch
Protocol link
Notification click/action
File/open-with
Share target, future

-> ActivationRouter
-> Auth/session/local-lock gate
-> Shell route
```

## Окна и AppWindow

Основная модель:

- один main window для Chats/Calls/Settings;
- optional secondary call window для активного звонка в P1, если это улучшит UX и не усложнит privacy;
- optional media viewer window в P2;
- route state и window placement сохранять только после успешного authenticated shell bootstrap.

Требования к окнам:

- использовать `Window`/`AppWindow` APIs для title, icon, min size, restore placement и future compact overlay;
- поддерживать Snap Layouts и стандартные caption controls;
- не ломать drag region при custom title bar;
- минимальный размер окна должен сохранять usable navigation и composer;
- multi-monitor placement должен восстанавливаться безопасно: если монитор исчез, окно возвращается в visible work area;
- per-monitor DPI должен корректно менять layout без blurred custom rendering.

## Title bar и навигация

- Windows 11: integrated title bar с Mica/Mica Alt backdrop там, где это не мешает readability.
- Windows 10: Acrylic или plain solid fallback. Нельзя имитировать Mica кастомными gradient/background hacks.
- NavigationView left mode является default для desktop shell.
- Compact mode должен оставаться полноценным, но desktop first layout - list/detail.
- CommandBar/Flyout/MenuFlyout использовать для действий, которые на Android были FAB/sheet.

## Материалы и резервные режимы

Визуальный источник APSApp для materials - [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md). Windows implementation должна сохранить Android cloudy/frosted glass как продуктовый слой, но использовать нативные Windows mechanisms и fallback.

| Surface | Windows 11 | Windows 10 |
|---|---|---|
| App backdrop | Mica/Mica Alt где уместно | Acrylic или solid theme brush |
| Title bar | Custom title bar with system caption controls | Standard/custom title bar без Mica dependency |
| Dialogs/flyouts | WinUI controls, theme resources | те же controls с fallback brushes |
| High contrast | System colors, no decorative dependency | System colors, no decorative dependency |

Правила:

- Никакой важный текст, status или control не должен зависеть от Mica/Acrylic.
- Backdrop применяется как базовый слой, не на отдельные controls.
- Header/composer/list glass должен блюрить реальный source content, а не пустой декоративный фон.
- Overlay chrome отделяется от source layer; nested live blur избегается, внутри source используется static frosted fill.
- Live blur radius capped at 34 DIP, если конкретный Windows API/производительность не требуют меньшего значения.
- В High Contrast и при выключенной transparency приложение должно оставаться читаемым.
- Цвета берутся из theme resources; hardcoded colors допустимы только в токенах design system с contrast test.

## Уведомления, badges и taskbar

P0:

- использовать Windows App SDK `AppNotificationManager` для local app notifications;
- message/call notification activation проходит через `ActivationRouter`;
- payload в cloud/WNS, если появится, остается metadata-only;
- message preview показывается только после local fetch/decrypt и только если user setting разрешает preview;
- unread badge на taskbar показывает aggregate unread count;
- notification click не раскрывает chat content до local lock.

P1:

- notification actions: reply/open/mute для messages, accept/decline для calls;
- группировка notifications по conversation там, где Windows API это поддерживает без утечки plaintext;
- Jump List для быстрых top-level routes: Chats, Calls, Settings.

Правила для tray:

- system tray не является P0 requirement;
- если нужен background presence, добавить P1 ADR и user-visible setting;
- tray interop должен быть изолирован в Windows adapter, например через Win32 `Shell_NotifyIcon`, без product policy внутри interop layer.

## Безопасная локальная интеграция

- Tokens и refresh secrets хранить в Credential Locker или DPAPI-protected storage.
- Local master key для encrypted DB/cache защищать DPAPI current-user scope, если не выбран другой audited механизм.
- Windows Hello использовать только как local unlock helper или local key release gesture. Он не заменяет backend auth, password, TOTP или server session validation.
- После logout удалять tokens, protected keys, push registration и pending activation intents.
- Clipboard операции для sensitive content должны быть явными; auto-copy secrets запрещен.
- Screenshot/privacy mode на Windows является best-effort: документировать ограничения, не обещать Android `FLAG_SECURE` parity, пока не доказан реальный Windows equivalent.

## Файлы, drag/drop и media

P0:

- attachment picker через Windows App SDK/WinRT picker APIs;
- drag-and-drop files into composer;
- paste image/file из clipboard, если источник безопасно читается;
- save-as для downloaded media через native save picker;
- long paths, Unicode filenames и пробелы в paths должны быть covered tests;
- temp decrypted files лежат в app-private cache и чистятся cache policy.

P1/P2:

- file association/open-with только для owner-approved formats;
- Share Target - P2, после стабилизации auth/local-lock activation path;
- thumbnail generation может использовать native/media libraries, если managed path недостаточен.

## Accessibility и ввод

P0 gates для accessibility:

- keyboard-only navigation для всех top-level routes;
- `Ctrl+F` search, `Esc` close/cancel, `Ctrl+N` create where scoped, `F5` refresh where relevant;
- `F6`/`Shift+F6` pane traversal для list/detail/info layout;
- visible focus indicators;
- accessible names/descriptions для icon-only buttons;
- Narrator smoke pass для login, chat list, conversation, composer, calls и settings;
- high contrast, text scaling, reduce motion и color-independent status.

Touch/pen:

- touch должен работать на tablets/2-in-1, но desktop mouse/keyboard остается primary;
- hit targets не должны становиться меньше Windows recommended interactive sizes;
- context menus доступны правой кнопкой, keyboard и touch long-press where appropriate.

## Производительность и отзывчивость

P0 targets до beta:

- cold start to local session decision: до 2 секунд на reference Windows 11 x64 machine без network wait;
- route switch не блокирует UI thread дольше 100 ms;
- chat list/timeline используют virtualization;
- file hashing/upload preparation выполняется off UI thread;
- notification activation to route intent accepted: до 500 ms без учета auth/local-lock;
- memory budget для idle authenticated shell фиксируется в QA после первого skeleton profiling.

Diagnostics:

- использовать `Microsoft.Extensions.Logging` с redaction;
- добавить EventSource/ETW provider только если нужен stable local performance evidence;
- crash/feedback bundle не включает plaintext messages, tokens или raw local paths.

## Резервные режимы Windows 10

Приемка Windows 10 22H2:

- app starts, auth/session/local-lock работают;
- chat list, conversations, composer, media и calls P0 работают;
- notifications открывают correct route через lock gate;
- no Mica dependency;
- no Windows 11-only API call without version guard;
- packaging/install/upgrade проходит на supported compatibility machine.

Windows 10 может получать `QA PARTIAL`, если:

- отсутствует Windows 11-only visual enhancement;
- OS lifecycle/security posture не подходит для production support;
- push/background delivery требует packaged identity или API behavior, который owner не хочет поддерживать для compatibility tier.

## Release gates

Перед первой beta:

- ADR-003 принят и не конфликтует с roadmap.
- Windows 11 Tier A manual matrix имеет `QA PASS` или owner-approved `QA PARTIAL`.
- Windows 10 22H2 compatibility matrix имеет явные PASS/PARTIAL/BLOCKED rows.
- MSIX install/upgrade validated на Windows 11 и Windows 10 compatibility machine.
- Notification privacy evidence подтверждает metadata-only cloud boundary.
- Accessibility smoke evidence сохранен для Narrator, keyboard-only и High Contrast.
- DPI/multi-monitor checks покрывают 100%, 125%, 150%, 200% scaling.
