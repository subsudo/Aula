#if DEBUG
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using XHub.Models;

namespace XHub;

/// <summary>
/// Nur fuer Tests (DEBUG): liefert Dummy-Teilnehmer fuer Actas Suche/Detail,
/// inkl. Dummy-Foto und Dummy-Stundenplan. Im Release-Build (Exe) komplett weg.
/// </summary>
internal static class DebugDummies
{
    private const string KeyPrefix = "dummy::";
    private static string? _photoPath;

    public static bool IsDummy(ParticipantIndexEntry entry) =>
        entry.ParticipantKey?.StartsWith(KeyPrefix, StringComparison.Ordinal) == true;

    public static List<ParticipantIndexEntry> CreateEntries()
    {
        var photo = EnsureDummyPhoto();
        return new List<ParticipantIndexEntry>
        {
            Make("Mustermann, Maximilian", "MaMu", photo, ("#3CB371", "Anwesenheit i.O."), ("#D17878", "Offen")),
            Make("Roth, Lea", "LeRo", photo, ("#C8A96C", "Hinweis")),
            Make("Abebe, Yonas", "YoAb", photo),
            Make("Schneider, Anna-Sophie", "AnSc", photo, ("#3CB371", "a"), ("#D17878", "b"), ("#C8A96C", "c")),
            Make("Li, Wei", "WeLi", photo),
        };
    }

    private static ParticipantIndexEntry Make(string name, string initials, string photo,
        params (string Color, string Text)[] hints)
    {
        var entry = new ParticipantIndexEntry
        {
            ParticipantKey = KeyPrefix + name,
            DisplayName = name,
            Initials = initials,
            FolderPath = string.Empty,
            DocumentPath = string.Empty,
            ImagePath = photo,
        };
        entry.ActiveHints = hints
            .Select(h => new ParticipantHintDisplay { MarkerColor = h.Color, Text = h.Text })
            .ToList();
        return entry;
    }

    public static ParticipantMiniScheduleSummary CreateSchedule()
    {
        var summary = new ParticipantMiniScheduleSummary { State = ParticipantMiniScheduleState.Ready };
        Add(summary, "Mo", ParticipantMiniScheduleHalfDay.Morning, "BU", "CN", "U3");
        Add(summary, "Mo", ParticipantMiniScheduleHalfDay.Afternoon, "BI", "MB", "U1");
        Add(summary, "Di", ParticipantMiniScheduleHalfDay.Morning, "DAZ", "AF", "U2");
        Add(summary, "Mi", ParticipantMiniScheduleHalfDay.Afternoon, "BU", "CN", "U3");
        summary.GetCell("Do", ParticipantMiniScheduleHalfDay.Morning).Status = ParticipantMiniScheduleCellStatus.External;
        summary.GetCell("Fr", ParticipantMiniScheduleHalfDay.Afternoon).Status = ParticipantMiniScheduleCellStatus.Dispensed;
        return summary;
    }

    private static void Add(ParticipantMiniScheduleSummary summary, string day,
        ParticipantMiniScheduleHalfDay half, string group, string teacher, string room)
    {
        summary.GetCell(day, half).Entries.Add(new ParticipantMiniScheduleEntry
        {
            Group = group,
            Teacher = teacher,
            Room = room
        });
    }

    private static string EnsureDummyPhoto()
    {
        if (!string.IsNullOrEmpty(_photoPath) && File.Exists(_photoPath))
        {
            return _photoPath;
        }

        try
        {
            var path = Path.Combine(Path.GetTempPath(), "Aula-dummy-foto.png");
            var resource = Application.GetResourceStream(new Uri("pack://application:,,,/Assets/icon.png"));
            if (resource is not null)
            {
                using var input = resource.Stream;
                using var output = File.Create(path);
                input.CopyTo(output);
                _photoPath = path;
            }
        }
        catch
        {
            _photoPath = string.Empty;
        }

        return _photoPath ?? string.Empty;
    }
}
#endif
