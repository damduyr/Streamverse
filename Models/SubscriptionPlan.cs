using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Streamverse.Models
{
   
    public class SubscriptionPlan
    {
        [Key]
        public int PlanID { get; set; }
        public int AdminID { get; set; }
        public string PlanName{ get; set; }
        public int PlanDuration { get; set; }
        public decimal Price { get; set; }
        public int MaxScreens { get; set; }
    }
}