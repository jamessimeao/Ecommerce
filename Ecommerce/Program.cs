#define USE_SWAGGER

using Ecommerce.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

#if USE_SWAGGER
builder.Services.AddSwaggerGen();
#endif

// Add services to the container.
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if(connectionString == null)
{
    Console.WriteLine("Failed to read the connection string.");
    return;
}
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add a connection to the PostgreSQL database without Entity Framework.
// This will be used for Dapper.
builder.Services.AddTransient<IDbConnection>(
    (IServiceProvider serviceProvider) => new NpgsqlConnection(connectionString)
);

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

#if USE_SWAGGER
app.UseSwagger();
app.UseSwaggerUI();
app.MapSwagger();
#endif

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

#if USE_SWAGGER
// Not included in the MVC template.
// Map the Identity Api, so I can at work with the endpoints in swagger.
app.MapIdentityApi<IdentityUser>();
#endif

app.MapControllerRoute(
    name: "default",
    pattern: "")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
