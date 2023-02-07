using Dapper;
using phanmemnguyenhue.helper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace phanmemnguyenhue.library
{
    public class Projects
    {
        #region Properties

        public string ActBy { get; set; }
        public int ProjectId { get; set; }
        public int SiteId { get; set; }
        public int InvestorId { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public double PriceFrom { get; set; }
        public double PriceTo { get; set; }
        public double ComputedPriceFrom { get; set; }
        public double ComputedPriceTo { get; set; }
        public string PriceDisplay { get; set; }
        public double AreaFrom { get; set; }
        public double AreaTo { get; set; }
        public double ComputedAreaFrom { get; set; }
        public double ComputedAreaTo { get; set; }
        public string AreaDisplay { get; set; }
        public int Apartments { get; set; }
        public int Buildings { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        private readonly string _connectionString;

        #endregion

        #region Constructors

        public Projects()
        {
            _connectionString = ConstantHelper.CommonConstr;
        }

        public Projects(string connection)
        {
            _connectionString = connection;
        }

        ~Projects()
        {

        }

        public virtual void Dispose()
        {

        }

        #endregion

        #region Methods

        public async Task<List<Projects>> GetListBy(int siteId, int provinceId, int districtId = 0)
        {
            List<Projects> resultVar;
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    if (districtId > 0)
                    {
                        resultVar = await connection.QueryAsync<Projects>(string.Format("SELECT * FROM [dbo].[Projects] WHERE ([SiteId] = {0}) AND ([ProvinceId] = {1} AND ([DistrictId] = {2})) ORDER BY [Name]", siteId, provinceId, districtId)) as List<Projects>;
                    }
                    else
                    {
                        resultVar = await connection.QueryAsync<Projects>(string.Format("SELECT * FROM [dbo].[Projects] WHERE ([SiteId] = {0}) AND ([ProvinceId] = {1}) ORDER BY [Name]", siteId, provinceId)) as List<Projects>;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultVar;
        }

        public async Task<List<Projects>> GetBy(string projectIds)
        {
            List<Projects> resultVar = new List<Projects>();
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    if (!string.IsNullOrEmpty(this.ActBy))
                        param.Add("@ActBy", this.ActBy, DbType.String);
                    param.Add("@SiteId", this.SiteId, DbType.Int32);
                    param.Add("@ProjectIds", projectIds, DbType.String);
                    resultVar = await connection
                        .QueryAsync<Projects>("Projects_GetListBy", param, commandType: CommandType.StoredProcedure)
                        as List<Projects>;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultVar;
        }

        #endregion

        #region Static Methods

        public static async Task<List<Projects>> Static_GetListBy(int siteId, int provinceId, int districtId = 0)
        {
            Projects projects = new Projects();
            return await projects.GetListBy(siteId, provinceId, districtId);
        }

        public static async Task<List<Projects>> Static_GetBy(string actBy, int siteId, string projectIds)
        {
            try
            {
                Projects projects = new Projects
                {
                    ActBy = actBy,
                    SiteId = siteId
                };

                return await projects.GetBy(projectIds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Projects Static_Get(int projectId, List<Projects> list)
        {
            Projects resultVar = new Projects();
            try
            {
                if (projectId > 0 && list != null && list.Count > 0)
                {
                    resultVar = list.FirstOrDefault(i => i.ProjectId == projectId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultVar;
        }

        #endregion
    }
}
