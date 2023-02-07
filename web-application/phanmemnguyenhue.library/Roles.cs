using Dapper;
using phanmemnguyenhue.helper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace phanmemnguyenhue.library
{
    public class Roles
    {
        #region Properties

        public string ActBy { get; set; }
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte BuildIn { get; set; }
        public byte StatusId { get; set; }
        public int? DisplayOrder { get; set; }
        public int UserId { get; set; }
        public int UserRoleId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }

        private readonly string _connectionString;

        #endregion

        #region Constructors

        public Roles()
        {
            _connectionString = ConstantHelper.CommonConstr;
        }

        public Roles(string connection)
        {
            _connectionString = connection;
        }

        ~Roles()
        {

        }

        public virtual void Dispose()
        {

        }

        #endregion

        #region Methods
        public async Task<Tuple<string, string>> Insert()
        {
            try
            {
                return await InsertOrUpdate();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Tuple<string, string>> Update()
        {
            try
            {
                return await InsertOrUpdate();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Tuple<string, string>> InsertOrUpdate()
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
                    if (!string.IsNullOrEmpty(this.Name))
                        param.Add("@Name", this.Name, DbType.String);
                    if (!string.IsNullOrEmpty(this.Description))
                        param.Add("@Description", this.Description, DbType.String);
                    param.Add("@BuildIn", this.BuildIn, DbType.Byte);
                    param.Add("@StatusId", this.StatusId, DbType.Byte);
                    if (this.DisplayOrder.HasValue)
                        param.Add("@DisplayOrder", this.DisplayOrder, DbType.Int32);
                    param.Add("@RoleId", this.RoleId, DbType.Int32, ParameterDirection.InputOutput);
                    param.Add("@ActionStatus", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
                    param.Add("@ActionMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 250);

                    await connection.ExecuteAsync("Roles_InsertOrUpdate", param, commandType: CommandType.StoredProcedure);

                    this.RoleId = param.Get<int>("RoleId");
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

        public async Task<Tuple<string, string>> Delete()
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
                    param.Add("@ActionStatus", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
                    param.Add("@ActionMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 250);

                    await connection.ExecuteAsync("Roles_Delete", param, commandType: CommandType.StoredProcedure);

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

        public async Task<Roles> Get()
        {
            Roles resultVar = new Roles();
            byte searchByDateType = 0;
            DateTime dateFrom = DateTime.MinValue, dateTo = DateTime.MinValue;
            int orderByClauseId = 0, pageIndex = 0, pageSize = 1;
            Tuple<List<Roles>, int> tuple;
            try
            {
                tuple = await GetPage(dateFrom, dateTo, searchByDateType, orderByClauseId, pageIndex, pageSize);
                if (tuple != null && tuple.Item1 != null && tuple.Item1.Count > 0) resultVar = tuple.Item1[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultVar;
        }

        public async Task<Tuple<List<Roles>, int>> GetPage(DateTime dateFrom, DateTime dateTo, byte searchByDateType, int orderByClauseId, int pageIndex, int pageSize)
        {
            int rowCount = 0;
            List<Roles> rolesList = null;
            Tuple<List<Roles>, int> resultVar = Tuple.Create(rolesList, rowCount);

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
                    if (!string.IsNullOrEmpty(this.Name))
                        param.Add("@Name", this.Name, DbType.String);
                    if (!string.IsNullOrEmpty(this.Description))
                        param.Add("@Description", this.Description, DbType.String);
                    param.Add("@BuildIn", this.BuildIn, DbType.Byte);
                    param.Add("@StatusId", this.StatusId, DbType.Byte);
                    if (!string.IsNullOrEmpty(this.CreatedBy))
                        param.Add("@CreatedBy", this.CreatedBy, DbType.String);
                    if (!string.IsNullOrEmpty(this.UpdatedBy))
                        param.Add("@UpdatedBy", this.UpdatedBy, DbType.String);
                    param.Add("@SearchByDateType", searchByDateType, DbType.Byte);
                    if (dateFrom != DateTime.MinValue)
                        param.Add("@DateFrom", dateFrom, DbType.DateTime);
                    if (dateTo != DateTime.MinValue)
                        param.Add("@DateTo", dateTo, DbType.DateTime);
                    param.Add("@OrderByClauseId", orderByClauseId, DbType.Int32);
                    param.Add("@PageSize", pageSize, DbType.Int32);
                    param.Add("@PageIndex", pageIndex, DbType.Int32);
                    param.Add("@RowCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    rolesList = await connection
                        .QueryAsync<Roles>("Roles_GetPage", param, commandType: CommandType.StoredProcedure)
                        as List<Roles>;

                    rowCount = param.Get<int>("RowCount");

                    resultVar = Tuple.Create(rolesList, rowCount);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultVar;
        }

        public async Task<List<Roles>> GetList()
        {
            List<Roles> resultVar = new List<Roles>();
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    resultVar = await connection.QueryAsync<Roles>("SELECT * FROM [dbo].[Roles] ORDER BY [DisplayOrder],[RoleId] DESC") as List<Roles>;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultVar;
        }

        public async Task<List<Roles>> GetListBy()
        {
            List<Roles> resultVar = new List<Roles>();

            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    DynamicParameters param = new DynamicParameters();
                    if (!string.IsNullOrEmpty(this.ActBy))
                        param.Add("@ActBy", this.ActBy, DbType.String);
                    param.Add("@UserId", this.UserId, DbType.Int32);

                    resultVar = await connection
                        .QueryAsync<Roles>("Roles_GetListBy", param, commandType: CommandType.StoredProcedure)
                         as List<Roles>;
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

        public static async Task<List<Roles>> Static_GetList()
        {
            Roles roles = new Roles();
            return await roles.GetList();
        }

        public static async Task<List<Roles>> Static_GetListBy(string actBy, int userId)
        {
            Roles roles = new Roles
            {
                ActBy = actBy,
                UserId = userId
            };

            return await roles.GetListBy();
        }
        public static Roles Static_Get(int roleId, List<Roles> list)
        {
            Roles resultVar = new Roles();
            try
            {
                if (roleId > 0 && list != null && list.Count > 0)
                {
                    resultVar = list.FirstOrDefault(i => i.RoleId == roleId) ?? new Roles();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultVar;
        }

        public static async Task<Roles> Static_GetById(string actBy, int roleId)
        {
            Roles roles = new Roles
            {
                ActBy = actBy,
                RoleId = roleId
            };

            return await roles.Get();
        }

        #endregion
    }
}
