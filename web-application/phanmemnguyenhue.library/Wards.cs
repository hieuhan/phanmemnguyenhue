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
    public class Wards
    {
        #region Properties

        public string ActBy { get; set; }
        public int SiteId { get; set; }
        public int WardId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
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

        public Wards()
        {
            _connectionString = ConstantHelper.CommonConstr;
        }

        public Wards(string connection)
        {
            _connectionString = connection;
        }

        ~Wards()
        {

        }

        public virtual void Dispose()
        {

        }

        #endregion

        #region Methods

        public async Task<List<Wards>> GetListByDistrict(int siteId, int districtId)
        {
            List<Wards> resultVar = new List<Wards>();
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    resultVar = await connection.QueryAsync<Wards>(string.Format("SELECT [WardId],[Name],[Description],[ProvinceId],[DistrictId],[CreatedAt] FROM [dbo].[Wards] WHERE ([SiteId] = {0}) AND ([DistrictId] = {1}) ORDER BY [Name]", siteId, districtId)) as List<Wards>;
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

        public static Wards Static_Get(int wardId, List<Wards> list)
        {
            Wards resultVar = new Wards();
            try
            {
                if (wardId > 0 && list != null && list.Count > 0)
                {
                    resultVar = list.FirstOrDefault(i => i.WardId == wardId);
                }
            }
            catch (Exception ex)
            {
                resultVar = null;
                throw ex;
            }

            return resultVar;
        }

        public static async Task<List<Wards>> Static_GetListByDistrict(int siteId, int districtId)
        {
            Wards districts = new Wards();
            return await districts.GetListByDistrict(siteId, districtId);
        }

        #endregion
    }
}
