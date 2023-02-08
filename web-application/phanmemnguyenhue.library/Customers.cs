using Dapper;
using phanmemnguyenhue.helper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace phanmemnguyenhue.library
{
    public class Customers
    {
        #region Properties
        public string ActBy { get; set; }
        public int CustomerId { get; set; }
        public int SiteId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public string Profile { get; set; }
        public string Note { get; set; }
        public int TotalProducts { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastCrawledAt { get; set; }

        private readonly string _connectionString;

        #endregion

        #region Constructors

        public Customers()
        {
            _connectionString = ConstantHelper.CommonConstr;
        }

        public Customers(string connection)
        {
            _connectionString = connection;
        }

        ~Customers()
        {

        }

        public virtual void Dispose()
        {

        }

        #endregion

        #region Methods

        public async Task<Customers> Get(int landTypeId, int actionTypeId, int provinceId, int districtId, int wardId, int streetId, int projectId)
        {
            Customers retVal = new Customers();
            byte verified = 0, isVideo = 0, showEmail = 0, searchByDateType = 0;
            string keywords = string.Empty;
            int categoryId = 0, orderByClauseId = 0, pageIndex = 0, pageSize = 1;
            DateTime dateFrom = DateTime.MinValue, dateTo = DateTime.MinValue;
            Tuple<List<Customers>, int> tuple;
            try
            {
                tuple = await GetPage(keywords, dateFrom, dateTo, categoryId, landTypeId, actionTypeId, provinceId, districtId, wardId, streetId, projectId, verified, isVideo, showEmail, searchByDateType, orderByClauseId, pageIndex, pageSize);
                if (tuple.Item1 != null && tuple.Item1.Count > 0) retVal = tuple.Item1[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retVal;
        }

        public async Task<Tuple<List<Customers>, int>> GetPage(string keywords, DateTime dateFrom, DateTime dateTo, int categoryId, int landTypeId, int actionTypeId, int provinceId, int districtId, int wardId, int streetId, int projectId, byte verified, byte isVideo, byte showEmail, byte searchByDateType, int orderByClauseId, int pageIndex, int pageSize)
        {
            int rowCount = 0;
            List<Customers> customersList = new List<Customers>();
            Tuple<List<Customers>, int> resultVar = Tuple.Create(customersList, rowCount);

            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    if (!string.IsNullOrEmpty(this.ActBy))
                        param.Add("@ActBy", this.ActBy, DbType.String);
                    param.Add("@CustomerId", this.CustomerId, DbType.Int32);
                    param.Add("@SiteId", this.SiteId, DbType.Int32);
                    if (!string.IsNullOrEmpty(this.FullName))
                        param.Add("@FullName", this.FullName, DbType.String);
                    if (!string.IsNullOrEmpty(this.Email))
                        param.Add("@Email", this.Email, DbType.String);
                    if (!string.IsNullOrEmpty(this.PhoneNumber))
                        param.Add("@PhoneNumber", this.PhoneNumber, DbType.String);
                    if (!string.IsNullOrEmpty(keywords))
                        param.Add("@Keywords", keywords, DbType.String);
                    param.Add("@CategoryId", categoryId, DbType.Int32);
                    param.Add("@LandTypeId", landTypeId, DbType.Int32);
                    param.Add("@ActionTypeId", actionTypeId, DbType.Int32);
                    param.Add("@ProvinceId", provinceId, DbType.Int32);
                    param.Add("@DistrictId", districtId, DbType.Int32);
                    param.Add("@WardId", wardId, DbType.Int32);
                    param.Add("@StreetId", streetId, DbType.Int32);
                    param.Add("@ProjectId", projectId, DbType.Int32);
                    param.Add("@Verified", verified, DbType.Byte);
                    param.Add("@IsVideo", isVideo, DbType.Byte);
                    param.Add("@ShowEmail", showEmail, DbType.Byte);
                    param.Add("@SearchByDateType", searchByDateType, DbType.Byte);
                    if (dateFrom != DateTime.MinValue)
                        param.Add("@DateFrom", dateFrom, DbType.DateTime);
                    if (dateTo != DateTime.MinValue)
                        param.Add("@DateTo", dateTo, DbType.DateTime);
                    param.Add("@OrderByClauseId", orderByClauseId, DbType.Int32);
                    param.Add("@PageSize", pageSize, DbType.Int32);
                    param.Add("@PageIndex", pageIndex, DbType.Int32);
                    param.Add("@RowCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    customersList = await connection
                        .QueryAsync<Customers>("Customers_GetPage", param, commandType: CommandType.StoredProcedure)
                         as List<Customers>;

                    rowCount = param.Get<int>("RowCount");

                    resultVar = Tuple.Create(customersList, rowCount);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultVar;
        }

        public async Task<List<Customers>> GetBy(string customerIds)
        {
            List<Customers> resultVar = new List<Customers>();
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    if (!string.IsNullOrEmpty(this.ActBy))
                        param.Add("@ActBy", this.ActBy, DbType.String);
                    param.Add("@SiteId", this.SiteId, DbType.Int32);
                    param.Add("@CustomerIds", customerIds, DbType.String);
                    resultVar = await connection
                        .QueryAsync<Customers>("Customers_GetListBy", param, commandType: CommandType.StoredProcedure)
                        as List<Customers>;
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

        public static async Task<List<Customers>> Static_GetBy(string actBy, int siteId, string customerIds)
        {
            try
            {
                Customers customers = new Customers
                {
                    ActBy = actBy,
                    SiteId = siteId
                };

                return await customers.GetBy(customerIds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Customers Static_Get(byte customerId, List<Customers> list)
        {
            Customers retVal = new Customers();
            if (customerId > 0 && list != null && list.Count > 0)
            {
                retVal = list.Find(i => i.CustomerId == customerId) ?? new Customers();
            }
            return retVal;
        }

        public static async Task<Customers> Static_GetById(string actBy, int customerId)
        {
            int landTypeId = 0, actionTypeId = 0, provinceId = 0, districtId = 0, wardId = 0, streetId = 0, projectId = 0;
            Customers customers = new Customers
            {
                ActBy = actBy,
                CustomerId = customerId
            };
            return await customers.Get(landTypeId, actionTypeId, provinceId, districtId, wardId, streetId, projectId);
        }


        #endregion
    }
}
