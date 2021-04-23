using System;
using System.Collections.Generic;
using System.Linq;
using _NET_Web_API.Data;
using _NET_Web_API.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace _NET_Web_API.Controllers
{     
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
            return Ok(AddTagsToPost(_repository.GetPostBySlug(slug)));
        }
        
        [HttpGet("tags")]
        public ActionResult <IEnumerable<Post>> GetAllTags()
        {
            var posts = _repository.GetTags();
            return Ok(posts);
        }

        [HttpGet("tags/{slug}")]
        public ActionResult <IEnumerable<Tag>> GetTagsBySlug(string slug)
        {
            var tags = _repository.GetTagsBySlug(slug);
            return Ok(tags);
        }

        private List<String> FindTagsOfPost(Post post)
        {
            var listOfTags = _repository.GetTags();
            List<String> addTags = new List<String>();
            foreach (var tag in listOfTags) 
            {
                if(post.Slug == tag.Slug)
                {
                    addTags.Add(tag.TagDescription);
                }
            }
            return addTags;
        }

        private PostWithTags AddTagsToPost(Post oldPost)
        {
            PostWithTags newPost = new PostWithTags();
            var listOfTags = FindTagsOfPost(oldPost);
            String[] arrayOfTags = {};
            List<string> addTagsToPost = new List<string>();
            foreach (var tag in listOfTags) 
            {
                addTagsToPost.Add(tag);
            }
            String[] addedTags = addTagsToPost.ToArray();

            newPost.Slug = oldPost.Slug;
            newPost.Title = oldPost.Title;
            newPost.Description = oldPost.Description;
            newPost.Body = oldPost.Body;
            newPost.TagList = addedTags;
            newPost.CreatedAt = oldPost.CreatedAt;
            newPost.UpdatedAt = oldPost.UpdatedAt;
            return newPost;
        }
    }
}