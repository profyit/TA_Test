using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskManagement.Middleware;
using TaskManagement.Middleware.TaskManagerApi.Middleware;
using TaskManagementBLLayer.Services;
using TaskManagementDBLayer;
using TaskManagementDBLayer.Entities.TaskManagerApi.Data;





var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

RegisterObjects(builder);
CreateDB(builder);
//SetJWTToken(builder);

void SetJWTToken(WebApplicationBuilder builder)
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var issuer = jwtSettings["Issuer"];
    var audience = jwtSettings["Audience"];
    var key = jwtSettings["Key"];
    var durationInMinutes = int.Parse(jwtSettings["DurationInMinutes"] ?? "60");

   
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ClockSkew = TimeSpan.Zero 
            };
        });
}



builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") 
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<TaskDbContext>(); 

       
        dbContext.Database.EnsureCreated();
        Console.WriteLine("Database checked/created successfully.");
    }
    catch (Exception ex)
    {
      
        var logger = services.GetRequiredService<ILogger<Program>>(); 
        logger.LogError(ex, "An error occurred creating the DB.");

    }
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("MyAllowSpecificOrigins");
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
//app.UseAuthentication();
//app.UseAuthorization();



app.Run();


void RegisterObjects(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<ICustomLogService, CustomLogService>();
    builder.Services.AddTransient<ITasksDBService, TasksDBService>();
    builder.Services.AddTransient<ITasksService, TasksService>();
   

}

app.UseMiddleware<DeveloperNameMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();



app.Run();

static void CreateDB(WebApplicationBuilder builder)
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
             ?? "Server=(localdb)\\mssqllocaldb;Database=TasksDB;Trusted_Connection=True;MultipleActiveResultSets=true";

    builder.Services.AddDbContext<TaskDbContext>(options =>
             options.UseSqlServer(connectionString));
}

