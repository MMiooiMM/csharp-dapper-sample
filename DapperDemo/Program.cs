using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DapperDemo.Model;
using Microsoft.Extensions.Configuration;

namespace DapperDemo
{
    internal class Program
    {
        private static string connectionString;

        private static async Task Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            connectionString = config.GetConnectionString("DefaultConnection");

            await SampleConcurrencyAsync();
        }

        private static async Task SampleEasyQueryAsync()
        {
            Console.WriteLine(nameof(SampleEasyQueryAsync));
            using var connection = new SqlConnection(connectionString);

            string query = "SELECT * FROM Customers;";
            var customers = await connection.QueryAsync<Customer>(query);
            foreach (var customer in customers)
            {
                Console.WriteLine($"{nameof(customer.CompanyName)}: {customer.CompanyName}");
            }
            Console.WriteLine($"Count: {customers.ToList().Count}");
        }

        private static async Task SampleParameterQueryAsync()
        {
            Console.WriteLine(nameof(SampleParameterQueryAsync));
            using var connection = new SqlConnection(connectionString);

            string query = "SELECT * FROM Customers WHERE City = @City;";
            var customers = await connection.QueryAsync<Customer>(query, new { City = "London" });

            foreach (var customer in customers)
            {
                Console.WriteLine($"{nameof(customer.CompanyName)}: {customer.CompanyName}");
            }
            Console.WriteLine($"Count: {customers.ToList().Count}");
        }

        private static async Task SampleQueryFirstAsync()
        {
            Console.WriteLine(nameof(SampleQueryFirstAsync));
            using var connection = new SqlConnection(connectionString);

            string query = "SELECT * FROM Customers WHERE City = @City;";
            var customer = await connection.QueryFirstAsync<Customer>(query,
                new
                {
                    City = "London"
                });

            Console.WriteLine($"{nameof(customer.CompanyName)}: {customer.CompanyName}");
        }

        private static async Task SampleQueryFirstOrDefaultAsync()
        {
            Console.WriteLine(nameof(SampleQueryFirstOrDefaultAsync));
            using var connection = new SqlConnection(connectionString);

            string query = "SELECT * FROM Customers WHERE City = @City;";
            var customer = await connection.QueryFirstOrDefaultAsync<Customer>(query,
                new
                {
                    City = "London7"
                });

            if (customer is null)
            {
                Console.WriteLine("Data NotFound");
            }
            else
            {
                Console.WriteLine($"{nameof(customer.CompanyName)}: {customer.CompanyName}");
            }
        }

        private static async Task SampleQuerySingleAsync()
        {
            Console.WriteLine(nameof(SampleQuerySingleAsync));
            using var connection = new SqlConnection(connectionString);

            string query = "SELECT * FROM Customers WHERE City = @City AND PostalCode = @PostalCode;";
            var customer = await connection.QuerySingleAsync<Customer>(query,
                new
                {
                    City = "London",
                    PostalCode = "EC2 5NT"
                });

            Console.WriteLine($"{nameof(customer.CompanyName)}: {customer.CompanyName}");
        }

        private static async Task SampleQuerySingleOrDefaultAsync()
        {
            Console.WriteLine(nameof(SampleQuerySingleOrDefaultAsync));
            using var connection = new SqlConnection(connectionString);

            string query = "SELECT * FROM Customers WHERE City = @City;";
            var customer = await connection.QuerySingleOrDefaultAsync<Customer>(query,
                new
                {
                    City = "London7"
                });

            if (customer is null)
            {
                Console.WriteLine("Data NotFound");
            }
            else
            {
                Console.WriteLine($"{nameof(customer.CompanyName)}: {customer.CompanyName}");
            }
        }

        private static async Task SampleQueryMultipleAsync()
        {
            Console.WriteLine(nameof(SampleQueryMultipleAsync));
            using var connection = new SqlConnection(connectionString);
            string query = "SELECT * FROM Customers WHERE City = @City;" +
                "SELECT * FROM Customers WHERE City = @City AND PostalCode = @PostalCode;";
            var result = await connection.QueryMultipleAsync(query,
                new
                {
                    City = "London",
                    PostalCode = "EC2 5NT"
                });

            while (!result.IsConsumed)
            {
                var customers = await result.ReadAsync<Customer>();
                foreach (var customer in customers)
                {
                    Console.WriteLine($"{nameof(customer.CompanyName)}: {customer.CompanyName}");
                }
                Console.WriteLine($"Count: {customers.ToList().Count}");
            }
        }

        private static async Task SampleExecuteInsertAsync()
        {
            Console.WriteLine(nameof(SampleExecuteInsertAsync));
            using var connection = new SqlConnection(connectionString);

            var data = new
            {
                CustomerID = Guid.NewGuid().ToString().Substring(0, 5).ToUpper(),
                CompanyName = "Test Company"
            };

            string query = "INSERT INTO Customers(CustomerID, CompanyName) VALUES (@CustomerID, @CompanyName)";
            var result = await connection.ExecuteAsync(query, data);

            Console.WriteLine($"Count: {result}");
        }

        private static async Task SampleExecuteMultiInsertAsync()
        {
            Console.WriteLine(nameof(SampleExecuteMultiInsertAsync));
            using var connection = new SqlConnection(connectionString);

            var data = new[] {
                new {
                    CustomerID = Guid.NewGuid().ToString().Substring(0, 5).ToUpper(),
                    CompanyName = "Test Company"
                },
                new {
                    CustomerID = Guid.NewGuid().ToString().Substring(0, 5).ToUpper(),
                    CompanyName = "Test Company"
                }
            };

            string query = "INSERT INTO Customers(CustomerID, CompanyName) VALUES (@CustomerID, @CompanyName)";
            var result = await connection.ExecuteAsync(query, data);

            Console.WriteLine($"Count: {result}");
        }

        private static async Task SampleExecuteStoreProcedureAsync()
        {
            Console.WriteLine(nameof(SampleExecuteStoreProcedureAsync));
            using var connection = new SqlConnection(connectionString);

            string storeProcedureName = "CustOrderHist";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@CustomerID", "AROUT", DbType.String, ParameterDirection.Input);
            parameters.Add("@return_value", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await connection.ExecuteAsync(
                storeProcedureName,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var result = parameters.Get<int>("return_value");
            Console.WriteLine($"Count: {result}");
        }

        private static async Task SampleExecuteReaderAsync()
        {
            Console.WriteLine(nameof(SampleExecuteReaderAsync));
            using var connection = new SqlConnection(connectionString);

            string query = "SELECT * FROM Customers;";

            var result = await connection.ExecuteReaderAsync(query);

            DataTable table = new DataTable();
            table.Load(result);

            Console.WriteLine($"Count: {table.Rows.Count}");
        }

        private static async Task SampleExecuteReaderStoreProcedureAsync()
        {
            Console.WriteLine(nameof(SampleExecuteReaderStoreProcedureAsync));
            using var connection = new SqlConnection(connectionString);

            string storeProcedureName = "CustOrderHist";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@CustomerID", "AROUT", DbType.String, ParameterDirection.Input);

            var result = await connection.ExecuteReaderAsync(
                storeProcedureName,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            DataTable table = new DataTable();
            table.Load(result);

            Console.WriteLine($"Count: {table.Rows.Count}");
        }

        private static async Task SampleExecuteScalarReaderAsync()
        {
            Console.WriteLine(nameof(SampleExecuteScalarReaderAsync));
            using var connection = new SqlConnection(connectionString);

            string query = "SELECT * FROM Customers;";

            var result = await connection.ExecuteScalarAsync(query);

            Console.WriteLine($"First Row First Column: {result}");
        }

        private static async Task SampleTransactionCommitAsync()
        {
            Console.WriteLine(nameof(SampleTransactionCommitAsync));
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            string executeString = "INSERT INTO Customers(CustomerID, CompanyName) " +
                "VALUES (@CustomerID, @CompanyName)";
            string queryString = "SELECT * FROM Customers " +
                "WHERE CustomerID = @CustomerID AND CompanyName = @CompanyName";

            var data = new
            {
                CustomerID = Guid.NewGuid().ToString().Substring(0, 5).ToUpper(),
                CompanyName = "Test Company"
            };

            using var transaction = await connection.BeginTransactionAsync();
            var insert = await connection.ExecuteAsync(executeString, data, transaction);
            Console.WriteLine($"在交易中對 Customer 表插入 {insert} 筆資料。");
            var customer = await connection.QueryAsync<Customer>(queryString, data, transaction);
            Console.WriteLine($"在交易中查詢新增的資料：{customer.Count()} 筆。");
            await transaction.CommitAsync();

            customer = await connection.QueryAsync<Customer>(queryString, data);
            Console.WriteLine($"在交易外查詢新增的資料：{customer.Count()} 筆。");
        }

        private static async Task SampleTransactionRollbackAsync()
        {
            Console.WriteLine(nameof(SampleTransactionRollbackAsync));
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            string executeString = "INSERT INTO Customers(CustomerID, CompanyName) " +
                "VALUES (@CustomerID, @CompanyName)";
            string queryString = "SELECT * FROM Customers " +
                "WHERE CustomerID = @CustomerID AND CompanyName = @CompanyName";

            var data = new
            {
                CustomerID = Guid.NewGuid().ToString().Substring(0, 5).ToUpper(),
                CompanyName = "Test Company"
            };

            using var transaction = await connection.BeginTransactionAsync();
            var insert = await connection.ExecuteAsync(executeString, data, transaction);
            Console.WriteLine($"在交易中對 Customer 表插入 {insert} 筆資料。");
            var customer = await connection.QueryAsync<Customer>(queryString, data, transaction);
            Console.WriteLine($"在交易中查詢新增的資料：{customer.Count()} 筆。");
            await transaction.RollbackAsync();

            customer = await connection.QueryAsync<Customer>(queryString, data);
            Console.WriteLine($"在交易外查詢新增的資料：{customer.Count()} 筆。");
        }

        private static async Task SampleConcurrencyAsync()
        {
            Console.WriteLine(nameof(SampleTransactionRollbackAsync));
            using var connection = new SqlConnection(connectionString);

            string queryString = "SELECT * FROM Customers";

            string updateString = "UPDATE Customers SET ContactName = @ContactName " +
                      "WHERE CustomerID = @CustomerID AND CompanyName = @CompanyName " +
                      "AND RowVer = @RowVer";

            var customer = await connection.QueryFirstAsync<Customer>(queryString);
            customer.ContactName = "Test Contact";
            var result = await connection.ExecuteAsync(updateString, customer);

            Console.WriteLine($"Count: {result}");

            result = await connection.ExecuteAsync(updateString, customer);

            Console.WriteLine($"Count: {result}");
        }
    }
}