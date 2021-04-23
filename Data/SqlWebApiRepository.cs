using System.Collections.Generic;
using System.Linq;
using _NET_Web_API.Models;
using Microsoft.Data.Sqlite;

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
        public Post GetPostBySlug(string slug)
        {
            return _context.Posts.FirstOrDefault(p => p.Slug == slug);
        }
        public IEnumerable<Tag> GetTags()
        {
            return _context.Tags.ToList();
        }
        public IEnumerable<Tag> GetTagsBySlug(string slug)
        {
            return _context.Tags.Where(p => p.Slug == slug).ToList();
        }

        /*protected void LoadBlogPosts()
        {
            string cs = @"URI=file:.\Posts.db";
            using var con = new SqliteConnection(cs);
            con.Open(); 
            string query =  @"CREATE VIEW View_BlogPostCounts AS
                    SELECT b.Name, Count(p.PostId) as PostCount
                    FROM Blogs b
                    JOIN Posts p on p.BlogId = b.BlogId
                    GROUP BY b.Name";
            using var cmd = new SqliteCommand(query,con);

        }*/
    }
}