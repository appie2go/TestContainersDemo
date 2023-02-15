using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foo.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.Sql(@" -- EXEC SP_GetBooks @filter = 'test'
CREATE PROCEDURE SP_GetBooks
    @filter varchar(50)
    AS
    BEGIN
        SELECT Id, Title FROM Books WHERE Title LIKE '%' + @filter + '%';
    END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.Sql("DROP PROCEDURE SP_GetBooks");
        }
    }
}
