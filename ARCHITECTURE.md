# Aula — vereinte „best-of"-App aus Acta + Scola

> **Hinweis:** Dieses Dokument beschreibt das **Design/den Plan**, nicht den
> Tagesstand. Aktueller Stand, offene Punkte und nächste Schritte stehen in
> [DEVLOG.md](DEVLOG.md). Der folgende Status-Absatz ist historisch (Stufe 1).

> Status: **Stufe 1 fertig** — Acta als Basis kopiert, baut als eigenständige App
> `Aula.exe`. Die Original-Apps *Acta* (`XHub\XHub\`) und *Scola Flash*
> (`Scola Flash\`) bleiben **unangetastet**. Aula liegt separat in `XHub\Aula\`.

## Ziel

Eine App, die das Beste aus beiden vereint. **Acta ist das Rückgrat** (TN-Index,
Suche, Listen, Detailbereich, Notizen, Bemerkungen). **Scolas Stärken werden
hineingepflanzt.**

## Gesperrter Scope

- **Scolas Funktionalität bleibt erhalten** — höchste Priorität: **Batch-Funktionen**
  (Liste/Rohtext → matchen, BU/BI/BE einfügen, positionsbasierte Batch-Einträge,
  BI-To-do-Sammeldokument).
- **Stundenplan**: nicht mehr separat, sondern im **Acta-Detailbereich** sichtbar.
- **Neu — Drag & Drop von TN in Acta**: gezogene TN ordnen sich **automatisch
  alphabetisch nach Namen** ein.
- **Neu — TN einzeln löschen**: kleines **×** pro TN in der Scola-Arbeitsliste
  (entfernt nur aus der Liste, löscht **nicht** die Akte auf dem Server).
- **Bemerkungen** bleiben gemeinsam (schon gelöst, siehe unten).
- **Gleiche Klick-Gesten** (z. B. Ctrl-Klick) auf beiden Seiten.

## Schlüsselbefund: Bemerkungen sind schon gemeinsam

Acta und Scola schreiben heute beide in **denselben** Hints-Speicher:

```
K:\FuturX\34_Bildung\02_Grundlagen\20_Arbeitsinstrumente\300_AppData_Scola_Acta\ParticipantHints\participant-hints.json
```

Hints sind **nach Dokumentpfad der Verlaufsakte** verschlüsselt → der Akten-Pfad ist
der app-übergreifende TN-Schlüssel (`SharedParticipant.DocumentPath`).

## Technische Basis (Stufe 1)

- `Aula\` = Kopie von Actas Projektquellen (Controls, Converters, Models, Services,
  Views, App, MainWindow). **Interner Namespace bleibt `XHub`** (wie bei Acta, wo
  Produkt „Acta" aber Namespace „XHub" heißt) → keine riskante Massen-Umbenennung.
- `Aula.csproj`: `AssemblyName/Product/Title = Aula`, Version 0.1.0. **Ohne** Actas
  eingebettete Updater-Logik (für v0.1 bewusst weggelassen; Start braucht sie nicht).
- App-Isolierung gegenüber Acta:
  - AppData-Ordner `%LOCALAPPDATA%\Aula` (statt `XHub`) → überschreibt Actas echte
    Einstellungen/Listen/Caches nicht.
  - Single-Instance-Mutex/Pipe `Aula.*` (statt `Acta.*`) → Aula und Acta können
    **parallel** laufen während der Migration.
- `Shared\` (für die Integrationsstufen vorbereitet): `SessionContext`
  (SelectedParticipant + MarkedInScola), `SharedParticipant` (Key = DocumentPath),
  `SharedPaths` (gemeinsamer Hints-Pfad).

## Best-of-Landkarte

| Bereich | Quelle | Ziel in Aula |
|---|---|---|
| TN-Index, Archiv, Kategorien | Acta | Rückgrat (vorhanden) |
| Suche, Listen, Notizen, Detailmodule | Acta | Rückgrat (vorhanden) |
| Liste/Rohtext → matchen | Scola | einpflanzen |
| BU/BI/BE springen + einfügen | Scola | einpflanzen |
| **Batch-Einträge** (Priorität) | Scola | einpflanzen |
| BI-To-do-Sammeldokument | Scola | einpflanzen |
| Mini-Stundenplan | beide (XHub-Stil) | in den Detailbereich |
| Word-COM | beide | EIN gemeinsamer WordStaHost |
| Bemerkungen | gemeinsam | bereits geteilt |

## Prinzip

> **Layout zuletzt, gemeinsamer Zustand zuerst.** Ein Word-Host, eine Config, eine
> TN-Auswahl. Die Word-Host-Konsolidierung ist die größte Korrektheitsfalle.

## Stufenplan

1. **Acta-Basis** ✅ — baut als `Aula.exe`, isoliert von Acta.
2. **Drag & Drop + Auto-Sortierung** im Acta-TN-Bereich.
3. **Scola-Quellen einziehen**: Parser, FolderMatcher, BiDocxExtraction, Batch,
   BI-To-do — unter eigenem Namespace `VerlaufsakteApp` in dieselbe Assembly.
4. **Scola-UI als Panel/Aktionen** in Aula (nicht als separates Fenster).
5. **× zum Entfernen** aus der Scola-Arbeitsliste.
6. **Mini-Stundenplan in den Detailbereich** verlegen.
7. **Word-Host & Config konsolidieren** (ein Host für beide Hälften).
8. **Klick-Gesten vereinheitlichen.**

## Was NICHT angefasst wird

- `XHub\XHub\` (Acta) und `Scola Flash\` (Scola) bleiben unverändert.
- `Aula\` liegt außerhalb von `XHub\XHub\`, damit Actas SDK-Glob es nicht einzieht.
