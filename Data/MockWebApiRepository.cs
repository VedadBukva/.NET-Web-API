using System;
using System.Collections.Generic;
using _NET_Web_API.Models;

namespace _NET_Web_API.Data
{
    public class MockWebApiRepository : IWebApiRepository
    {
        public Post GetPostsBySlug(string slug)
        {
            return new Post{Slug="example", Title="Example", Description="Example", Body="Example", TagList="tag", CreatedAt="22-04-2021", UpdatedAt="22-04-2021"};
        }

        public IEnumerable<Post> GetPosts()
        {
           var posts = new List<Post>
           {
               new Post{Slug="example", Title="Example", Description="Example", Body="Example", TagList="tag", CreatedAt="22-04-2021", UpdatedAt="22-04-2021"},
               new Post{Slug="example1", Title="Example1", Description="Example1", Body="Example1", TagList="tag", CreatedAt="22-04-2021", UpdatedAt="22-04-2021"},
               new Post{Slug="example2", Title="Example2", Description="Example2", Body="Example2", TagList="tag", CreatedAt="22-04-2021", UpdatedAt="22-04-2021"}
           };
           return posts;
        }
    }
}