# ADR-004: Desktop layout, Android palette и glass visual language

Дата: 2026-05-30
Статус: принято

## Контекст

Windows-клиент должен быть полноценным desktop companion для APSApp, но не должен выглядеть как безликий WinUI shell или копия Telegram. Пользователь отдельно зафиксировал направление: раскладка а-ля Telegram по desktop-эргономике, уникальные расположения APSApp и цветовая палитра из Android-приложения. Дополнительно нужно сохранить мутное стекло/cloudy glass, которое уже восстановлено и проверено в Android QA.

Android-приложение уже содержит продуктовую палитру и glass primitives в:

- `Color.kt`
- `ApsAppTheme.kt`
- `ApsDesignSystem.kt`
- `MessengerThemeTokens.kt`
- QA artifacts по dark chat list, settings, light appearance, direct chat и comments/media screens.

## Решение

1. Windows shell использует Telegram-like desktop ergonomics как знакомую модель:
   - левая APS rail;
   - list pane;
   - conversation/call/settings stage;
   - optional context pane.

2. Telegram не является визуальным источником.
   Нельзя копировать Telegram цвета, точные размеры, иконографику, row layout, header layout или message composition.

3. Источником палитры является Android APS palette.
   Windows design tokens должны быть выведены из `Color.kt` и `MessengerThemeTokens.kt`. Hardcoded colors вне resource layer запрещены.

4. Cloudy/frosted glass является частью продуктовой идентичности.
   Windows 11 использует Mica/Acrylic/SystemBackdrop там, где это нативно и читаемо. Windows 10 получает Acrylic/static frosted fallback без потери core functionality.

5. Правило source/overlay обязательно.
   Glass layer должен блюрить реальный content source; header/bottom dock и overlay chrome отделены от source. Nested live blur избегается; внутри blur source используется static frosted fill. Live blur radius capped at 34 DIP.

6. Desktop density адаптирует Android shapes.
   Крупные Android radii сохраняются для shell panels/composer/profile/settings, но повторяемые list rows/cards используют более плотные 6-8 DIP radius.

## Последствия

Плюсы:

- Windows-клиент сохраняет узнаваемость APSApp.
- UI получает familiar desktop messenger model без юридического и визуального копирования Telegram.
- Палитра, стекло и QA references становятся проверяемым контрактом.
- Windows 10 fallback заранее описан и не будет блокировать Windows 11-native polish.

Компромиссы:

- Нужно поддерживать отдельный design token layer для Windows.
- Glass behavior придется тестировать отдельно при transparency off, High Contrast, DPI и Windows 10.
- Нельзя быстро заменить UI на generic WinUI templates без нарушения ADR.

## Проверка

- [DESIGN_SYSTEM.md](../DESIGN_SYSTEM.md) содержит palette, layout, glass rules и QA references.
- [QA_STRATEGY.md](../QA_STRATEGY.md) включает visual QA matrix.
- Первый UI skeleton должен завести design resources до реализации production screens.
- Beta QA должна включать screenshot evidence для Windows 11 dark/light и Windows 10 fallback dark/light.
