using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Streamverse.Models
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewID { get; set; }
        [ForeignKey("UserProfile")]
        public int ProfileID { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        [ForeignKey("Content")]
        public int ContentID { get; set; }
        public virtual Content Content { get; set; }
        [Required(ErrorMessage = "RatingValue is required.")]
        public int RatingValue { get; set; }
        public string RatingText { get; set; }
    }
}