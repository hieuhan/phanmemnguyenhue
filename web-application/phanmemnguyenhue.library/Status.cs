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
    public class Status
    {
        #region Properties

        public string ActBy { get; set; }
        public byte StatusId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? DisplayOrder { get; set; }

        private readonly string _connectionString;

        #endregion

        #region Constructors

        public Status()
        {
            _connectionString = ConstantHelper.CommonConstr;
        }

        public Status(string connection)
        {
            _connectionString = connection;
        }

        ~Status()
        {

        }

        public virtual void Dispose()
        {

        }

        #endregion

        #region Methods

        public async Task<List<Status>> GetList()
        {
            List<Status> resultVar = new List<Status>();
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    resultVar = await connection.QueryAsync<Status>("SELECT * FROM [dbo].[Status]") as List<Status>;
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

        public static async Task<List<Status>> Static_GetList()
        {
            Status Status = new Status();
            return await Status.GetList();
        }

        public static Status Static_Get(byte statusId, List<Status> list)
        {
            Status resultVar = new Status();

            try
            {
                if (statusId > 0 && list != null && list.Count > 0)
                {
                    resultVar = list.FirstOrDefault(i => i.StatusId == statusId) ?? new Status();
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
    }
}
