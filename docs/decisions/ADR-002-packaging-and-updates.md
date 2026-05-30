# ADR-002: Windows-клиент упаковывается через MSIX first

## Статус

Proposed

## Дата

2026-05-30

## Контекст

У Android есть store/sideload-specific OTA поведение. Windows-клиенту нужна собственная политика packaging и updates. Desktop-клиент должен корректно интегрироваться с Windows notifications, app identity, install/upgrade behavior и migration локальных данных.

## Решение

MSIX является основным packaging target для beta и release. Unpackaged launch разрешен только для developer builds.

Update policy:

- P0 beta: ручная раздача signed MSIX.
- P1: `appinstaller` или owner-approved update channel, если понадобится.
- Future: Microsoft Store только если владелец выберет store distribution.

Android APK OTA правила напрямую к Windows не применяются.

## Рассмотренные альтернативы

### Raw executable / zip

Плюсы:

- быстрее стартовать;
- просто для internal dev.

Минусы:

- слабее install/update identity;
- сложнее notification/deep-link integration;
- проще потерять migration guarantees.

Разрешено только для local dev, не для release.

### MSIX

Плюсы:

- Windows-native packaging;
- чистая install/upgrade identity;
- хорошо подходит для notifications, app links и будущего Store;
- безопаснее uninstall behavior.

Минусы:

- нужна подпись;
- packaging constraints нужно учитывать рано.

Принято как release path.

### Microsoft Store с первого дня

Плюсы:

- удобные updates для пользователя;
- store trust/distribution.

Минусы:

- лишний compliance/review loop до стабилизации клиента;
- владелец пока не выбрал этот канал.

Отложено.

## Последствия

- Release build должен производить signed MSIX до beta.
- Local storage paths и migration должны проверяться на MSIX upgrade.
- Update UX не должен слепо копировать Android OTA assumptions.

