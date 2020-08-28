using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InteractivePreGeneratedViews;
using Oracle.ManagedDataAccess.Client;

namespace InteractivePreGeneratedViews
{
    public class OracleViewCacheFactory : DatabaseViewCacheFactory
    {
        private readonly string _connectionString;

        /// <summary>
        /// Creates a new instance of <see cref="SqlServerViewCacheFactory"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// Connection string used to connect to the database containing table with view definitions.
        /// Must not be null.
        /// </param>
        /// <param name="tableName">
        /// Name of the table where views are stored. 
        /// Optional - if not provided '__ViewCache' will be used as the table name.
        /// </param>
        /// <param name="schemaName">
        /// Schema of the table where views are stored. 
        /// Optional - if not provided 'dbo' will be used as the table schema.
        /// </param>
        public OracleViewCacheFactory(string connectionString, string tableName = null, string schemaName = null)
            : base(tableName, schemaName)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            _connectionString = connectionString;
        }

        /// <summary>
        /// Creates connection to be used to connect to the database containing table with view definitions.
        /// </summary>
        /// <returns>
        /// A <see cref="DbConnection"/> instance.
        /// </returns>
        protected override DbConnection CreateConnection()
        {
            return new OracleConnection(_connectionString);
        }

        /// <summary>
        /// Checks if the table storing cached views exists.
        /// </summary>
        /// <param name="viewCacheContext">
        /// An <see cref="DatabaseViewCacheContext"/> instance used to handle view caching.
        /// </param>
        /// <returns>
        /// <code>true</code> if table containing views exists. Otherwise <code>false</code>.
        /// </returns>
        protected override bool ViewTableExists(DatabaseViewCacheContext viewCacheContext)
        {
            return viewCacheContext.Database.SqlQuery<int>(
                    "SELECT COUNT(1) " +
                    "FROM user_tables  " +
                    "WHERE TABLE_NAME = upper('{1}') ", 
                    viewCacheContext.TableName)
                    .Single() > 0;
        }

        /// <summary>
        /// Creates the table for storing views.
        /// </summary>
        /// <param name="viewCacheContext">
        /// An <see cref="DatabaseViewCacheContext"/> instance used to handle view caching.
        /// </param>
        /// <remarks>
        /// The table has to use <see cref="DatabaseViewCacheContext.TableName"/> 
        /// and <see cref="DatabaseViewCacheContext.SchemaName"/> as table and schema names.
        /// </remarks>
        protected override void CreateViewTable(DatabaseViewCacheContext viewCacheContext)
        {
            viewCacheContext.Database.ExecuteSqlCommand(
                (((IObjectContextAdapter)viewCacheContext).ObjectContext).CreateDatabaseScript());
        }
    }
}
