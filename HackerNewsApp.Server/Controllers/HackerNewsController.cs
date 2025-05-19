using HackerNewsApp.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace HackerNewsApp.Server.Controllers
{
    [ApiController]
    public class HackerNewsController : ControllerBase
    {
        private readonly ILogger<HackerNewsController> _logger;
        private readonly IHackerNewsService _service;

        public HackerNewsController(ILogger<HackerNewsController> logger, IHackerNewsService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("latest")]
        public async Task<IEnumerable<HackerNewsItem>> GetLatestStories([FromQuery] int pageNum, [FromQuery] int pageSize)
        {
            return await _service.GetLatestStories(pageNum, pageSize);
        }

        [HttpGet("search")]
        public async Task<IEnumerable<HackerNewsItem>> SearchStories([FromQuery] string query, [FromQuery] int pageNum, [FromQuery] int pageSize)
        {
            return await _service.SearchStories(query, pageNum, pageSize);
        }
    }
}
