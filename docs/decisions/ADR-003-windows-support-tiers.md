# ADR-003: Windows 11 как primary target и Windows 10 22H2 как compatibility tier

## Статус

Accepted

## Дата

2026-05-30

## Контекст

Windows-клиент должен быть нативным и качественным на Windows 10/11. При этом на дату 2026-05-30 Windows 10 Home/Pro уже вышла из обычного Microsoft support lifecycle 2025-10-14. Windows App SDK остается backward-compatible с Windows 10, но production support для клиента не должен игнорировать lifecycle операционной системы.

Нам нужно не просто написать "Windows 10/11 support", а явно разделить:

- где APSApp делает основной UX/release effort;
- где сохраняет compatibility для владельца и controlled QA;
- какие Windows 11 features можно использовать без деградации core behavior;
- когда Windows 10 получает fallback или `QA PARTIAL`.

## Решение

Windows 11 current supported releases, x64 - primary target, Tier A.

Windows 10 22H2 x64 - compatibility target, Tier B:

- core auth, messenger, media, calls, local lock, notifications activation и MSIX upgrade должны работать, если это возможно без security компромисса;
- Windows 11-only visual/system behavior получает Acrylic/solid/standard fallback;
- Windows 10 gaps фиксируются как compatibility fallback, `QA PARTIAL` или `QA BLOCKED`, но не отменяют Windows 11-native UX.

Windows 11 ARM64 - P2 future target после стабилизации x64.

Windows 10 pre-22H2, Windows 7/8/8.1 и Windows Server как desktop target - out of scope без нового owner decision.

## Последствия

- `WINDOWS_NATIVE_SPEC.md` становится source-of-truth для Windows 10/11 native behavior.
- Release QA обязана явно разделять Windows 11 Tier A и Windows 10 Tier B evidence.
- Core product/security parity нельзя ослаблять ради Windows 10 compatibility.
- Нельзя использовать Preview/Experimental Windows App SDK APIs в P0/P1 без нового ADR.
- Mica/Mica Alt, richer title bar и Windows 11 shell affordances можно использовать, если Windows 10 fallback documented и tested.
- Будущий отказ от Windows 10 compatibility или расширение до ARM64 требует ADR/update roadmap.
