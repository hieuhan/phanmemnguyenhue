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
    public class UserStatus
    {
        #region Properties

        public string ActBy { get; set; }
        public byte UserStatusId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? DisplayOrder { get; set; }

        private readonly string _connectionString;

        #endregion

        #region Constructors

        public UserStatus()
        {
            _connectionString = ConstantHelper.CommonConstr;
        }

        public UserStatus(string connection)
        {
            _connectionString = connection;
        }

        ~UserStatus()
        {

        }

        public virtual void Dispose()
        {

        }

        #endregion

        #region Methods

        public async Task<List<UserStatus>> GetList()
        {
            List<UserStatus> resultVar = new List<UserStatus>();
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    resultVar = await connection.QueryAsync<UserStatus>("SELECT * FROM [dbo].[UserStatus]") as List<UserStatus>;
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

        public static async Task<List<UserStatus>> Static_GetList()
        {
            UserStatus userStatus = new UserStatus();
            return await userStatus.GetList();
        }

        public static UserStatus Static_Get(byte userStatusId, List<UserStatus> list)
        {
            UserStatus resultVar = null;

            if (userStatusId > 0 && list != null && list.Count > 0)
            {
                resultVar = list.FirstOrDefault(i => i.UserStatusId == userStatusId);
            }

            return resultVar;
        }

        #endregion
    }
}
