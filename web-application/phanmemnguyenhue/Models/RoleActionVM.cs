using phanmemnguyenhue.library;
using System.Collections.Generic;

namespace phanmemnguyenhue.Models
{
    public class RoleActionVM : BaseViewModel
    {
        public int RoleId { get; set; }
        public int ActionId { get; set; }
        public int ParentId { get; set; }
        public string ParentName { get; set; }
        public string ParentDescription { get; set; }
        public Roles Roles { get; set; }
        public List<Actions> ActionsList { get; set; }
        public List<Actions> ParentActionsList { get; set; }
        public List<ActionStatus> ActionStatusList { get; set; }
    }
}