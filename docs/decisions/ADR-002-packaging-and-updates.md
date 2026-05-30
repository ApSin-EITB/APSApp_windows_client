# ADR-002: Package Windows Client as MSIX First

## Status

Proposed

## Date

2026-05-30

## Context

Android has store and sideload-specific OTA behavior. Windows needs its own packaging and update policy. A desktop client must integrate with Windows notifications, identity, install/upgrade behavior and local data migration.

## Decision

Use MSIX as the first-class packaging target for beta and release. Allow unpackaged launch only for developer builds.

Update policy:

- P0 beta: manual signed MSIX distribution.
- P1: app-installer or owner-approved update channel if needed.
- Future: Microsoft Store only if owner chooses store distribution.

Android APK OTA rules do not apply directly to Windows.

## Alternatives Considered

### Raw executable / zip

Pros:

- fastest to start;
- simple for internal dev.

Cons:

- weaker install/update identity;
- harder notification/deep-link integration;
- easier to lose migration guarantees.

Allowed for local dev only, not release.

### MSIX

Pros:

- Windows-native packaging;
- clean install/upgrade identity;
- good fit for notifications, app links, future Store;
- safer uninstall behavior.

Cons:

- signing required;
- packaging constraints need early attention.

Accepted as release path.

### Microsoft Store From Day One

Pros:

- user-friendly updates;
- store trust/distribution.

Cons:

- extra compliance/review loop before client is stable;
- owner has not selected this channel yet.

Deferred.

## Consequences

- Release build must produce signed MSIX before beta.
- Local storage paths and migration must be tested across MSIX upgrades.
- Update UX must not copy Android OTA assumptions blindly.

