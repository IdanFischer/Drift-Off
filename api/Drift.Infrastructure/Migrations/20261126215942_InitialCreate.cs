using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drift.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    display_name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    time_zone = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sleep_goals",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_hours = table.Column<decimal>(type: "numeric(3,1)", precision: 3, scale: 1, nullable: false),
                    target_bedtime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    target_wake_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    bedtime_window_minutes = table.Column<int>(type: "integer", nullable: false),
                    effective_from = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sleep_goals", x => x.id);
                    table.CheckConstraint("ck_sleep_goals_target_hours", "target_hours BETWEEN 4 AND 12");
                    table.ForeignKey(
                        name: "fk_sleep_goals_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sleep_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    night_of = table.Column<DateOnly>(type: "date", nullable: false),
                    slept_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    woke_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sleep_logs", x => x.id);
                    table.CheckConstraint("ck_sleep_logs_woke_after_slept", "woke_at > slept_at");
                    table.ForeignKey(
                        name: "fk_sleep_logs_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "created_at", "display_name", "email", "time_zone" },
                values: new object[] { new Guid("6f1b2c3d-0000-4000-8000-000000000001"), new DateTimeOffset(new DateTime(2026, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Demo", "demo@drift.local", "Asia/Jerusalem" });

            migrationBuilder.InsertData(
                table: "sleep_goals",
                columns: new[] { "id", "bedtime_window_minutes", "effective_from", "target_bedtime", "target_hours", "target_wake_time", "user_id" },
                values: new object[] { new Guid("6f1b2c3d-0000-4000-8000-000000000002"), 30, new DateOnly(2026, 11, 1), new TimeOnly(23, 0, 0), 7.0m, new TimeOnly(6, 30, 0), new Guid("6f1b2c3d-0000-4000-8000-000000000001") });

            migrationBuilder.CreateIndex(
                name: "ix_sleep_goals_user_id_effective_from",
                table: "sleep_goals",
                columns: new[] { "user_id", "effective_from" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sleep_logs_user_id_night_of",
                table: "sleep_logs",
                columns: new[] { "user_id", "night_of" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sleep_goals");

            migrationBuilder.DropTable(
                name: "sleep_logs");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
