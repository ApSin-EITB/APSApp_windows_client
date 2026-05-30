# Data, Sync и Storage

Обновлено: 2026-05-30

## Локальные хранилища

Целевая схема:

| Хранилище | Данные | Защита |
|---|---|---|
| Credential Locker / DPAPI | tokens, local master keys и device secrets | OS-protected |
| SQLite | chats, messages, outbox, sync cursors, media jobs, queryable settings | encrypted DB или encrypted sensitive payloads |
| App settings | theme/language/reduce motion/non-sensitive prefs | обычные local settings |
| App-private cache | media previews, playback temp files и upload payloads | app-private + cache policy |
| Logs | redacted diagnostics | bounded retention |

## Message persistence

Persist:

- chat metadata;
- message rows;
- outbox rows;
- server ids / client ids;
- delivery/read status;
- media descriptors;
- upload jobs;
- local failure states;
- sync cursors.

Не persist:

- plaintext direct message content вне protected DB/payload model;
- token/key material в ordinary tables;
- fake placeholder rows для unrecoverable legacy backup residue;
- direct-only transport secrets в private structured descriptors.

## Sync model

Startup:

1. Load protected session.
2. Unlock local lock, если требуется.
3. Restore local DB snapshot.
4. Connect/auth WebSocket.
5. Run global sync catch-up.
6. Reconcile outbox только после завершения catch-up или timeout.

Reconnect:

- Backoff with jitter.
- Preserve route state.
- Не duplicate-ить send для already-acked logical messages.
- Merge own-message replay сначала по `clientId`, затем по `sid`.

Contacts:

- `contacts_changed` является только dirty signal.
- После сигнала fetch-ить authoritative contact snapshot.

## Outbox rules

- У каждого outgoing logical message стабильный `clientId`.
- Attachment slots используют стабильную identity `messageId:attachmentIndex`.
- Text-only message можно send-ить сразу после local outbox persist.
- Attachment message можно send-ить только после появления reusable `binding_id` у каждого slot.
- Retry сначала reuse-ит existing object/binding, если они валидны.
- Retry не должен создавать duplicate per-slot outbox rows.

## Media preparation

Attachment kinds:

- image
- gif
- video
- audio
- voice
- pdf
- file

Правила:

- Сначала prepare payload в app-private file.
- `total_size` берется из prepared payload file.
- Image normalization и send-as-original должны соответствовать descriptor semantics.
- Large media transforms выполняются off UI thread.
- Thumbnails/previews bounded и cached.

## Upload jobs

Persist:

- upload id;
- accepted chunk size;
- uploaded sequence state;
- uploaded bytes;
- object id;
- binding id;
- descriptor;
- failure/retry state.

Recovery:

- `404 upload_not_found` и `409 upload_reinit_required`: reset upload session, retry later.
- `409 upload_in_progress` для `chat_attachment`: один `cancel-active`, reset, один fresh `init`.
- `409 upload_size_mismatch`: terminal failure после reset.
- Non-chat scopes не делают broad auto-cancel active uploads.

## Media reads

Open/preview/playback paths:

- grant;
- descriptor;
- stream/range;
- file-backed cache, если platform requires local file;
- descriptor-aware decrypt/reconstruct.

Правила:

- Нельзя держать full remote original в memory для large open/preview/recovery.
- Invalid chunked encryption metadata должна fail fast.
- Encrypted notification previews не reconstruct-ят full media.

## Backup restore

Direct key backup:

- same-device only;
- successful import должен сопровождаться `restore-complete`;
- pure read не является proof.

User-data backup:

- global per account;
- select exact snapshots по `backup_id`;
- rank candidates по actual content recency;
- empty import считается unsuccessful;
- current gzip payload encoding поддерживается;
- legacy readable envelopes поддерживаются только если безопасны.

Import filters:

- skip poison placeholders, например bare `Attachment` rows без real media;
- skip unresolved local transport rows: `ERROR`, `FAILED`, `PENDING`, `SENDING`, `QUEUED`, `UPLOADING`, `ENCRYPTING`, outgoing without server id where applicable;
- repair chat previews, которые ссылались на skipped rows;
- skip reactions for skipped messages.

## Cache policy

User settings:

- clear cache;
- cache limit: 5 GB, 10 GB, 15 GB, unlimited;
- media download policy: always, Wi-Fi only, data saver.

Windows specifics:

- Detect metered network, если Windows это exposes.
- Apply cache eviction in background без блокировки UI.
- Не удалять currently playing/open files до close.

## Migrations

- Каждое DB schema change имеет migration tests.
- Каждая sensitive storage migration имеет rollback/failure handling.
- Old incompatible Android/private-group backup residue не должен попасть в live Windows runtime.
- Dev builds могут включать diagnostics; release builds не должны expose raw sensitive rows.
