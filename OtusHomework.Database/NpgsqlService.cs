﻿using Microsoft.Extensions.Configuration;
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
            var query = @"CREATE TABLE IF NOT EXISTS public.users
                            (
                                user_id uuid NOT NULL,
                                first_name character varying(30) NOT NULL,
                                second_name character varying(30) NOT NULL,
                                birthdate character varying(11) NOT NULL,
                                biography character varying(1000) NOT NULL,
                                city character varying(255) NOT NULL,
                                password character varying(255) NOT NULL,
                                CONSTRAINT pk_users PRIMARY KEY (user_id)
                            );
                           CREATE INDEX IF NOT EXISTS users_fname_sname_idx ON public.users(first_name varchar_pattern_ops, second_name varchar_pattern_ops);";
            ExecuteNonQueryAsync(query, []).Wait();
        }
    }
}
