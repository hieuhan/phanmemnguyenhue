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
    public class Sites
    {
        #region Properties
        public string ActBy { get; set; }
        public int ActionTypeId { get; set; }
        public int SiteId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }

        private readonly string _connectionString;

        #endregion

        #region Constructors

        public Sites()
        {
            _connectionString = ConstantHelper.CommonConstr;
        }

        public Sites(string connection)
        {
            _connectionString = connection;
        }

        ~Sites()
        {

        }

        public virtual void Dispose()
        {

        }

        #endregion

        #region Methods

        public async Task<List<Sites>> GetList()
        {
            List<Sites> resultVar = new List<Sites>();

            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    resultVar = await connection.QueryAsync<Sites>("SELECT * FROM [dbo].[Sites] ORDER BY [DisplayOrder],[Name]") as List<Sites>;
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

        public static async Task<List<Sites>> Static_GetList()
        {
            Sites Sites = new Sites();
            return await Sites.GetList();
        }

        public static Sites Static_Get(int siteId, List<Sites> list)
        {
            Sites resultVar = null;

            if (siteId > 0 && list != null && list.Count > 0)
            {
                resultVar = list.FirstOrDefault(i => i.SiteId == siteId);
            }

            return resultVar;
        }

        #endregion
    }
}
