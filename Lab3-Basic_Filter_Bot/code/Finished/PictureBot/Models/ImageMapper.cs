using Microsoft.Azure.Search.Models;

namespace PictureBot.Models
{
    public class ImageMapper 
    {
        public static SearchHit ToSearchHit(SearchResult<SearchHit> hit)
        {
            var searchHit = hit.Document;            
            
            return searchHit;
        }

    }
}