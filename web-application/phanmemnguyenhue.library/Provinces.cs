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
    public class Provinces
    {
        #region Properties

        public string ActBy { get; set; }
        public int SiteId { get; set; }
        public int ProvinceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte StatusId { get; set; }
        public int? DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }

        private readonly string _connectionString;

        #endregion

        #region Constructors

        public Provinces()
        {
            _connectionString = ConstantHelper.CommonConstr;
        }

        public Provinces(string connection)
        {
            _connectionString = connection;
        }

        ~Provinces()
        {

        }

        public virtual void Dispose()
        {

        }

        #endregion

        #region Methods

        public async Task<List<Provinces>> GetList(int siteId)
        {
            List<Provinces> resultVar = new List<Provinces>();
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    resultVar = await connection.QueryAsync<Provinces>(string.Format("SELECT * FROM [dbo].[Provinces] WHERE [SiteId]={0} ORDER BY [DisplayOrder],[Name]", siteId)) as List<Provinces>;
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

        public static async Task<List<Provinces>> Static_GetList(int siteId)
        {
            Provinces provinces = new Provinces();

            return await provinces.GetList(siteId);
        }

        public static Provinces Static_Get(byte provinceId, List<Provinces> list)
        {
            Provinces resultVar = new Provinces();
            try
            {
                if (provinceId > 0 && list != null && list.Count > 0)
                {
                    resultVar = list.FirstOrDefault(i => i.ProvinceId == provinceId);
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
