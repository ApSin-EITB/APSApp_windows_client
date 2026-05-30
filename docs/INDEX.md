# Документация APSApp Windows Client

Обновлено: 2026-05-30

## Основные документы

- [SPEC.md](SPEC.md) - полная продуктовая и инженерная спецификация.
- [ARCHITECTURE.md](ARCHITECTURE.md) - архитектура и технические границы Windows-клиента.
- [WINDOWS_NATIVE_SPEC.md](WINDOWS_NATIVE_SPEC.md) - нативное поведение Windows 10/11, уровни поддержки, lifecycle, shell, notifications, DPI и accessibility.
- [API_CONTRACTS.md](API_CONTRACTS.md) - backend/runtime контракты, которые потребляет Windows-клиент.
- [SECURITY.md](SECURITY.md) - threat model, секреты, токены, E2EE и защита локальных данных.
- [UI_UX_SPEC.md](UI_UX_SPEC.md) - IA Windows-клиента и screen-level UX.
- [DATA_SYNC_STORAGE.md](DATA_SYNC_STORAGE.md) - локальная модель данных, sync, backups и media cache.
- [FEATURE_PARITY.md](FEATURE_PARITY.md) - карта parity между Android и Windows.
- [IMPLEMENTATION_PLAN.md](IMPLEMENTATION_PLAN.md) - порядок реализации и acceptance checks.
- [QA_STRATEGY.md](QA_STRATEGY.md) - test plan и release gates.
- [ROADMAP.md](ROADMAP.md) - implementation milestones.

## ADR

- [decisions/ADR-001-windows-client-stack.md](decisions/ADR-001-windows-client-stack.md)
- [decisions/ADR-002-packaging-and-updates.md](decisions/ADR-002-packaging-and-updates.md)
- [decisions/ADR-003-windows-support-tiers.md](decisions/ADR-003-windows-support-tiers.md)

## Каноничные upstream-источники

Windows-клиент должен отслеживать эти документы при изменении контрактов:

- Android feature inventory: `APSApp_android_app/docs/APP_FEATURE_INVENTORY.md`
- Android network/API: `APSApp_android_app/docs/NETWORK_AND_API.md`
- Android messenger integration: `APSApp_android_app/docs/module_messenger/INTEGRATION_CONTRACT.md`
- Android messenger security: `APSApp_android_app/docs/module_messenger/SECURITY.md`
- Android messenger data layer: `APSApp_android_app/docs/module_messenger/DATA_LAYER.md`
- Core backend API: `APSApp_app_backend/docs/API.md`
- Messenger storage API: `APSApp_messenger_storage_backend/docs/02_API.md` и `openapi.yaml`
- Calls signaling API: `APSApp_messenger_calls_api/docs/API_SIGNALING.md`
- Workspace rules: `aps-rules.md`

## Правило синхронизации

Если меняется backend-контракт или Android runtime contract, соответствующая Windows-спецификация обновляется в том же PR или фиксируется отдельная follow-up задача. Нельзя оставлять этот репозиторий со stale parity-утверждениями.
