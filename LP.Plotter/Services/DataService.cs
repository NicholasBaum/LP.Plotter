using System.Text.Json;
using System.Text.Json.Nodes;
using LP.Plotter.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LP.Plotter.Services;

public class DataService
{
    private readonly HttpClient httpClient = new();

    public DataService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<VChannelSet> LoadChannels(CsvInfo info)
    {
        var response = await httpClient.GetAsync(info.Url);
        response.EnsureSuccessStatusCode();
        var csvDataString = await response.Content.ReadAsStringAsync();
        return VChannelSet.Create(info, csvDataString);
    }

    public async Task<VChannelSet> LoadAsRun(IEnumerable<CsvInfo> infos)
    {
        var tasks = infos.Select(LoadChannels).ToList();
        var sets = await Task.WhenAll(tasks);
        return VChannelSet.CreateMerged(sets);
    }

    public async Task<List<CsvInfo>> GetFileInfos()
    {
        var response = await httpClient.GetAsync("csvdata/files_index.txt");
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync();
        return text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(x => new CsvInfo()
        {
            FileName = Path.GetFileName(x.Replace("\\", "/")),
            Path = x,
            Url = "csvdata" + x.Replace("#", "%23"),
            Channels = null,
        }).ToList();
    }

    private async Task<List<CsvInfo>> GetFileInfosFromGithub()
    {
        string url = "https://api.github.com/repos/NicholasBaum/TyrePlot/git/trees/gh-pages?recursive=1";
        HttpResponseMessage response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<JsonObject>(json);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var files = data["tree"].AsArray()
            .Where(d => d["path"].ToString().EndsWith(".csv"))
            .Select(x => new CsvInfo()
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
