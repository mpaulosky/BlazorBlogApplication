using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web.Migrations
{
    /// <inheritdoc />
    public partial class ArticleEntityNavigationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_ApplicationUserDto_AuthorId",
                schema: "identity",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_Articles_CategoryDto_CategoryId",
                schema: "identity",
                table: "Articles");

            migrationBuilder.DropTable(
                name: "ApplicationUserDto",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "CategoryDto",
                schema: "identity");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_AspNetUsers_AuthorId",
                schema: "identity",
                table: "Articles",
                column: "AuthorId",
                principalSchema: "identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Categories_CategoryId",
                schema: "identity",
                table: "Articles",
                column: "CategoryId",
                principalSchema: "identity",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_AspNetUsers_AuthorId",
                schema: "identity",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Categories_CategoryId",
                schema: "identity",
                table: "Articles");

            migrationBuilder.CreateTable(
                name: "ApplicationUserDto",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserDto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryDto",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Archived = table.Column<bool>(type: "boolean", nullable: false),
                    CategoryName = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryDto", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_ApplicationUserDto_AuthorId",
                schema: "identity",
                table: "Articles",
                column: "AuthorId",
                principalSchema: "identity",
                principalTable: "ApplicationUserDto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_CategoryDto_CategoryId",
                schema: "identity",
                table: "Articles",
                column: "CategoryId",
                principalSchema: "identity",
                principalTable: "CategoryDto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
