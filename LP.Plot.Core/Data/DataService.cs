using LP.Plot.Core.Signal;
using static LP.Plot.Core.Data.Helper;

namespace LP.Plot.Core.Data;

public class DataService
{
    private readonly HttpClient httpClient = new();

    public DataService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<StaticSignal> LoadSignal_S() => CreateSignals(await LoadTestLap()).First();

    public async Task<List<StaticSignal>> LoadSignal_M() => CreateSignals(await LoadTestLap());

    public async Task<List<StaticSignal>> LoadSignal_L() => CreateSignals(await LoadTestRun());

    public async Task<List<StaticSignal>> LoadSignal_XL() => CreateSignals(await LoadTestSession());

    public async Task<ChannelDataSet> LoadTestLap()
    {
        var infos = await this.GetFileInfos();
        var runInfos = infos.Where(x => x.Path.Contains("T2303_IMO_#29\\D1PMRun1"));
        return await LoadChannels(runInfos.First());
    }

    public async Task<ChannelDataSet> LoadTestRun()
    {
        var infos = await this.GetFileInfos();
        var runInfos = infos.Where(x => x.Path.Contains("T2303_IMO_#29\\D1PMRun1"));
        var run = await MergeLaps(runInfos);
        return run;
    }

    public async Task<ChannelDataSet> LoadTestSession()
    {
        var infos = await this.GetFileInfos();
        var runInfos = infos.Where(x => x.Path.Contains("T2303_IMO_#29"));
        var run = await MergeLaps(runInfos);
        return run;
    }

    public async Task<ChannelDataSet> LoadChannels(CsvInfo info)
    {
        var response = await httpClient.GetAsync(info.Url);
        response.EnsureSuccessStatusCode();
        var csvDataString = await response.Content.ReadAsStringAsync();
        return ChannelDataSet.Create(info, csvDataString);
    }

    public async Task<ChannelDataSet> MergeLaps(IEnumerable<CsvInfo> infos)
    {
        var tasks = infos.Select(LoadChannels).ToList();
        var sets = await Task.WhenAll(tasks);
        return ChannelDataSet.CreateMerged(sets);
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
        }).ToList();
    }
}
