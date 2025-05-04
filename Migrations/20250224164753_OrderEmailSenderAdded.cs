using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStore.Migrations
{
    /// <inheritdoc />
    public partial class OrderEmailSenderAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key constraint only if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Orders_AspNetUsers_UserId')
                BEGIN
                    ALTER TABLE [Orders] DROP CONSTRAINT [FK_Orders_AspNetUsers_UserId];
                END
            ");

            // Drop the index only if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Orders_UserId' AND object_id = OBJECT_ID('Orders'))
                BEGIN
                    DROP INDEX [IX_Orders_UserId] ON [Orders];
                END
            ");

            // Drop the UserId column only if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE name = 'UserId' AND object_id = OBJECT_ID('Orders'))
                BEGIN
                    ALTER TABLE [Orders] DROP COLUMN [UserId];
                END
            ");

            // Rename the Status column to UserEmail only if Status exists and UserEmail does not exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE name = 'Status' AND object_id = OBJECT_ID('Orders'))
                AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'UserEmail' AND object_id = OBJECT_ID('Orders'))
                BEGIN
                    EXEC sp_rename 'Orders.Status', 'UserEmail', 'COLUMN';
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse the column rename only if UserEmail exists and Status does not exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE name = 'UserEmail' AND object_id = OBJECT_ID('Orders'))
                AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'Status' AND object_id = OBJECT_ID('Orders'))
                BEGIN
                    EXEC sp_rename 'Orders.UserEmail', 'Status', 'COLUMN';
                END
            ");

            // Add the UserId column back
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            // Recreate the index
            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            // Recreate the foreign key constraint
            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}