# ADR-001: Основной стек Windows-клиента - C# WinUI 3 с нативными C/C++ островами

## Статус

Accepted

## Дата

2026-05-30

## Контекст

APSApp Windows Client должен быть нативным desktop-клиентом для Windows и при этом сохранять продуктовые контракты Android/backend: auth, direct E2EE, messenger sync, storage media, calls, settings, notifications и QA evidence.

Владелец спросил, почему не C++, затем уточнил, что C# подходит, но часть C/C++ скорее всего понадобится; также была упомянута Qt.

## Решение

Основной app/UI стек: C# + WinUI 3 / Windows App SDK.

C/C++ разрешены как нативные острова для:

- WebRTC/media pipeline;
- crypto/libsignal bindings, если managed-вариант недостаточно production-grade;
- performance hot paths;
- низкоуровневого Windows interop.

Qt не является основным UI stack. Возвращаться к Qt только если продуктовая цель изменится на cross-platform desktop.

## Рассмотренные альтернативы

### Полностью C++ / C++/WinRT

Плюсы:

- максимально нативный стек;
- прямой доступ к Windows API;
- полезен для media и low-level interop.

Минусы:

- выше memory-safety риск и стоимость разработки app-level UI/state/auth/sync кода;
- медленнее продуктовые итерации;
- дороже тестирование и сопровождение будущими агентами;
- основная сложность APSApp-клиента не CPU-bound.

Отклонено как основной стек. Оставлено для native islands.

### C# + WinUI 3

Плюсы:

- нативный Windows UI stack;
- высокая скорость разработки для MVVM, async, DI, tests, settings и API clients;
- хорошая интеграция с MSIX, notifications, Credential Locker/DPAPI;
- проще сопровождение для будущих разработчиков и агентов;
- native interop остается доступным.

Минусы:

- могут понадобиться native wrappers для libsignal/WebRTC/media;
- managed runtime становится частью deployment story;
- часть low-level API менее прямая, чем в C++.

Принято.

### Qt

Плюсы:

- зрелый UI toolkit;
- сильная C++ экосистема;
- потенциал cross-platform desktop.

Минусы:

- не Windows-native first;
- добавляет второй UI/runtime abstraction layer;
- хуже стыкуется с WinUI, MSIX, Windows notifications и credential APIs;
- cross-platform сейчас не является продуктовой целью.

Отклонено для текущего Windows-native клиента.

## Последствия

- Первый solution skeleton должен быть на C# WinUI.
- Native code должен быть узким и явно обоснованным.
- Product policy и интерпретация backend-контрактов остаются в managed domain code.
- Будущий переход на Qt или all-C++ требует нового ADR и согласования с владельцем.

