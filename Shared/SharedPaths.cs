namespace XHub.Shared;

/// <summary>
/// Zentrale Server-Pfade, die Acta und Scola heute schon gemeinsam nutzen.
/// Aula verwendet bewusst dieselben Pfade, damit Bemerkungen geteilt bleiben.
/// </summary>
public static class SharedPaths
{
    /// <summary>
    /// Gemeinsamer Bemerkungen-(Hints-)Speicher. Identisch zu
    /// XHub.Services.ParticipantHintsService.DefaultStorePath und
    /// VerlaufsakteApp.Services.ParticipantHintsService.DefaultStorePath.
    /// Hints sind nach Dokumentpfad der Verlaufsakte verschluesselt.
    /// </summary>
    public const string ParticipantHintsStorePath =
        @"K:\FuturX\34_Bildung\02_Grundlagen\20_Arbeitsinstrumente\300_AppData_Scola_Acta\ParticipantHints\participant-hints.json";
}
