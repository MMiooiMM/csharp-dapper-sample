using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            await Sample2ParameterQueryAsync();
        }

        private static async Task Sample1EasyQueryAsync()
        {
            Console.WriteLine("範例一：簡易查詢");
            using var connection = new SqlConnection(connectionString);

            string query = "SELECT TOP 5 * FROM Customers;";
            var customers = await connection.QueryAsync<Customer>(query);
            foreach (var customer in customers)
            {
                Console.WriteLine($"{nameof(customer.CompanyName)}: {customer.CompanyName}");
            }
        }

        private static async Task Sample2ParameterQueryAsync()
        {
            Console.WriteLine("範例二：參數查詢");
            using var connection = new SqlConnection(connectionString);

            string query = "SELECT TOP 5 * FROM Customers WHERE CustomerID = @CustomerID;";
            var customers = await connection.QueryAsync<Customer>(query, new { CustomerID = "BONAP" });

            foreach (var customer in customers)
            {
                Console.WriteLine($"{nameof(customer.CompanyName)}: {customer.CompanyName}");
            }
        }
    }
}