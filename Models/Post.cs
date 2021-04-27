using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace _NET_Web_API.Models
{
    #region DbModels
    [Keyless]
    public class Post
    {
        [Required]
        public string Slug { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Body { get; set; }
        public string TagList { get; set; }
        [Required]
        public string CreatedAt { get; set; }
        [Required]
        public string UpdatedAt { get; set; }
    }

    [Keyless]
    public class Tag
    {
        [Required]
        public string Slug { get; set; }
        [Required]
        public string TagDescription { get; set; }
    }
    #endregion

    #region RepresentativeModels
    public class PostWithTags
    {
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }
        public string[] TagList { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }

    public class Tags
    {
        public string[] tags { get; set; }
    }

    public class BlogPost
    {
        public PostWithTags blogPost { get; set; }
    }

    public class BlogPosts
    {
        public PostWithTags[] blogPosts { get; set; }
        public int postCount { get; set; }    
    }
    #endregion
}