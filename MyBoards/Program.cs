using Microsoft.EntityFrameworkCore;
using MyBoards.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MyBoardsContext>(
    option => option.UseSqlServer(builder.Configuration.GetConnectionString("MyBoardsConnectionString"))
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetService<MyBoardsContext>();

var pendingMigrations = dbContext.Database.GetPendingMigrations();
if (pendingMigrations.Any())
{
    dbContext.Database.Migrate();
}

var users = dbContext.Users.ToList();
if (!users.Any())
{
    var user1 = new User()
    {
        Email = "testing@mail.com",
        FullName = "First User",
        Address = new Address()
        {
            City = "Katowice",
            Street = "Mikolowska"
        }
    };

    var user2 = new User()
    {
        Email = "secondtest@mail.com",
        FullName = "Second User",
        Address = new Address()
        {
            City = "Wroclaw",
            Street = "Malopanewska"
        }
    };

    dbContext.Users.AddRange(user1, user2);
    dbContext.SaveChanges();
}

app.MapGet("data",  async (MyBoardsContext db) =>
{
    var countedComments = await db
        .Comments.GroupBy(x => x.AuthorId)
        .Select(c => new { c.Key, Count = c.Count() })
        .ToListAsync();

    var userWithMostComments = countedComments
        .OrderByDescending(x => x.Count)
        .First();

    var moreInfoAboutUser = db
        .Users
        .Where(x => x.Id == userWithMostComments.Key);


    return new { moreInfoAboutUser, commentCount = userWithMostComments.Count };

});

app.MapPost("update", async (MyBoardsContext db) =>
{
    Epic epic = await db.Epics.FirstAsync(epic => epic.Id == 1);

    var rejectedState = await db.WorkItemStates.FirstAsync(a => a.Value == "Rejected");

    epic.State = rejectedState;

    await db.SaveChangesAsync();

    return epic;
});

app.MapPost("create", async (MyBoardsContext db) =>
{
    var address = new Address()
    {
        Id = Guid.Parse("b323dd7d-776a-4cf6-a92a-12df154b4a2c"),
        City = "Katowice",
        Country = "Poland",
        Street = "Sciegiennego"
    };

    var user = new User()
    {
        Email = "test.user@test.com",
        FullName = "User Test",
        Address = address
    };

    db.Users.Add(user);
    await db.SaveChangesAsync();
    
    return user;
});
app.Run();