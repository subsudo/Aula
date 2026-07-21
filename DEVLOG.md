# DEVLOG

Gemeinsames Handover zwischen Codex und Claude. Am Ende jeder Session
aktualisieren. Kurz halten.

## Aktueller Stand

Aula **v0.1.7**, WPF/.NET 8, komprimiertes Single-File (self-contained win-x64).
Acta-Basis mit eingezogener Scola-Engine (`VerlaufsakteApp`). Notizen-Funktion
entfernt, Theme „2a" (warm/hell, kein Dark Mode). Auto-Update über GitHub-Releases.
Zwei Word-Engines (Acta / Scola) serialisiert über `XHub.Shared.WordAccessGate`.

## Letzte Änderungen

- 2026-07-21 (v0.1.7): Zwei terminierende WPF-Abstuerze mit
  `UCEERR_RENDERTHREADFAILURE` aus dem Tageslog adressiert: Aula verwendet nun
  prozessweit Software-Rendering und protokolliert Rendering-Modus/-Tier beim
  Start. Der Scola-Klassenimport nutzt nicht mehr einen eigenen unvollstaendigen
  Ordnerscan, sondern exakt den bereits geladenen Acta-TN-Index. Der eindeutige
  Tokenabgleich ist reihenfolgeunabhaengig und entfernt bei Bedarf nachgestellten
  Freitext; damit wird insbesondere `Jovana Vukicevic-Kljajic` dem Acta-Ordner
  `Vukicevic-Kljajic Jovana` zugeordnet.
- 2026-07-19: Separate App Einteilung BU **v0.1.24** verschmälert das rechte
  Detailpanel von 372 auf 316 Pixel, also um rund 15 %. Empfehlungstext und
  Themenzeilen wurden für die geringere Breite kompakt gehalten; fachliche Logik
  und Datenbindung bleiben unverändert.
- 2026-07-19: Separate App Einteilung BU **v0.1.23** gestaltet den rechten
  Detailbereich nach `Designvorschlag 2` neu: Themenbox mit BU-Verteilungsbalken,
  separates Prüfmodusfeld, kompakter Wochenplan, Beratungsbox, dauerhaft
  sichtbare Notiz und Farbmarkierungs-Chips. TN-Karten-Tags sind nun etwas
  dunkler als die Kartenfläche. Ausschliesslich das UI-Projekt wurde geändert;
  Core und Business-Logik blieben unverändert.
- 2026-07-18: Separate App Einteilung BU **v0.1.22** hebt Status-Tags von
  Kartenflächen ab. Tags werden nochmals heller pastellisiert und erhalten eine
  klarere statusfarbene Kontur; besonders `AM` wirkt dadurch leichter. Die
  Excel-Originalfarben und der Rückschrieb bleiben unverändert.
- 2026-07-18: Separate App Einteilung BU **v0.1.21** trennt Status und
  Bewertung visuell eindeutig. Excel-Statusfarben erscheinen in der App mit
  72 % Weiss pastellisiert, bleiben beim Rückschrieb aber originalgetreu.
  Korrekte und fixierte statuslose Karten sind neutral; der grüne Haken bedeutet
  allein `kein Handlungsbedarf`, sein Innenring kennzeichnet eine Fixierung.
  `NUR BI` im Pool orientiert sich nun am pastelligen Excel-Blau. Orange
  Umteilungswarnungen und der 7-Pixel-Statusbalken bleiben erhalten.
- 2026-07-18 (v0.1.6): Scola-Klassenimport bezieht nun denselben aktiven
  Austrittsbereich wie der Acta-Index ein. Dadurch werden verschachtelte
  TN-Ordner unter `03_Austritt\031_im Austritt` ebenfalls gematcht; konkret ist
  der in den Logs belegte Fall `Hawa Bangoura` behoben. Vorhandenes
  reihenfolgeunabhängiges Token-Matching und das Entfernen nachgestellter
  Freitexte wie `Stipendien` über den eindeutigen Ordner-Fallback bleiben aktiv.
- 2026-07-18: Separate App Einteilung BU **v0.1.20** setzt die geklärte
  Warnkarten-Hierarchie um: `Umteilen` dominiert mit orangefarbener Fläche und
  Rahmen; eine vorhandene Excel-Statusfarbe bleibt als überlagerter 7-Pixel-
  Balken links und im Statuslabel sichtbar, ohne den Namen zu verschieben.
  Reine BI-Teilnehmende im Pool tragen explizit den kompakten Tag `NUR BI`.
- 2026-07-18: Separate App Einteilung BU **v0.1.19** führt gekürzte
  Excel-Namen bei eindeutigem Token-Subset-/Präfixmatch auf den vollständigen
  TN-Aktennamen zurück, ohne den Excel-Text zu verändern. Dadurch entfallen
  künstliche Doppelpersonen bei Maria Schüpfer, João Santos, Kovelilan und Levin;
  echte Mehrdeutigkeiten bleiben offen. Neues Nutzer-Icon ist in EXE und beiden
  Fenstern eingebunden.
- 2026-07-18: Logdiagnose Einteilung BU: Gekürzte Excel-Namen werden aktuell
  zusätzlich zu den vollständigen TN-Ordnernamen als zweite Person an Aulas
  Alias-Matcher übergeben. Dadurch entstehen künstliche Mehrdeutigkeiten wie
  `Schüpfer Maria Alexandra Lara`/`Maria Schüpfer` und
  `Santos Carvalho Casaca João Pedro`/`João Santos Carvalho Casaca`.
  Empfohlene nächste Änderung: Excel-Name vor dem Stundenplanabgleich über einen
  eindeutigen Token-Subset-/Präfixvergleich mit dem kanonischen TN-Ordner
  verknüpfen; nur bei genau einem Kandidaten automatisch übernehmen.
- 2026-07-18: Separate App Einteilung BU **v0.1.18** bindet Häkchen beim
  Excel-Rückschrieb an BU-Datum plus zugehörige Ziele-Spalte statt an die
  Spaltenposition. Statusfarben ändern nur noch die Füllung; Name bleibt
  linksbündig und normalgewichtig. Historische Warnlogik ist an die UI
  angeschlossen, neutrale Wochen werden übersprungen, Gleichstände korrekt
  grün/hellgrün und echte Umteilungswerte orange dargestellt. Dezente
  `NAME?`-/`PLAN?`-Hinweise sowie neutraler Fixmodus mit Innenring ergänzt.
- 2026-07-18: Separate App Einteilung BU **v0.1.17** erhöht ausschliesslich den
  vertikalen Kachelabstand von 5 auf 7 Pixel; Höhen bleiben unverändert bei
  32 Pixel beziehungsweise 42 Pixel für Hinweiskacheln. Prüfmodus-Schalter sind
  kleiner und als kompakte Rechtecke mit 5-Pixel-Abrundung gestaltet.
- 2026-07-18: Separate App Einteilung BU **v0.1.16** vereinheitlicht normale
  und Pool-Kacheln auf 32 Pixel, reduziert Hinweiskacheln auf 42 Pixel und
  vergrössert den vertikalen Abstand auf 5 Pixel.
- 2026-07-18: Separate App Einteilung BU **v0.1.15** erhöht den zu knappen
  vertikalen Abstand zwischen allen TN-Kacheln von 1 auf 3 Pixel und flacht die
  Kacheln zugleich leicht auf 36/46 Pixel beziehungsweise 40 Pixel im Pool ab.
- 2026-07-18: Separate App Einteilung BU **v0.1.14** erhöht die Karten bei nur
  einem Pixel Abstand, entfernt Karten-Hover und den grossen Detailchip
  `zugeteilt bei …`. Notizen haben bei knapper Breite Vorrang vor dem Namen.
  Umteilungskandidaten zeigen dauerhaft orangefarbenen Rand plus `!` und im
  Detail die BU-Verteilung. Neuer persistenter Prüfmodus
  `Auto / Fix CN / Fix MM / Fix AN`; Fixierungen unterdrücken Empfehlungen,
  bleiben über Neustarts erhalten und `Auto` hebt sie ohne Verschiebung auf.
- 2026-07-18: Separate App Einteilung BU **v0.1.13** entfernt die Schatten der
  TN-Kacheln und verdichtet Höhen/Abstände weiter. Statuslabels sind kleiner,
  weniger fett und matter. Eine manuelle Umteilung zwischen Lehrpersonen setzt
  automatisch `Neu umgeteilt`; auf der Kachel steht eindeutig `UMGETEILT`.
  Undo/Redo stellt Position und vorherigen Status gemeinsam wieder her.
- 2026-07-18: Separate App Einteilung BU **v0.1.12** behebt den sofortigen
  StackOverflow beim Anklicken eines zugeteilten TN. Statt eines versehentlichen
  rekursiven Selbstaufrufs wird der Auswahlzustand korrekt gesetzt. Globales
  Logging für unbehandelte Fehler ergänzt; gezielter UI-Automationstest öffnete
  eine reale TN-Detailansicht und der Prozess blieb stabil.
- 2026-07-18: Separate App Einteilung BU **v0.1.11** aktiviert das sichere
  Excel-Speichern. TN werden als Fünferblock aus Name plus vier Checkboxen
  sortiert/umgeteilt; Werte, boolescher Typ und Spezialstil bleiben erhalten.
  Die sichtbare Sortierung wird nach Excel geschrieben. TN können reversibel in
  `Nicht zugeteilt` entfernt und samt Häkchen wieder zugeteilt werden. Auswahl ist
  visuell gerahmt; Karten sind kompakter und Statuslabels eckiger. Smoke-Test und
  Roundtrip auf temporärer Kopie der realen Excel erfolgreich; Original blieb
  unangetastet.
- 2026-07-18: Separate App Einteilung BU **v0.1.10**: Statusfarben füllen nun
  die vollständige abgerundete TN-Karte. Statuschip und eigener grüner
  Bestätigungshaken stehen kollisionsfrei rechts; Karten erhalten einen günstigen
  Flächenschatten. Sehr schmalen Aula-artigen Scrollbalken ergänzt. Neue globale
  Sortierung nach manueller Reihenfolge, Farbmarkierung oder Alphabet; die
  manuelle Reihenfolge ist innerhalb jeder Spalte per Drag-and-drop änderbar.
- 2026-07-18: Separate App Einteilung BU **v0.1.9** visuell nach dem
  Designvorschlag `1a` überarbeitet, ohne bestehende Funktionen zu entfernen.
  Hanken Grotesk ist lokal eingebettet; die Detailansicht zeigt BU/PR/MO/Konv.
  als CN/MM/AN-Matrix und trennt korrekt, Umteilung prüfen, ohne BU sowie
  extern/dispensiert visuell. Manuelles Fixieren bleibt eine bewusste Aktion und
  ist per Undo/Redo umkehrbar. Der App-Core blieb unverändert; Release-Build,
  Smoke-Tests und Starttest erfolgreich.
- 2026-07-17: Einteilung BU **v0.1.8** grundlegend übersichtlicher aufgebaut.
  Kein separater Tab und kein horizontales Scrollband mehr: Nicht zugeteilt,
  CN, MM und AN sind vertikale Spalten; Details rechts wurden von Kommentar,
  Farblegende und Punkt-/Haken-Symbolik befreit. Alle TN zeigen aktuelle
  Fächer- und CN/MM/AN-Wochenzählungen. Mini-Plan entspricht nun Aulas Anordnung
  mit Fach oben sowie Lehrperson/Zimmer nebeneinander. Release, Smoke-Tests und
  Standalone-Starttest erfolgreich.
- 2026-07-17: Einteilung BU **v0.1.7** als vollständig self-contained,
  komprimierte Windows-x64-Einzel-EXE vorbereitet. Eigenes Publish-Profil
  eingebaut; kein installiertes .NET auf dem Zielrechner erforderlich.
  Publish erfolgreich (ca. 71,7 MB).
- 2026-07-17: Einteilung BU auf **v0.1.6** erweitert. Vereinfachte eigene
  Stundenplan-/Aliaslogik durch die aus Aula kopierte `WeeklyScheduleService`-
  Engine ersetzt (Herkunft festgehalten). Aula-nahe Wochenraster-Darstellung,
  Foto, Beratungsperson und Odoo-Button ergänzt. Nicht zugeteilte TN liegen nun
  im Hauptbildschirm in einer Drag-and-drop-Ablage und können direkt CN/MM/AN
  zugewiesen werden. Eigenständiger Release-Build und Smoke-Tests grün;
  Aula-Kontrollbuild mit deaktiviertem Offline-NuGet-Audit 0 Fehler/0 Warnungen.
- 2026-07-17: Einteilung BU auf **v0.1.5** erweitert. TN-Index berücksichtigt
  nur direkte Namensordner, `05_Timeout` und `03_Austritt\031_Im Austritt`.
  Archiv, Vorlage, Start, Lehrbegleitung und alle übrigen Austrittsordner werden
  ausgeschlossen. `031_Im Austritt` bleibt ohne Stundenplan-/Umteilungsvorschlag
  sichtbar. Ordnernamen werden reihenfolgeunabhängig mit Excel abgeglichen;
  Mini-Pläne versuchen zusätzlich nur global eindeutige Einzelaliase.
- 2026-07-17: Einteilung BU auf **v0.1.4** erweitert. Produktivtest zeigte eine
  gefundene KW-Datei ohne TN-Beobachtungen, einen lange laufenden
  Nicht-zugeteilt-Abgleich und farbige Kacheln mit falschem Label `Ohne Status`.
  Status wird nun zusätzlich über die Füllfarbe erkannt; die DOCX wird einmal
  geparst und gecacht. Neues Tageslog samt `Logordner`-Knopf protokolliert
  Ladezeiten, Pfade, Beobachtungen, Dokumentstruktur und Namenskandidaten.
- 2026-07-17: Einteilung BU auf **v0.1.3** erweitert: Zusatz/Freitext und
  Farbmarkierung sind im rechten Detailpanel bearbeitbar. Die sieben Farben aus
  der Excel-Legende werden dort als eigene Legende gezeigt. Kacheln reagieren
  sofort; Undo/Redo umfasst nun Umteilung, Freitext und Statusfarbe. Noch kein
  Excel-Schreibzugriff.
- 2026-07-17: Einteilung BU auf **v0.1.2** erweitert: TN-Kacheln sind jetzt
  einzeilig und 31 Pixel hoch; der Farbcode füllt jeweils die ganze Kachel.
  Drag-and-drop zwischen CN, MM und AN sowie Undo/Redo sind im App-Arbeitsstand
  aktiv. Die Änderung wird bewusst noch nicht in Excel gespeichert und kann per
  Neuladen verworfen werden.
- 2026-07-17: Einteilung BU auf **v0.1.1** erweitert: Alle Personen aus den
  konfigurierten Teilnehmerakten, die nicht in der Zuteilungs-Excel vorkommen,
  werden im Kommentarfeld und im Reiter `Nicht zugeteilt` aufgeführt. Aktive BU
  erhält `BU – noch einzuteilen`, reine BI-Einteilung `Nur BI`, übrige Fälle
  `Ohne BU`. Extern/dispensiert löst keinen BU-Einteilungstrigger aus. Build 0/0
  und alle eigenständigen Smoke-Tests erfolgreich.
- 2026-07-17: Eigenständigen Prototyp **Einteilung BU v0.1.0** als separate
  Solution ohne Aula-Projektverweis unter `.einteilung-bu-staging` erstellt.
  Excel-Roundtrip mit Konfliktprüfung, sieben Backups, minimaler Zelländerung und
  Re-Validierung ist als Core-Service umgesetzt und durch eine künstliche
  Arbeitsmappe abgesichert. Dazu kommen DOCX-Wochenplanleser, BU-/Neutral-
  Auffälligkeitslogik, drei Zuteilungsspalten, Einstellungen, Detailraster und
  eine erste Nur-BI-Liste. Die Oberfläche bleibt in v0.1.0 lesend; Bearbeitung,
  Drag-and-drop, Undo/Redo und Teilnehmerakten-Metadaten sind nächste Schritte.
  Der gewünschte Zielordner `C:\Users\chris\Desktop\Einteilung BU` war in der
  laufenden Sandbox nicht beschreibbar; das Projekt kann später 1:1 dorthin
  verschoben werden. Keine Änderung an Aula selbst und keine Versionsänderung.
- 2026-07-16: Vorhaben „Berichtsverantwortung“ fachlich aufgegleist und bestehende
  `Zuteilungsliste_BU.xlsx` analysiert: drei Verantwortliche (CN/Christoph,
  MM/Marvin, AN/Andrea), sieben Farbcodes sowie monatliche BU-/Ziel-Häkchen.
  Nutzerentscheid: eigenständige zweite App, ausdrücklich keine Integration in Aula.
  Noch keine App-Funktion umgesetzt und daher keine Versionsänderung.
- 2026-07-10: Separates Handover-Dokument für das Vorhaben FuturX-Weiterbildungsradar
  erstellt: Quellen, Budgetlogik, FuturX-Profil, Gmail-Kontostatus und nächste Schritte
  stehen in `FuturX-Weiterbildungsradar-Handover.md`.
- 2026-07-10 (v0.1.5): Detailliertes Word-Lifecycle-Logging für Ghost-Dokumente
  eingebaut, standardmäßig deaktiviert und als Benutzeroption in den Einstellungen
  verfügbar. Bei Aktivierung protokollieren beide Engines Word-PIDs/Fenster und
  offene Dokumente mit Name, FullName, Path, Saved und ReadOnly in einer gemeinsamen
  `app-YYYY-MM-DD.log`-Datei. Das Logging ändert das Word-Verhalten nicht.
- 2026-07-18: Die Logs `app-2026-07-16.log` und `app-2026-07-17.log` ausgewertet.
  Ghost-Dokumente sind belegt: u. a. `Dokument8/15/23` am 16.07. sowie `Dokument1`
  am 17.07. blieben nach `AfterCloseTransientEmptyDocuments` bis zum Ende der
  Operation offen. Die Scola-Engine schließt ein Dokument nur bei
  `IsUnsaved && !IsActive && IsHardEmpty`; bei mehreren Ghosts war
  `IsHardEmpty=False`. Zudem durchsucht Scola beim Klassenimport nur
  `ServerBasePath`/den optionalen zweiten Pfad, während der Acta-Index den
  `ExitBasePath` separat einbezieht. Dadurch konnte `Hawa Bangoura` in Acta
  gefunden werden, in Scola aber nicht; der Ordner liegt unter
  `...\03_Austritt\031_im Austritt\Bangoura Hawa`. `Stipendien` wurde in den
  Logs als Namensanhang sichtbar, war in diesem Fall aber nicht die Hauptursache.
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

- **Berichtsverantwortung**: separates App-Vorhaben mit Excel-Quelle,
  Statusbearbeitung, Auffälligkeitsmarkierung, TN-Detail/Stundenplan und späterem
  Drag & Drop; Aula bleibt davon unangetastet. Stundenplanlogik: BU hat Vorrang;
  PR/MO sind ergänzende Signale; BI/LB sind ausgeschlossen. „Extern“ und
  „dispensiert“ sind neutrale, nicht beurteilbare Beobachtungen und dürfen keinen
  Trigger auslösen. Eine aktuelle abweichende BU-Einteilung ist sofort ein Hinweis;
  dieselbe Auffälligkeit in zwei aufeinanderfolgenden Kalenderwochen ist bereits
  stark. Eine Woche mit nur „extern“/„dispensiert“ zählt nicht als zweite Woche.
  Zusätzlich ist eine eigene Übersicht für TN ohne BU-Einteilung gewünscht, inklusive
  Nur-BI- und anderer Fachzuteilungen. App-Änderungen müssen mit Konfliktprüfung exakt
  in dieselbe Excel-Datei zurückgeschrieben werden; Excel-Änderungen müssen durch
  Neuladen wieder in die App gelangen. Nur ein finales Speichern aus der App erzeugt
  zuvor ein Backup im Unterordner `Backup Zuteilungsliste BU`; maximal sieben
  Versionen. Undo/Redo und Wiederherstellung einer Dateiversion sind gewünscht.
- **Ghost-Dokumente**: Die am 21.07. protokollierten `Dokument7/39/47` waren
  nach Rueckmeldung des Nutzers selbst geoeffnete Dokumente und keine
  beobachteten Ghosts. Deshalb keine automatische Bereinigung auf Basis dieser
  Eintraege; Word-Lifecycle-Logger bei einem echten neuen Vorfall erneut nutzen.
- **Stundenplan-Zuordnung** mehrdeutig bei gleichen Vornamen (`Mohammad M/A/R`).
  Vorschlag: Tie-Breaker, der Vornamen-Initialen bevorzugt. **Noch nicht entschieden.**

## Nächste Schritte

- Den separaten Staging-Ordner nach `C:\Users\chris\Desktop\Einteilung BU`
  verschieben und dort als eigenes Repository führen. Danach Editierbefehle mit
  Undo/Redo, Änderungsübersicht, Drag-and-drop und Speichern an den bereits
  getesteten Excel-Schreibkern anbinden. Anschließend Teilnehmerakten-Abgleich
  für Odoo, Beratungsperson und Foto ergänzen.
- v0.1.7 mit Software-Rendering und dem gemeinsamen Acta/Scola-Namensindex lokal
  testen; Ghost-Cleanup nur bei einem erneut beobachteten echten Vorfall angehen.
- Danach GitHub-Release v0.1.5 ziehen.
- Entscheidung zum Stundenplan-Tie-Breaker einholen und ggf. umsetzen.

## Test-Status

- 2026-07-21: Aula v0.1.7 Release-Build 0 Fehler / 0 Warnungen. Isolierter
  Namens-Smoke-Test gegen einen kuenstlichen Acta-Index erfolgreich fuer
  `Jovana Vukicevic-Kljajic`/`Vukicevic-Kljajic Jovana`, nachgestellten
  Freitext (`Hawa Bangoura Stipendien`), gekuerzte lange Namen,
  Klammer-Spitznamen und bewusst mehrdeutige Dubletten. Self-contained
  Single-File-Publish erfolgreich: `Aula.exe`, 145'602'038 Bytes, SHA-256
  `22C794B7369BD5B6FCA2923B4507F48E0CF115F9B9B5AE3FA35AF7CB70A7DA58`.
- 2026-07-19: Einteilung BU v0.1.24 gebaut → 0 Fehler / 0 Warnungen; alle
  Smoke-Tests und Aula-Gegenbuild ebenfalls erfolgreich. Standalone-EXE:
  72'404'360 Bytes, SHA-256
  `E9C667178135A3582948F2722333A17D853651A49F67197D1679310761319F8C`.
- 2026-07-19: Einteilung BU v0.1.23 gebaut → 0 Fehler / 0 Warnungen; alle
  Smoke-Tests und Aula-Gegenbuild ebenfalls erfolgreich. Automatisierter
  UI-Test wählte eine reale TN-Kachel und den Status-Chip `Neu umgeteilt`; App
  blieb stabil und die Excel hashgleich. Standalone-EXE: 72'404'369 Bytes,
  SHA-256 `F16FD80C7A4B3236FF10FFDB6B20E979587ED8545BBAB212EAEAC02D117339CD`.
- 2026-07-18: Einteilung BU v0.1.22 gebaut → 0 Fehler / 0 Warnungen; alle
  Smoke-Tests und Aula-Gegenbuild ebenfalls erfolgreich. Standalone-EXE:
  72'401'968 Bytes, SHA-256
  `EC47ACEE0B5713E3E97E888C45D2506E6F8365A9503F3C110A0F3598BC3B2464`.
- 2026-07-18: Einteilung BU v0.1.21 gebaut → 0 Fehler / 0 Warnungen; alle
  Smoke-Tests und Aula-Gegenbuild ebenfalls erfolgreich. Die reale Excel blieb
  hashgleich. Standalone-EXE: 72'401'818 Bytes, SHA-256
  `609CEA8C1074C27A6BEC1C7CF836CD9C4D62D63D87685C50743BAE1D2983AFFB`.
- 2026-07-18: Aula v0.1.6 Namenszuordnung mit künstlicher aktiver/Austritts-
  Ordnerstruktur geprüft: `Hawa Bangoura Stipendien`, umgekehrte Reihenfolge,
  verkürzter langer Name und Klammer-Spitzname jeweils eindeutig gefunden.
  Release-Build 0 Fehler / 0 Warnungen; self-contained Single-File-Publish
  `publish\aula-selfcontained-win-x64\Aula.exe` erfolgreich.
- 2026-07-18: Einteilung BU v0.1.20 gebaut → 0 Fehler / 0 Warnungen; alle
  Smoke-Tests und Aula-Gegenbuild ebenfalls erfolgreich. Standalone-EXE:
  72'401'703 Bytes, SHA-256
  `64F0C0CD772CCE180E7B5AFC4986301E612507E537D16C6FA5D66B024611EF96`.
- 2026-07-18: `dotnet build Aula.csproj -c Release -p:NuGetAudit=false`
  erfolgreich → 0 Fehler / 0 Warnungen. Der normale Build hatte zusätzlich nur
  `NU1900`, weil die NuGet-Sicherheitsdaten offline nicht erreichbar waren.
- 2026-07-18: Einteilung BU v0.1.19 gebaut → 0 Fehler / 0 Warnungen; neue
  Kurzname-, Präfix-, Mehrdeutigkeits-, Pool- und Stundenplankürzel-Tests sowie
  visuelle Prüfung des aus der Standalone-EXE extrahierten Icons erfolgreich.
- 2026-07-18: Einteilung BU v0.1.18 gebaut → 0 Fehler / 0 Warnungen; Fixture-
  Smoke-Tests, temporärer Roundtrip und visuelle Prüfung einer Kopie der echten
  Excel, Standalone-Start sowie automatisierter TN-Auswahl-/Detailpanel-Test
  erfolgreich. Original-Excel unverändert. Aula-Gegenbuild 0/0.
- 2026-07-18: Einteilung BU v0.1.17 gebaut → 0 Fehler / 0 Warnungen; alle
  Smoke-Tests und UI-Prüfung der vier kompakteren Prüfmodus-Schalter erfolgreich.
- 2026-07-18: Einteilung BU v0.1.16 gebaut → 0 Fehler / 0 Warnungen; alle
  Smoke-Tests und UI-Messung der 32-Pixel-Kacheln mit 5-Pixel-Abstand bei
  150-%-Skalierung erfolgreich; Auswahl stabil. Aula-Gegenbuild 0/0.
- 2026-07-18: Einteilung BU v0.1.15 gebaut → 0 Fehler / 0 Warnungen; alle
  Smoke-Tests und visueller Test des 3-Pixel-Kartenabstands erfolgreich;
  Aula-Gegenbuild ebenfalls 0 Fehler / 0 Warnungen.
- 2026-07-18: Einteilung BU v0.1.14 gebaut → 0 Fehler / 0 Warnungen; alle
  Smoke-Tests, visueller TN-Auswahltest und isolierter Fix-Persistenztest über
  einen vollständigen Neustart erfolgreich. Temporäre Einstellung entfernt;
  Aula-Gegenbuild ebenfalls 0 Fehler / 0 Warnungen.
- 2026-07-18: Einteilung BU v0.1.13 gebaut → 0 Fehler / 0 Warnungen; alle
  Smoke-Tests und automatisierter UI-Drag-and-drop zwischen Lehrpersonen grün,
  Statuslabel `UMGETEILT` sichtbar und App stabil.
- 2026-07-18: Einteilung BU v0.1.12 gebaut → 0 Fehler / 0 Warnungen; alle
  Smoke-Tests sowie automatisierter Klick auf eine reale TN-Kachel erfolgreich,
  Detailansicht sichtbar und App weiterhin aktiv.
- 2026-07-18: Einteilung BU v0.1.11 gebaut → 0 Fehler / 0 Warnungen; Smoke-Test
  inklusive aller vier Checkboxwerte/-stile, Sortierung und Entfernen erfolgreich.
  Zusätzlicher Roundtrip auf temporärer Kopie der realen Excel sowie
  Standalone-Start erfolgreich; Aula-Gegenbuild 0 Fehler/0 Warnungen.
- 2026-07-17: Einteilung BU v0.1.5 gebaut → 0 Fehler / 0 Warnungen; alle
  Smoke-Tests inklusive künstlicher TN-Ordnerstruktur und Aliasfällen erfolgreich.
- 2026-07-17: Einteilung BU v0.1.4 gebaut → 0 Fehler / 0 Warnungen; alle
  Smoke-Tests erfolgreich.
- 2026-07-17: Einteilung BU v0.1.3 gebaut → 0 Fehler / 0 Warnungen; alle
  Smoke-Tests erfolgreich.
- 2026-07-17: Einteilung BU v0.1.2 gebaut → 0 Fehler / 0 Warnungen; alle
  Smoke-Tests erfolgreich. Aula-Gegenbuild ebenfalls 0 Fehler / 0 Warnungen.
- 2026-07-17: Einteilung BU v0.1.1 erneut gebaut → 0 Fehler / 0 Warnungen;
  neue Tests für nicht zugeteilte BU-, Nur-BI-, sonstige und dispensierte Fälle
  erfolgreich. `dotnet build Aula.csproj -c Release` anschließend ebenfalls
  erfolgreich → 0 Fehler / 0 Warnungen.
- 2026-07-17: `dotnet build .einteilung-bu-staging\EinteilungBU.sln -c Release`
  → 0 Fehler / 0 Warnungen; alle eigenständigen Smoke-Tests erfolgreich.
- 2026-07-16: `dotnet build Aula.csproj -c Release` → 0 Fehler / 0 Warnungen
  nach der reinen Konzept-/DEVLOG-Session.
- 2026-07-10: `dotnet build Aula.csproj -c Release` → 0 Fehler / 0 Warnungen.
  Publish Single-File v0.1.5 ok (~139 MB); dabei nur `NU1900`, weil NuGet-
  Sicherheitsdaten offline nicht geladen werden konnten. Keine automatisierten
  Tests im Repo.
