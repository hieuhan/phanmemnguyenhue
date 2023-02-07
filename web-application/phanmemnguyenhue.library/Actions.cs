using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using phanmemnguyenhue.helper;
using Dapper;

namespace phanmemnguyenhue.library
{
    public class Actions
    {
        #region Properties

        public string ActBy { get; set; }
        public int ActionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public int ParentId { get; set; }
        public byte StatusId { get; set; }
        public byte Display { get; set; }
        public string IconPath { get; set; }
        public int? DisplayOrder { get; set; }
        public string HierarchyId { get; set; }
        public string TreeOrder { get; set; }
        public int UserId { get; set; }

        public int UserActionId { get; set; }
        public int RoleId { get; set; }

        public int RoleActionId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }

        private readonly string _connectionString;

        #endregion

        #region Constructors

        public Actions()
        {
            _connectionString = ConstantHelper.CommonConstr;
        }

        public Actions(string connection)
        {
            _connectionString = connection;
        }

        ~Actions()
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
                    if (!string.IsNullOrEmpty(this.Path))
                        param.Add("@Path", this.Path, DbType.String);
                    if (!string.IsNullOrEmpty(this.Controller))
                        param.Add("@Controller", this.Controller, DbType.String);
                    if (!string.IsNullOrEmpty(this.Action))
                        param.Add("@Action", this.Action, DbType.String);
                    param.Add("@ParentId", this.ParentId, DbType.Int32);
                    param.Add("@DisplayOrder", this.DisplayOrder, DbType.Int32);
                    param.Add("@StatusId", this.StatusId, DbType.Byte);
                    param.Add("@Display", this.Display, DbType.Byte);
                    if (!string.IsNullOrEmpty(this.IconPath))
                        param.Add("@IconPath", this.IconPath, DbType.String);
                    param.Add("@ActionId", this.ActionId, DbType.Int32, ParameterDirection.InputOutput);
                    param.Add("@ActionStatus", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
                    param.Add("@ActionMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 250);

                    await connection.ExecuteAsync("Actions_InsertOrUpdate", param, commandType: CommandType.StoredProcedure);

                    this.ActionId = param.Get<int>("ActionId");
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
                    param.Add("@ActionId", this.ActionId, DbType.Int32);
                    param.Add("@ActionStatus", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
                    param.Add("@ActionMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 250);
                    await connection.ExecuteAsync("Actions_Delete", param, commandType: CommandType.StoredProcedure);
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

        public async Task<Actions> Get(byte selfJoin = 0)
        {
            Actions resultVar = new Actions();
            byte searchByDateType = 0;
            DateTime dateFrom = DateTime.MinValue, dateTo = DateTime.MinValue;
            int orderByClauseId = 0, pageIndex = 0, pageSize = 1;
            Tuple<List<Actions>, int> tuple;
            try
            {
                tuple = await GetPage(dateFrom, dateTo, searchByDateType, selfJoin, orderByClauseId, 0,
                    pageIndex, pageSize);

                if (tuple != null && tuple.Item1 != null && tuple.Item1.Count > 0)
                    resultVar = tuple.Item1[0];
            }
            catch (Exception ex)
            {
                resultVar = null;
                throw ex;
            }

            return resultVar;
        }

        public async Task<Tuple<List<Actions>, int>> GetPage(DateTime dateFrom, DateTime dateTo, byte searchByDateType, byte selfJoin, int orderByClauseId, int otherFunctionId, int pageIndex, int pageSize)
        {
            int rowCount = 0;
            List<Actions> actionsList = null;
            Tuple<List<Actions>, int> resultVar = Tuple.Create(actionsList, rowCount);

            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    if (!string.IsNullOrEmpty(this.ActBy))
                        param.Add("@ActBy", this.ActBy, DbType.String);
                    param.Add("@ActionId", this.ActionId, DbType.Int32);
                    if (!string.IsNullOrEmpty(this.Name))
                        param.Add("@Name", this.Name, DbType.String);
                    if (!string.IsNullOrEmpty(this.Description))
                        param.Add("@Description", this.Description, DbType.String);
                    if (!string.IsNullOrEmpty(this.Path))
                        param.Add("@Path", this.Path, DbType.String);
                    param.Add("@ParentId", this.ParentId, DbType.Int32);
                    param.Add("@StatusId", this.StatusId, DbType.Byte);
                    param.Add("@Display", this.Display, DbType.Byte);
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
                    if (this.UserId > 0)
                        param.Add("@UserId", this.UserId, DbType.Int32);
                    if (this.RoleId > 0)
                        param.Add("@RoleId", this.RoleId, DbType.Int32);
                    param.Add("@SelfJoin", selfJoin, DbType.Byte);
                    param.Add("@PageSize", pageSize, DbType.Int32);
                    param.Add("@PageIndex", pageIndex, DbType.Int32);
                    param.Add("@RowCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    actionsList = await connection
                        .QueryAsync<Actions>("Actions_GetPage", param, commandType: CommandType.StoredProcedure)
                        as List<Actions>;

                    rowCount = param.Get<int>("RowCount");

                    resultVar = Tuple.Create(actionsList, rowCount);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultVar;
        }

        public async Task<List<Actions>> GetParentActions()
        {
            List<Actions> resultVar = new List<Actions>();

            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    resultVar = await connection.QueryAsync<Actions>(
                        string.Format("SELECT * FROM [dbo].[ActionsHierarchy]() WHERE (ISNULL([ParentId],0) = 0) AND ([StatusId] = {0}) ORDER BY [HierarchyId]",
                            ConstantHelper.ActionStatusIdActivated)) as List<Actions>;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultVar;
        }

        public async Task<List<Actions>> GetListIsDisplayed()
        {
            List<Actions> resultVar = new List<Actions>();
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                resultVar = await connection.QueryAsync<Actions>(string.Format("SELECT * FROM [dbo].[Actions] WHERE ([Display]=1) AND [StatusId]={0} ORDER BY [TreeOrder]", ConstantHelper.ActionStatusIdActivated)) as List<Actions>;
            }

            return resultVar;
        }

        public List<Actions> GetByUser()
        {
            List<Actions> resultVar = new List<Actions>();
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
                    resultVar = connection
                        .Query<Actions>("Actions_GetByUser", param, commandType: CommandType.StoredProcedure)
                         as List<Actions>;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultVar;
        }

        public async Task<List<Actions>> GetByUserAsync()
        {
            List<Actions> resultVar = new List<Actions>();
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
                    resultVar = await connection
                        .QueryAsync<Actions>("Actions_GetByUser", param, commandType: CommandType.StoredProcedure)
                         as List<Actions>;
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

        public static List<Actions> Static_GetByUser(string actBy, int userId)
        {
            Actions actions = new Actions
            {
                ActBy = actBy,
                UserId = userId
            };

            return actions.GetByUser();
        }

        public static async Task<List<Actions>> Static_GetByUserAsync(string actBy, int userId)
        {
            Actions actions = new Actions
            {
                ActBy = actBy,
                UserId = userId
            };

            return await actions.GetByUserAsync();
        }

        public static async Task<List<Actions>> Static_GetParentActions()
        {
            Actions actions = new Actions();

            return await actions.GetParentActions();
        }

        public static async Task<List<Actions>> Static_GetListIsDisplayed()
        {
            Actions actions = new Actions();

            return await actions.GetListIsDisplayed();
        }

        public static async Task<Actions> Static_GetById(string actBy, int actionId, byte selfJoin = 0)
        {
            Actions actions = new Actions
            {
                ActBy = actBy,
                ActionId = actionId
            };

            return await actions.Get(selfJoin);
        }

        #endregion
    }
}
