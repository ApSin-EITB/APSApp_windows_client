# Data, Sync and Storage Specification

Updated: 2026-05-30

## Local Data Stores

Target stores:

| Store | Contents | Protection |
|---|---|---|
| Credential Locker / DPAPI | tokens, local master keys, device secrets | OS-protected |
| SQLite | chats, messages, outbox, sync cursors, media jobs, settings requiring query | encrypted DB or encrypted sensitive payloads |
| App settings | theme/language/reduce motion/non-sensitive prefs | normal local settings |
| App-private cache | media previews, playback temp files, upload payloads | app-private + cache policy |
| Logs | redacted diagnostics | bounded retention |

## Message Persistence

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

Do not persist:

- plaintext direct message content outside the protected DB/payload model;
- token/key material in ordinary tables;
- fake placeholder rows for unrecoverable legacy backup residue;
- direct-only transport secrets in private structured descriptors.

## Sync Model

Startup:

1. Load protected session.
2. Unlock local lock if required.
3. Restore local DB snapshot.
4. Connect/auth WebSocket.
5. Run global sync catch-up.
6. Reconcile outbox only after catch-up completes or times out.

Reconnect:

- Backoff with jitter.
- Preserve route state.
- Avoid duplicate send of already-acked logical messages.
- Merge own-message replay by `clientId` first, `sid` second.

Contacts:

- `contacts_changed` is dirty signal only.
- Fetch authoritative contact snapshot after signal.

## Outbox Rules

- Every outgoing logical message has stable `clientId`.
- Attachment slots use stable `messageId:attachmentIndex` identity.
- Text-only can send immediately after local outbox persist.
- Attachment message can send only after every slot has reusable `binding_id`.
- Retry reuses existing object/binding if valid.
- Retry must not create duplicate per-slot outbox rows.

## Media Preparation

Attachment kinds:

- image
- gif
- video
- audio
- voice
- pdf
- file

Rules:

- Prepare payload into app-private file first.
- `total_size` comes from prepared payload file.
- Image normalization and send-as-original must match descriptor semantics.
- Large media transforms run off UI thread.
- Thumbnails/previews are bounded and cached.

## Upload Jobs

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

- `404 upload_not_found` and `409 upload_reinit_required`: reset upload session, retry later.
- `409 upload_in_progress` for `chat_attachment`: one `cancel-active`, reset, one fresh `init`.
- `409 upload_size_mismatch`: terminal failure after reset.
- Non-chat scopes do not broad auto-cancel active uploads.

## Media Reads

Open/preview/playback paths:

- grant;
- descriptor;
- stream/range;
- file-backed cache if platform needs local file;
- descriptor-aware decrypt/reconstruct.

Rules:

- No full remote original in memory for large open/preview/recovery.
- Invalid chunked encryption metadata fails fast.
- Encrypted notification previews do not reconstruct full media.

## Backup Restore

Direct key backup:

- same-device only;
- successful import must be followed by `restore-complete`;
- pure read is not proof.

User-data backup:

- global per account;
- select exact snapshots by `backup_id`;
- rank candidates by actual content recency;
- empty import is unsuccessful;
- current gzip payload encoding supported;
- legacy readable envelopes supported only when safe.

Import filters:

- skip poison placeholders such as bare `Attachment` rows without real media;
- skip unresolved local transport rows: `ERROR`, `FAILED`, `PENDING`, `SENDING`, `QUEUED`, `UPLOADING`, `ENCRYPTING`, outgoing without server id where applicable;
- repair chat previews that referenced skipped rows;
- skip reactions for skipped messages.

## Cache Policy

User settings:

- clear cache;
- cache limit: 5 GB, 10 GB, 15 GB, unlimited;
- media download policy: always, Wi-Fi only, data saver.

Windows specifics:

- Detect metered network where Windows exposes it.
- Apply cache eviction in background without blocking UI.
- Keep currently playing/open files safe from deletion until closed.

## Migrations

- Every DB schema change has migration tests.
- Every sensitive storage migration has rollback/failure handling.
- Old incompatible Android/private-group backup residue must not enter live Windows runtime.
- Dev builds can include diagnostics; release builds must not expose raw sensitive rows.

