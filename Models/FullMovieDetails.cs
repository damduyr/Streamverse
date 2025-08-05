using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Streamverse.Models
{
    public class FullMovieDetails
    {
        [Key]
        public int MovieID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string ContentType { get; set; }
        public string GenreName { get; set; }
        public int DurationInMinutes { get; set; }
        public string TrailerURL { get; set; }
        public string PosterURL { get; set; }
        public string VideoUrl { get; set; }
        public decimal OverallRating { get; set; }
    }
}