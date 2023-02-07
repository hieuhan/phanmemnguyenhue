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
    public class ActionStatus
    {
        #region Properties

        public string ActBy { get; set; }
        public byte ActionStatusId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? DisplayOrder { get; set; }

        private readonly string _connectionString;

        #endregion

        #region Constructors

        public ActionStatus()
        {
            _connectionString = ConstantHelper.CommonConstr;
        }

        public ActionStatus(string connection)
        {
            _connectionString = connection;
        }

        ~ActionStatus()
        {

        }

        public virtual void Dispose()
        {

        }

        #endregion

        #region Methods

        public async Task<List<ActionStatus>> GetList()
        {
            List<ActionStatus> resultVar = new List<ActionStatus>();

            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    resultVar = await connection.QueryAsync<ActionStatus>("SELECT * FROM [dbo].[ActionStatus]") as List<ActionStatus>;
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

        public static async Task<List<ActionStatus>> Static_GetList()
        {
            ActionStatus actionStatus = new ActionStatus();
            return await actionStatus.GetList();
        }

        public static ActionStatus Static_Get(byte actionStatusId, List<ActionStatus> list)
        {
            ActionStatus resultVar = null;

            if (actionStatusId > 0 && list != null && list.Count > 0)
            {
                resultVar = list.FirstOrDefault(i => i.ActionStatusId == actionStatusId);
            }

            return resultVar;
        }

        #endregion
    }
}
