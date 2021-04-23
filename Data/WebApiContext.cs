using _NET_Web_API.Models;
using Microsoft.EntityFrameworkCore;

namespace _NET_Web_API
{
    public class WebApiContext : DbContext
    {
        public WebApiContext(DbContextOptions<WebApiContext> opt) : base(opt)
        {

        }

        public DbSet<Post>  Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }

        
    }
}