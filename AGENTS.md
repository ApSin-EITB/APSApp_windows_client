# AGENTS.md instructions for APSApp_windows_client

## Рабочие правила

- Пользуйся скилами, когда задача совпадает с доступным skill.
- Этот репозиторий spec-first до появления runtime-кода: не добавляй реализацию, которая противоречит `docs/SPEC.md` и ADR.
- Если что-то не получается, не уходи в обходной путь без согласования с владельцем.
- Спорь с владельцем, если предлагается технически рискованное решение.
- Не копируй секреты, токены, пароли, TOTP secrets, private keys, production logs или raw local-only paths в репозиторий.

## Канон Windows-клиента

- Основной стек: C# + WinUI 3 / Windows App SDK.
- C/C++ допускается как нативный остров для WebRTC/media, crypto bindings, hot paths и низкоуровневого Windows interop.
- Qt не является primary UI stack. Рассматривать Qt только если появится отдельная цель: кроссплатформенный desktop-клиент вне Windows-native UX.
- Android-контекст является продуктовым источником parity, но Windows UI не должен механически копировать мобильную навигацию.

## Source of truth

- Главная спецификация: `docs/SPEC.md`.
- Архитектура: `docs/ARCHITECTURE.md`.
- API/runtime контракты: `docs/API_CONTRACTS.md`.
- Безопасность: `docs/SECURITY.md`.
- UX: `docs/UI_UX_SPEC.md`.
- Data/sync/storage: `docs/DATA_SYNC_STORAGE.md`.
- QA: `docs/QA_STRATEGY.md`.
- Решения: `docs/decisions/`.

## QA правила

- Не ставь `QA PASS` по широкой функции после одного representative сценария.
- Для Windows QA проверять минимум: Windows 10 22H2, Windows 11 current, x64, русскую локаль, светлую/темную тему, fresh install, upgrade install, restart/resume.
- Для уведомлений и E2EE проверять privacy boundary: cloud/push payload не содержит plaintext сообщения.
- Для media/calls проверять не только UI, но и backend evidence, локальное состояние, reconnect, cache loss, restart.

