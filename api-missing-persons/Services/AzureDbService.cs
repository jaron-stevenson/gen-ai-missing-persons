﻿namespace api_missing_persons.Services
{
    using api_missing_persons.Interfaces;
    using api_missing_persons.Models;
    using Dapper;
    using System.Data;
    using System.Data.SqlClient;
    using System.Text.Json;

    public class AzureDbService(string connectionString) : IAzureDbService
    {
        public async Task<string> GetDbResults(string query)
        {
            using IDbConnection connection = new SqlConnection(connectionString);

            var dbResult = await connection.QueryAsync<dynamic>(query);
            var jsonString = JsonSerializer.Serialize(dbResult);

            return jsonString;
        }

        public async Task<PersonDetail> GetMissingPerson(string name, int age, DateTime dateReported)
        {
            using IDbConnection connection = new SqlConnection(connectionString);

            var sql = @"SELECT *
                        FROM dbo.MissingPersons
                        WHERE LOWER(TRIM(Name)) = @Name
                          AND Age = @Age
                          AND DateReported = @DateReported
                          OR LastSeen = @DateReported";

            var personDetail = await connection.QueryFirstOrDefaultAsync<PersonDetail>(sql, new
            {
                Name = name,
                Age = age,
                DateReported = dateReported
            });

            return personDetail;
        }

        public async Task<int> UpdateMissingPerson(int id, DateTime dateFound)
        {
            using IDbConnection connection = new SqlConnection(connectionString);

            var sql = @"UPDATE dbo.MissingPersons
                        SET DateFound = @DateFound,
                            CurrentStatus = 'Found'
                        WHERE Id = @Id";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                DateFound = dateFound,
                Id = id
            });

            return rowsAffected;
        }
    }    
}
