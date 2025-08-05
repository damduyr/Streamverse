using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Streamverse.Models
{
    public class HomePageViewModel
    {
        public IEnumerable<MovieSummmaryViewModel> AllMovies { get; set; }
        public IEnumerable<MovieSummmaryViewModel> Top5TrendingMovies { get; set; }
        public IEnumerable<MovieSummmaryViewModel> FilteredMovies { get; set; }
        public IEnumerable<MovieSummmaryViewModel> TopRegionalMovies { get; set; }
        public IEnumerable<MovieSummmaryViewModel> SortedWatchlistMovies { get; set; }
        public IEnumerable<MovieSummmaryViewModel> Top5WatchlistMovies { get; set; }
        public class MovieSummmaryViewModel
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
}