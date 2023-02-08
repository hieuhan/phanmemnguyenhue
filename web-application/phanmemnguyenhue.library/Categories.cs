using phanmemnguyenhue.helper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace phanmemnguyenhue.library
{
    public class Categories
    {
        #region Properties
        public string ActBy { get; set; }
        public int SiteId { get; set; }
        public int ParentId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TreeOrder { get; set; }
        public int? DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }

        private readonly string _connectionString;

        #endregion

        #region Constructors

        public Categories()
        {
            _connectionString = ConstantHelper.CommonConstr;
        }

        public Categories(string connection)
        {
            _connectionString = connection;
        }

        ~Categories()
        {

        }

        public virtual void Dispose()
        {

        }

        #endregion

        #region Methods

        public async Task<List<Categories>> GetList(int siteId)
        {
            List<Categories> resultVar = new List<Categories>();

            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    resultVar = await connection.QueryAsync<Categories>(string.Format("SELECT * FROM [dbo].[Categories] WHERE [SiteId]={0} ORDER BY [TreeOrder]", siteId)) as List<Categories>;
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

        #region Static Methods

        public static async Task<List<Categories>> Static_GetList(int siteId)
        {
            Categories Categories = new Categories();
            return await Categories.GetList(siteId);
        }

        public static Categories Static_Get(int siteId, List<Categories> list)
        {
            Categories resultVar = null;

            if (siteId > 0 && list != null && list.Count > 0)
            {
                resultVar = list.FirstOrDefault(i => i.SiteId == siteId);
            }

            return resultVar;
        }

        #endregion
    }
}
