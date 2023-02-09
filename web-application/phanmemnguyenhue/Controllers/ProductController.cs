using phanmemnguyenhue.helper;
using phanmemnguyenhue.library;
using phanmemnguyenhue.Models;
using phanmemnguyenhue.Services.Attributes;
using phanmemnguyenhue.Services.Extensions;
using phanmemnguyenhue.Services.Sercurity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace phanmemnguyenhue.Controllers
{
    [AuthorizeRole]
    public class ProductController : Controller
    {
        // GET: Admin/Product
        [HttpGet]
        public async Task<ActionResult> Index(int siteId = 1000, int customerId = 0, int categoryId = 0, int actionTypeId = 0, int landTypeId = 0, int provinceId = 0, int districtId = 0, int wardId = 0, int streetId = 0, int projectId = 0, int productTypeId = 0, int page = 0, int pageSize = 200)
        {
            byte verified = 0, isVideo = 0;
            string viewPath = "~/Views/Product/Index.cshtml";

            MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

            if (productTypeId == 1)
            {
                verified = 1;
            }
            else if (productTypeId == 2)
            {
                isVideo = 1;
            }

            Products products = new Products
            {
                ActBy = myPrincipal.UserName,
                SiteId = siteId,
                CustomerId = customerId,
                CategoryId = categoryId,
                ActionTypeId = actionTypeId,
                LandTypeId = landTypeId,
                ProvinceId = provinceId,
                DistrictId = districtId,
                WardId = wardId,
                StreetId = streetId,
                ProjectId = projectId,
                Verified = verified,
                IsVideo = isVideo
            };

            Task<List<Sites>> sitesTask = Sites.Static_GetListByUser(myPrincipal.UserName);
            Task<List<Categories>> categoriesTask = Categories.Static_GetList(siteId);
            Task<List<ActionTypes>> actionTypesTask = ActionTypes.Static_GetList(siteId);
            Task<List<LandTypes>> landTypesTask = LandTypes.Static_GetList(siteId);
            Task<List<Provinces>> provincesTask = Provinces.Static_GetList(siteId);
            Task<List<Districts>> districtsTask = null;
            Task<List<Wards>> wardsTask = null;
            Task<List<Streets>> streetsTask = null;
            Task<List<Projects>> projectsTask = null;
            var productGetPageTask = products.GetPage(0, 0, page > 0 ? page - 1 : page, pageSize);

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

                        await Task.WhenAll(sitesTask, categoriesTask, actionTypesTask, landTypesTask, provincesTask, districtsTask, wardsTask, streetsTask, projectsTask, productGetPageTask);
                    }
                    else
                    {
                        await Task.WhenAll(sitesTask, categoriesTask, actionTypesTask, landTypesTask, provincesTask, districtsTask, wardsTask, projectsTask, productGetPageTask);
                    }
                }
                else
                {
                    projectsTask = Projects.Static_GetListBy(siteId, provinceId);

                    streetsTask = Streets.Static_GetListByProvince(siteId, provinceId);

                    await Task.WhenAll(sitesTask, categoriesTask, actionTypesTask, landTypesTask, provincesTask, streetsTask, projectsTask, productGetPageTask);
                }
            }
            else
            {
                await Task.WhenAll(sitesTask, categoriesTask, actionTypesTask, landTypesTask, provincesTask, productGetPageTask);
            }

            ProductVM model = new ProductVM
            {
                SiteId = siteId,
                CustomerId = customerId,
                CategoryId = categoryId,
                ProductTypeId = productTypeId,
                SitesList = sitesTask != null && sitesTask.Result.IsAny() ? sitesTask.Result : new List<Sites>(),
                CategoriesList = categoriesTask != null && categoriesTask.Result.IsAny() ? categoriesTask.Result : new List<Categories>(),
                ActionTypesList = actionTypesTask != null && actionTypesTask.Result.IsAny() ? actionTypesTask.Result : new List<ActionTypes>(),
                LandTypesList = landTypesTask != null && landTypesTask.Result.IsAny() ? landTypesTask.Result : new List<LandTypes>(),
                ProvincesList = provincesTask != null && provincesTask.Result.IsAny() ? provincesTask.Result : new List<Provinces>(),
                DistrictsList = districtsTask != null && districtsTask.Result.IsAny() ? districtsTask.Result : new List<Districts>(),
                WardsList = wardsTask != null && wardsTask.Result.IsAny() ? wardsTask.Result : new List<Wards>(),
                StreetsList = streetsTask != null && streetsTask.Result.IsAny() ? streetsTask.Result : new List<Streets>(),
                ProjectsList = projectsTask != null && projectsTask.Result.IsAny() ? projectsTask.Result : new List<Projects>(),
                ProductsList = productGetPageTask.Result.Item1,
                Pagination = new PaginationVM(page, pageSize, productGetPageTask.Result.Item2)
            };

            if (model.ProductsList.IsAny())
            {
                model.CustomersList = await Customers.Static_GetBy(myPrincipal.UserName, siteId, string.Join(",", model.ProductsList.Select(x => x.CustomerId)));

                //Task<List<Customers>> customersTask = Customers.Static_GetBy(myPrincipal.UserName, siteId, string.Join(",", model.ProductsList.Select(x => x.CustomerId)));
                //Task<List<Projects>> productProjectsTask = Projects.Static_GetBy(myPrincipal.UserName, siteId, string.Join(",", model.ProductsList.Select(x => x.ProjectId)));

                //await Task.WhenAll(customersTask, productProjectsTask);

                //model.CustomersList = customersTask.Result;

                //model.ProductProjectsList = productProjectsTask.Result;

                //if (model.ProductProjectsList.IsAny())
                //{
                //    model.InvestorsList = await Investors.Static_GetBy(myPrincipal.UserName, siteId, string.Join(",", model.ProductProjectsList.Select(x => x.InvestorId)));
                //}
            }

            if(model.SitesList.IsAny() && siteId > 0)
            {
                var site = model.SitesList.FirstOrDefault(x => x.SiteId == siteId);

                if(site != null && !string.IsNullOrWhiteSpace(site.ProductViewPath))
                {
                    viewPath = site.ProductViewPath;
                }
            }

            return View(viewPath, model);
        }

        [HttpGet]
        public async Task<ActionResult> Sale(int siteId = 1000, int customerId = 0, int landTypeId = 0, int provinceId = 0, int districtId = 0, int wardId = 0, int streetId = 0, int projectId = 0, int productTypeId = 0, int page = 0, int pageSize = 200)
        {
            byte verified = 0, isVideo = 0;

            MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

            if (productTypeId == 1)
            {
                verified = 1;
            }
            else if (productTypeId == 2)
            {
                isVideo = 1;
            }

            ActionTypes actionTypes = null;

            List<ActionTypes> actionTypesList = await ActionTypes.Static_GetList(siteId);

            if(actionTypesList.IsAny())
            {
                actionTypes = actionTypesList.FirstOrDefault(x => x.ActionTypeCode == "SALE");
            } 
            
            //if(actionTypes == null)
            //{
            //    return Redirect(Url.Action("NotFound", "Error"));
            //}

            Products products = new Products
            {
                ActBy = myPrincipal.UserName,
                SiteId = siteId,
                CustomerId = customerId,
                ActionTypeId = actionTypes != null && actionTypes.ActionTypeId > 0 ? actionTypes.ActionTypeId : 0,
                LandTypeId = landTypeId,
                ProvinceId = provinceId,
                DistrictId = districtId,
                WardId = wardId,
                StreetId = streetId,
                ProjectId = projectId,
                Verified = verified,
                IsVideo = isVideo
            };

            Task<List<Sites>> sitesTask = Sites.Static_GetListByUser(myPrincipal.UserName);
            Task<List<LandTypes>> landTypesTask = LandTypes.Static_GetList(siteId);
            Task<List<Provinces>> provincesTask = Provinces.Static_GetList(siteId);
            Task<List<Districts>> districtsTask = null;
            Task<List<Wards>> wardsTask = null;
            Task<List<Streets>> streetsTask = null;
            Task<List<Projects>> projectsTask = null;
            var productGetPageTask = products.GetPage(0, 0, page > 0 ? page - 1 : page, pageSize);

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

                        await Task.WhenAll(sitesTask, landTypesTask, provincesTask, districtsTask, wardsTask, streetsTask, projectsTask, productGetPageTask);
                    }
                    else
                    {
                        await Task.WhenAll(sitesTask, landTypesTask, provincesTask, districtsTask, wardsTask, projectsTask, productGetPageTask);
                    }
                }
                else
                {
                    projectsTask = Projects.Static_GetListBy(siteId, provinceId);

                    streetsTask = Streets.Static_GetListByProvince(siteId, provinceId);

                    await Task.WhenAll(sitesTask, landTypesTask, provincesTask, streetsTask, projectsTask, productGetPageTask);
                }
            }
            else
            {
                await Task.WhenAll(sitesTask, landTypesTask, provincesTask, productGetPageTask);
            }

            ProductVM model = new ProductVM
            {
                SiteId= siteId,
                CustomerId = customerId,
                ProductTypeId = productTypeId,
                SitesList = sitesTask != null && sitesTask.Result.IsAny() ? sitesTask.Result : new List<Sites>(),
                //ActionTypeId = actionTypes.ActionTypeId,
                LandTypesList = landTypesTask != null && landTypesTask.Result.IsAny() ? landTypesTask.Result : new List<LandTypes>(),
                ProvincesList = provincesTask != null && provincesTask.Result.IsAny() ? provincesTask.Result : new List<Provinces>(),
                DistrictsList = districtsTask != null && districtsTask.Result.IsAny() ? districtsTask.Result : new List<Districts>(),
                WardsList = wardsTask != null && wardsTask.Result.IsAny() ? wardsTask.Result : new List<Wards>(),
                StreetsList = streetsTask != null && streetsTask.Result.IsAny() ? streetsTask.Result : new List<Streets>(),
                ProjectsList = projectsTask != null && projectsTask.Result.IsAny() ? projectsTask.Result : new List<Projects>(),
                ProductsList = productGetPageTask.Result.Item1,
                Pagination = new PaginationVM(page, pageSize, productGetPageTask.Result.Item2)
            };

            if (model.ProductsList.IsAny())
            {
                model.CustomersList = await Customers.Static_GetBy(myPrincipal.UserName, siteId, string.Join(",", model.ProductsList.Select(x => x.CustomerId)));

                //Task<List<Customers>> customersTask = Customers.Static_GetBy(myPrincipal.UserName, siteId, string.Join(",", model.ProductsList.Select(x => x.CustomerId)));
                //Task<List<Projects>> productProjectsTask = Projects.Static_GetBy(myPrincipal.UserName, siteId, string.Join(",", model.ProductsList.Select(x => x.ProjectId)));

                //await Task.WhenAll(customersTask, productProjectsTask);

                //model.CustomersList = customersTask.Result;

                //model.ProductProjectsList = productProjectsTask.Result;

                //if (model.ProductProjectsList.IsAny())
                //{
                //    model.InvestorsList = await Investors.Static_GetBy(myPrincipal.UserName, siteId, string.Join(",", model.ProductProjectsList.Select(x => x.InvestorId)));
                //}
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Rent(int siteId = 1000, int customerId = 0, int landTypeId = 0, int provinceId = 0, int districtId = 0, int wardId = 0, int streetId = 0, int projectId = 0, int productTypeId = 0, int page = 0, int pageSize = 200)
        {
            byte verified = 0, isVideo = 0;

            MyPrincipal myPrincipal = AppExtensions.GetCurrentUser();

            if (productTypeId == 1)
            {
                verified = 1;
            }
            else if (productTypeId == 2)
            {
                isVideo = 1;
            }

            ActionTypes actionTypes = null;

            List<ActionTypes> actionTypesList = await ActionTypes.Static_GetList(siteId);

            if (actionTypesList.IsAny())
            {
                actionTypes = actionTypesList.FirstOrDefault(x => x.ActionTypeCode == "RENT");
            }

            //if (actionTypes == null)
            //{
            //    return Redirect(Url.Action("NotFound", "Error"));
            //}

            Products products = new Products
            {
                ActBy = myPrincipal.UserName,
                SiteId = siteId,
                CustomerId = customerId,
                ActionTypeId = actionTypes != null && actionTypes.ActionTypeId > 0 ? actionTypes.ActionTypeId : 0,
                LandTypeId = landTypeId,
                ProvinceId = provinceId,
                DistrictId = districtId,
                WardId = wardId,
                StreetId = streetId,
                ProjectId = projectId,
                Verified = verified,
                IsVideo = isVideo
            };

            Task<List<Sites>> sitesTask = Sites.Static_GetListByUser(myPrincipal.UserName);
            Task<List<LandTypes>> landTypesTask = LandTypes.Static_GetList(siteId);
            Task<List<Provinces>> provincesTask = Provinces.Static_GetList(siteId);
            Task<List<Districts>> districtsTask = null;
            Task<List<Wards>> wardsTask = null;
            Task<List<Streets>> streetsTask = null;
            Task<List<Projects>> projectsTask = null;
            var productGetPageTask = products.GetPage(0, 0, page > 0 ? page - 1 : page, pageSize);

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

                        await Task.WhenAll(sitesTask, landTypesTask, provincesTask, districtsTask, wardsTask, streetsTask, projectsTask, productGetPageTask);
                    }
                    else
                    {
                        await Task.WhenAll(sitesTask, landTypesTask, provincesTask, districtsTask, wardsTask, projectsTask, productGetPageTask);
                    }
                }
                else
                {
                    projectsTask = Projects.Static_GetListBy(siteId, provinceId);

                    streetsTask = Streets.Static_GetListByProvince(siteId, provinceId);

                    await Task.WhenAll(sitesTask, landTypesTask, provincesTask, streetsTask, projectsTask, productGetPageTask);
                }
            }
            else
            {
                await Task.WhenAll(sitesTask, landTypesTask, provincesTask, productGetPageTask);
            }

            ProductVM model = new ProductVM
            {
                SiteId= siteId,
                CustomerId = customerId,
                ProductTypeId = productTypeId,
                SitesList = sitesTask != null && sitesTask.Result.IsAny() ? sitesTask.Result : new List<Sites>(),
                //ActionTypeId = actionTypes.ActionTypeId,
                LandTypesList = landTypesTask != null && landTypesTask.Result.IsAny() ? landTypesTask.Result : new List<LandTypes>(),
                ProvincesList = provincesTask != null && provincesTask.Result.IsAny() ? provincesTask.Result : new List<Provinces>(),
                DistrictsList = districtsTask != null && districtsTask.Result.IsAny() ? districtsTask.Result : new List<Districts>(),
                WardsList = wardsTask != null && wardsTask.Result.IsAny() ? wardsTask.Result : new List<Wards>(),
                StreetsList = streetsTask != null && streetsTask.Result.IsAny() ? streetsTask.Result : new List<Streets>(),
                ProjectsList = projectsTask != null && projectsTask.Result.IsAny() ? projectsTask.Result : new List<Projects>(),
                ProductsList = productGetPageTask.Result.Item1,
                Pagination = new PaginationVM(page, pageSize, productGetPageTask.Result.Item2)
            };

            if (model.ProductsList.IsAny())
            {
                model.CustomersList = await Customers.Static_GetBy(myPrincipal.UserName, siteId, string.Join(",", model.ProductsList.Select(x => x.CustomerId)));

                //Task<List<Customers>> customersTask = Customers.Static_GetBy(myPrincipal.UserName, siteId, string.Join(",", model.ProductsList.Select(x => x.CustomerId)));
                //Task<List<Projects>> productProjectsTask = Projects.Static_GetBy(myPrincipal.UserName, siteId, string.Join(",", model.ProductsList.Select(x => x.ProjectId)));

                //await Task.WhenAll(customersTask, productProjectsTask);

                //model.CustomersList = customersTask.Result;

                //model.ProductProjectsList = productProjectsTask.Result;

                //if (model.ProductProjectsList.IsAny())
                //{
                //    model.InvestorsList = await Investors.Static_GetBy(myPrincipal.UserName, siteId, string.Join(",", model.ProductProjectsList.Select(x => x.InvestorId)));
                //}
            }

            return View(model);
        }
    }
}