using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBoards.Migrations
{
    public partial class ViewTopAuthorsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            Create view View_TopAuthors as 
            Select top 5 FullName,us.Id, COUNT(AuthorId) as 'WorkItemsCreated'
            from Users us
            join WorkItems wi on us.Id = wi.AuthorId
            group by us.Id, FullName
            order by WorkItemsCreated desc
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            Drop view View_TopAuthors 
            ");
        }
    }
}
