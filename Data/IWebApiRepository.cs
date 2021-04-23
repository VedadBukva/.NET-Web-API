using System.Collections.Generic;
using _NET_Web_API.Models;

namespace _NET_Web_API.Data
{
    public interface IWebApiRepository
    {
         IEnumerable<Post> GetPosts();
         Post GetPostBySlug(string slug);
         IEnumerable<Tag> GetTags();
         IEnumerable<Tag> GetTagsBySlug(string slug);
    }
}