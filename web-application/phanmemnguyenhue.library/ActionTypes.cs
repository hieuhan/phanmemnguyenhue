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
    public class ActionTypes
    {
        #region Properties
        public string ActBy { get; set; }
        public int ActionTypeId { get; set; }
        public int SiteId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ActionTypeCode { get; set; }
        public int? DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }

        private readonly string _connectionString;

        #endregion

        #region Constructors

        public ActionTypes()
        {
            _connectionString = ConstantHelper.CommonConstr;
        }

        public ActionTypes(string connection)
        {
            _connectionString = connection;
        }

        ~ActionTypes()
        {

        }

        public virtual void Dispose()
        {

        }

        #endregion

        #region Methods

        public async Task<List<ActionTypes>> GetList(int siteId)
        {
            List<ActionTypes> resultVar = new List<ActionTypes>();

            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    resultVar = await connection.QueryAsync<ActionTypes>(string.Format("SELECT * FROM [dbo].[ActionTypes] WHERE [SiteId]={0} ORDER BY [DisplayOrder],[Name]", siteId)) as List<ActionTypes>;
                }
            }
            catch (Exception ex)
            {
                resultVar = null;
                throw ex;
            }

            return resultVar;
        }

        #endregion

        #region Static Methods

        public static async Task<List<ActionTypes>> Static_GetList(int siteId)
        {
            ActionTypes ActionTypes = new ActionTypes();
            return await ActionTypes.GetList(siteId);
        }

        public static ActionTypes Static_Get(byte ActionTypeId, List<ActionTypes> list)
        {
            ActionTypes resultVar = null;

            if (ActionTypeId > 0 && list != null && list.Count > 0)
            {
                resultVar = list.FirstOrDefault(i => i.ActionTypeId == ActionTypeId);
            }

            return resultVar;
        }

        #endregion
    }
}
