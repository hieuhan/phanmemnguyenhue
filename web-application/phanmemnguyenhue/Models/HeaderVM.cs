using phanmemnguyenhue.library;
using System.Collections.Generic;

namespace phanmemnguyenhue.Models
{
    public class HeaderVM
    {
        public List<Actions> ActionsList { get; set; }
        public List<Actions> ParentActionsList { get; set; }
    }
}