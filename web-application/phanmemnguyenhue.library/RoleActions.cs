using Dapper;
using phanmemnguyenhue.helper;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace phanmemnguyenhue.library
{
    public class RoleActions
    {
        #region Properties

        public string ActBy { get; set; }
        public int RoleActionId { get; set; }
        public int RoleId { get; set; }
        public int ActionId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        private readonly string _connectionString;

        #endregion

        #region Constructors

        public RoleActions()
        {
            _connectionString = ConstantHelper.CommonConstr;
        }

        public RoleActions(string connection)
        {
            _connectionString = connection;
        }

        ~RoleActions()
        {

        }

        public virtual void Dispose()
        {

        }

        #endregion

        #region Methods

        public async Task<Tuple<string, string>> InsertMultiple(string actionId = "", string actionIdDelete = "")
        {
            string actionStatus = string.Empty, actionMessage = string.Empty;
            Tuple<string, string> resultVar = Tuple.Create(actionStatus, actionMessage);

            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    if (!string.IsNullOrEmpty(this.ActBy))
                        param.Add("@ActBy", this.ActBy, DbType.String);
                    param.Add("@RoleId", this.RoleId, DbType.Int32);
                    param.Add("@ActionId", actionId, DbType.String);
                    param.Add("@ActionIdDelete", actionIdDelete, DbType.String);
                    param.Add("@ActionStatus", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
                    param.Add("@ActionMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 250);

                    await connection.ExecuteAsync("RoleActions_InsertMultiple", param, commandType: CommandType.StoredProcedure);

                    actionStatus = param.Get<string>("ActionStatus");
                    actionMessage = param.Get<string>("ActionMessage");

                    resultVar = Tuple.Create(actionStatus, actionMessage);
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

        public static async Task<Tuple<string, string>> Static_InsertMultiple(string actBy, int roleId, string actionId = "", string actionIdDelete = "")
        {
            RoleActions roleActions = new RoleActions
            {
                ActBy = actBy,
                RoleId = roleId
            };
            return await roleActions.InsertMultiple(actionId, actionIdDelete);
        }

        #endregion
    }
}
