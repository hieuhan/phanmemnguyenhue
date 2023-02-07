using phanmemnguyenhue.helper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace phanmemnguyenhue.library
{
    public class Streets
    {
        #region Properties

        public string ActBy { get; set; }
        public int SiteId { get; set; }
        public int StreetId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int WardId { get; set; }
        public string WardName { get; set; }
        public string WardDescription { get; set; }
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string ProvinceDescription { get; set; }
        public int DistrictId { get; set; }
        public string DistrictName { get; set; }
        public string DistrictDescription { get; set; }
        public int? DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }

        private readonly string _connectionString;

        #endregion

        #region Constructors

        public Streets()
        {
            _connectionString = ConstantHelper.CommonConstr;
        }

        public Streets(string connection)
        {
            _connectionString = connection;
        }

        ~Streets()
        {

        }

        public virtual void Dispose()
        {

        }

        #endregion

        #region Methods
        public async Task<List<Streets>> GetListByWard(int siteId, int wardId)
        {
            List<Streets> resultVar = new List<Streets>();
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    resultVar = await connection.QueryAsync<Streets>(string.Format("SELECT [StreetId],[Name],[Description],[ProvinceId],[DistrictId],[WardId],[CreatedAt] FROM [dbo].[Streets] WHERE ([SiteId] = {0}) AND ([WardId] = {1}) ORDER BY [Name]", siteId, wardId)) as List<Streets>;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultVar;
        }

        public async Task<List<Streets>> GetListByProvince(int siteId, int provinceId)
        {
            List<Streets> resultVar = new List<Streets>();
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    resultVar = await connection.QueryAsync<Streets>(string.Format("SELECT [StreetId],[Name],[Description],[ProvinceId],[DistrictId],[WardId],[CreatedAt] FROM [dbo].[Streets] WHERE ([SiteId] = {0}) AND ([ProvinceId] = {1}) ORDER BY [Name]", siteId, provinceId)) as List<Streets>;
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

        public static async Task<List<Streets>> Static_GetListByWard(int siteId, int wardId)
        {
            Streets streets = new Streets();
            return await streets.GetListByWard(siteId, wardId);
        }

        public static async Task<List<Streets>> Static_GetListByProvince(int siteId, int provinceId)
        {
            Streets streets = new Streets();
            return await streets.GetListByProvince(siteId, provinceId);
        }

        public static Streets Static_Get(byte streetId, List<Streets> list)
        {
            Streets resultVar = new Streets();
            try
            {
                if (streetId > 0 && list != null && list.Count > 0)
                {
                    resultVar = list.FirstOrDefault(i => i.StreetId == streetId) ?? new Streets();
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
