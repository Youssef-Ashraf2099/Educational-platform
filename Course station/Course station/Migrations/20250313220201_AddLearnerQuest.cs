using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Course_station.Migrations
{
    /// <inheritdoc />
    public partial class AddLearnerQuest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.CreateTable(
                name: "LearnerQuest",
                columns: table => new
                {
                    LearnerId = table.Column<int>(type: "int", nullable: false),
                    QuestId = table.Column<int>(type: "int", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearnerQuest", x => new { x.LearnerId, x.QuestId });
                    table.ForeignKey(
                        name: "FK_LearnerQuest_Learner",
                        column: x => x.LearnerId,
                        principalTable: "Learner",
                        principalColumn: "Learner_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LearnerQuest_Quest",
                        column: x => x.QuestId,
                        principalTable: "Quest",
                        principalColumn: "QuestID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LearnerQuest_QuestId",
                table: "LearnerQuest",
                column: "QuestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LearnerQuest");

        }
    }
}
