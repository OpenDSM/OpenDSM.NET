using Newtonsoft.Json.Linq;

namespace OpenDSM.NET.Models;
public sealed class DSMProduct
{

    public string ID {get;}
    public string Name {get;}
    public string About {get;}
    public string ShortSummery {get;}
    public IReadOnlyCollection<string> Keywords {get;}
    public IReadOnlyCollection<int> Tags {get;}
    

    public DSMProduct(string name, string git_repo_name, string short_summery, string? yt_key, bool subscription, bool use_git_readme, float price, string[] keywords, int[] tags)
    {

    }

    internal DSMProduct(string json) : this(JObject.Parse(json)) { }
    internal DSMProduct(JObject json) : this(
        (string)json["name"],
        (string)json["git_repo_name"],
        (string)json["short_summery"],
        (string)json["yt_key"],
        (bool)json["subscription"],
        (bool)json["use_git_readme"],
        (float)json["price"],
        Array.ConvertAll(JArray.FromObject(json["keywords"]).ToArray(), i => i.ToString()),
        Array.ConvertAll(JArray.FromObject(json["tags"]).ToArray(), i => Convert.ToInt32(i))
        )
    { }
}