﻿using System.Text.Json;
using System.Text.Json.Nodes;

namespace LP.Plotter.Data;

public class DataService
{
    private readonly HttpClient httpClient = new();

    public DataService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<List<DataObject>> GetFileInfos()
    {
        string url = "https://api.github.com/repos/NicholasBaum/TyrePlot/git/trees/gh-pages?recursive=1";
        HttpResponseMessage response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<JsonObject>(json);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var files = data["tree"].AsArray()
            .Where(d => d["path"].ToString().EndsWith(".csv"))
            .Select(x => new DataObject()
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
