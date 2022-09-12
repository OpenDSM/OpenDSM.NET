using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenDSM.NET.Exceptions;
using OpenDSM.NET.Models;

namespace OpenDSM.NET.Requests;
public enum ImageType
{
    Profile,
    Banner
}
/// <summary>
/// For all request made to the "user" endpoint
/// </summary>
public static class UserRequests
{
    /// <summary>
    /// Gets a users information
    /// </summary>
    /// <param name="client">The DSM Client</param>
    /// <param name="id">The users id</param>
    /// <returns></returns>
    public static DSMUser GetUser(DSMClient client, int id)
    {
        using HttpResponseMessage response = Get(client, $"/users/{id}");
        if (response.IsSuccessStatusCode)
        {
            return new DSMUser(response.Content.ReadAsStringAsync().Result);
        }
        string message = (string)JsonConvert.DeserializeObject<JObject>(response.Content.ReadAsStringAsync().Result)["message"];
        if (message.StartsWith("No user found"))
        {
            throw new UnresolvedQueryResultException();
        }
        throw new HttpRequestException($"Server Responded with Error Code {response.StatusCode}");
    }

    /// <summary>
    /// Searches for user using query
    /// </summary>
    /// <param name="client">The DSM Client</param>
    /// <param name="query">The search query</param>
    /// <param name="page">The page offset</param>
    /// <param name="items_per_page">The number of items per page</param>
    /// <returns></returns>
    public static DSMUser[] GetUserFromQuery(DSMClient client, string query, int page, int items_per_page)
    {
        using HttpResponseMessage response = Get(client, $"/search/users?query={query}&page={page}&count={items_per_page}");
        if (response.IsSuccessStatusCode)
        {
            JArray array = JsonConvert.DeserializeObject<JArray>(response.Content.ReadAsStringAsync().Result);
            DSMUser[] users = new DSMUser[array.Count()];
            Parallel.For(0, users.Length, i =>
            {
                users[i] = new(JsonConvert.SerializeObject(array[0]));
            });
            return users;
        }

        throw new UnresolvedQueryResultException();
    }

    /// <summary>
    /// Creates a user and returns the created user
    /// </summary>
    /// <param name="client">The DSM Client</param>
    /// <param name="username">The users username</param>
    /// <param name="email">The users email</param>
    /// <param name="password">The users password</param>
    /// <returns></returns>
    public static DSMUser CreateUser(DSMClient client, string username, string email, string password)
    {
        using HttpResponseMessage response = Post(client, "/user", new KeyValuePair<string, string>[]
        {
            new KeyValuePair<string, string>("username",username),
            new KeyValuePair<string, string>("email",email),
            new KeyValuePair<string, string>("password",password)
        });
        if ((response.Content.Headers.ContentType?.MediaType ?? "").Equals("application/json"))
        {
            JObject json = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            if (response.IsSuccessStatusCode)
            {
                return new(JsonConvert.SerializeObject(json["user"]));
            }

            string message = (string)(json["message"] ?? "");
            throw new System.Net.WebException($"Unable to create user!\n{message}");
        }
        throw new System.Net.WebException("Unable to create user!");
    }

    /// <summary>
    /// Returns a list of all users repositories
    /// </summary>
    /// <param name="client">The DSM Client</param>
    /// <returns></returns>
    public static IReadOnlyDictionary<int, string> GetRepositories(DSMClient client)
    {
        using HttpResponseMessage response = Get(client, "/user/git/repositories");

        if ((response.Content.Headers.ContentType?.MediaType ?? "").Equals("application/json"))
        {
            if (response.IsSuccessStatusCode)
            {
                JArray array = JArray.Parse(response.Content.ReadAsStringAsync().Result);
                Dictionary<int, string> repos = new();
                Parallel.ForEach(array, jobj =>
                {
                    repos.Add((int)jobj["id"], (string)jobj["name"]);
                });
                return repos;
            }
            string? message = (string)JObject.Parse(response.Content.ReadAsStringAsync().Result)["message"];

            throw new UnresolvedQueryResultException(message ?? "");
        }

        throw new HttpRequestException($"Server didn't respond with JSON!  Server responded with status code {response.StatusCode}, and \"{(response.Content == null ? "content was null" : response.Content.Headers.ContentType == null ? "content type was null" : $"{response.Content.Headers.ContentType}")}\"");
    }

    /// <summary>
    /// Uploads an Image from a path
    /// </summary>
    /// <param name="client">The DSM Client</param>
    /// <param name="type">The type of image</param>
    /// <param name="filePath">The absolute path to file</param>
    /// <returns></returns>
    public static bool UploadImageFromPath(DSMClient client, ImageType type, string filePath)
    {
        using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return UploadImage(client, type, stream);
    }
    /// <summary>
    /// Uploads an Image from FileStream
    /// </summary>
    /// <param name="client">The DSM Client</param>
    /// <param name="type">The type of image</param>
    /// <param name="stream">The file stream</param>
    /// <returns></returns>
    public static bool UploadImage(DSMClient client, ImageType type, FileStream stream)
    {
        MemoryStream ms = new();
        stream.CopyTo(ms);
        return UploadImage(client, type, ms);
    }
    /// <summary>
    /// Uploads an Image from Memory Stream
    /// </summary>
    /// <param name="client">The DSM Client</param>
    /// <param name="type">The type of image</param>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static bool UploadImage(DSMClient client, ImageType type, MemoryStream stream)
    {
        return UploadImage(client, type, Convert.ToBase64String(stream.GetBuffer()));
    }

    /// <summary>
    /// Uploads Image as base64
    /// </summary>
    /// <param name="client">The DSM Client</param>
    /// <param name="type">The type of image</param>
    /// <param name="base64">The base64 representation</param>
    /// <returns></returns>
    public static bool UploadImage(DSMClient client, ImageType type, string base64)
    {
        using HttpResponseMessage response = Post(client, $"/images/user/{type}", base64);
        return response.IsSuccessStatusCode;
    }
}
