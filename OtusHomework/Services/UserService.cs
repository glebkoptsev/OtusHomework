﻿using Npgsql;
using NpgsqlTypes;
using OtusHomework.Database;
using OtusHomework.Database.Entities;
using OtusHomework.Database.Security;
using OtusHomework.DTOs;

namespace OtusHomework.Services
{
    public class UserService(NpgsqlService npgsqlService)
    {
        private readonly NpgsqlService npgsqlService = npgsqlService;

        public async Task<User?> GetUserAsync(Guid id)
        {
            string query = @"SELECT ""First_name"",""Second_name"",""Birthdate"",""Biography"",""City"", ""Password""
                             FROM public.""Users""
                             WHERE ""User_id"" = @User_id";
            var parameters = new NpgsqlParameter[]
            {
                new("User_id", NpgsqlDbType.Uuid) { Value = id }
            };
            var data = await npgsqlService.GetQueryResultAsync(query, parameters, ["First_name", "Second_name", "Birthdate", "Biography", "City", "Password"]);
            if (data.Count == 0) return null;
            return new User(id, data[0]);
        }

        public async Task<UserRegisterResponse> RegisterUserAsync(UserRegisterRequest request)
        {
            string query = @"INSERT INTO public.""Users""(""User_id"", ""First_name"",""Second_name"",""Birthdate"",""Biography"",""City"",""Password"") 
                                VALUES (@User_id, @First_name, @Second_name, @Birthdate, @Biography, @City, @Password)";
            var userId = Guid.NewGuid();
            var parameters = new NpgsqlParameter[]
            {
                new("User_id", NpgsqlDbType.Uuid) { Value = userId },
                new(nameof(request.First_name), NpgsqlDbType.Varchar) { Value = request.First_name },
                new(nameof(request.Second_name), NpgsqlDbType.Varchar) { Value = request.Second_name },
                new(nameof(request.Birthdate), NpgsqlDbType.Varchar) { Value = request.Birthdate },
                new(nameof(request.Biography), NpgsqlDbType.Varchar) { Value = request.Biography },
                new(nameof(request.City), NpgsqlDbType.Varchar) { Value = request.City },
                new(nameof(request.Password), NpgsqlDbType.Varchar) { Value = PasswordHasher.Hash(request.Password) }
            };
            await npgsqlService.ExecuteNonQueryAsync(query, parameters);
            return new UserRegisterResponse { User_id = userId };
        }

        public async Task<List<User>?> SearchUserAsync(string first_name, string second_name)
        {
            string query = @"SELECT ""First_name"",""Second_name"",""Birthdate"",""Biography"",""City"", ""Password"", ""User_id""
                             FROM public.""Users""
                             WHERE ""First_name"" like @First_name and ""Second_name""like @Second_name";

            var parameters = new NpgsqlParameter[]
            {
                new("First_name", NpgsqlDbType.Varchar) { Value = first_name + '%' },
                new("Second_name", NpgsqlDbType.Varchar) { Value = second_name + '%'}
            };
            var data = await npgsqlService.GetQueryResultAsync(query, parameters, ["First_name", "Second_name", "Birthdate", "Biography", "City", "Password", "User_id"]);
            if (data.Count == 0) return null;
            var users = new List<User>();
            foreach (var user in data) 
            {
                users.Add(new User(Guid.Parse(user["User_id"].ToString()!), user));
            }
            return users;
        }
    }
}
