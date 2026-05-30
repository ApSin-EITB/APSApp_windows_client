# Security Specification

Updated: 2026-05-30

## Security Goals

- Preserve Android's privacy model on Windows.
- Keep direct 1x1 messages end-to-end encrypted.
- Keep group/channel/private comments on the current `server_encrypted_v1` trusted-server model.
- Keep tokens, device keys, backups and local message history protected at rest.
- Prevent plaintext message leakage through notifications, logs, crash reports or cloud push providers.

## Threat Model

Primary threats:

- stolen Windows user profile directory;
- malware/user-level process trying to read local app files;
- accidental token/message leakage through logs or feedback reports;
- network attacker;
- stale sync/outbox replay after reconnect;
- notification cloud provider receiving plaintext content;
- native module memory-safety vulnerabilities;
- downgrade or compatibility bugs that import old backup residue incorrectly.

Non-goals:

- protection against a fully compromised administrator/root machine;
- hiding content from the active signed-in Windows user;
- bypassing backend ACL decisions.

## Auth and Token Storage

- Access/refresh tokens are stored in Windows Credential Locker or DPAPI-protected local storage.
- Tokens are never stored in plaintext config files.
- Tokens are never sent in URLs.
- Logs must redact `Authorization`, cookies, refresh tokens, one-time auth codes and 2FA backup codes.
- Logout removes tokens, active WebSocket credentials and push registration state.

## Device Identity

The desktop client has a stable per-install device identifier used for backend contracts where Android uses registration/device IDs.

Rules:

- Store device ID in protected local storage.
- Do not show raw device ID in normal UI.
- Rotate only through explicit account/device reset flow.
- Changing device ID affects key backup and must be treated as security-sensitive.

## Direct 1x1 E2EE

Direct chats follow the current Android direct E2EE model:

- Signal/libsignal-compatible identity, signed pre-key, one-time pre-key and session behavior.
- Server stores ciphertext and key directory metadata, not plaintext.
- Decryption failure is a visible local state, not a reason to synthesize fake plaintext.
- Key backup restore is same-device for direct private state.

Implementation:

- The C# app depends on `IPrivateChatCrypto`.
- The concrete crypto engine may be native if required.
- Native crypto ABI must be narrow and versioned.
- Test vectors and Android parity tests are mandatory before beta.

## Group / Channel / Private Comments

Live privacy model:

- `server_encrypted_v1`
- signed `enc=4` envelopes
- `private_server` media/storage contract

Rules:

- Do not resurrect historical sender-key/private-group Android residue.
- Do not persist client transport secrets in private structured descriptors.
- Do not treat group/private-channel media as direct E2EE media.

## Local Database Protection

Beta gate:

- Chat history, outbox, media descriptors and sync state must be encrypted at rest or contain only encrypted payloads with DPAPI-protected keys.
- Token/key material must not be inside the ordinary DB without separate protection.
- Local DB migration tests must prove old incompatible residue is ignored safely.

Preferred approach:

```text
DPAPI / Credential Locker
  protects local master key

Encrypted SQLite or encrypted payload columns
  protects chat history and sensitive metadata

App-private cache
  stores media temp files, cleared by cache policy
```

## Media Security

Direct 1x1:

- media remains client-encrypted;
- descriptors may carry direct transport crypto fields where current contract allows;
- local decrypt must stream to app-private cache when platform playback requires a file.

Group/channel/comments:

- storage mode is `private_server`;
- descriptors must not include direct `k/iv`;
- access is server-authorized through grant/stream.

All modes:

- Do not read large remote media fully into memory.
- Notification previews must not reconstruct large/encrypted media.
- Cache loss must use descriptor-aware repair, not legacy URL fallbacks.

## Notifications

Privacy boundary:

- No plaintext message body in cloud push payloads.
- Metadata-only wake/sync signal if WNS or another cloud push is added.
- Client can show plaintext only after local fetch and local decrypt, and only if user privacy settings allow previews.
- Lock-screen previews obey "hide message content" setting.

## TLS and Network Security

- System trust TLS validation is mandatory.
- Dev contour may disable pinning only if documented and explicit; it must not disable TLS validation.
- Production certificate pinning should be evaluated before beta, matching Android's pinning posture where feasible for Windows.
- HTTP logging must redact sensitive headers and message bodies.

## Native Code Rules

Native C/C++ modules:

- are allowed only behind narrow interfaces;
- must not parse arbitrary backend JSON unless unavoidable;
- must be fuzz/testable for untrusted binary/media input;
- must not own auth/session state;
- must expose exception-free ABI boundaries;
- must be built with modern compiler hardening flags.

## Feedback and Diagnostics

Feedback reports may include:

- app version/build;
- OS version;
- feature state;
- sanitized logs;
- optional user-selected screenshots.

Feedback reports must not include:

- tokens/passwords/TOTP secrets;
- private keys;
- raw backup payloads;
- plaintext message bodies unless user explicitly attaches a screenshot containing them;
- unredacted local file paths.

## Security Release Gates

- No bearer tokens in URLs verified by tests/log scan.
- Notification payload privacy verified with backend/client evidence.
- Local DB encryption/protection verified by filesystem inspection and code tests.
- Direct E2EE Android interoperability verified.
- Backup restore imports no fake `Attachment` placeholders or unresolved local transport rows.
- Native modules have unit tests and memory-safety review before release.

