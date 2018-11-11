using System.Threading.Tasks;
using UrlShortnerApi.DAL;
using UrlShortnerApi.Models;
using UrlShortnerApi.Extensions;
using System.Text;
using System.Net;
using UrlShortnerApi.Dependencies;
using Microsoft.Azure.Documents;
using System.Linq;

namespace UrlShortnerApi.Services
{
    public class ShortUrlService : IShortUrlService
    {
        private readonly IDocumentDBRepository<ShortUrl> _repository;
        private readonly ISystemClock _clock;
        public ShortUrlService(IDocumentDBRepository<ShortUrl> repository, ISystemClock clock)
        {
            _repository = repository;
            _clock = clock;
        }
        public async Task<string> CreateShortUrl(string originalUrl)
        {
            /* Decode Url to avoid mismatch if url is encoded */
            var decodedUrl = WebUtility.UrlDecode(originalUrl);
            var base64Code = decodedUrl.MD5Hash().ToBase64();

            int startIndex = 0;
            int length = 6;
            var shortCode = base64Code.Substring(startIndex, length);

            var hasShortUrl = await HasShortUrl(shortCode, originalUrl);

            if (hasShortUrl)
                return shortCode;

            while (!(await CanUseHash(shortCode)))
            {
                startIndex += 1;
                length += 1;
                shortCode = base64Code.Substring(startIndex, length);
            }

            return await CreateShortUrl(originalUrl, shortCode, base64Code);
        }

        public async Task<string> GetOriginalUrl(string shortCode)
        {
            var document = await _repository.GetItemAsync(shortCode);
            return document.IsNull() ? string.Empty : document.OriginalUrl;
        }

        private async Task<string> CreateShortUrl(string originalUrl, string shortCode, string base64Code)
        {
            var document = await _repository.CreateItemAsync(new ShortUrl
            {
                Id = shortCode,
                Hash = base64Code,
                OriginalUrl = originalUrl,
                CreatedDate = _clock.Now(),
                UpdatedDate = _clock.Now()
            });

            return document.Id;
        }

        private async Task<bool> CanUseHash(string hash)
        {
            var shortUrl = await _repository.GetItemAsync(hash);
            return shortUrl.IsNull();
        }

        private async Task<bool> HasShortUrl(string shortCode, string originalUrl)
        {

            var shortUrl = await _repository.GetItemsAsync(x => x.Id == shortCode && x.OriginalUrl == originalUrl);
            return shortUrl.Any();
        }
    }
}