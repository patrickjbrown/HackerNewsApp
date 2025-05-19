namespace HackerNewsApp.Server.Services
{
    public interface IHackerNewsService
    {
        public Task<IEnumerable<HackerNewsItem>> GetLatestStories(int pageNum, int pageSize);

        public Task<IEnumerable<HackerNewsItem>> SearchStories(string query, int pageNum, int pageSize);
    }
}
