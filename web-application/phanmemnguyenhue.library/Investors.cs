using Dapper;
using phanmemnguyenhue.helper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace phanmemnguyenhue.library
{
    public class Investors
    {
        #region Properties

        public string ActBy { get; set; }
        public int InvestorId { get; set; }
        public int SiteId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        private readonly string _connectionString;

        #endregion

        #region Constructors

        public Investors()
        {
            _connectionString = ConstantHelper.CommonConstr;
        }

        public Investors(string connection)
        {
            _connectionString = connection;
        }

        ~Investors()
        {

        }

        public virtual void Dispose()
        {

        }

        #endregion

        #region Methods

        public async Task<List<Investors>> GetBy(string investorIds)
        {
            List<Investors> resultVar = new List<Investors>();
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
                    param.Add("@InvestorIds", investorIds, DbType.String);
                    resultVar = await connection
                        .QueryAsync<Investors>("Investors_GetListBy", param, commandType: CommandType.StoredProcedure)
                        as List<Investors>;
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

        public static async Task<List<Investors>> Static_GetBy(string actBy, int siteId, string investorIds)
        {
            try
            {
                Investors investors = new Investors
                {
                    ActBy = actBy,
                    SiteId = siteId
                };

                return await investors.GetBy(investorIds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
