using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class Estate
    {
        public int EstateId { get; set; }
        public string EstateName { get; set; }
        public Nullable<double> Longitude { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> BuildingArea { get; set; }
        public Nullable<double> ResidentialArea { get; set; }
        public Nullable<int> ResidentialCount { get; set; }
        public Nullable<double> HouseArea { get; set; }
        public Nullable<int> HouseCount { get; set; }
        public Nullable<double> BasementArea { get; set; }
        public Nullable<int> CarplaceCount { get; set; }
        public Nullable<double> MatchingArea { get; set; }
        public Nullable<double> GreenArea { get; set; }
        public string ImageList { get; set; }
        public string Address { get; set; }
        public string Introduct { get; set; }
    }
}
