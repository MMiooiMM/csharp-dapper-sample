using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using DapperDemo.Model;

namespace DapperDemo
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            await Sample2ParameterQuery();
        }

        private static async Task Sample1EasyQuery()
        {
            using var connection = new SqlConnection("Server=.;Database=Northwind;Integrated Security=True;");

            string query = "SELECT TOP 5 * FROM Customers;";
            var customers = await connection.QueryAsync<Customer>(query);
            foreach (var customer in customers)
            {
                Console.WriteLine($"{nameof(customer.CompanyName)}: {customer.CompanyName}");
            }
        }

        private static async Task Sample2ParameterQuery()
        {
            using var connection = new SqlConnection("Server=.;Database=Northwind;Integrated Security=True;");

            string query = "SELECT TOP 5 * FROM Customers WHERE CustomerID = @CustomerID;";
            var customers = await connection.QueryAsync<Customer>(query, new { CustomerID = "BONAP" });

            foreach (var customer in customers)
            {
                Console.WriteLine($"{nameof(customer.CompanyName)}: {customer.CompanyName}");
            }
        }
    }
}