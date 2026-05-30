# APSApp Windows Client Docs

Updated: 2026-05-30

## Primary Docs

- [SPEC.md](SPEC.md) - full product/engineering specification.
- [ARCHITECTURE.md](ARCHITECTURE.md) - architecture and technical boundaries.
- [API_CONTRACTS.md](API_CONTRACTS.md) - backend contracts consumed by the Windows client.
- [SECURITY.md](SECURITY.md) - threat model, secrets, tokens, E2EE, local data protection.
- [UI_UX_SPEC.md](UI_UX_SPEC.md) - Windows IA and screen-level UX.
- [DATA_SYNC_STORAGE.md](DATA_SYNC_STORAGE.md) - local data model, sync, backups, media cache.
- [FEATURE_PARITY.md](FEATURE_PARITY.md) - Android-to-Windows parity map.
- [IMPLEMENTATION_PLAN.md](IMPLEMENTATION_PLAN.md) - ordered implementation tasks and acceptance checks.
- [QA_STRATEGY.md](QA_STRATEGY.md) - test plan and release gates.
- [ROADMAP.md](ROADMAP.md) - implementation milestones.

## ADRs

- [decisions/ADR-001-windows-client-stack.md](decisions/ADR-001-windows-client-stack.md)
- [decisions/ADR-002-packaging-and-updates.md](decisions/ADR-002-packaging-and-updates.md)

## Canonical Upstream Sources

The Windows client must track these upstream documents when contracts change:

- Android feature inventory: `APSApp_android_app/docs/APP_FEATURE_INVENTORY.md`
- Android network/API: `APSApp_android_app/docs/NETWORK_AND_API.md`
- Android messenger integration: `APSApp_android_app/docs/module_messenger/INTEGRATION_CONTRACT.md`
- Android messenger security: `APSApp_android_app/docs/module_messenger/SECURITY.md`
- Android messenger data layer: `APSApp_android_app/docs/module_messenger/DATA_LAYER.md`
- Core backend API: `APSApp_app_backend/docs/API.md`
- Messenger storage API: `APSApp_messenger_storage_backend/docs/02_API.md` and `openapi.yaml`
- Calls signaling API: `APSApp_messenger_calls_api/docs/API_SIGNALING.md`
- Workspace rules: `aps-rules.md`

## Sync Rule

When a backend or Android runtime contract changes, update the matching Windows spec in the same PR or record an explicit follow-up issue. Do not let this repository drift into stale parity claims.
