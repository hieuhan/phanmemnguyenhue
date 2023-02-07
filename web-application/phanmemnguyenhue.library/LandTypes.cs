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
    public class LandTypes
    {
        #region Properties
        public string ActBy { get; set; }
        public int LandTypeId { get; set; }
        public int SiteId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }

        private readonly string _connectionString;

        #endregion

        #region Constructors

        public LandTypes()
        {
            _connectionString = ConstantHelper.CommonConstr;
        }

        public LandTypes(string connection)
        {
            _connectionString = connection;
        }

        ~LandTypes()
        {

        }

        public virtual void Dispose()
        {

        }

        #endregion

        #region Methods

        public async Task<List<LandTypes>> GetList(int siteId)
        {
            List<LandTypes> resultVar = new List<LandTypes>();

            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    resultVar = await connection.QueryAsync<LandTypes>(string.Format("SELECT * FROM [dbo].[LandTypes] WHERE [SiteId]={0} ORDER BY [DisplayOrder],[Name]", siteId)) as List<LandTypes>;
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

        public static async Task<List<LandTypes>> Static_GetList(int siteId)
        {
            LandTypes LandTypes = new LandTypes();
            return await LandTypes.GetList(siteId);
        }

        public static LandTypes Static_Get(byte LandTypeId, List<LandTypes> list)
        {
            LandTypes resultVar = null;

            if (LandTypeId > 0 && list != null && list.Count > 0)
            {
                resultVar = list.FirstOrDefault(i => i.LandTypeId == LandTypeId);
            }

            return resultVar;
        }

        #endregion
    }
}
