using System.Net.Http;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using _NET_Web_API.Data;
using _NET_Web_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace _NET_Web_API.Controllers
{
    [Route("api")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IWebApiRepository _repository;
        public PostsController(IWebApiRepository repository)
        {   
            _repository = repository;
        }

        #region PostsRequests
        [HttpGet("posts")]
        public ActionResult <IEnumerable<Post>> GetAllPosts()
        {
            var posts = _repository.GetPosts().ToList();
            if(posts.Count() == 0)
            {
                return StatusCode(404,"ERROR: No posts in database!");
            }
            List<PostWithTags> listOfNewPosts = new List<PostWithTags>();
            foreach(var post in posts)
            {
                listOfNewPosts.Add(AddTagsToPost(post));
            }
            if(listOfNewPosts.Count() > 1 )
            {
                BlogPosts blogPosts = new BlogPosts();
                blogPosts.postCount = 0;

                List<PostWithTags> listOfPosts = new List<PostWithTags>();
                foreach(var post in listOfNewPosts)
                {
                    listOfPosts.Add(post);
                    blogPosts.postCount++;
                }
                blogPosts.blogPosts = listOfPosts.ToArray();
                string tagFromQueryString = HttpContext.Request.Query["tag"].ToString();
                if(!tagFromQueryString.Equals(""))
                {
                    return Ok(SortPostsByTag(blogPosts,tagFromQueryString));
                }
                return Ok(blogPosts);
            }
            else
            {
                BlogPost blogPost = new BlogPost();
                blogPost.blogPost = listOfNewPosts[0];
                return Ok(blogPost);
            }
        }

        [HttpGet("posts/{slug}")]
        public ActionResult <Post> GetPostBySlug(string slug)
        {
            BlogPost blogPost = new BlogPost();
            blogPost.blogPost = AddTagsToPost(_repository.GetPostBySlug(slug));
            return Ok(blogPost);
        }

        [HttpPost("posts")]
        public ActionResult CreateBlogPost([FromBody] BlogPost bP)
        {
            if(ArePostAttributesFilledCorectly(bP.blogPost))
            {
                bP.blogPost.Slug = GeneratePostSlug(bP.blogPost.Title.Trim());
                bP.blogPost.Title = bP.blogPost.Title.Trim();
                bP.blogPost.Description = bP.blogPost.Description.Trim();
                bP.blogPost.Body = bP.blogPost.Body.Trim();
                                
                DateTime today = DateTime.Parse(DateTime.Now.ToString());
                bP.blogPost.CreatedAt = today.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                for(var i=0; i<bP.blogPost.TagList.Count(); i++)
                {
                    bP.blogPost.TagList[i] = bP.blogPost.TagList[i].Trim();
                }

                var connectionStringBuilder = new SqliteConnectionStringBuilder(); 
                connectionStringBuilder.DataSource = "./Posts.db";
                using(var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
                {
                    connection.Open();
                    using(var transaction = connection.BeginTransaction())
                    {
                        var insertBlogPost = connection.CreateCommand();
                        insertBlogPost.CommandText = "INSERT INTO Posts Values(@slug,@title,@description,@body,'',@today,'')";
                        insertBlogPost.Parameters.AddWithValue("@slug",bP.blogPost.Slug);
                        insertBlogPost.Parameters.AddWithValue("@title",bP.blogPost.Title);
                        insertBlogPost.Parameters.AddWithValue("@description",bP.blogPost.Description);
                        insertBlogPost.Parameters.AddWithValue("@body",bP.blogPost.Body);
                        insertBlogPost.Parameters.AddWithValue("@today",bP.blogPost.CreatedAt);
                        insertBlogPost.ExecuteNonQuery();

                        foreach(var tag in bP.blogPost.TagList)
                        {
                            var insertTagsOfBlogPost = connection.CreateCommand();
                            insertTagsOfBlogPost.CommandText = "INSERT INTO Tags Values(@slug,@tag)";
                            insertTagsOfBlogPost.Parameters.AddWithValue("@slug",bP.blogPost.Slug);
                            insertTagsOfBlogPost.Parameters.AddWithValue("@tag",tag);
                            insertTagsOfBlogPost.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    connection.Close();
                }
                return Ok(bP);
            }
            return StatusCode(400,"ERROR: Required fields are title, description and body! (Tag list is optional)");
        }

        [HttpPut("posts/{slug}")]
        public ActionResult UpdateBlogPost(string slug, [FromBody] BlogPost bP)
        {
            List<PostWithTags> listOfNewPosts = new List<PostWithTags>();
            foreach(var post in _repository.GetPosts().ToList())
            {
                listOfNewPosts.Add(AddTagsToPost(post));
            }
            if(!ArePostAttributesEmpty(bP.blogPost))
            {
                foreach(var post in listOfNewPosts)
                {
                    if(post.Slug == slug)
                    {
                        if(bP.blogPost.Title != null) {
                            bP.blogPost.Slug = GeneratePostSlug(bP.blogPost.Title.Trim());
                            bP.blogPost.Title = bP.blogPost.Title.Trim();
                        }
                        if(bP.blogPost.Description != null) {
                            bP.blogPost.Description = bP.blogPost.Description.Trim();
                        }
                        if(bP.blogPost.Body != null) {
                            bP.blogPost.Body = bP.blogPost.Body.Trim();
                        }
                        bP.blogPost.CreatedAt = post.CreatedAt;
                        foreach(var tag in post.TagList)
                        {
                            bP.blogPost.TagList.Append(tag);
                        }
                        

                        DateTime today = DateTime.Parse(DateTime.Now.ToString());
                        bP.blogPost.UpdatedAt = today.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                        for(var i=0; i<bP.blogPost.TagList.Count(); i++)
                        {
                            bP.blogPost.TagList[i] = bP.blogPost.TagList[i].Trim();
                        }

                        var connectionStringBuilder = new SqliteConnectionStringBuilder();
                        connectionStringBuilder.DataSource = "./Posts.db";
                        using(var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
                        {
                            connection.Open();
                            using(var transaction = connection.BeginTransaction())
                            {
                                try
                                {
                                var updateBlogPost = connection.CreateCommand();
                                if(bP.blogPost.Description != null && bP.blogPost.Body != null && bP.blogPost.Title != null)
                                {
                                    updateBlogPost.CommandText = "UPDATE Posts Set Slug = @slug, Title = @title, Description = @description, Body = @body, UpdatedAt = @updatedAt Where Slug = @requiredSlug";
                                    updateBlogPost.Parameters.AddWithValue("@requiredSlug",slug);
                                    updateBlogPost.Parameters.AddWithValue("@slug",bP.blogPost.Slug);
                                    updateBlogPost.Parameters.AddWithValue("@title",bP.blogPost.Title);
                                    updateBlogPost.Parameters.AddWithValue("@description",bP.blogPost.Description);
                                    updateBlogPost.Parameters.AddWithValue("@body",bP.blogPost.Body);
                                    updateBlogPost.Parameters.AddWithValue("@updatedAt",bP.blogPost.UpdatedAt);
                                }
                                else if(bP.blogPost.Description ==null && bP.blogPost.Body == null)
                                {
                                    updateBlogPost.CommandText = "UPDATE Posts Set Slug = @slug, Title = @title, UpdatedAt = @updatedAt Where Slug = @requiredSlug";
                                    updateBlogPost.Parameters.AddWithValue("@requiredSlug",slug);
                                    updateBlogPost.Parameters.AddWithValue("@slug",bP.blogPost.Slug);
                                    updateBlogPost.Parameters.AddWithValue("@title",bP.blogPost.Title);
                                    updateBlogPost.Parameters.AddWithValue("@updatedAt",bP.blogPost.UpdatedAt);
                                    bP.blogPost.Body = post.Body;
                                    bP.blogPost.Description = post.Description;
                                }
                                else if(bP.blogPost.Title == null && bP.blogPost.Description == null)
                                {
                                    updateBlogPost.CommandText = "UPDATE Posts Set Body = @body, UpdatedAt = @updatedAt Where Slug = @requiredSlug";
                                    updateBlogPost.Parameters.AddWithValue("@requiredSlug",slug);
                                    updateBlogPost.Parameters.AddWithValue("@body",bP.blogPost.Body);
                                    updateBlogPost.Parameters.AddWithValue("@updatedAt",bP.blogPost.UpdatedAt);
                                    bP.blogPost.Title = post.Title;
                                    bP.blogPost.Description = post.Description;
                                }
                                else if(bP.blogPost.Title == null && bP.blogPost.Body == null)
                                {
                                    updateBlogPost.CommandText = "UPDATE Posts Set Description = @description, UpdatedAt = @updatedAt Where Slug = @requiredSlug";
                                    updateBlogPost.Parameters.AddWithValue("@requiredSlug",slug);
                                    updateBlogPost.Parameters.AddWithValue("@description",bP.blogPost.Description);
                                    updateBlogPost.Parameters.AddWithValue("@updatedAt",bP.blogPost.UpdatedAt);
                                    bP.blogPost.Title = post.Title;
                                    bP.blogPost.Body = post.Body;
                                }
                                else if(bP.blogPost.Body == null)
                                {
                                    updateBlogPost.CommandText = "UPDATE Posts Set Slug = @slug, Title = @title, Description = @description, UpdatedAt = @updatedAt Where Slug = @requiredSlug";
                                    updateBlogPost.Parameters.AddWithValue("@requiredSlug",slug);
                                    updateBlogPost.Parameters.AddWithValue("@slug",bP.blogPost.Slug);
                                    updateBlogPost.Parameters.AddWithValue("@title",bP.blogPost.Title);
                                    updateBlogPost.Parameters.AddWithValue("@description",bP.blogPost.Description);
                                    updateBlogPost.Parameters.AddWithValue("@updatedAt",bP.blogPost.UpdatedAt);
                                    bP.blogPost.Body = post.Body;
                                }
                                else if(bP.blogPost.Description == null)
                                {
                                    updateBlogPost.CommandText = "UPDATE Posts Set Slug = @slug, Title = @title, Body = @body, UpdatedAt = @updatedAt Where Slug = @requiredSlug";
                                    updateBlogPost.Parameters.AddWithValue("@requiredSlug",slug);
                                    updateBlogPost.Parameters.AddWithValue("@slug",bP.blogPost.Slug);
                                    updateBlogPost.Parameters.AddWithValue("@title",bP.blogPost.Title);
                                    updateBlogPost.Parameters.AddWithValue("@body",bP.blogPost.Body);
                                    updateBlogPost.Parameters.AddWithValue("@updatedAt",bP.blogPost.UpdatedAt);
                                    bP.blogPost.Description = post.Description;
                                }
                                else if(bP.blogPost.Title == null)
                                {
                                    updateBlogPost.CommandText = "UPDATE Posts Set Body = @body, Description = @description, UpdatedAt = @updatedAt Where Slug = @requiredSlug";
                                    updateBlogPost.Parameters.AddWithValue("@requiredSlug",slug);
                                    updateBlogPost.Parameters.AddWithValue("@body",bP.blogPost.Body);
                                    updateBlogPost.Parameters.AddWithValue("@description",bP.blogPost.Description);
                                    updateBlogPost.Parameters.AddWithValue("@updatedAt",bP.blogPost.UpdatedAt);
                                    bP.blogPost.Title = post.Title;
                                }
                                
                                updateBlogPost.ExecuteNonQuery();
                            
                                foreach(var tag in bP.blogPost.TagList.Distinct())
                                {
                                    var updateTagsOfBlogPost = connection.CreateCommand();
                                    updateTagsOfBlogPost.CommandText = "UPDATE Tags Set Slug = @slug, TagDescription = @tag Where Slug = @requiredSlug";
                                    updateTagsOfBlogPost.Parameters.AddWithValue("@requiredSlug",slug);
                                    updateTagsOfBlogPost.Parameters.AddWithValue("@slug",bP.blogPost.Slug);
                                    updateTagsOfBlogPost.Parameters.AddWithValue("@tag",tag);
                                    updateTagsOfBlogPost.ExecuteNonQuery();
                                }
                                transaction.Commit();
                                }
                                catch (System.Exception)
                                {
                                    throw new Exception("WARNING: Update queries don't work properly!");
                                }
                            }
                            connection.Close();
                        }
                    return Ok(bP);
                    }
                }
            }
            return StatusCode(400,"ERROR: Bad PUT request!");
        }

        [HttpDelete("posts/{slug}")]
        public ActionResult DeleteEmployee(string slug)  
        {  
            Post post = this.FindPostBySlug(slug);  
            if (post == null)  
            {  
                return StatusCode(404,"ERROR: Required post not found!");  
            }  
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "./Posts.db";
            using(var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();
                using(var transaction = connection.BeginTransaction())
                {
                    var deletePost = connection.CreateCommand();
                    deletePost.CommandText = "DELETE FROM Posts WHERE Slug = @slug";
                    deletePost.Parameters.AddWithValue("@slug",slug);
                    deletePost.ExecuteNonQuery();
                    
                    foreach(var tag in AddTagsToPost(FindPostBySlug(slug)).TagList)
                    {
                    var deleteTag = connection.CreateCommand();
                    deleteTag.CommandText = "DELETE FROM Tags WHERE Slug = @slug";
                    deleteTag.Parameters.AddWithValue("@slug",slug);
                    deleteTag.ExecuteNonQuery();
                    }                                
                    transaction.Commit();
                }   
            connection.Close();
            }         
            return StatusCode(200,$"SUCCESS: Post with slug \"{slug}\" is deleted!");  
        } 
        #endregion
        
        #region TagsRequests
        [HttpGet("tags")]
        public ActionResult <IEnumerable<Post>> GetAllTags()
        {
            var tags = _repository.GetTags().ToList();
            List<String> listOfTags = new List<String>();
            foreach (var tag in tags) 
            {
                listOfTags.Add(tag.TagDescription);
            }
            String[] addedTags = listOfTags.Distinct().ToArray();
            Tags returnTags = new Tags();
            returnTags.tags = addedTags;
            return Ok(returnTags);
        }
        #endregion      

        #region AuxiliaryMethods
        private Boolean ArePostAttributesFilledCorectly(PostWithTags post)
        {
            if(post.Title == null || post.Title == String.Empty 
            || post.Description == null || post.Description == String.Empty 
            || post.Body == null || post.Body == String.Empty) 
            {
                return false;
            }
            return true;
        }
        private Boolean ArePostAttributesEmpty(PostWithTags post)
        {
            if(post.Title == null && post.Description == null && post.Body == null) 
            {
                return true;
            }
            return false;
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
            String[] addedTags = addTagsToPost.Distinct().ToArray();

            newPost.Slug = oldPost.Slug;
            newPost.Title = oldPost.Title;
            newPost.Description = oldPost.Description;
            newPost.Body = oldPost.Body;
            newPost.TagList = addedTags;
            newPost.CreatedAt = oldPost.CreatedAt;
            newPost.UpdatedAt = oldPost.UpdatedAt;
            return newPost;
        }

        public Post FindPostBySlug(string slug)
        {
            return _repository.GetPostBySlug(slug);
        }

        public string GeneratePostSlug(string title)
        {
            var resultSlug = new string(title.Where(c => !char.IsPunctuation(c)).ToArray());
            resultSlug = RemoveDiacritics(resultSlug);
            resultSlug = Regex.Replace(resultSlug, @"\s+", "-");
            return resultSlug;
        }

        public String RemoveDiacritics(string postTitle)
        {
        String normalizedString = postTitle.Normalize(NormalizationForm.FormD);
        StringBuilder stringBuilder = new StringBuilder();
    
        for (int i = 0; i < normalizedString.Length; i++)
            {
            Char c = normalizedString[i];
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                stringBuilder.Append(c);
                }
            }
    
        return stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToLower();
        }  

        public BlogPosts SortPostsByTag(BlogPosts bp,string findTag) {
            List<PostWithTags> tagFoundInPost = new List<PostWithTags>();
            List<PostWithTags> tagNotFoundInPost = new List<PostWithTags>();
            foreach(var post in bp.blogPosts)
            {
                var tagExist = false;
                foreach(var tag in post.TagList)
                {
                    if(tag.ToLower().Contains(findTag.ToLower()))
                    {
                        tagExist = true;
                        break;
                    }
                }
                if(tagExist == true)
                {
                    tagFoundInPost.Add(post);
                }
                else 
                {
                    tagNotFoundInPost.Add(post);
                }
            }

            List<PostWithTags> sortedPosts = new List<PostWithTags>();
            foreach(var post in tagFoundInPost)
            {
                sortedPosts.Add(post);
            }
            foreach(var post in tagNotFoundInPost)
            {
                sortedPosts.Add(post);
            }
            
            BlogPosts sortedPostsInBlogPosts = new BlogPosts();
            sortedPostsInBlogPosts.blogPosts = sortedPosts.ToArray();
            return sortedPostsInBlogPosts;
        }

        #endregion
    }
}