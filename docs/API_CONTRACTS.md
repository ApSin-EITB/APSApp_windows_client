# API-контракты

Обновлено: 2026-05-30

Документ фиксирует backend surfaces, которые потребляет Windows-клиент. Он не заменяет upstream backend docs, а отображает их на обязанности Windows-клиента.

## Runtime hosts

Production:

| Surface | Base URL |
|---|---|
| Core backend | `https://app.aplocal.ru/` |
| Messenger backend | `https://chat.app.aplocal.ru/` |
| Messenger WebSocket | `wss://chat.app.aplocal.ru/ws` |
| Messenger storage | `https://storage01.chat.app.aplocal.ru/` |
| Calls runtime | `https://calls.messenger.app.aplocal.ru/` |
| Update runtime | `https://update1.app.aplocal.ru/` |

Dev contour повторяет Android policy и должен включаться явно. Dev не должен молча fallback-иться на prod config, prod Firebase/push-style config или prod secrets.

## Core backend

Источник: `APSApp_app_backend/docs/API.md`.

Windows использует core backend для:

- login / refresh / current user;
- public auth exchange и signup-complete style flows, если они доступны для desktop;
- 2FA challenge verification;
- profile/account settings;
- chat directory/contact search и contact add;
- permissions/ACL snapshot;
- feedback reports;
- update metadata, если Windows update channel позже использует core proxy.

Правила:

- Bearer token передается только в `Authorization` header.
- `X-Device-ID` или эквивалентный desktop device header должен соответствовать backend contract после его фиксации.
- OAuth/provider secrets не живут в client repo.
- User-visible auth errors нормализуются и локализуются.

## Messenger WebSocket

Источник: `APSApp_android_app/docs/module_messenger/INTEGRATION_CONTRACT.md`.

Endpoint:

```text
GET /ws
```

Каноничный auth frame:

```json
{
  "t": "auth",
  "token": "<jwt>",
  "device_id": "<desktop_device_id_string>"
}
```

Client-to-server event families:

- `msg`
- `ack`
- `sync`
- `typing`

Server-to-client event families:

- `auth_ok` / `auth_ack`
- `msg`
- `ack`
- `typing`
- `sync`
- `contacts_changed`
- `error`

Правила:

- Query-parameter token auth запрещен.
- Startup/reconnect запускает sync catch-up до replay старого unresolved outbox.
- Outgoing group text становится локально `SENT` только после exact `clientId` ack или authoritative sync reconciliation.
- Own-message replay мерджится сначала по `clientId`, затем по `sid`.
- `contacts_changed` - только dirty signal; Windows fetch-ит authoritative contact snapshot.

## Key Directory и Backup

Источники: Android messenger integration/security docs.

Endpoints:

- `POST /v1/keys/publish`
- `GET /v1/keys/{userId}/devices`
- `GET /v1/keys/bundle?device_pk=...`
- `GET /v1/keys/server`
- `POST /v1/keys/backup`
- `GET /v1/keys/backup/latest?device_id=<device_id>`
- `POST /v1/keys/backup/restore-complete`
- `GET /v1/keys/backup/list`
- `POST /v1/keys/backup/rotate`
- `POST /v1/backup/upload`
- `GET /v1/backup/latest`
- `GET /v1/backup/{backup_id}`
- `POST /v1/backup/restore-complete`
- `GET /v1/backup/list`
- `POST /v1/backup/rotate`

Правила:

- Same-device direct key backup restore остается same-device.
- User-data backup является global per account; backup `device_id` - source metadata, не ownership selector.
- Нельзя upload empty user-data snapshot до появления локальных данных.
- Windows должен читать текущие gzip-encoded envelopes и legacy readable envelopes, когда это безопасно.
- Legacy private-group runtime residue не восстанавливается в live runtime.

## Chat metadata

Источник: Android messenger integration и backend docs.

Endpoints:

- `POST /v1/chats/group`
- `GET /v1/chats/{chatId}/members`
- `POST /v1/chats/{chatId}/members`
- channel/group endpoints по текущим messenger backend docs.

Правила:

- Role-gated operations должны соответствовать server ACL.
- Raw numeric user IDs не должны становиться user-facing search/add contract.
- Display names следуют messenger display-name policy: raw technical IDs не показываются как имена.

## Storage v1

Источники:

- `APSApp_messenger_storage_backend/docs/02_API.md`
- `APSApp_android_app/docs/module_messenger/INTEGRATION_CONTRACT.md`
- `APSApp_android_app/docs/NETWORK_AND_API.md`

Upload/read path:

1. `POST /v1/uploads/init`
2. `PUT /v1/uploads/{uploadId}/chunks/{seq}`
3. `POST /v1/uploads/{uploadId}/complete`
4. `POST /v1/uploads/{uploadId}/cancel`
5. `POST /v1/uploads/cancel-active`
6. `POST /v1/objects/{objectId}/bind`
7. `POST /v1/bindings/{bindingId}/grant`
8. `GET /v1/bindings/{bindingId}/descriptor`
9. `GET /v1/bindings/{bindingId}/stream`

Preferred upload chunk size:

| Payload size | Preferred chunk |
|---|---|
| `<= 20 MiB` | `256 KiB` |
| `> 20 MiB` and `<= 100 MiB` | `512 KiB` |
| `> 100 MiB` and `<= 512 MiB` | `2 MiB` |
| `> 512 MiB` | `4 MiB` |

Правила:

- Real slicing следует storage `accepted_chunk_size`.
- `total_size` берется из final prepared payload file.
- `409 upload_in_progress` для `scope=chat_attachment` может вызвать один `cancel-active` recovery и один fresh `init`.
- `409 upload_size_mismatch` является terminal после local reset.
- Read/open paths используют descriptor-aware streaming и file-backed cache.

## Media descriptor

Текущий outgoing descriptor совместим с Android `ChatMediaDescriptor(v=3, type="att")`.

Required fields where applicable:

- `kind`
- `binding_id`
- `mime`
- `file_name`
- `size_bytes`
- `preview_url`
- `local_preview_url`
- `width_px`
- `height_px`
- `duration_ms`
- `sample_rate_hz`
- `page_count`
- `waveform`
- `variants`

Direct E2EE-only fields:

- `enc_v`
- `enc_mode`
- `enc_chunk_plain_size`
- transport fields `k` and `iv`

Private structured group/channel/comment descriptors не должны persist или synthesize direct transport secrets.

## Calls runtime

Источники:

- `APSApp_messenger_calls_api/docs/API_SIGNALING.md`
- `APSApp_messenger_calls_api/docs/BACKEND_ANDROID_COMPAT.md`
- `APSApp_messenger_calls_api/docs/ARCHITECTURE.md`

Windows использует calls runtime для:

- join;
- end;
- invite;
- WebSocket call signaling;
- LiveKit/WebRTC media room access.

Правила:

- Calls runtime отделен от chat runtime.
- Chat WebSocket не является source of truth для call media signaling.
- Incoming call notification payloads остаются metadata-only.
- UI state должен выдерживать invite timeout, decline, missed, cancel, reconnect и media publication failures.

## Notifications

Android использует FCM как metadata-only wake/sync. Windows должен сохранить privacy boundary:

- notification cloud payloads, если появятся, содержат только routing metadata;
- client fetch/sync-ит message state и локально decrypt-ит перед показом message text;
- lock-screen message preview уважает privacy setting пользователя;
- calls могут показывать caller/chat metadata только если это разрешено privacy setting.

Первый Windows beta может использовать foreground WebSocket + local notifications до появления WNS. WNS требует отдельного backend contract и ADR.

