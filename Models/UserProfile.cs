using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Streamverse.Models
{
    public class UserProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProfileID { get; set; }
        public int UserID { get; set; }
        public string ProfileName { get; set; }
        public int Age { get; set; }
        public string Avatar { get; set; }
        public bool IsKidProfile { get; set; }

    }
}