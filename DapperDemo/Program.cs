using System;
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
            await Sample5QuerySingleAsync();
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
            var customer = await connection.QueryFirstAsync<Customer>(query, new { City = "London" });

            Console.WriteLine($"{nameof(customer.CompanyName)}: {customer.CompanyName}");
        }

        private static async Task Sample4QueryFirstOrDefaultAsync()
        {
            Console.WriteLine("Sample4：QueryFirst");
            using var connection = new SqlConnection(connectionString);

            string query = "SELECT * FROM Customers WHERE City = @City;";
            var customer = await connection.QueryFirstOrDefaultAsync<Customer>(query, new { City = "London7" });

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
            var customer = await connection.QuerySingleAsync<Customer>(query, new { City = "London", PostalCode = "EC2 5NT" });

            Console.WriteLine($"{nameof(customer.CompanyName)}: {customer.CompanyName}");
        }
    }
}