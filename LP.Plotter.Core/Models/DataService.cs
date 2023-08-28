using LP.Plot.Core.Primitives;
using LP.Plot.Core.Signal;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace LP.Plotter.Core.Models;

public class DataService
{
    private readonly HttpClient httpClient = new();

    public DataService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<ISignal> LoadSignal_M()
    {
        var data = await LoadTestRun();
        var signal = new StaticSignal(data.SpeedChannel.Points.Select(x => x.Y).ToArray(), new Span(data.SpeedChannel.Points.First().X, data.SpeedChannel.Points.Last().X));
        return signal;
    }

    public async Task<VChannelSet> LoadTestLap()
    {
        var infos = await this.GetFileInfos();
        var runInfos = infos.Where(x => x.Path.Contains("T2303_IMO_#29\\D1PMRun1"));
        return await LoadChannels(runInfos.First());
    }

    public async Task<VChannelSet> LoadTestRun()
    {
        var infos = await this.GetFileInfos();
        var runInfos = infos.Where(x => x.Path.Contains("T2303_IMO_#29\\D1PMRun1"));
        var run = await LoadAsRun(runInfos);
        return run;
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
