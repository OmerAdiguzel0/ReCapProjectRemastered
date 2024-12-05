using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class SeedCarData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Markalar
            migrationBuilder.InsertData(
                table: "brands",
                columns: new[] { "brand_id", "brand_name" },
                values: new object[,]
                {
                    { 1, "BMW" },
                    { 2, "Mercedes" },
                    { 3, "Audi" }
                });

            // Renkler
            migrationBuilder.InsertData(
                table: "colors",
                columns: new[] { "color_id", "color_name" },
                values: new object[,]
                {
                    { 1, "Siyah" },
                    { 2, "Beyaz" },
                    { 3, "Kırmızı" }
                });

            // Araçlar
            migrationBuilder.InsertData(
                table: "cars",
                columns: new[] { "car_id", "brand_id", "color_id", "daily_price", "model_year", "description", "min_findeks_score" },
                values: new object[,]
                {
                    { 1, 1, 1, 1000, 2022, "BMW 320i", 500 },
                    { 2, 2, 2, 1200, 2023, "Mercedes C200", 500 },
                    { 3, 3, 3, 1500, 2023, "Audi A4", 500 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "cars", keyColumn: "car_id", keyValue: 1);
            migrationBuilder.DeleteData(table: "cars", keyColumn: "car_id", keyValue: 2);
            migrationBuilder.DeleteData(table: "cars", keyColumn: "car_id", keyValue: 3);
            
            migrationBuilder.DeleteData(table: "brands", keyColumn: "brand_id", keyValue: 1);
            migrationBuilder.DeleteData(table: "brands", keyColumn: "brand_id", keyValue: 2);
            migrationBuilder.DeleteData(table: "brands", keyColumn: "brand_id", keyValue: 3);
            
            migrationBuilder.DeleteData(table: "colors", keyColumn: "color_id", keyValue: 1);
            migrationBuilder.DeleteData(table: "colors", keyColumn: "color_id", keyValue: 2);
            migrationBuilder.DeleteData(table: "colors", keyColumn: "color_id", keyValue: 3);
        }
    }
}
