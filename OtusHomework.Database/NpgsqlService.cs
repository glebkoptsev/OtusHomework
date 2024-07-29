using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace OtusHomework.Database
{
    public class NpgsqlService : IAsyncDisposable, IDisposable
    {
        private NpgsqlMultiHostDataSource Npgsql { get; }
        public NpgsqlService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("postgres")
                ?? throw new Exception("connection string not found");
            Npgsql = new NpgsqlDataSourceBuilder(connectionString).BuildMultiHost();
            CreateDbSchema();
        }

        public void Dispose()
        {
            Npgsql.Dispose();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await Npgsql.DisposeAsync();
            GC.SuppressFinalize(this);
        }

        public async Task<int> ExecuteNonQueryAsync(string query, NpgsqlParameter[] parameters)
        {
            await using var connection = await Npgsql.OpenConnectionAsync(TargetSessionAttributes.Primary);
            await using var cmd = new NpgsqlCommand(query, connection);
            if (parameters.Length > 0)
            {
                //await cmd.PrepareAsync();
                cmd.Parameters.AddRange(parameters);
            }
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<Dictionary<string, object>>> GetQueryResultAsync(string query, NpgsqlParameter[] parameters, string[] columns, TargetSessionAttributes targetSessionAttributes = TargetSessionAttributes.Any)
        {
            List<Dictionary<string, object>> result = [];
            await using var connection = await Npgsql.OpenConnectionAsync(targetSessionAttributes);
            await using var cmd = new NpgsqlCommand(query, connection);
            if (parameters.Length > 0)
            {
                //await cmd.PrepareAsync();
                cmd.Parameters.AddRange(parameters);
            }
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                foreach (var column in columns)
                {
                    row.Add(column, reader.GetValue(column));
                }
                result.Add(row);
            }
            return result;
        }

        private void CreateDbSchema()
        {
            var query = @"create table if not exists public.""Users"" (
	                        ""User_id"" uuid NOT NULL,
	                        ""First_name"" varchar(30) NOT NULL,
	                        ""Second_name"" varchar(30) NOT NULL,
	                        ""Birthdate"" varchar(11) NOT NULL,
	                        ""Biography"" varchar(1000) NOT NULL,
	                        ""City"" varchar(30) NOT NULL,
	                        ""Password"" varchar(255) NOT NULL,
	                        constraint ""PK_Users"" PRIMARY KEY (""User_id"")
                    )";
            ExecuteNonQueryAsync(query, []).Wait();
        }
    }
}
