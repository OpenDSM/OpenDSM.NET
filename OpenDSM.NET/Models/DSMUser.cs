
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

    internal DSMUser(string json) : this(JObject.Parse(json)) { }
    internal DSMUser(JObject json)
    {
        ID = (int)(json["id"] ?? 0);
        Username = (string)json["username"];
        Email = (string)json["email"];
        About = (string)json["about"];
        try
        {
            CreatedProducts = ((JArray)json["createdProducts"]).Values<int>().ToArray();
        }
        catch
        {
            CreatedProducts = Array.Empty<int>();
        }
        try
        {
            OwnedProducts = ((JArray)json["ownedProducts"]).Values<int>().ToArray();
        }
        catch
        {
            OwnedProducts = Array.Empty<int>();
        }
        JObject git = (JObject)json["git"];
        IsDeveloperAccount = (bool)git["isDeveloperAccount"];
        HasGitReadme = (bool)git["hasGitReadme"];
        UseGitReadme = (bool)git["useReadme"];
    }
}
