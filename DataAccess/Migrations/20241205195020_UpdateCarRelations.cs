using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class UpdateCarRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cars_brands_BrandId",
                table: "cars");

            migrationBuilder.DropForeignKey(
                name: "FK_cars_colors_ColorId",
                table: "cars");

            migrationBuilder.DropForeignKey(
                name: "FK_customers_users_UserId",
                table: "customers");

            migrationBuilder.DropForeignKey(
                name: "FK_rentals_cars_CarId",
                table: "rentals");

            migrationBuilder.DropForeignKey(
                name: "FK_rentals_customers_CustomerId",
                table: "rentals");

            migrationBuilder.DropForeignKey(
                name: "FK_user_operation_claims_operation_claims_OperationClaimId",
                table: "user_operation_claims");

            migrationBuilder.DropForeignKey(
                name: "FK_user_operation_claims_users_UserId",
                table: "user_operation_claims");

            migrationBuilder.AddForeignKey(
                name: "FK_cars_brands_BrandId",
                table: "cars",
                column: "BrandId",
                principalTable: "brands",
                principalColumn: "BrandId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_cars_colors_ColorId",
                table: "cars",
                column: "ColorId",
                principalTable: "colors",
                principalColumn: "ColorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_customers_users_UserId",
                table: "customers",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_rentals_cars_CarId",
                table: "rentals",
                column: "CarId",
                principalTable: "cars",
                principalColumn: "CarId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_rentals_customers_CustomerId",
                table: "rentals",
                column: "CustomerId",
                principalTable: "customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_user_operation_claims_operation_claims_OperationClaimId",
                table: "user_operation_claims",
                column: "OperationClaimId",
                principalTable: "operation_claims",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_user_operation_claims_users_UserId",
                table: "user_operation_claims",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cars_brands_BrandId",
                table: "cars");

            migrationBuilder.DropForeignKey(
                name: "FK_cars_colors_ColorId",
                table: "cars");

            migrationBuilder.DropForeignKey(
                name: "FK_customers_users_UserId",
                table: "customers");

            migrationBuilder.DropForeignKey(
                name: "FK_rentals_cars_CarId",
                table: "rentals");

            migrationBuilder.DropForeignKey(
                name: "FK_rentals_customers_CustomerId",
                table: "rentals");

            migrationBuilder.DropForeignKey(
                name: "FK_user_operation_claims_operation_claims_OperationClaimId",
                table: "user_operation_claims");

            migrationBuilder.DropForeignKey(
                name: "FK_user_operation_claims_users_UserId",
                table: "user_operation_claims");

            migrationBuilder.AddForeignKey(
                name: "FK_cars_brands_BrandId",
                table: "cars",
                column: "BrandId",
                principalTable: "brands",
                principalColumn: "BrandId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_cars_colors_ColorId",
                table: "cars",
                column: "ColorId",
                principalTable: "colors",
                principalColumn: "ColorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_customers_users_UserId",
                table: "customers",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_rentals_cars_CarId",
                table: "rentals",
                column: "CarId",
                principalTable: "cars",
                principalColumn: "CarId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_rentals_customers_CustomerId",
                table: "rentals",
                column: "CustomerId",
                principalTable: "customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_operation_claims_operation_claims_OperationClaimId",
                table: "user_operation_claims",
                column: "OperationClaimId",
                principalTable: "operation_claims",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_operation_claims_users_UserId",
                table: "user_operation_claims",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
