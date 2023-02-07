using Dapper;
using phanmemnguyenhue.helper;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace phanmemnguyenhue.library
{
    public class UserRoles
    {
        #region Properties

        public string ActBy { get; set; }
        public int UserRoleId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        private readonly string _connectionString;

        #endregion

        #region Constructors

        public UserRoles()
        {
            _connectionString = ConstantHelper.CommonConstr;
        }

        public UserRoles(string connection)
        {
            _connectionString = connection;
        }

        ~UserRoles()
        {

        }

        public virtual void Dispose()
        {

        }

        #endregion

        #region Methods

        public async Task<Tuple<string, string>> InsertMultiple(string roleId = "", string roleIdDelete = "")
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
                    param.Add("@UserId", this.UserId, DbType.Int32);
                    param.Add("@RoleId", roleId, DbType.String);
                    param.Add("@RoleIdDelete", roleIdDelete, DbType.String);
                    param.Add("@ActionStatus", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
                    param.Add("@ActionMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 250);

                    await connection.ExecuteAsync("UserRoles_InsertMultiple", param, commandType: CommandType.StoredProcedure);

                    actionStatus = param.Get<string>("ActionStatus");
                    actionMessage = param.Get<string>("ActionMessage");

                    resultVar = Tuple.Create(actionStatus, actionMessage);
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

        public static async Task<Tuple<string, string>> Static_InsertMultiple(string actBy, int userId, string roleId = "", string roleIdDelete = "")
        {
            UserRoles userRoles = new UserRoles
            {
                ActBy = actBy,
                UserId = userId
            };

            return await userRoles.InsertMultiple(roleId, roleIdDelete);
        }

        #endregion
    }
}
