using HackerNewsApp.Server.Clients;
using Microsoft.Extensions.Caching.Memory;

namespace HackerNewsApp.Server.Services
{
    public class HackerNewsService : IHackerNewsService
    {
        private readonly IHackerNewsClient _client;
        private readonly IMemoryCache _cache;

        public HackerNewsService(IHackerNewsClient client, IMemoryCache cache)
        {
            _client = client;
            _cache = cache;
        }

        /// <summary>
        /// Obtains a paginated list of the latest stories from HackerNews.
        /// </summary>
        /// <param name="pageNum">The page number to retrieve.</param>
        /// <param name="pageSize">The number of results per page.</param>
        /// <returns>A paginated list of the latest stories from HackerNews.</returns>
        public async Task<IEnumerable<HackerNewsItem>> GetLatestStories(int pageNum, int pageSize)
        {
            var stories = new List<HackerNewsItem>();

            // Obtain the list of latest story IDs from HackerNews, and narrow it down to the desired page
            var ids = await _client.GetLatestStoryIds();
            var pagedIds = ids.Skip(pageNum * pageSize).Take(pageSize);

            // For each id on the page, obtain the story
            foreach (var id in pagedIds) 
            {
                stories.Add(await GetItem(id));
            }

            return stories;
        }

        /// <summary>
        /// Searches the latest stories on HackerNews, and returns a paginated list of those containing the query text.
        /// </summary>
        /// <param name="query">The text to search for.</param>
        /// <param name="pageNum">The page number to retrieve.</param>
        /// <param name="pageSize">The number of results per page.</param>
        /// <returns>A paginated list of stories from HackerNews containing the query text.</returns>
        public async Task<IEnumerable<HackerNewsItem>> SearchStories(string query, int pageNum, int pageSize)
        {
            // NOTE: I could not find a built-in search function listed in the API documentation.
            // Unfortunately, that means that we have to pull back all the latest stories one by one,
            // checking to see if they contain the search terms. This has the additional drawback of 
            // being unable to reasonably search beyond the latest 500 stories. One *could* write it in such a
            // way that it keeps decreasing the item ID by 1 and searching until [pageSize] matches are found,
            // but if the search term wasn't present, that would mean querying the entire HN database 
            // one record at a time, and that's absolutely not a valid option.

            var stories = new List<HackerNewsItem>();

            // Obtain the list of latest story IDs from HackerNews,
            var ids = await _client.GetLatestStoryIds();

            foreach (var id in ids)
            {
                // If we have found enough results to fill the current page, stop searching
                if (stories.Count >= ((pageNum * pageSize) + pageSize))
                    break;

                var story = await GetItem(id);

                // If the story associated with the current id contains the query text, add it to the list
                if ((story.Title != null && story.Title.Contains(query))
                    || (story.Text != null && story.Text.Contains(query)))
                {
                    stories.Add(story);
                }
            }

            return stories.Skip(pageNum * pageSize).Take(pageSize);
        }

        /// <summary>
        /// Obtains an item from HackerNews with the specified id. Will pull from cache, if available.
        /// </summary>
        /// <param name="id">The id of the item to retrieve.</param>
        /// <returns>A HackerNewsItem with the specified id.</returns>
        public async Task<HackerNewsItem> GetItem(int id)
        {
            HackerNewsItem item = null;

            if (!_cache.TryGetValue(id, out item))
            {
                item = await _client.GetItem(id);
                _cache.Set(id, item);
            }

            return item;
        }
    }
}
