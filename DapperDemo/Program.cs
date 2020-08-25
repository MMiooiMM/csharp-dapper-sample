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
            await Sample1EasyQuery();
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
    }
}