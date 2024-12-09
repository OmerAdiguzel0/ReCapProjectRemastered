using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;

namespace DataAccess.Extensions
{
    public static class MigrationBuilderExtensions
    {
        public static bool ColumnExists(this MigrationBuilder migrationBuilder, string tableName, string columnName)
        {
            var sql = $@"
                SELECT COUNT(1)
                FROM information_schema.columns
                WHERE table_name = '{tableName}'
                AND column_name = '{columnName}'";

            using var connection = new NpgsqlConnection("Host=localhost;Port=5432;Database=rentacar;Username=oemiar;Password=1");
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            var result = (long)command.ExecuteScalar();
            return result > 0;
        }
    }
} 