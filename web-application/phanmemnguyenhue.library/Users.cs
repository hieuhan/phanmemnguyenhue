using Dapper;
using phanmemnguyenhue.helper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace phanmemnguyenhue.library
{
    public class Users
    {
        #region Properties

        public string ActBy { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public byte GetByUserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public byte GetByEmail { get; set; }
        public string PhoneNumber { get; set; }
        public byte GetByPhoneNumber { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }
        public DateTime BirthDay { get; set; }
        public string Comment { get; set; }
        public string RoleName { get; set; }
        public byte GenderId { get; set; }
        public byte UserStatusId { get; set; }
        public byte BuildIn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime LastLoggedInAt { get; set; }
        public string PasswordLastChangedBy { get; set; }
        public DateTime PasswordLastChangedAt { get; set; }

        private readonly string _connectionString;

        #endregion

        #region Constructors

        public Users()
        {
            _connectionString = ConstantHelper.CommonConstr;
        }

        public Users(string connection)
        {
            _connectionString = connection;
        }

        ~Users()
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
                    if (!string.IsNullOrEmpty(this.UserName))
                        param.Add("@UserName", this.UserName, DbType.String);
                    if (!string.IsNullOrEmpty(this.Password))
                        param.Add("@Password", this.Password, DbType.String);
                    if (!string.IsNullOrEmpty(this.FullName))
                        param.Add("@FullName", this.FullName, DbType.String);
                    if (!string.IsNullOrEmpty(this.Email))
                        param.Add("@Email", this.Email, DbType.String);
                    if (!string.IsNullOrEmpty(this.PhoneNumber))
                        param.Add("@PhoneNumber", this.PhoneNumber, DbType.String);
                    if (!string.IsNullOrEmpty(this.Address))
                        param.Add("@Address", this.Address, DbType.String);
                    if (!string.IsNullOrEmpty(this.Avatar))
                        param.Add("@Avatar", this.Avatar, DbType.String);
                    if (this.BirthDay != DateTime.MinValue)
                        param.Add("@BirthDay", this.BirthDay, DbType.Date);
                    if (!string.IsNullOrEmpty(this.Comment))
                        param.Add("@Comment", this.Comment, DbType.String);
                    param.Add("@GenderId", this.GenderId, DbType.Byte);
                    param.Add("@UserStatusId", this.UserStatusId, DbType.Byte);
                    param.Add("@UserId", this.UserId, DbType.Int32, ParameterDirection.InputOutput);
                    param.Add("@ActionStatus", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
                    param.Add("@ActionMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 250);

                    await connection.ExecuteAsync("Users_InsertOrUpdate", param, commandType: CommandType.StoredProcedure);
                    this.UserId = param.Get<int>("UserId");
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
                    param.Add("@UserId", this.UserId, DbType.Int32);
                    param.Add("@ActionStatus", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
                    param.Add("@ActionMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 250);

                    await connection.ExecuteAsync("Users_Delete", param, commandType: CommandType.StoredProcedure);
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

        public async Task<Users> Get()
        {
            Users resultVar = new Users();
            string keywords = string.Empty;
            byte searchByDateType = 0;
            DateTime dateFrom = DateTime.MinValue, dateTo = DateTime.MinValue;
            int orderByClauseId = 0, pageIndex = 0, pageSize = 1;

            try
            {
                Tuple<List<Users>, int> tuple = await GetPage(keywords, dateFrom, dateTo, searchByDateType, orderByClauseId, pageIndex, pageSize);

                if (tuple != null && tuple.Item1 != null && tuple.Item1.Count > 0) resultVar = tuple.Item1[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultVar;
        }

        public async Task<Tuple<List<Users>, int>> GetPage(string keywords, DateTime dateFrom, DateTime dateTo, byte searchByDateType, int orderByClauseId, int pageIndex, int pageSize)
        {
            int rowCount = 0;
            List<Users> usersList = new List<Users>();
            Tuple<List<Users>, int> resultVar = Tuple.Create(usersList, rowCount);

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
                    if (!string.IsNullOrEmpty(keywords))
                        param.Add("@Keywords", keywords, DbType.String);
                    if (!string.IsNullOrEmpty(this.UserName))
                        param.Add("@UserName", this.UserName, DbType.String);
                    param.Add("@GetByUserName", this.GetByUserName, DbType.Byte);
                    if (!string.IsNullOrEmpty(this.Password))
                        param.Add("@Password", this.Password, DbType.String);
                    if (!string.IsNullOrEmpty(this.FullName))
                        param.Add("@FullName", this.FullName, DbType.String);
                    if (!string.IsNullOrEmpty(this.Email))
                        param.Add("@Email", this.Email, DbType.String);
                    //param.Add("@GetByEmail", this.GetByEmail, DbType.Byte);
                    if (!string.IsNullOrEmpty(this.PhoneNumber))
                        param.Add("@PhoneNumber", this.PhoneNumber, DbType.String);
                    //param.Add("@GetByPhoneNumber", this.GetByPhoneNumber, DbType.Byte);
                    if (!string.IsNullOrEmpty(this.Address))
                        param.Add("@Address", this.Address, DbType.String);
                    if (!string.IsNullOrEmpty(this.Avatar))
                        param.Add("@Avatar", this.Avatar, DbType.String);
                    if (!string.IsNullOrEmpty(this.Comment))
                        param.Add("@Comment", this.Comment, DbType.String);
                    param.Add("@GenderId", this.GenderId, DbType.Byte);
                    param.Add("@UserStatusId", this.UserStatusId, DbType.Byte);
                    if (!string.IsNullOrEmpty(this.CreatedBy))
                        param.Add("@CreatedBy", this.CreatedBy, DbType.String);
                    if (!string.IsNullOrEmpty(this.UpdatedBy))
                        param.Add("@UpdatedBy", this.UpdatedBy, DbType.String);
                    if (!string.IsNullOrEmpty(this.PasswordLastChangedBy))
                        param.Add("@PasswordLastChangedBy", this.PasswordLastChangedBy, DbType.String);
                    param.Add("@SearchByDateType", searchByDateType, DbType.Byte);
                    if (dateFrom != DateTime.MinValue)
                        param.Add("@DateFrom", dateFrom, DbType.DateTime);
                    if (dateTo != DateTime.MinValue)
                        param.Add("@DateTo", dateTo, DbType.DateTime);
                    param.Add("@OrderByClauseId", orderByClauseId, DbType.Int32);
                    param.Add("@PageSize", pageSize, DbType.Int32);
                    param.Add("@PageIndex", pageIndex, DbType.Int32);
                    param.Add("@RowCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    usersList = await connection
                        .QueryAsync<Users>("Users_GetPage", param, commandType: CommandType.StoredProcedure)
                         as List<Users>;

                    rowCount = param.Get<int>("RowCount");

                    resultVar = Tuple.Create(usersList, rowCount);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultVar;
        }

        public async Task<Tuple<string, string>> Update_LastLoggedInAt()
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
                    param.Add("@ActionStatus", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
                    param.Add("@ActionMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 250);

                    await connection.ExecuteAsync("Users_Update_LastLoggedInAt", param, commandType: CommandType.StoredProcedure);

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

        public async Task<Tuple<string, string>> Update_LastPasswordChangeAt()
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
                    if (!string.IsNullOrEmpty(this.Password))
                        param.Add("@Password", this.Password, DbType.String);
                    param.Add("@ActionStatus", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
                    param.Add("@ActionMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 250);

                    await connection.ExecuteAsync("Users_Update_LastPasswordChangeAt", param, commandType: CommandType.StoredProcedure);

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

        public byte HasPriv(string path)
        {
            byte hasPriv = 0;
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@UserId", this.UserId, DbType.Int32);
                    if (!string.IsNullOrEmpty(path))
                        param.Add("@Path", path, DbType.String);
                    param.Add("@HasPriv", dbType: DbType.Byte, direction: ParameterDirection.Output);

                    connection.Execute("Users_HasPriv", param, commandType: CommandType.StoredProcedure);

                    hasPriv = param.Get<byte>("HasPriv");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return hasPriv;
        }

        public async Task<Tuple<string, string>> ChangePassword()
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
                    if (!string.IsNullOrEmpty(this.Password))
                        param.Add("@Password", this.Password, DbType.String);
                    param.Add("@ActionStatus", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);
                    param.Add("@ActionMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 250);

                    await connection.ExecuteAsync("Users_Update_Password", param, commandType: CommandType.StoredProcedure);

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

        public static async Task<Users> Static_GetById(string actBy, int userId)
        {
            try
            {
                Users users = new Users
                {
                    ActBy = actBy,
                    UserId = userId
                };

                return await users.Get();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<Tuple<string, string>> Static_Update_LastLoggedInAt(string actBy, int userId)
        {
            try
            {
                Users users = new Users
                {
                    ActBy = actBy,
                    UserId = userId
                };

                return await users.Update_LastLoggedInAt();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<Tuple<string, string>> Static_Update_LastPasswordChangeAt(string actBy, int userId, string password)
        {
            try
            {
                Users users = new Users
                {
                    ActBy = actBy,
                    UserId = userId,
                    Password = password
                };

                return await users.Update_LastPasswordChangeAt();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static byte Static_HasPriv(int userId, string path)
        {
            try
            {
                Users users = new Users
                {
                    UserId = userId
                };

                return users.HasPriv(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<Users> Static_GetByUserName(string userName)
        {
            try
            {
                Users users = new Users
                {
                    UserName = userName,
                    GetByUserName = 1
                };

                return await users.Get();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<Users> Static_GetByEmail(string actBy, int userId, string email)
        {
            try
            {
                Users users = new Users
                {
                    ActBy = actBy,
                    UserId = userId,
                    Email = email,
                    GetByEmail = 1
                };

                return await users.Get();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<Users> Static_GetByPhoneNumber(string actBy, int userId, string phoneNumber)
        {
            try
            {
                Users users = new Users
                {
                    ActBy = actBy,
                    UserId = userId,
                    PhoneNumber = phoneNumber,
                    GetByPhoneNumber = 1
                };

                return await users.Get();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

    }
}
