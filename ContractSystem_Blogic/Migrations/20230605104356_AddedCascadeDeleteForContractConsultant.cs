using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractSystem_Blogic.Migrations
{
    /// <inheritdoc />
    public partial class AddedCascadeDeleteForContractConsultant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractConsultant_Consultants_ConsultantId",
                table: "ContractConsultant");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractConsultant_Contracts_ContractId",
                table: "ContractConsultant");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractConsultant_Consultants_ConsultantId",
                table: "ContractConsultant",
                column: "ConsultantId",
                principalTable: "Consultants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractConsultant_Contracts_ContractId",
                table: "ContractConsultant",
                column: "ContractId",
                principalTable: "Contracts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractConsultant_Consultants_ConsultantId",
                table: "ContractConsultant");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractConsultant_Contracts_ContractId",
                table: "ContractConsultant");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractConsultant_Consultants_ConsultantId",
                table: "ContractConsultant",
                column: "ConsultantId",
                principalTable: "Consultants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractConsultant_Contracts_ContractId",
                table: "ContractConsultant",
                column: "ContractId",
                principalTable: "Contracts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
