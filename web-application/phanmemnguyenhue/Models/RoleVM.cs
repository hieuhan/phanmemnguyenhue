using phanmemnguyenhue.library;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace phanmemnguyenhue.Models
{
    public class RoleVM : BaseViewModel
    {
        public List<Status> StatusList { get; set; }
        public List<Roles> RolesList { get; set; }
    }

    public class RoleEditVM : BaseViewModel
    {
        public int RoleId { get; set; }

        [Display(Name = "Tên Quyền truy cập*")]
        [Required(ErrorMessage = "Vui lòng nhập {0}")]
        [StringLength(250, ErrorMessage = "{0} bao gồm từ {2} đến {1} ký tự.", MinimumLength = 3)]
        public string Name { get; set; }

        [Display(Name = "Mô tả")]
        [StringLength(250, ErrorMessage = "{0} bao gồm từ {2} đến {1} ký tự.", MinimumLength = 3)]
        public string Description { get; set; }

        [Display(Name = "Thứ tự hiển thị")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Vui lòng nhập {0} hợp lệ")]
        [Range(int.MinValue, int.MaxValue, ErrorMessage = "Vui lòng nhập {0} hợp lệ")]
        public int? DisplayOrder { get; set; }

        [Display(Name = "Thêm tiếp dữ liệu khác")]
        public bool AddAnother { get; set; }
        public List<Status> StatusList { get; set; }
    }
}