using CRM.Web.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.Web.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // ===== USER METHODS =====
        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = new List<User>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM users ORDER BY id", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetInt64(0),
                            LastName = reader.GetString(1),
                            FirstName = reader.GetString(2),
                            Patronymic = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                            Email = reader.GetString(4),
                            Phone = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                            Role = reader.IsDBNull(6) ? "менеджер" : reader.GetString(6),
                            CreatedDate = reader.GetDateTime(7)
                        });
                    }
                }
            }
            return users;
        }

        public async Task<User> GetUserByIdAsync(long id)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM users WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                Id = reader.GetInt64(0),
                                LastName = reader.GetString(1),
                                FirstName = reader.GetString(2),
                                Patronymic = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                Email = reader.GetString(4),
                                Phone = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                                Role = reader.IsDBNull(6) ? "менеджер" : reader.GetString(6),
                                CreatedDate = reader.GetDateTime(7)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<bool> IsEmailExistsAsync(string email, long? excludeUserId = null)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var query = "SELECT COUNT(*) FROM users WHERE email = @email";
                if (excludeUserId.HasValue)
                {
                    query += " AND id != @excludeId";
                }

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    if (excludeUserId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@excludeId", excludeUserId.Value);
                    }

                    var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    return count > 0;
                }
            }
        }

        public async Task<(bool Success, string ErrorMessage)> AddUserAsync(User user)
        {
            try
            {
                if (await IsEmailExistsAsync(user.Email))
                {
                    return (false, "Пользователь с таким email уже существует");
                }

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    using (var cmd = new NpgsqlCommand(
                        "INSERT INTO users (last_name, first_name, patronymic, email, phone, role, created_date) " +
                        "VALUES (@lastName, @firstName, @patronymic, @email, @phone, @role, @createdDate)", conn))
                    {
                        cmd.Parameters.AddWithValue("@lastName", user.LastName);
                        cmd.Parameters.AddWithValue("@firstName", user.FirstName);
                        cmd.Parameters.AddWithValue("@patronymic", string.IsNullOrWhiteSpace(user.Patronymic) ? (object)DBNull.Value : user.Patronymic);
                        cmd.Parameters.AddWithValue("@email", user.Email);
                        cmd.Parameters.AddWithValue("@phone", string.IsNullOrWhiteSpace(user.Phone) ? (object)DBNull.Value : user.Phone);
                        cmd.Parameters.AddWithValue("@role", string.IsNullOrWhiteSpace(user.Role) ? "менеджер" : user.Role);
                        cmd.Parameters.AddWithValue("@createdDate", DateTime.Now);

                        await cmd.ExecuteNonQueryAsync();
                        return (true, string.Empty);
                    }
                }
            }
            catch (PostgresException ex) when (ex.SqlState == "23505") // Ошибка уникальности
            {
                return (false, "Пользователь с таким email уже существует");
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка при сохранении: {ex.Message}");
            }
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateUserAsync(User user)
        {
            try
            {
                if (await IsEmailExistsAsync(user.Email, user.Id))
                {
                    return (false, "Пользователь с таким email уже существует");
                }

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    using (var cmd = new NpgsqlCommand(
                        "UPDATE users SET last_name = @lastName, first_name = @firstName, " +
                        "patronymic = @patronymic, email = @email, phone = @phone, role = @role " +
                        "WHERE id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", user.Id);
                        cmd.Parameters.AddWithValue("@lastName", user.LastName);
                        cmd.Parameters.AddWithValue("@firstName", user.FirstName);
                        cmd.Parameters.AddWithValue("@patronymic", string.IsNullOrWhiteSpace(user.Patronymic) ? (object)DBNull.Value : user.Patronymic);
                        cmd.Parameters.AddWithValue("@email", user.Email);
                        cmd.Parameters.AddWithValue("@phone", string.IsNullOrWhiteSpace(user.Phone) ? (object)DBNull.Value : user.Phone);
                        cmd.Parameters.AddWithValue("@role", user.Role);

                        await cmd.ExecuteNonQueryAsync();
                        return (true, string.Empty);
                    }
                }
            }
            catch (PostgresException ex) when (ex.SqlState == "23505")
            {
                return (false, "Пользователь с таким email уже существует");
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка при обновлении: {ex.Message}");
            }
        }

        public async Task<bool> DeleteUserAsync(long id)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("DELETE FROM users WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    var affectedRows = await cmd.ExecuteNonQueryAsync();
                    return affectedRows > 0;
                }
            }
        }

        // ===== CLIENT METHODS =====
        public async Task<List<Client>> GetAllClientsAsync()
        {
            var clients = new List<Client>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM clients ORDER BY id", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        clients.Add(new Client
                        {
                            Id = reader.GetInt64(0),
                            CompanyName = reader.GetString(1),
                            INN = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            Phone = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                            Email = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                            Address = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                            Status = reader.IsDBNull(6) ? "Лид" : reader.GetString(6),
                            CreatedDate = reader.GetDateTime(7),
                            ResponsibleUserId = reader.IsDBNull(8) ? (long?)null : reader.GetInt64(8)
                        });
                    }
                }
            }
            return clients;
        }

        public async Task<Client> GetClientByIdAsync(long id)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM clients WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Client
                            {
                                Id = reader.GetInt64(0),
                                CompanyName = reader.GetString(1),
                                INN = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                Phone = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                Email = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                Address = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                                Status = reader.IsDBNull(6) ? "Лид" : reader.GetString(6),
                                CreatedDate = reader.GetDateTime(7),
                                ResponsibleUserId = reader.IsDBNull(8) ? (long?)null : reader.GetInt64(8)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<(bool Success, string ErrorMessage)> AddClientAsync(Client client)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    using (var cmd = new NpgsqlCommand(
                        "INSERT INTO clients (company_name, inn, phone, email, address, status, created_date, responsible_user_id) " +
                        "VALUES (@companyName, @inn, @phone, @email, @address, @status, @createdDate, @responsibleUserId)", conn))
                    {
                        cmd.Parameters.AddWithValue("@companyName", client.CompanyName);
                        cmd.Parameters.AddWithValue("@inn", string.IsNullOrWhiteSpace(client.INN) ? (object)DBNull.Value : client.INN);
                        cmd.Parameters.AddWithValue("@phone", string.IsNullOrWhiteSpace(client.Phone) ? (object)DBNull.Value : client.Phone);
                        cmd.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(client.Email) ? (object)DBNull.Value : client.Email);
                        cmd.Parameters.AddWithValue("@address", string.IsNullOrWhiteSpace(client.Address) ? (object)DBNull.Value : client.Address);
                        cmd.Parameters.AddWithValue("@status", string.IsNullOrWhiteSpace(client.Status) ? "Лид" : client.Status);
                        cmd.Parameters.AddWithValue("@createdDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@responsibleUserId", client.ResponsibleUserId.HasValue ? (object)client.ResponsibleUserId.Value : DBNull.Value);

                        await cmd.ExecuteNonQueryAsync();
                        return (true, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка при сохранении: {ex.Message}");
            }
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateClientAsync(Client client)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    using (var cmd = new NpgsqlCommand(
                        "UPDATE clients SET company_name = @companyName, inn = @inn, phone = @phone, " +
                        "email = @email, address = @address, status = @status, responsible_user_id = @responsibleUserId " +
                        "WHERE id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", client.Id);
                        cmd.Parameters.AddWithValue("@companyName", client.CompanyName);
                        cmd.Parameters.AddWithValue("@inn", string.IsNullOrWhiteSpace(client.INN) ? (object)DBNull.Value : client.INN);
                        cmd.Parameters.AddWithValue("@phone", string.IsNullOrWhiteSpace(client.Phone) ? (object)DBNull.Value : client.Phone);
                        cmd.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(client.Email) ? (object)DBNull.Value : client.Email);
                        cmd.Parameters.AddWithValue("@address", string.IsNullOrWhiteSpace(client.Address) ? (object)DBNull.Value : client.Address);
                        cmd.Parameters.AddWithValue("@status", client.Status);
                        cmd.Parameters.AddWithValue("@responsibleUserId", client.ResponsibleUserId.HasValue ? (object)client.ResponsibleUserId.Value : DBNull.Value);

                        await cmd.ExecuteNonQueryAsync();
                        return (true, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка при обновлении: {ex.Message}");
            }
        }

        public async Task<bool> DeleteClientAsync(long id)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("DELETE FROM clients WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    var affectedRows = await cmd.ExecuteNonQueryAsync();
                    return affectedRows > 0;
                }
            }
        }

        // ===== DEAL METHODS =====
        public async Task<List<Deal>> GetAllDealsAsync()
        {
            var deals = new List<Deal>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM deals ORDER BY id", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        deals.Add(new Deal
                        {
                            Id = reader.GetInt64(0),
                            DealName = reader.GetString(1),
                            ClientId = reader.GetInt64(2),
                            Amount = reader.GetDecimal(3),
                            Stage = reader.IsDBNull(4) ? "Переговоры" : reader.GetString(4),
                            ResponsibleUserId = reader.IsDBNull(5) ? (long?)null : reader.GetInt64(5),
                            CreatedDate = reader.GetDateTime(6),
                            DeadlineDate = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7)
                        });
                    }
                }
            }
            return deals;
        }

        public async Task<Deal> GetDealByIdAsync(long id)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM deals WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Deal
                            {
                                Id = reader.GetInt64(0),
                                DealName = reader.GetString(1),
                                ClientId = reader.GetInt64(2),
                                Amount = reader.GetDecimal(3),
                                Stage = reader.IsDBNull(4) ? "Переговоры" : reader.GetString(4),
                                ResponsibleUserId = reader.IsDBNull(5) ? (long?)null : reader.GetInt64(5),
                                CreatedDate = reader.GetDateTime(6),
                                DeadlineDate = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<(bool Success, string ErrorMessage)> AddDealAsync(Deal deal)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    using (var cmd = new NpgsqlCommand(
                        "INSERT INTO deals (deal_name, client_id, amount, stage, responsible_user_id, created_date, deadline_date) " +
                        "VALUES (@dealName, @clientId, @amount, @stage, @responsibleUserId, @createdDate, @deadlineDate)", conn))
                    {
                        cmd.Parameters.AddWithValue("@dealName", deal.DealName);
                        cmd.Parameters.AddWithValue("@clientId", deal.ClientId);
                        cmd.Parameters.AddWithValue("@amount", deal.Amount);
                        cmd.Parameters.AddWithValue("@stage", string.IsNullOrWhiteSpace(deal.Stage) ? "Переговоры" : deal.Stage);
                        cmd.Parameters.AddWithValue("@responsibleUserId", deal.ResponsibleUserId.HasValue ? (object)deal.ResponsibleUserId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@createdDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@deadlineDate", deal.DeadlineDate.HasValue ? (object)deal.DeadlineDate.Value : DBNull.Value);

                        await cmd.ExecuteNonQueryAsync();
                        return (true, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка при сохранении: {ex.Message}");
            }
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateDealAsync(Deal deal)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    using (var cmd = new NpgsqlCommand(
                        "UPDATE deals SET deal_name = @dealName, client_id = @clientId, amount = @amount, " +
                        "stage = @stage, responsible_user_id = @responsibleUserId, deadline_date = @deadlineDate " +
                        "WHERE id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", deal.Id);
                        cmd.Parameters.AddWithValue("@dealName", deal.DealName);
                        cmd.Parameters.AddWithValue("@clientId", deal.ClientId);
                        cmd.Parameters.AddWithValue("@amount", deal.Amount);
                        cmd.Parameters.AddWithValue("@stage", deal.Stage);
                        cmd.Parameters.AddWithValue("@responsibleUserId", deal.ResponsibleUserId.HasValue ? (object)deal.ResponsibleUserId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@deadlineDate", deal.DeadlineDate.HasValue ? (object)deal.DeadlineDate.Value : DBNull.Value);

                        await cmd.ExecuteNonQueryAsync();
                        return (true, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка при обновлении: {ex.Message}");
            }
        }

        public async Task<bool> DeleteDealAsync(long id)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("DELETE FROM deals WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    var affectedRows = await cmd.ExecuteNonQueryAsync();
                    return affectedRows > 0;
                }
            }
        }

        // ===== DROPDOWN METHODS =====
        public async Task<List<User>> GetUsersForDropdownAsync()
        {
            var users = new List<User>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT id, last_name, first_name FROM users ORDER BY last_name, first_name", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetInt64(0),
                            LastName = reader.GetString(1),
                            FirstName = reader.GetString(2)
                        });
                    }
                }
            }
            return users;
        }

        public async Task<List<Client>> GetClientsForDropdownAsync()
        {
            var clients = new List<Client>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT id, company_name FROM clients ORDER BY company_name", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        clients.Add(new Client
                        {
                            Id = reader.GetInt64(0),
                            CompanyName = reader.GetString(1)
                        });
                    }
                }
            }
            return clients;
        }

        // ===== STATISTICS FOR HOME PAGE =====
        public async Task<int> GetUsersCountAsync()
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM users", conn))
                {
                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        public async Task<int> GetClientsCountAsync()
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM clients", conn))
                {
                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        public async Task<int> GetDealsCountAsync()
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM deals", conn))
                {
                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        public async Task<decimal> GetTotalDealsAmountAsync()
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT COALESCE(SUM(amount), 0) FROM deals", conn))
                {
                    var result = await cmd.ExecuteScalarAsync();
                    return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
                }
            }
        }
    }
}