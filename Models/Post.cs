using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace _NET_Web_API.Models
{
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
}