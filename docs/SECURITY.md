# Спецификация безопасности

Обновлено: 2026-05-30

## Цели безопасности

- Сохранить privacy model Android-клиента на Windows.
- Оставить direct 1x1 сообщения end-to-end encrypted.
- Оставить группы, каналы и private comments на текущей trusted-server модели `server_encrypted_v1`.
- Защитить tokens, device keys, backups и локальную историю сообщений at rest.
- Не допустить утечки plaintext сообщений через notifications, logs, crash reports или cloud push providers.

## Threat model

Основные угрозы:

- украденный Windows user profile directory;
- malware/user-level process, читающий local app files;
- случайная утечка token/message через logs или feedback reports;
- network attacker;
- stale sync/outbox replay после reconnect;
- notification cloud provider, получающий plaintext content;
- memory-safety vulnerabilities в native module;
- downgrade или compatibility bugs, которые некорректно импортируют старый backup residue.

Не цели:

- защита от полностью скомпрометированной administrator/root машины;
- скрытие контента от активного signed-in Windows user;
- обход backend ACL decisions.

## Auth и token storage

- Access/refresh tokens хранятся в Windows Credential Locker или DPAPI-protected local storage.
- Tokens никогда не хранятся в plaintext config files.
- Tokens никогда не передаются в URL.
- Logs должны redact-ить `Authorization`, cookies, refresh tokens, one-time auth codes и 2FA backup codes.
- Logout удаляет tokens, active WebSocket credentials и push registration state.

## Device identity

Desktop-клиент имеет стабильный per-install device identifier для backend-контрактов, где Android использует registration/device IDs.

Правила:

- Store device ID в protected local storage.
- Не показывать raw device ID в обычном UI.
- Rotate только через явный account/device reset flow.
- Смена device ID влияет на key backup и считается security-sensitive.

## Direct 1x1 E2EE

Direct chats следуют текущей Android direct E2EE model:

- Signal/libsignal-compatible identity, signed pre-key, one-time pre-key и session behavior.
- Server хранит ciphertext и key directory metadata, не plaintext.
- Decryption failure - видимое локальное состояние, а не причина synthesise-ить fake plaintext.
- Key backup restore является same-device для direct private state.

Implementation:

- C# app зависит от `IPrivateChatCrypto`.
- Concrete crypto engine может быть native, если это требуется.
- Native crypto ABI должен быть узким и versioned.
- Test vectors и Android parity tests обязательны до beta.

## Group / Channel / Private Comments

Live privacy model:

- `server_encrypted_v1`
- signed `enc=4` envelopes
- `private_server` media/storage contract

Правила:

- Не resurrect-ить historical sender-key/private-group Android residue.
- Не persist-ить client transport secrets в private structured descriptors.
- Не трактовать group/private-channel media как direct E2EE media.

## Защита локальной базы

Beta gate:

- Chat history, outbox, media descriptors и sync state должны быть encrypted at rest или содержать только encrypted payloads с DPAPI-protected keys.
- Token/key material не должен быть внутри ordinary DB без отдельной защиты.
- Local DB migration tests должны доказывать, что old incompatible residue безопасно игнорируется.

Preferred approach:

```text
DPAPI / Credential Locker
  защищает local master key

Encrypted SQLite или encrypted payload columns
  защищает chat history и sensitive metadata

App-private cache
  хранит media temp files и очищается cache policy
```

## Media security

Direct 1x1:

- media остается client-encrypted;
- descriptors могут нести direct transport crypto fields там, где current contract это разрешает;
- local decrypt должен stream-ить в app-private cache, если platform playback требует файл.

Group/channel/comments:

- storage mode: `private_server`;
- descriptors не должны включать direct `k/iv`;
- access авторизуется server-side через grant/stream.

Все режимы:

- Не читать large remote media целиком в memory.
- Notification previews не должны reconstruct large/encrypted media.
- Cache loss должен идти через descriptor-aware repair, не через legacy URL fallbacks.

## Notifications

Privacy boundary:

- Никакого plaintext message body в cloud push payloads.
- Metadata-only wake/sync signal, если появится WNS или другой cloud push.
- Client может показать plaintext только после local fetch и local decrypt, и только если user privacy settings разрешают previews.
- Lock-screen previews уважают настройку "hide message content".

## TLS и network security

- System trust TLS validation обязательна.
- Dev contour может отключать pinning только явно и документированно; TLS validation отключать нельзя.
- Production certificate pinning нужно оценить до beta, сохраняя posture Android там, где это реально для Windows.
- HTTP logging должен redact-ить sensitive headers и message bodies.

## Правила native code

Native C/C++ modules:

- разрешены только за узкими interfaces;
- не должны parse-ить arbitrary backend JSON без необходимости;
- должны быть fuzz/testable для untrusted binary/media input;
- не владеют auth/session state;
- expose-ят exception-free ABI boundaries;
- собираются с modern compiler hardening flags.

## Feedback и diagnostics

Feedback reports могут включать:

- app version/build;
- OS version;
- feature state;
- sanitized logs;
- optional user-selected screenshots.

Feedback reports не должны включать:

- tokens/passwords/TOTP secrets;
- private keys;
- raw backup payloads;
- plaintext message bodies, кроме случая, когда user сам явно attached screenshot с таким содержимым;
- unredacted local file paths.

## Security release gates

- Тестами/log scan подтверждено, что bearer tokens не попадают в URL.
- Notification payload privacy подтверждена backend/client evidence.
- Local DB encryption/protection подтверждена filesystem inspection и code tests.
- Direct E2EE interoperability с Android подтверждена.
- Backup restore не импортирует fake `Attachment` placeholders или unresolved local transport rows.
- Native modules имеют unit tests и memory-safety review до release.

