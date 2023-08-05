using System.Text.Json;
using System.Text.Json.Nodes;

namespace LP.Plotter.Data;

public class DataService
{
    private readonly HttpClient httpClient = new();

    public DataService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<CsvData> LoadChannels(CsvData csvData)
    {     
        var response = await httpClient.GetAsync(csvData.Url);
        response.EnsureSuccessStatusCode();
        var csv = await response.Content.ReadAsStringAsync();
        var channels = CsvData.ParseCSV(csv);
        return new CsvData()
        {
            FileName = csvData.FileName,
            Path = csvData.Path,
            Url = csvData.Url,
            Channels = channels,
        };
    }

    public async Task<List<CsvData>> GetFileInfos()
    {
        var response = await httpClient.GetAsync("csvdata/files_index.txt");
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync();
        return text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(x => new CsvData()
        {
            FileName = Path.GetFileName(x.Replace("\\", "/")),
            Path = x,
            Url = "csvdata" + x.Replace("#", "%23"),
            Channels = null,
        }).ToList();
    }

    private async Task<List<CsvData>> GetFileInfosFromGithub()
    {
        string url = "https://api.github.com/repos/NicholasBaum/TyrePlot/git/trees/gh-pages?recursive=1";
        HttpResponseMessage response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<JsonObject>(json);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var files = data["tree"].AsArray()
            .Where(d => d["path"].ToString().EndsWith(".csv"))
            .Select(x => new CsvData()
            {
                FileName = Path.GetFileName(x["path"].ToString()),
                Path = x["path"].ToString(),
                Url = x["url"].ToString(),
                Channels = null,
            })
            .ToList();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        return files;
    }
}
