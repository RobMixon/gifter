using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gifter.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        //this property can be null
        public string Caption { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public int UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }
        public List<Comment> Comments { get; set; }

    }
}