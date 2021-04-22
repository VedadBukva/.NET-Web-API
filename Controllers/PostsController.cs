using System.Collections.Generic;
using _NET_Web_API.Data;
using _NET_Web_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace _NET_Web_API.Controllers
{     
    // api/posts
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IWebApiRepository _repository;
        public PostsController(IWebApiRepository repository)
        {   
            _repository = repository;
        }

        [HttpGet]
        public ActionResult <IEnumerable<Post>> GetAllPosts()
        {
            var posts = _repository.GetPosts();
            return Ok(posts);
        }

        [HttpGet("{slug}")]
        public ActionResult <Post> GetPostBySlug(string slug)
        {
            var post = _repository.GetPostsBySlug(slug);
            return Ok(post);
        }
    }
}