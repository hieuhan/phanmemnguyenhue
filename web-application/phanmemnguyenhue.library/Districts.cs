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
    public class Districts
    {
        #region Properties

        public string ActBy { get; set; }
        public int SiteId { get; set; }
        public int DistrictId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public byte StatusId { get; set; }
        public int? DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }

        private readonly string _connectionString;

        #endregion

        #region Constructors

        public Districts()
        {
            _connectionString = ConstantHelper.CommonConstr;
        }

        public Districts(string connection)
        {
            _connectionString = connection;
        }

        ~Districts()
        {

        }

        public virtual void Dispose()
        {

        }

        #endregion

        #region Methods

        public async Task<List<Districts>> GetListByProvince(int siteId, int provinceId)
        {
            List<Districts> resultVar;
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    resultVar = await connection.QueryAsync<Districts>(string.Format("SELECT * FROM [dbo].[Districts] WHERE ([SiteId] = {0}) AND ([ProvinceId] = {1}) ORDER BY [Name]", siteId, provinceId)) as List<Districts>;
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

        public static async Task<List<Districts>> Static_GetListByProvince(int siteId, int provinceId)
        {
            Districts districts = new Districts();
            return await districts.GetListByProvince(siteId, provinceId);
        }

        public static Districts Static_Get(int districtId, List<Districts> list)
        {
            Districts resultVar = new Districts();
            try
            {
                if (districtId > 0 && list != null && list.Count > 0)
                {
                    resultVar = list.FirstOrDefault(i => i.DistrictId == districtId);
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
