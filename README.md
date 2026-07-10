# Aula

Interne WPF-Desktop-App (.NET 8, Windows) für die TN-/Verlaufsakten-Arbeit —
vereint das Beste aus *Acta* (TN-Index, Suche, Detailbereich, Bemerkungen) und
*Scola* (Batch-Einträge, BU/BI/BE, BI-To-do, Stundenplan). Interner Namespace
bleibt `XHub`; die eingezogene Scola-Engine lebt im Namespace `VerlaufsakteApp`.

## Für KI-Assistenten (Codex & Claude)

Vor der Arbeit lesen: **[DEVLOG.md](DEVLOG.md)** (aktueller Stand & Handover) und
die Arbeitsregeln in **[AGENTS.md](AGENTS.md)**. Design-Hintergrund bei Bedarf in
[ARCHITECTURE.md](ARCHITECTURE.md).

## Bauen & Starten

```bash
# Kompilieren / prüfen (schneller Gegencheck)
dotnet build Aula.csproj -c Release

# Auslieferbare Exe (komprimiertes Single-File, self-contained win-x64)
dotnet publish Aula.csproj -c Release -r win-x64 --self-contained true \
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true \
  -o publish/aula-selfcontained-win-x64
```

Ergebnis: `publish/aula-selfcontained-win-x64/Aula.exe` (~146 MB).

## Tests

Kein automatisiertes Testprojekt. Verifikation = `dotnet build -c Release` (muss
0 Fehler/0 Warnungen liefern) + manueller Smoke-Test der geänderten Funktion.

## Version & Auto-Update

- Version steht in `Aula.csproj` (`<Version>`, `<AssemblyVersion>`, `<FileVersion>`,
  `<InformationalVersion>` — alle gleich halten).
- Auto-Update läuft über GitHub-Releases und reagiert **nur auf höhere
  Versionsnummern**. Jeder ausgelieferte Stand muss die Version erhöhen.
