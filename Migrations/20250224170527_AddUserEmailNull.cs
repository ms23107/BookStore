using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStore.Migrations
{
    /// <inheritdoc />
    public partial class AddUserEmailNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add the UserEmail column if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'UserEmail' AND object_id = OBJECT_ID('Orders'))
                BEGIN
                    ALTER TABLE [Orders] ADD [UserEmail] NVARCHAR(MAX) NULL;
                END
            ");

            // Modify the UserEmail column to be nullable
            migrationBuilder.AlterColumn<string>(
                name: "UserEmail",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Modify the UserEmail column to be non-nullable
            migrationBuilder.AlterColumn<string>(
                name: "UserEmail",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            // Drop the UserEmail column if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE name = 'UserEmail' AND object_id = OBJECT_ID('Orders'))
                BEGIN
                    ALTER TABLE [Orders] DROP COLUMN [UserEmail];
                END
            ");
        }
    }
}