using HackerNewsApp.Server.Clients;
using HackerNewsApp.Server.Services;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;

namespace HackerNewsApp.Server.Tests
{
    [TestClass]
    public sealed class HackerNewsServiceTests
    {
        private HackerNewsService _service;
        private IHackerNewsClient _client;
        private IMemoryCache _cache;

        [TestInitialize]
        public void Setup()
        {
            _client = Substitute.For<IHackerNewsClient>();
            _cache = Substitute.For<IMemoryCache>();
            _service = new HackerNewsService(_client, _cache);
        }

        [TestMethod]
        public async Task GetItem_PullsFromCacheWhenAvailable_AndFromHNClientWhenNotCached()
        {
            // Arrange
            const int CACHED_ID = 1;
            const int NON_CACHED_ID = 2;

            // We will make two calls to GetItem. One for a cached item, and one for a non-cached item
            HackerNewsItem item = null;
            _cache.TryGetValue(CACHED_ID, out item)
                .Returns(x => {
                    x[1] = new HackerNewsItem { Id = CACHED_ID };
                    return true;
                });
            _cache.TryGetValue(NON_CACHED_ID, out item)
                .Returns(x => {
                    x[1] = null;
                    return false;
                });
            _client.GetItem(NON_CACHED_ID).Returns(new HackerNewsItem { Id = NON_CACHED_ID });

            // Act
            var cachedResult = await _service.GetItem(CACHED_ID);
            var nonCachedResult = await _service.GetItem(NON_CACHED_ID);

            // Assert
            Assert.AreEqual(CACHED_ID, cachedResult.Id);
            Assert.AreEqual(NON_CACHED_ID, nonCachedResult.Id);
        }

        [TestMethod]
        public async Task GetLatestStories_ReturnsPagedResults()
        {
            // Arrange
            const int PAGE_NUM = 1;
            const int PAGE_SIZE = 25;

            var latestIds = Enumerable.Range(1, 500).OrderByDescending(i => i).ToArray();
            _client.GetLatestStoryIds().Returns(latestIds);

            foreach (var id in latestIds)
            {
                _cache.TryGetValue(id, out HackerNewsItem h).ReturnsForAnyArgs(false);
                _client.GetItem(id).Returns(new HackerNewsItem { Id = id });
            }

            var expectedMaxId = latestIds.Skip(PAGE_NUM * PAGE_SIZE).First();
            var expectedMinId = latestIds.Skip(PAGE_NUM * PAGE_SIZE).First() - PAGE_SIZE + 1;

            // Act
            var results = await _service.GetLatestStories(PAGE_NUM, PAGE_SIZE);

            // Assert
            Assert.AreEqual(PAGE_SIZE, results.Count());
            Assert.AreEqual(expectedMaxId, results.First().Id);
            Assert.AreEqual(expectedMinId, results.Last().Id);
        }

        [TestMethod]
        public async Task SearchStories_ReturnsPagedAndFilteredResults()
        {
            // Arrange
            const string QUERY_TEXT = "search";
            const int PAGE_NUM = 1;
            const int PAGE_SIZE = 25;
            const int NON_MATCHES = 250;

            var latestIds = Enumerable.Range(1, 500).OrderByDescending(i => i).ToArray();
            _client.GetLatestStoryIds().Returns(latestIds);
            _cache.TryGetValue(Arg.Any<int>(), out Arg.Any<HackerNewsItem>()).ReturnsForAnyArgs(false);

            /** 
             *  To test search functionality, we'll set up test data as follows...
             *  
             *  IDs above the NON_MATCHES threshold: Will not contain search query
             *  IDs at/below the NON_MATCHES threshold and even: Will contain search query in the title
             *  IDs at/below the NON_MATCHES threshold and odd: Will contain search query in the text
             *  
             *  This ensures we look at all 3 types of stories, and makes the expected pagination math easier to grok
             */

            foreach (var id in latestIds)
            {
                if (id > NON_MATCHES)
                {
                    _client.GetItem(id).Returns(new HackerNewsItem() { Id = id, Title = "NO MATCH", Text = "NO MATCH" });
                }
                else if (id % 2 == 0)
                {
                    _client.GetItem(id).Returns(new HackerNewsItem() { Id = id, Title = string.Format("Title {0} {1}", QUERY_TEXT, id), Text = "NO MATCH" });
                }
                else
                {
                    _client.GetItem(id).Returns(new HackerNewsItem() { Id = id, Title = "NO MATCH", Text = string.Format("Text {0} {1}", QUERY_TEXT, id) });
                }
            }

            var expectedMaxId = latestIds.Skip(NON_MATCHES + (PAGE_NUM * PAGE_SIZE)).First();
            var expectedMinId = latestIds.Skip(NON_MATCHES + (PAGE_NUM * PAGE_SIZE)).First() - PAGE_SIZE + 1;

            // Act
            var results = await _service.SearchStories(QUERY_TEXT, PAGE_NUM, PAGE_SIZE);

            // Assert
            Assert.AreEqual(25, results.Count());
            Assert.AreEqual(expectedMaxId, results.First().Id);
            Assert.AreEqual(expectedMinId, results.Last().Id);

            foreach (var result in results)
            {
                Assert.IsTrue(result.Title.Contains(QUERY_TEXT) || result.Text.Contains(QUERY_TEXT));
            }
        }
    }
}
