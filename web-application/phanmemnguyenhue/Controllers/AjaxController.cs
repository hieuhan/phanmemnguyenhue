using phanmemnguyenhue.helper;
using phanmemnguyenhue.library;
using phanmemnguyenhue.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace phanmemnguyenhue.Controllers
{
    public class AjaxController : Controller
    {
        // GET: Admin/Ajax
        public async Task<JsonResult> GetDistrictsBy(int siteId = 0, int provinceId = 0)
        {
            List<ObjectJsonVM> resultVar = new List<ObjectJsonVM>();

            if (siteId > 0 && provinceId > 0)
            {
                var districtsList = await Districts.Static_GetListByProvince(siteId, provinceId);
                if (districtsList.IsAny())
                {
                    resultVar.AddRange(districtsList.Select(x => new ObjectJsonVM()
                    {
                        Id = x.DistrictId,
                        Name = x.Name
                    }));
                }
            }

            return Json(resultVar, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetWardsBy(int siteId = 0, int districtId = 0)
        {
            List<ObjectJsonVM> resultVar = new List<ObjectJsonVM>();

            if (siteId > 0 && districtId > 0)
            {
                var wardsList = await Wards.Static_GetListByDistrict(siteId, districtId);
                if (wardsList.IsAny())
                {
                    resultVar.AddRange(wardsList.Select(x => new ObjectJsonVM()
                    {
                        Id = x.WardId,
                        Name = x.Name
                    }));
                }
            }

            return Json(resultVar, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetStreetsBy(int siteId = 0, int wardId = 0)
        {
            List<ObjectJsonVM> resultVar = new List<ObjectJsonVM>();

            if (siteId > 0 && wardId > 0)
            {
                var streetsList = await Streets.Static_GetListByWard(siteId, wardId);
                if (streetsList.IsAny())
                {
                    resultVar.AddRange(streetsList.Select(x => new ObjectJsonVM()
                    {
                        Id = x.StreetId,
                        Name = x.Name
                    }));
                }
            }

            return Json(resultVar, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetProjectsBy(int siteId = 0, int provinceId = 0, int districtId = 0)
        {
            List<ObjectJsonVM> resultVar = new List<ObjectJsonVM>();

            if (siteId > 0 && provinceId > 0)
            {
                var projectsList = await Projects.Static_GetListBy(siteId, provinceId, districtId);

                if (projectsList.IsAny())
                {
                    resultVar.AddRange(projectsList.Select(x => new ObjectJsonVM()
                    {
                        Id = x.ProjectId,
                        Name = x.Name
                    }));
                }
            }

            return Json(resultVar, JsonRequestBehavior.AllowGet);
        }
    }
}