using System.Collections.Generic;
using _NET_Web_API.Models;

namespace _NET_Web_API.Data
{
    public interface IWebApiRepository
    {
         IEnumerable<Post> GetPosts();
         Post GetPostsBySlug(string slug);
    }
}