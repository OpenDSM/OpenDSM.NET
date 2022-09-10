
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpenDSM.NET.Models;

public class DSMUser
{
    public int ID { get; }
    public string Username { get; }
    public string Email { get; }
    public string About { get; }
    public int[] CreatedProducts { get; }
    public int[] OwnedProducts { get; }
    public bool IsDeveloperAccount { get; }
    public bool HasGitReadme { get; }
    public bool UseGitReadme { get; }

    internal DSMUser(string json)
    {
        JObject user = JsonConvert.DeserializeObject<JObject>(json);
        ID = (int)user["id"];
        Username = (string)user["username"];
        Email = (string)user["email"];
        About = (string)user["about"];
        try
        {
            CreatedProducts = ((JArray)user["createdProducts"]).Values<int>().ToArray();
        }
        catch
        {
            CreatedProducts = Array.Empty<int>();
        }
        try
        {
            OwnedProducts = ((JArray)user["ownedProducts"]).Values<int>().ToArray();
        }
        catch
        {
            OwnedProducts = Array.Empty<int>();
        }
        JObject git = (JObject)user["git"];
        IsDeveloperAccount = (bool)git["isDeveloperAccount"];
        HasGitReadme = (bool)git["hasGitReadme"];
        UseGitReadme = (bool)git["useReadme"];
    }
}
