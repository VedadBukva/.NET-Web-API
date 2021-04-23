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

        #region Posts
        [HttpGet]
        public ActionResult <IEnumerable<Post>> GetAllPosts()
        {
            var posts = _repository.GetPosts().ToList();
            List<PostWithTags> listOfNewPosts = new List<PostWithTags>();
            foreach(var post in posts)
            {
                listOfNewPosts.Add(AddTagsToPost(post));
            }
            if(posts.Count() > 1 )
            {
                BlogPosts blogPosts = new BlogPosts();
                blogPosts.postCount = 0;

                List<PostWithTags> listOfPosts = new List<PostWithTags>();
                foreach(var post in listOfNewPosts)
                {
                    listOfPosts.Add(post);
                    blogPosts.postCount++;
                }
                PostWithTags[] postArray = listOfPosts.ToArray();
                blogPosts.blogPosts = postArray;
                return Ok(blogPosts);
            }
            else
            {
                BlogPost blogPost = new BlogPost();
                blogPost.blogPost = posts[0];
                return Ok(blogPost);
            }
        }

        [HttpGet("{slug}")]
        public ActionResult <Post> GetPostBySlug(string slug)
        {
            return Ok(AddTagsToPost(_repository.GetPostBySlug(slug)));
        }
        #endregion
        
        #region Tags
        [HttpGet("tags")]
        public ActionResult <IEnumerable<Post>> GetAllTags()
        {
            var tags = _repository.GetTags().ToList();
            List<String> listOfTags = new List<String>();
            foreach (var tag in tags) 
            {
                listOfTags.Add(tag.TagDescription);
            }
            String[] addedTags = listOfTags.ToArray();
            Tags returnTags = new Tags();
            returnTags.tags = addedTags;
            return Ok(returnTags);
        }
        #endregion      

        #region Methods
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
        #endregion
    }
}