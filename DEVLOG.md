# DEVLOG

Gemeinsames Handover zwischen Codex und Claude. Am Ende jeder Session
aktualisieren. Kurz halten.

## Aktueller Stand

Aula **v0.1.4**, WPF/.NET 8, komprimiertes Single-File (self-contained win-x64).
Acta-Basis mit eingezogener Scola-Engine (`VerlaufsakteApp`). Notizen-Funktion
entfernt, Theme „2a" (warm/hell, kein Dark Mode). Auto-Update über GitHub-Releases.
Zwei Word-Engines (Acta / Scola) serialisiert über `XHub.Shared.WordAccessGate`.

## Letzte Änderungen

- 2026-07-10 (v0.1.4, Commit `aa0c02d`): Stundenplan ignoriert Word-Sperrdateien
  (`~$KW_28.docx`) im Datei-Scan; FolderMatcher ignoriert Spitznamen in Klammern
  (`Mohammad (Mohi) Reza Moradi`).
- 2026-07-10 (v0.1.3): Exe-Kompression, Tastatur (Ctrl+F fokussiert Suche, Esc
  leert/schließt), manueller Update-Check in den Einstellungen, Stundenplan-
  Wochen-Cache-Fallback (behält Pläne, wenn die Quelle fehlt, Woche aber läuft).
- Früher: Einstellungs-Redesign (kompakte Karten, ohne Scrollbalken), Notizen
  entfernt, Status-Balken auf volle Breite, feinere Wochenplan-Tabelle.

## Wichtige Entscheidungen

- Word-Engines bytegleich zu den Original-Apps halten (Risiko-Minimierung); nur
  über `WordAccessGate` gekapselt. Grund: die Word-COM-Konsolidierung ist die
  größte Korrektheitsfalle.
- Version bei jedem ausgelieferten Stand erhöhen (Auto-Update braucht höhere Nummer).

## Offene Punkte

- **Ghost-Dokumente**: vom Nutzer beobachtet, in den Logs aber nicht reproduzierbar
  (jeder Word-Zugriff zeigt `initialUnsaved=0`, `before=0/now=0`). Nächster
  Diagnoseschritt: beim Start einer eigenen Word-Instanz die Namen aller offenen
  Docs mitloggen. Noch nicht gebaut.
- **Stundenplan-Zuordnung** mehrdeutig bei gleichen Vornamen (`Mohammad M/A/R`).
  Vorschlag: Tie-Breaker, der Vornamen-Initialen bevorzugt. **Noch nicht entschieden.**
- `Ayhan Burcu`: deren Verlaufsakte fehlt das Lesezeichen `BU_BILDUNG_TABELLE`
  (Datei-/Vorlagenproblem, kein App-Bug).

## Nächste Schritte

- v0.1.4 lokal testen → danach GitHub-Release v0.1.4 ziehen.
- Entscheidung zum Stundenplan-Tie-Breaker einholen und ggf. umsetzen.
- Optional: Doc-Namen-Logging für die Ghost-Diagnose.

## Test-Status

- 2026-07-10: `dotnet build -c Release` → 0 Fehler / 0 Warnungen; `publish`
  Single-File ok (~146 MB). Keine automatisierten Tests im Repo.
