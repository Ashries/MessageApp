using MessageApp.Data;
using MessageApp.Interfaces;
using MessageApp.Services;
using MessageApp.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework (Using InMemory database for development)
builder.Services.AddDbContext<MessageContext>(options =>
    options.UseInMemoryDatabase("MessageAppDb"));

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMessageService, MessageService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add custom middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Initialize database with sample data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MessageContext>();
    context.Database.EnsureCreated();
    
    // Add sample data
    if (!context.Users.Any())
    {
        context.Users.Add(new MessageApp.Models.User 
        { 
            Username = "testuser", 
            Password = BCrypt.Net.BCrypt.HashPassword("password123"),
            FirstName = "Test",
            LastName = "User"
        });
        context.SaveChanges();
    }
}

app.Run();