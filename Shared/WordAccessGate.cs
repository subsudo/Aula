using System.Threading;

namespace XHub.Shared;

/// <summary>
/// Prozessweites Tor, das Word-COM-Operationen ueber BEIDE STA-Hosts hinweg
/// serialisiert (Actas <c>XHub.Services.WordStaHost</c> und der Batch-Host
/// <c>VerlaufsakteApp.Services.WordStaHost</c>). Verhindert, dass z. B. ein
/// laufender Batch und eine gleichzeitige Acta-Aktenaktion zur selben Zeit auf
/// dieselbe Word-Instanz zugreifen. Stufe-7-Konsolidierung ohne Umbau der
/// beiden WordService-Lifecycles.
/// </summary>
public static class WordAccessGate
{
    public static readonly SemaphoreSlim Gate = new(1, 1);
}
