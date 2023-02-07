using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace phanmemnguyenhue.Models
{
    public class BaseViewModel
    {
        public byte StatusId { get; set; }
        public byte ReviewStatusId { get; set; }
        public string Keywords { get; set; }
        public string PreviousPage { get; set; }
        public int SearchByType { get; set; }
        public byte SearchByDateType { get; set; }
        public int OrderByClauseId { get; set; }
        public int SearchByClauseId { get; set; }
        public int SortedBy { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public PaginationVM Pagination { get; set; }
    }
}