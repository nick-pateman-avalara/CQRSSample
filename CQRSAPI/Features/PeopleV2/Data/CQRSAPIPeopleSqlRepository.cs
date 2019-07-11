using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRSAPI.Data;
using CQRSAPI.Features.PeopleV2.Models;
using CQRSAPI.Helpers;
using Dapper;

namespace CQRSAPI.Features.PeopleV2.Data
{

    public class CqrsApiPeopleSqlRepository : IRepository<Person>
    {

        private readonly string _connectionString;

        public string TableName => "People";

        public string ConnectionString => (string.IsNullOrEmpty(_connectionString) ? Startup.LocalTestConnectionString : _connectionString);

        public CqrsApiPeopleSqlRepository()
        { }

        public CqrsApiPeopleSqlRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Person> AddAsync(Person item, CancellationToken cancellationToken)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                string sqlQuery = $"INSERT INTO {TableName} " +
                                  "VALUES (@FirstName, @LastName, @Age); " +
                                  "SELECT SCOPE_IDENTITY()";
                int id = await db.ExecuteScalarAsync<int>(sqlQuery, item);
                item.Id = id;
                return (item);
            }
        }

        public async Task<Person> FindAsync(int id, CancellationToken cancellationToken)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                IEnumerable<Person> selectResults = await db.QueryAsync<Person>($"SELECT TOP 1 * FROM {TableName} " +
                                                                                "WHERE Id = @Id ",
                    new { id });
                return selectResults.FirstOrDefault();
            }
        }

        public async Task<List<Person>> FindAllAsync(
            int pageSize,
            int pageNumber,
            List<KeyValuePair<string, string>> queryParams,
            CancellationToken cancellationToken)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                string wherePart = QueryHelpers.Generate<Person>(queryParams, out var outParams);

                int offset = pageSize * (pageNumber - 1);
                IEnumerable<Person> selectResults = await db.QueryAsync<Person>($"SELECT * FROM {TableName} " +
                    (string.IsNullOrEmpty(wherePart) ? string.Empty : wherePart) +
                    $"ORDER BY Id OFFSET {offset} ROWS " +
                    $"FETCH NEXT {pageSize} ROWS ONLY",
                    outParams.Count > 0 ? outParams : null);
                return selectResults.ToList();
            }
        }

        public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                int rowsAffected = await db.ExecuteAsync($"DELETE FROM {TableName} " + 
                                                         "WHERE Id = @Id",
                    new { id });
                return (rowsAffected);
            }
        }

        public async Task<int> UpdateAsync(Person item, CancellationToken cancellationToken)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                string sqlQuery = $"UPDATE {TableName} SET FirstName = @FirstName, " +
                                  "LastName = @LastName " + 
                                  "WHERE Id = @Id";
                int rowsAffected = await db.ExecuteAsync(sqlQuery, item);
                return rowsAffected;
            }
        }

        public async Task<bool> Exists(int id, CancellationToken cancellationToken)
        {
            Person item = await FindAsync(id, cancellationToken);
            return (item != null);
        }

    }

}
