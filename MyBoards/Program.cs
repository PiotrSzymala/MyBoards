using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using MyBoards.Dto;
using MyBoards.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddDbContext<MyBoardsContext>(
    option => option
        //.UseLazyLoadingProxies()
        .UseSqlServer(builder.Configuration.GetConnectionString("MyBoardsConnectionString"))
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

app.MapGet("pagination", async (MyBoardsContext db) =>
{
    // user input
    var filter = "a";
    string sortBy = "FullName"; // "FullName" "Email" null
    bool sortByDescending = false;
    int pageNumber = 1;
    int pageSize = 10;
    //

    var query = db.Users
        .Where(u => filter == null || (u.Email.ToLower().Contains(filter.ToLower()) ||
                                       u.FullName.ToLower().Contains(filter.ToLower())));

    var totalCount = query.Count();

    if (sortBy != null)
    {
        var columnsSelector = new Dictionary<string, Expression<Func<User, object>>>
        {
            { nameof(User.Email), user => user.Email },
            { nameof(User.FullName), user => user.FullName }
        };

        var sortByExpression = columnsSelector[sortBy];
        query = sortByDescending
            ? query.OrderByDescending(sortByExpression)
            : query.OrderBy(sortByExpression);
    }

    var result = query.Skip(pageSize * (pageNumber - 1))
        .Take(pageSize)
        .ToList();

    var pagedResult = new PagedResult<User>(result, totalCount, pageSize, pageNumber);

    return pagedResult;
});

app.MapGet("data", async (MyBoardsContext db) =>
{
    var withAddress = true;

    var user = db.Users
        .First(u => u.Id == Guid.Parse("EBFBD70D-AC83-4D08-CBC6-08DA10AB0E61"));

    if (withAddress)
    {
        var result = new { FullName = user.FullName, Address = $"{user.Address.Street} {user.Address.City}" };
        return result;
    }

    return new { FullName = user.FullName, Address = "-" };
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

app.MapDelete("delete", async (MyBoardsContext db) =>
{
    var user = await db.Users
        .FirstAsync(u => u.Id == Guid.Parse("DC231ACF-AD3C-445D-CC08-08DA10AB0E61"));

    var userComments = db.Comments.Where(u => u.AuthorId == user.Id).ToList();
    db.RemoveRange(userComments);
    await db.SaveChangesAsync();

    db.Users.Remove(user);

    await db.SaveChangesAsync();
});
app.Run();