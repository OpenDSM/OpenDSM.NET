using Newtonsoft.Json.Linq;
using OpenDSM.NET.Models;

namespace OpenDSM.NET.Requests;

/// <summary>
/// The type of list returned
/// </summary>
public enum ProductListType
{
    ///<summery>
    /// Gets the latest products
    /// </summery>
    Latest,

    ///<summery>
    /// Gets the most popular products
    /// </summery>
    Popular,
}

/// <summary>
/// Handles all requests pertaining to products
/// </summary>
public sealed class ProductRequests
{
    private readonly DSMClient client;
    public ProductRequests(DSMClient client)
    {
        this.client = client;
    }

    /// <summary>
    /// Gets a product based on id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public DSMProduct GetProduct(string id)
    {

        HttpResponseMessage response = client.Get($"/products/{id}");
        string body = response.Content.ReadAsStringAsync().Result;
        if (response.IsSuccessStatusCode)
        {
            return new DSMProduct(body);
        }
        throw new Exceptions.UnresolvedQueryResultException(JObject.Parse(body)["message"]?.ToString() ?? "");
    }
    /// <summary>
    /// Gets a list of products sorted by specified type
    /// </summary>
    /// <param name="type">The product list type</param>
    /// <param name="page">The page offset</param>
    /// <param name="items_per_page">The number of items per page</param>
    /// <returns></returns>
    public IReadOnlyCollection<DSMProduct> GetProducts(ProductListType type, int page = 0, int items_per_page = 20)
    {
        DSMProduct[] products = new DSMProduct[items_per_page];
        HttpResponseMessage response = client.Get($"/products?type={type}&page={page}&items_per_page={items_per_page}");
        string body = response.Content.ReadAsStringAsync().Result;
        if (response.IsSuccessStatusCode)
        {
            JArray array = JArray.Parse(body);
            for (int i = 0; i < array.Count; i++)
            {
                if (i > items_per_page) throw new Exceptions.UnresolvedQueryResultException($"Server returned more results than expected!");
                products[i] = new(array[i].ToObject<JObject>());
            }
        }
        return products.Where(i => i != null).ToArray();
    }
    /// <summary>
    /// Searches for products based on query
    /// </summary>
    /// <param name="query">The search query</param>
    /// <param name="page">The page offset</param>
    /// <param name="items_per_page">The number of items per page</param>
    /// <returns></returns>
    public IReadOnlyCollection<DSMProduct> Search(string query, int page = 0, int items_per_page = 20, params int[] tags)
    {
        DSMProduct[] products = new DSMProduct[items_per_page];
        HttpResponseMessage response = client.Get($"/search/applications?query={query}&page={page}&items_per_page={items_per_page}&tags={string.Join(";", tags)}");
        string body = response.Content.ReadAsStringAsync().Result;
        if (response.IsSuccessStatusCode)
        {
            JArray array = JArray.Parse(body);
            for (int i = 0; i < array.Count; i++)
            {
                if (i > items_per_page) throw new Exceptions.UnresolvedQueryResultException($"Server returned more results than expected!");
                products[i] = new(array[i].ToObject<JObject>());
            }
        }
        return products.Where(i => i != null).ToArray();
    }

    /// <summary>
    /// Gets a list of tags and their ids
    /// </summary>
    /// <returns></returns>
    public IReadOnlyDictionary<int, string> GetTags()
    {
        Dictionary<int, string> tags = new();
        HttpResponseMessage response = client.Get("/products/tags");
        if (response.IsSuccessStatusCode)
        {
            JObject json = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            foreach ((string name, JToken? value) in json)
            {
                if (value != null)
                    tags.Add(Convert.ToInt32(name), value.ToString());
            }
        }
        return tags;
    }
}