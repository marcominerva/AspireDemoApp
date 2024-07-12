using Microsoft.EntityFrameworkCore;

namespace AspireDemoApp.ApiService.DataAccessLayer;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Person> People { get; set; }
}

public class Person
{
    public Guid Id { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string City { get; set; }
}