using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDo.API.Migrations
{
    public partial class AddDemoUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO public.\"Users\" (\"Login\", \"Password\", \"CreatedDate\", \"LastLoginDate\") VALUES ('demo', '$2a$11$eWGTOW94QfnCDa33v77E3uLwbfK5Noy60zjrr4zUleJa3qHnVpkX6', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM public.\"Users\" WHERE \"Login\" = 'demo';");
        }
    }
}
