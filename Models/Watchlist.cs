using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Streamverse.Models
{
    public class Watchlist
    {
        [Key]
        public int WatchlistID { get; set; }
        [ForeignKey("UserProfile")]
        public int ProfileID { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        [ForeignKey("Content")]
        public int ContentID { get; set; }
        public virtual Content Content { get; set; }
        public DateTime DateAdded { get; set; }
    }
}