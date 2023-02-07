using phanmemnguyenhue.library;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace phanmemnguyenhue.Models
{
    public class ActionsVM : BaseViewModel
    {
        public int ActionId { get; set; }
        public int ParentId { get; set; }
        public string ParentName { get; set; }
        public string ParentDescription { get; set; }
        public List<Actions> ActionsList { get; set; }
        public List<Actions> ParentActionsList { get; set; }
        public List<ActionStatus> ActionStatusList { get; set; }
    }

    public class ActionEditVM : BaseViewModel
    {
        public int ActionId { get; set; }

        [Display(Name = "Tên chức năng*")]
        [Required(ErrorMessage = "Vui lòng nhập {0}")]
        [StringLength(255, ErrorMessage = "{0} bao gồm từ {2} đến {1} ký tự.", MinimumLength = 3)]
        public string Name { get; set; }

        [Display(Name = "Mô tả chức năng")]
        [StringLength(255, ErrorMessage = "{0} bao gồm từ {2} đến {1} ký tự.", MinimumLength = 3)]
        public string Description { get; set; }

        public string ParentName { get; set; }

        public string ParentDescription { get; set; }

        [Display(Name = "Đường dẫn*")]
        [Required(ErrorMessage = "Vui lòng nhập {0}")]
        [StringLength(255, ErrorMessage = "{0} bao gồm từ {2} đến {1} ký tự.", MinimumLength = 1)]
        public string Path { get; set; }

        [Display(Name = "Controller*")]
        [Required(ErrorMessage = "Vui lòng nhập {0}")]
        [StringLength(50, ErrorMessage = "{0} bao gồm từ {2} đến {1} ký tự.", MinimumLength = 1)]
        public string Controller { get; set; }

        [Display(Name = "Action*")]
        [Required(ErrorMessage = "Vui lòng nhập {0}")]
        [StringLength(50, ErrorMessage = "{0} bao gồm từ {2} đến {1} ký tự.", MinimumLength = 1)]
        public string Action { get; set; }

        [Display(Name = "Chức năng cha")]
        public int ParentId { get; set; }

        [Display(Name = "Hiển thị")]
        public byte Display { get; set; }

        [Display(Name = "Thứ tự hiển thị")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Vui lòng nhập {0} hợp lệ")]
        [Range(int.MinValue, int.MaxValue, ErrorMessage = "Vui lòng nhập {0} hợp lệ")]
        public int? DisplayOrder { get; set; }

        [AllowHtml]
        [Display(Name = "Icon")]
        [StringLength(1000, ErrorMessage = "{0} bao gồm từ {2} đến {1} ký tự.", MinimumLength = 3)]
        public string IconPath { get; set; }

        [Display(Name = "Thêm tiếp dữ liệu khác")]
        public bool AddAnother { get; set; }

        public List<Actions> ParentActionsList { get; set; }
        public List<ActionStatus> ActionStatusList { get; set; }
    }
}