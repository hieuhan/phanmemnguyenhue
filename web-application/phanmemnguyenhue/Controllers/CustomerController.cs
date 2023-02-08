using ClosedXML.Excel;
using phanmemnguyenhue.helper;
using phanmemnguyenhue.library;
using phanmemnguyenhue.Models;
using phanmemnguyenhue.Services.Attributes;
using phanmemnguyenhue.Services.Extensions;
using phanmemnguyenhue.Services.Sercurity;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace phanmemnguyenhue.Controllers
{
    [AuthorizeRole]
    public class CustomerController : Controller
    {
        // GET: Admin/Customer

        public async Task<ActionResult> Index(string keywords = "", int siteId = 1000, int categoryId = 0, int actionTypeId = 0, int landTypeId = 0, int provinceId = 0, int districtId = 0, int wardId = 0, int streetId = 0, int projectId = 0, int productTypeId = 0, int dataDisplayId = 0, int sortedBy = 0, int page = 0, int pageSize = 200)
        {
            MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

            Customers customers = new Customers
            {
                ActBy = myPrincipal.UserName,
                SiteId = siteId
            };

            byte verified = 0, isVideo = 0, showEmail = 0;
            string viewPath = "~/Views/Customer/Index.cshtml";

            if (productTypeId == 1)
            {
                verified = 1;
            }
            else if (productTypeId == 2)
            {
                isVideo = 1;
            }

            if (dataDisplayId == 2)
            {
                showEmail = 1;
            }

            Task<List<Sites>> sitesTask = Sites.Static_GetList();
            Task<List<Categories>> categoriesTask = Categories.Static_GetList(siteId);
            Task<List<ActionTypes>> actionTypesTask = ActionTypes.Static_GetList(siteId);
            Task<List<LandTypes>> landTypesTask = LandTypes.Static_GetList(siteId);
            Task<List<Provinces>> provincesTask = Provinces.Static_GetList(siteId);
            Task<List<Districts>> districtsTask = null;
            Task<List<Wards>> wardsTask = null;
            Task<List<Streets>> streetsTask = null;
            Task<List<Projects>> projectsTask = null;
            var customerGetPageTask = customers.GetPage(keywords, DateTime.MinValue, DateTime.MinValue, categoryId, landTypeId, actionTypeId, provinceId, districtId, wardId, streetId, projectId, verified, isVideo, showEmail, 0, sortedBy, page > 0 ? page - 1 : page, pageSize);

            if (provinceId > 0)
            {
                districtsTask = Districts.Static_GetListByProvince(siteId, provinceId);

                if (districtId > 0)
                {
                    projectsTask = Projects.Static_GetListBy(siteId, provinceId, districtId);

                    wardsTask = Wards.Static_GetListByDistrict(siteId, districtId);

                    if (wardId > 0)
                    {
                        streetsTask = Streets.Static_GetListByWard(siteId, wardId);

                        await Task.WhenAll(sitesTask, categoriesTask, actionTypesTask, landTypesTask, provincesTask, districtsTask, wardsTask, streetsTask, projectsTask, customerGetPageTask);
                    }
                    else
                    {
                        await Task.WhenAll(sitesTask, categoriesTask, actionTypesTask, landTypesTask, provincesTask, districtsTask, wardsTask, projectsTask, customerGetPageTask);
                    }
                }
                else
                {
                    projectsTask = Projects.Static_GetListBy(siteId, provinceId);

                    streetsTask = Streets.Static_GetListByProvince(siteId, provinceId);

                    await Task.WhenAll(sitesTask, categoriesTask, actionTypesTask, landTypesTask, provincesTask, streetsTask, projectsTask, customerGetPageTask);
                }
            }
            else
            {
                await Task.WhenAll(sitesTask, categoriesTask, actionTypesTask, landTypesTask, provincesTask, customerGetPageTask);
            }

            CustomerVM model = new CustomerVM
            {
                SiteId= siteId,
                CategoryId = categoryId,
                ActionTypeId = actionTypeId,
                LandTypeId = landTypeId,
                ProvinceId = provinceId,
                DistrictId = districtId,
                WardId = wardId,
                StreetId = streetId,
                SortedBy = sortedBy,
                ProductTypeId = productTypeId,
                DataDisplayId = dataDisplayId,
                SitesList = sitesTask != null && sitesTask.Result.IsAny() ? sitesTask.Result : new List<Sites>(),
                CategoriesList = categoriesTask != null && categoriesTask.Result.IsAny() ? categoriesTask.Result : new List<Categories>(),
                ActionTypesList = actionTypesTask != null && actionTypesTask.Result.IsAny() ? actionTypesTask.Result : new List<ActionTypes>(),
                LandTypesList = landTypesTask != null && landTypesTask.Result.IsAny() ? landTypesTask.Result : new List<LandTypes>(),
                ProvincesList = provincesTask != null && provincesTask.Result.IsAny() ? provincesTask.Result : new List<Provinces>(),
                DistrictsList = districtsTask != null && districtsTask.Result.IsAny() ? districtsTask.Result : new List<Districts>(),
                WardsList = wardsTask != null && wardsTask.Result.IsAny() ? wardsTask.Result : new List<Wards>(),
                StreetsList = streetsTask != null && streetsTask.Result.IsAny() ? streetsTask.Result : new List<Streets>(),
                ProjectsList = projectsTask != null && projectsTask.Result.IsAny() ? projectsTask.Result : new List<Projects>(),
                CustomersList = customerGetPageTask.Result.Item1,
                Pagination = new PaginationVM(page, pageSize, customerGetPageTask.Result.Item2)
            };

            if (Request.HttpMethod.ToLower() == "post")
            {
                if (model.CustomersList.IsAny())
                {
                    int number = 0;
                    DataTable dt = new DataTable("phanmemnguyenhue");

                    if (model.DataDisplayId == 1)
                    {
                        dt.Columns.AddRange(new DataColumn[4] { new DataColumn("STT"),
                                                     new DataColumn("Họ và tên"),
                                                     new DataColumn("Số điện thoại"),
                                                     new DataColumn("Bài đăng")});
                    }
                    else if (model.DataDisplayId == 2)
                    {
                        dt.Columns.AddRange(new DataColumn[4] { new DataColumn("STT"),
                                                     new DataColumn("Họ và tên"),
                                                     new DataColumn("Email"),
                                                     new DataColumn("Bài đăng")});
                    }
                    else
                    {
                        dt.Columns.AddRange(new DataColumn[5] { new DataColumn("STT"),
                                                     new DataColumn("Họ và tên"),
                                                     new DataColumn("Số điện thoại"),
                                                     new DataColumn("Email"),
                                                     new DataColumn("Bài đăng")});
                    }

                    for (int index = 0; index < model.CustomersList.Count; index++)
                    {
                        number = index + (model.Pagination.PageIndex > 0 ? model.Pagination.PageIndex - 1 : model.Pagination.PageIndex) * model.Pagination.PageSize + 1;

                        if (model.DataDisplayId == 1)
                        {
                            dt.Rows.Add(number, model.CustomersList[index].FullName, model.CustomersList[index].PhoneNumber, model.CustomersList[index].TotalProducts.ToString("#,###"));
                        }
                        else if (model.DataDisplayId == 2)
                        {
                            dt.Rows.Add(number, model.CustomersList[index].FullName, model.CustomersList[index].Email.DefaultIfEmpty(), model.CustomersList[index].TotalProducts.ToString("#,###"));
                        }
                        else
                        {
                            dt.Rows.Add(number, model.CustomersList[index].FullName, model.CustomersList[index].PhoneNumber, model.CustomersList[index].Email.DefaultIfEmpty(), model.CustomersList[index].TotalProducts.ToString("#,###"));
                        }
                    }

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        string headerRowName = string.Empty;
                        var ws = wb.Worksheets.Add("phanmemnguyenhue");

                        // Adding HeaderRow
                        ws.Cell("A1").Value = "Danh sách khách hàng";
                        ws.Cell("A1").Style.Font.Bold = true;

                        if (model.ProvinceId > 0 && model.ProvincesList.IsAny())
                        {
                            var province = model.ProvincesList.FirstOrDefault(x => x.ProvinceId == model.ProvinceId);

                            if (province != null && !string.IsNullOrWhiteSpace(province.Name))
                            {
                                headerRowName += province.Name;
                            }
                        }

                        if (model.DistrictId > 0 && model.DistrictsList.IsAny())
                        {
                            var district = model.DistrictsList.FirstOrDefault(x => x.DistrictId == model.DistrictId);

                            if (district != null && !string.IsNullOrWhiteSpace(district.Name))
                            {
                                if (!string.IsNullOrWhiteSpace(headerRowName))
                                {
                                    headerRowName += ", ";
                                }

                                headerRowName += district.Name;
                            }
                        }

                        if (model.WardId > 0 && model.WardsList.IsAny())
                        {
                            var wards = model.WardsList.FirstOrDefault(x => x.WardId == model.WardId);

                            if (wards != null && !string.IsNullOrWhiteSpace(wards.Name))
                            {
                                if (!string.IsNullOrWhiteSpace(headerRowName))
                                {
                                    headerRowName += ", ";
                                }

                                headerRowName += wards.Name;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(headerRowName))
                        {
                            ws.Cell("A2").Value = headerRowName;
                        }

                        ws.Cell("A4").Style.Font.Bold = true;
                        ws.Cell("B4").Style.Font.Bold = true;
                        ws.Cell("C4").Style.Font.Bold = true;
                        ws.Cell("D4").Style.Font.Bold = true;
                        ws.Cell("E4").Style.Font.Bold = true;

                        if (dt.Rows.Count > 0)
                        {
                            ws.Cell("A4").Value = dt.Columns[0].ColumnName;
                            ws.Cell("B4").Value = dt.Columns[1].ColumnName;
                            ws.Cell("C4").Value = dt.Columns[2].ColumnName;

                            if (dt.Columns.Count > 3)
                            {
                                ws.Cell("D4").Value = dt.Columns[3].ColumnName;
                            }

                            if (dt.Columns.Count > 4)
                            {
                                ws.Cell("E4").Value = dt.Columns[4].ColumnName;
                            }

                            // Adding DataRows
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                ws.Cell("A" + (i + 5)).Value = dt.Rows[i][0];
                                ws.Cell("B" + (i + 5)).Value = dt.Rows[i][1];
                                ws.Cell("C" + (i + 5)).Value = dt.Rows[i][2];
                                if (dt.Rows[i].ItemArray.Length > 3)
                                {
                                    ws.Cell("D" + (i + 5)).Value = dt.Rows[i][3];
                                }
                                if (dt.Rows[i].ItemArray.Length > 4)
                                {
                                    ws.Cell("E" + (i + 5)).Value = dt.Rows[i][4];
                                }
                            }
                        }

                        ws.Column(2).AdjustToContents();
                        ws.Column(3).AdjustToContents();
                        ws.Column(4).AdjustToContents();

                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"phanmemnguyenhue_{DateTime.Now:ddMMyyyHHmmss}.xlsx");
                        }
                    }
                }
            }

            if (model.SitesList.IsAny() && siteId > 0)
            {
                var site = model.SitesList.FirstOrDefault(x => x.SiteId == siteId);

                if (site != null && !string.IsNullOrWhiteSpace(site.CustomerViewPath))
                {
                    viewPath = site.CustomerViewPath;
                }
            }

            return View(viewPath, model);
        }
    }
}