using Refit;

namespace HackerNewsApp.Server.Clients
{
    public interface IHackerNewsClient
    {
        [Get("/item/{id}.json")]
        Task<HackerNewsItem> GetItem(int id);

        [Get("/newstories.json")]
        Task<int[]> GetLatestStoryIds();
    }
}
