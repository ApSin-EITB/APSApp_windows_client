# ADR-001: Use C# WinUI 3 as Primary Stack with Native C/C++ Islands

## Status

Accepted

## Date

2026-05-30

## Context

The APSApp Windows client needs a native Windows desktop experience while preserving Android/backend product contracts: auth, direct E2EE, messenger sync, storage media, calls, settings, notifications and QA evidence.

The owner asked why not C++, then clarified that C# is acceptable while some C/C++ may be needed, and Qt also exists as an option.

## Decision

Use C# + WinUI 3 / Windows App SDK as the primary app and UI stack.

Allow C/C++ native islands for:

- WebRTC/media pipeline;
- crypto/libsignal bindings if managed options are insufficient;
- performance hot paths;
- low-level Windows interop where managed APIs are not enough.

Qt is not the primary UI stack. Revisit Qt only if the product goal changes to cross-platform desktop.

## Alternatives Considered

### All C++ / C++/WinRT

Pros:

- fully native;
- direct access to Windows APIs;
- useful for media and low-level interop.

Cons:

- higher memory-safety and development cost for app-level UI/state/auth/sync code;
- slower product iteration;
- more expensive tests and future agent maintenance;
- most APSApp client complexity is not CPU-bound.

Rejected as primary stack. Kept for native islands.

### C# + WinUI 3

Pros:

- native Windows UI stack;
- strong productivity for MVVM, async, DI, tests, settings and API clients;
- good Windows integration with MSIX, notifications, Credential Locker/DPAPI;
- easier for future agents/developers to maintain;
- native interop remains available.

Cons:

- may need native wrappers for libsignal/WebRTC/media;
- managed runtime is part of deployment story;
- some low-level APIs are less direct.

Accepted.

### Qt

Pros:

- mature UI toolkit;
- strong C++ ecosystem;
- cross-platform desktop potential.

Cons:

- not Windows-native first;
- adds a second UI/runtime abstraction;
- weaker alignment with WinUI, MSIX, Windows notifications and credential APIs;
- cross-platform is not the current product goal.

Rejected for the current Windows-native client.

## Consequences

- First solution skeleton should be C# WinUI.
- Native code must be narrow and explicitly justified.
- Public app policy and backend contract interpretation stay in managed domain code.
- Any future switch to Qt or all-C++ needs a new ADR and owner approval.

