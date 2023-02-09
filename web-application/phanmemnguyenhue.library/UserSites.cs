using Dapper;
using phanmemnguyenhue.helper;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace phanmemnguyenhue.library
{
    public class UserSites
    {
        #region Properties

        public string ActBy { get; set; }
        public int UserSiteId { get; set; }
        public int UserId { get; set; }
        public int SiteId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        private readonly string _connectionString;

        #endregion

        #region Constructors

        public UserSites()
        {
            _connectionString = ConstantHelper.CommonConstr;
        }

        public UserSites(string connection)
        {
            _connectionString = connection;
        }

        ~UserSites()
        {

        }

        public virtual void Dispose()
        {

        }

        #endregion

        #region Methods

        public async Task<Tuple<string, string>> InsertMultiple(string SiteId = "", string SiteIdDelete = "")
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
                    param.Add("@SiteId", SiteId, DbType.String);
                    param.Add("@SiteIdDelete", SiteIdDelete, DbType.String);
                    param.Add("@ActionStatus", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
                    param.Add("@ActionMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 250);

                    await connection.ExecuteAsync("UserSites_InsertMultiple", param, commandType: CommandType.StoredProcedure);

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

        public static async Task<Tuple<string, string>> Static_InsertMultiple(string actBy, int userId, string SiteId = "", string SiteIdDelete = "")
        {
            UserSites userSites = new UserSites
            {
                ActBy = actBy,
                UserId = userId
            };

            return await userSites.InsertMultiple(SiteId, SiteIdDelete);
        }

        #endregion
    }
}
