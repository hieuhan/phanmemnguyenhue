using phanmemnguyenhue.library;
using System.Collections.Generic;

namespace phanmemnguyenhue.Models
{
    public class CustomerVM : BaseViewModel
    {
        public int SiteId { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }
        public int StreetId { get; set; }
        public int ProjectId { get; set; }
        public int LocationTypeId { get; set; }
        public int LandTypeId { get; set; }
        public int ActionTypeId { get; set; }
        public int ProductTypeId { get; set; }
        public int CategoryId { get; set; }
        public int DataDisplayId { get; set; }
        public List<Sites> SitesList { get; set; }
        public List<Categories> CategoriesList { get; set; }
        public List<ActionTypes> ActionTypesList { get; set; }
        public List<LandTypes> LandTypesList { get; set; }
        public List<Provinces> ProvincesList { get; set; }
        public List<Districts> DistrictsList { get; set; }
        public List<Wards> WardsList { get; set; }
        public List<Streets> StreetsList { get; set; }
        public List<Projects> ProjectsList { get; set; }
        public List<Customers> CustomersList { get; set; }
    }
}