using System.Collections.Generic;
using System.Linq;
using _NET_Web_API.Models;

namespace _NET_Web_API.Data
{
    public class SqlWebApiRepository : IWebApiRepository
    {
        private readonly WebApiContext _context;
        public SqlWebApiRepository(WebApiContext context)
        {
            _context = context;
        }


        public IEnumerable<Post> GetPosts()
        {
            return _context.Posts.ToList();
        }

        public Post GetPostsBySlug(string slug)
        {
            return _context.Posts.FirstOrDefault(p => p.Slug == slug);
        }
    }
}