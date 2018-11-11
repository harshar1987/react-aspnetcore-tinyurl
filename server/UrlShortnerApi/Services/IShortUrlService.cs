using System.Threading.Tasks;
using UrlShortnerApi.Models;

namespace UrlShortnerApi.Services {

    public interface IShortUrlService
    {
           Task<string> GetOriginalUrl(string shortCode);

           Task<string> CreateShortUrl(string originalUrl);
    }
}