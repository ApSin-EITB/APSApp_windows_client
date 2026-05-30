# API Contracts

Updated: 2026-05-30

This document records the backend surfaces the Windows client consumes. It does not replace upstream backend docs; it maps them into Windows-client responsibilities.

## Runtime Hosts

Production:

| Surface | Base URL |
|---|---|
| Core backend | `https://app.aplocal.ru/` |
| Messenger backend | `https://chat.app.aplocal.ru/` |
| Messenger WebSocket | `wss://chat.app.aplocal.ru/ws` |
| Messenger storage | `https://storage01.chat.app.aplocal.ru/` |
| Calls runtime | `https://calls.messenger.app.aplocal.ru/` |
| Update runtime | `https://update1.app.aplocal.ru/` |

Dev contour mirrors Android policy and must be configured explicitly. Dev must never silently fall back to prod Firebase/push-style config or prod secrets.

## Core Backend

Source: `APSApp_app_backend/docs/API.md`.

Windows uses core backend for:

- login / refresh / current user;
- public auth exchange and signup-complete style flows when available for desktop;
- 2FA challenge verification;
- profile/account settings;
- chat directory/contact search and contact add;
- permissions/ACL snapshot;
- feedback reports;
- update metadata if Windows update channel uses core proxy later.

Rules:

- Bearer token must be sent in `Authorization` header only.
- `X-Device-ID` or equivalent desktop device header must match backend contract once established.
- OAuth/provider secrets never live in the client repo.
- User-visible auth errors are normalized and localized.

## Messenger WebSocket

Source: `APSApp_android_app/docs/module_messenger/INTEGRATION_CONTRACT.md`.

Endpoint:

```text
GET /ws
```

Canonical auth frame:

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

Rules:

- Query-parameter token auth is forbidden.
- Startup/reconnect runs sync catch-up before old unresolved outbox replay.
- Outgoing group text becomes locally `SENT` only after exact `clientId` ack or authoritative sync reconciliation.
- Own-message replay merges by `clientId` first and `sid` second.
- Contacts changed event is only a dirty signal; Windows fetches the authoritative contact snapshot.

## Key Directory and Backup

Source: Android messenger integration/security docs.

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

Rules:

- Same-device direct key backup restore remains same-device.
- User-data backup is global per account; backup `device_id` is source metadata, not ownership selector.
- Do not upload empty user-data snapshot before local data exists.
- Windows must read gzip-encoded current envelopes and legacy readable envelopes when supported.
- Legacy private-group runtime residue is not restored into live runtime.

## Chat Metadata

Source: Android messenger integration and backend docs.

Endpoints:

- `POST /v1/chats/group`
- `GET /v1/chats/{chatId}/members`
- `POST /v1/chats/{chatId}/members`
- channel/group endpoints as defined by current messenger backend docs.

Rules:

- Role-gated operations must match server ACL.
- Raw numeric user IDs must not be exposed as user-facing search/add contract.
- Display names follow the messenger display-name policy: no raw technical IDs as names.

## Storage v1

Sources:

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

Rules:

- Real slicing follows storage `accepted_chunk_size`.
- `total_size` is from final prepared payload file.
- `409 upload_in_progress` for `scope=chat_attachment` may trigger one `cancel-active` recovery and one fresh `init`.
- `409 upload_size_mismatch` is terminal after local reset.
- Read/open paths use descriptor-aware streaming and file-backed cache.

## Media Descriptor

Current outgoing descriptor is compatible with Android `ChatMediaDescriptor(v=3, type="att")`.

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

Private structured group/channel/comment descriptors must not persist or synthesize direct transport secrets.

## Calls Runtime

Sources:

- `APSApp_messenger_calls_api/docs/API_SIGNALING.md`
- `APSApp_messenger_calls_api/docs/BACKEND_ANDROID_COMPAT.md`
- `APSApp_messenger_calls_api/docs/ARCHITECTURE.md`

Windows uses calls runtime for:

- join;
- end;
- invite;
- WebSocket call signaling;
- LiveKit/WebRTC media room access.

Rules:

- Calls runtime is separate from chat runtime.
- Chat WebSocket is not call media signaling source of truth.
- Incoming call notification payloads must stay metadata-only.
- UI state must tolerate invite timeout, decline, missed, cancel, reconnect and media publication failures.

## Notifications

Android uses FCM metadata-only wake/sync. Windows must preserve the privacy boundary:

- notification cloud payloads, if any, carry routing metadata only;
- client fetches/syncs message state and locally decrypts before showing message text;
- lock-screen message preview respects user privacy setting;
- calls may show caller/chat metadata only as allowed by privacy setting.

Initial Windows beta may use foreground WebSocket + local notifications before WNS exists. WNS requires a separate backend contract and ADR.

