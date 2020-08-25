using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DapperDemo.Model;

namespace DapperDemo
{
    internal class Program
    {
        private const string connectionString = "Server=.;Database=Northwind;Integrated Security=True;";

        private static async Task Main(string[] args)
        {
            await Sample10ExecuteMultiInsertAsync();
        }

        private static async Task Sample1EasyQueryAsync()
        {
            Console.WriteLine("Sample1：EasyQuery");
            using var connection = new SqlConnection(connectionString);

            string query = "SELECT * FROM Customers;";
            var customers = await connection.QueryAsync<Customer>(query);
            foreach (var customer in customers)
            {
                Console.WriteLine($"{nameof(customer.CompanyName)}: {customer.CompanyName}");
            }
            Console.WriteLine($"Count: {customers.ToList().Count}");
        }

        private static async Task Sample2ParameterQueryAsync()
        {
            Console.WriteLine("Sample2：ParameterQuery");
            using var connection = new SqlConnection(connectionString);

            string query = "SELECT * FROM Customers WHERE City = @City;";
            var customers = await connection.QueryAsync<Customer>(query, new { City = "London" });

            foreach (var customer in customers)
            {
                Console.WriteLine($"{nameof(customer.CompanyName)}: {customer.CompanyName}");
            }
            Console.WriteLine($"Count: {customers.ToList().Count}");
        }

        private static async Task Sample3QueryFirstAsync()
        {
            Console.WriteLine("Sample3：QueryFirst");
            using var connection = new SqlConnection(connectionString);

            string query = "SELECT * FROM Customers WHERE City = @City;";
            var customer = await connection.QueryFirstAsync<Customer>(query,
                new
                {
                    City = "London"
                });

            Console.WriteLine($"{nameof(customer.CompanyName)}: {customer.CompanyName}");
        }

        private static async Task Sample4QueryFirstOrDefaultAsync()
        {
            Console.WriteLine("Sample4：QueryFirst");
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

        private static async Task Sample5QuerySingleAsync()
        {
            Console.WriteLine("Sample5：QuerySingle");
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

        private static async Task Sample6QuerySingleOrDefaultAsync()
        {
            Console.WriteLine("Sample6：QuerySingleOrDefault");
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

        private static async Task Sample7QueryMultipleAsync()
        {
            Console.WriteLine("Sample7：QueryMultiple");
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

        private static async Task Sample8ExecuteInsertAsync()
        {
            Console.WriteLine("Sample8：ExecuteInsert");
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

        private static async Task Sample9ExecuteStoreProcedureAsync()
        {
            Console.WriteLine("Sample9：ExecuteStoreProcedure");
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

        private static async Task Sample10ExecuteMultiInsertAsync()
        {
            Console.WriteLine("Sample10：ExecuteMultiInsert");
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
    }
}