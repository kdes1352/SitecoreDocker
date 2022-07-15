using System;
using System.Collections.Generic;
using Sitecore.Data.Items;

namespace AllinaHealth.Models.ViewModels.Reporting
{
    public class ModifiedItemsViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool UseModifiedDate { get; set; }
        public List<Item> List { get; set; }
        public string Sort { get; set; }
    }
}