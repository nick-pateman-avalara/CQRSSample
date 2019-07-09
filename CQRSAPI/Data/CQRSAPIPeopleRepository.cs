using CQRSAPI.Models;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CQRSAPI.Data
{

    public class CqrsApiPeopleRepository : IRepository<Person>
    {

        private readonly string _connectionString;
        private readonly string _prefix;

        public string TableName => string.Format("{0}.People", string.IsNullOrEmpty(_prefix) ? "dbo" : _prefix);

        public string ConnectionString => (string.IsNullOrEmpty(_connectionString) ? Startup.LocalTestConnectionString : _connectionString);

        public CqrsApiPeopleRepository()
        { }

        public CqrsApiPeopleRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public CqrsApiPeopleRepository(
            string connectionString,
            string prefix)
        {
            _connectionString = connectionString;
            _prefix = prefix;
        }

        public async Task<Person> AddAsync(Person item, CancellationToken cancellationToken)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                string sqlQuery = $"INSERT INTO {TableName} (FirstName, LastName, Age) " +
                                  "VALUES(@FirstName, @LastName, @Age); " +
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
                IEnumerable<Person> selectResults = await db.QueryAsync<Person>($"SELECT TOP 1 * FROM {TableName} WHERE Id = @Id ",
                    new { id });
                return selectResults.FirstOrDefault();
            }
        }

        public async Task<List<Person>> FindAllAsync(
            int pageSize,
            int pageNumber,
            CancellationToken cancellationToken)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                int offset = pageSize * (pageNumber - 1);
                IEnumerable<Person> selectResults = await db.QueryAsync<Person>($"SELECT * FROM {TableName} " +
                    $"ORDER BY Id OFFSET {offset} ROWS " +
                    $"FETCH NEXT {pageSize} ROWS ONLY");
                return selectResults.ToList();
            }
        }

        public async Task<int> DeleteAsync(Person item, CancellationToken cancellationToken)
        {
            return (await DeleteAsync(item.Id, cancellationToken));
        }

        public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                int rowsAffected = await db.ExecuteAsync($"DELETE FROM {TableName} WHERE Id = @Id",
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
