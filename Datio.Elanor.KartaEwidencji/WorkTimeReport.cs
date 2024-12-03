using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Datio.Elanor.KartaEwidencji;
using Microsoft.Extensions.DependencyInjection;
using Soneta.Business;
using Soneta.Business.UI;
using Soneta.Kadry;
using Soneta.KadryPlace.Reports;
using Soneta.Tools;

[assembly: Worker(typeof(WorkTimeReport), typeof(Pracownicy))]

namespace Datio.Elanor.KartaEwidencji;

public class WorkTimeReport
{
    [Context]
    public Session Session { get; set; }

    [Context]
    public Context Context { get; set; }

    [Context]
    public Pracownik[] Pracownicy { get; set; }

    [Context]
    public EwidencjaCzasuPracySzczegolowaSnippet.PrnParams Params { get; set; }

    [Action(
        name: "Generuj kartę ewidencji",
        Description = "Generowanie listy wydruków karty ewidencji czasu pracy w formacie ZIP",
        Target = ActionTarget.ToolbarWithText | ActionTarget.Menu | ActionTarget.LocalMenu,
        Mode = ActionMode.SingleSession
    )]
    public object Save()
    {
        if (Pracownicy.IsEmptyOrNull())
        {
            return "Nie wybrano żadnych pracowników".Translate();
        }

        var sources = Pracownicy.Select(pracownik =>
        {
            var stream = GeneratePrintout(pracownik);
            // month_year_companyName_name_lastname_code
            var fileName = string.Join('_',
            [
                Params.Miesiąc.Month,
                Params.Miesiąc.Year,
                Session.Global.Features.GetString("OznaczenieFirmy"),
                pracownik.Imie,
                pracownik.Nazwisko,
                pracownik.Kod,
            ]);

            return (stream, $"{fileName}.pdf");
        });

        var zipStream = CreateZip(sources);

        return new NamedStream("EwidencjaCzasuPracySzczegolowa.zip", zipStream.ToArray());
    }

    private Stream GeneratePrintout(Pracownik pracownik)
    {
        Context.Set(pracownik);

        var reportResult = new ReportResult
        {
            Format = ReportResultFormat.PDF,
            Context = Context,
            DataType = pracownik.GetType(),
            Encrypt = pracownik.PESEL,
            Preview = false,
            TemplateFileName = "EwidencjaCzasuPracySzczegolowa.repx"
        };

        var reportService = Session.GetRequiredService<IReportService>();
        var stream = reportService.GenerateReport(reportResult);

        return stream;
    }

    private static MemoryStream CreateZip(IEnumerable<(Stream, string)> sources)
    {
        // Create a MemoryStream to hold the ZIP file
        var zipStream = new MemoryStream();

        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var (sourceStream, fileName) in sources)
            {
                // Create a new entry in the ZIP archive
                var entry = archive.CreateEntry(fileName, CompressionLevel.Optimal);

                // Write the contents of the source stream to the entry
                using var entryStream = entry.Open();
                sourceStream.CopyTo(entryStream);
            }
        }

        // Reset the position of the MemoryStream to the beginning
        zipStream.Position = 0;

        return zipStream;
    }
}
