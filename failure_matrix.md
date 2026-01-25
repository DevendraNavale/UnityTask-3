# Failure Matrix

| Test Case | Action | Expected Result | Actual Result | Status |
|---------|-------|----------------|--------------|--------|
Rapid Restart | Spam Play/Stop | No crash, no dup managers | No issues | PASS |
Scene Reload Spam | Reload scene 10x | Single managers only | Singleton preserved | PASS |
Unit Cap Violation | Spawn beyond cap | Spawn blocked + log | Correct | PASS |
Projectile Cap Violation | Fire beyond cap | Projectile blocked | Correct | PASS |
No Energy Spawn | Force spawn | Rejected | Correct | PASS |
