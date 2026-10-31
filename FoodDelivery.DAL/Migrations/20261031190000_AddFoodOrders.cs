using FoodDelivery.DAL.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDelivery.DAL.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261031190000_AddFoodOrders")]
public partial class AddFoodOrders : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "FoodOrders",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<string>(type: "text", nullable: true),
                RestaurantId = table.Column<int>(type: "integer", nullable: false),
                RestaurantName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                RecipientName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                RecipientEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                RecipientPhone = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                DeliveryAddress = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FoodOrders", x => x.Id);
                table.ForeignKey(
                    name: "FK_FoodOrders_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_FoodOrders_Restaurants_RestaurantId",
                    column: x => x.RestaurantId,
                    principalTable: "Restaurants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "FoodOrderItems",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                FoodOrderId = table.Column<int>(type: "integer", nullable: false),
                DishName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                Quantity = table.Column<int>(type: "integer", nullable: false),
                LineTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FoodOrderItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_FoodOrderItems_FoodOrders_FoodOrderId",
                    column: x => x.FoodOrderId,
                    principalTable: "FoodOrders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(name: "IX_FoodOrderItems_FoodOrderId", table: "FoodOrderItems", column: "FoodOrderId");
        migrationBuilder.CreateIndex(name: "IX_FoodOrders_CreatedAtUtc", table: "FoodOrders", column: "CreatedAtUtc");
        migrationBuilder.CreateIndex(name: "IX_FoodOrders_RestaurantId", table: "FoodOrders", column: "RestaurantId");
        migrationBuilder.CreateIndex(name: "IX_FoodOrders_UserId", table: "FoodOrders", column: "UserId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "FoodOrderItems");
        migrationBuilder.DropTable(name: "FoodOrders");
    }
}
