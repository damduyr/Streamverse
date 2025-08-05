using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Streamverse.Models
{
    public class Movie
    {
        [Key, ForeignKey("Content")]
        public int MovieID { get; set; }
        public int DurationInMinutes { get; set; }
        public string TrailerURL { get; set; }
        public string PosterURL { get; set; }
        public string VideoUrl { get; set; }
        public decimal OverallRating { get; set; }
        public virtual Content Content { get; set; }
    }
}