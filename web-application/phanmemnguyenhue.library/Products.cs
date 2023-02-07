using Dapper;
using phanmemnguyenhue.helper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace phanmemnguyenhue.library
{
    public class Products
    {
        #region Properties

        public string ActBy { get; set; }
        public int ProductId { get; set; }
        public int SiteId { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string ProductUrl { get; set; }
        public string ImagePath { get; set; }
        public string ImageRelates { get; set; }
        public int ProductCode { get; set; }
        public string ProductContent { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }
        public int StreetId { get; set; }
        public int ProjectId { get; set; }
        public int ProductProjectId { get; set; }
        public int CustomerId { get; set; }
        public string Breadcrumb { get; set; }
        public string Address { get; set; }
        public byte Verified { get; set; }
        public byte IsVideo { get; set; }
        public double Area { get; set; }
        public string AreaDisplay { get; set; }
        public double Price { get; set; }
        public string PriceDisplay { get; set; }
        public double ComputedPrice { get; set; }
        public double Facade { get; set; }
        public string FacadeDisplay { get; set; }
        public double WayIn { get; set; }
        public string WayInDisplay { get; set; }
        public byte Floors { get; set; }
        public string HouseDirection { get; set; }
        public string BalconyDirection { get; set; }
        public short Rooms { get; set; }
        public short Toilets { get; set; }
        public string Juridical { get; set; }
        public string Interiors { get; set; }
        public int ProductTypeId { get; set; }
        public int ActionTypeId { get; set; }
        public int LandTypeId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime PublishedAt { get; set; }
        public DateTime ExpirationAt { get; set; }

        private readonly string _connectionString;

        #endregion

        #region Constructors

        public Products()
        {
            _connectionString = ConstantHelper.CommonConstr;
        }

        public Products(string connection)
        {
            _connectionString = connection;
        }

        ~Products()
        {

        }

        public virtual void Dispose()
        {

        }

        #endregion

        #region Methods

        public async Task<Products> Get()
        {
            Products retVal = new Products();
            byte searchByDateType = 0;
            //DateTime dateFrom = DateTime.MinValue, dateTo = DateTime.MinValue;
            int orderByClauseId = 0, pageIndex = 0, pageSize = 1;
            Tuple<List<Products>, int> tuple;
            try
            {
                tuple = await GetPage(searchByDateType, orderByClauseId, pageIndex, pageSize);
                if (tuple.Item1 != null && tuple.Item1.Count > 0) retVal = tuple.Item1[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retVal;
        }

        public async Task<Tuple<List<Products>, int>> GetPage(byte searchByDateType, int orderByClauseId, int pageIndex, int pageSize)
        {
            int rowCount = 0;
            List<Products> listProducts;
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    if (!string.IsNullOrEmpty(this.ActBy))
                        param.Add("@ActBy", this.ActBy, DbType.String);
                    param.Add("@ProductId", this.ProductId, DbType.Int32);
                    param.Add("@SiteId", this.SiteId, DbType.Int32);
                    if (!string.IsNullOrEmpty(this.Title))
                        param.Add("@Title", this.Title, DbType.String);
                    if (!string.IsNullOrEmpty(this.ProductUrl))
                        param.Add("@ProductUrl", this.ProductUrl, DbType.String);
                    param.Add("@ProductCode", this.ProductCode, DbType.Int32);
                    param.Add("@ProvinceId", this.ProvinceId, DbType.Int32);
                    param.Add("@DistrictId", this.DistrictId, DbType.Int32);
                    param.Add("@WardId", this.WardId, DbType.Int32);
                    param.Add("@StreetId", this.StreetId, DbType.Int32);
                    param.Add("@ProjectId", this.ProjectId, DbType.Int32);
                    param.Add("@CustomerId", this.CustomerId, DbType.Int32);
                    param.Add("@Verified", this.Verified, DbType.Byte);
                    param.Add("@IsVideo", this.IsVideo, DbType.Byte);
                    param.Add("@ProductTypeId", this.ProductTypeId, DbType.Int32);
                    param.Add("@ActionTypeId", this.ActionTypeId, DbType.Int32);
                    param.Add("@LandTypeId", this.LandTypeId, DbType.Int32);
                    param.Add("@SearchByDateType", searchByDateType, DbType.Byte);
                    //if (dateFrom != DateTime.MinValue)
                    //    param.Add("@DateFrom", dateFrom, DbType.DateTime);
                    //if (dateTo != DateTime.MinValue)
                    //    param.Add("@DateTo", dateTo, DbType.DateTime);
                    param.Add("@OrderByClauseId", orderByClauseId, DbType.Int32);
                    param.Add("@PageSize", pageSize, DbType.Int32);
                    param.Add("@PageIndex", pageIndex, DbType.Int32);
                    param.Add("@RowCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    listProducts = await connection
                        .QueryAsync<Products>("Products_GetPage", param, commandType: CommandType.StoredProcedure)
                        as List<Products>;
                    rowCount = param.Get<int>("RowCount");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Tuple.Create(listProducts, rowCount);
        }

        #endregion

        #region Static Methods


        public static async Task<Products> Static_GetById(string actBy, int productId)
        {
            Products products = new Products
            {
                ActBy = actBy,
                ProductId = productId
            };

            return await products.Get();
        }

        #endregion
    }
}
