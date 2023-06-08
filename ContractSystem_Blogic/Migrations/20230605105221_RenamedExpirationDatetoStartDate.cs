using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractSystem_Blogic.Migrations
{
    /// <inheritdoc />
    public partial class RenamedExpirationDatetoStartDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpirationDate",
                table: "Contracts",
                newName: "StartDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Contracts",
                newName: "ExpirationDate");
        }
    }
}
