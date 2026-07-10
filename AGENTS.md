# Arbeitsanweisung (Codex & Claude)

Diese Datei ist die **gemeinsame Regelquelle** für beide KI-Assistenten.
`CLAUDE.md` verweist nur hierher — Regeln also nur hier pflegen.

## Vor der Arbeit lesen

1. `README.md` — was das Projekt ist, wie man baut & testet.
2. `DEVLOG.md` — aktueller Stand, offene Punkte, nächste Schritte.

`DEVLOG.md` ist das **gemeinsame Handover** zwischen Codex und Claude. Es ist die
einzige Datei, die bei jeder Session aktualisiert wird.

## Regeln

- Änderungen **klein und nachvollziehbar** halten.
- **Keine Architektur erfinden**, die nicht im Code sichtbar ist. Design-Kontext
  steht in `ARCHITECTURE.md`.
- Am **Ende jeder Session `DEVLOG.md` aktualisieren**: nur relevante Änderungen,
  Entscheidungen, offene Punkte und nächste Schritte. Keine langen Erklärungen,
  keine Doppelungen über mehrere Dateien.
- Unsichere Punkte mit `Unklar:` markieren.
- Vor dem Abschluss `dotnet build Aula.csproj -c Release` laufen lassen (muss
  0 Fehler/0 Warnungen liefern). Wenn nicht ausführbar, in `DEVLOG.md` notieren.

## Projekt-Regeln (nicht verletzen)

- **Version bei jedem ausgelieferten Stand erhöhen** (`Aula.csproj`, alle vier
  Versionsfelder gleich). Auto-Update greift nur bei höherer Nummer.
- **`AssemblyName` (`Aula`) nicht ändern.**
- **Zwei getrennte Word-Engines nicht divergieren lassen:** `XHub.Services.WordService`
  (Acta-Seite) und die Scola-Engine unter `VerlaufsakteApp` sind bewusst nah an den
  Original-Apps. Änderungen am Word-COM-Verhalten nur mit klarem Grund und im DEVLOG
  festhalten.
- Kein GitHub-Release erstellen, solange der Nutzer nicht bestätigt hat, dass der
  Stand lokal getestet ist.
