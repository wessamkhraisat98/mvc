using CafeteriaInventoryPurchase.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/Account/Login");
string database = builder.Configuration["CafeteriaDatabase"] == "DatabaseFirst"
    ? "CafeteriaDB"
    : "CafeteriaEmployeesDB";
builder.Services.AddDbContext<CafeteriaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(database)));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(error => error.Run(context =>
    {
        context.Response.StatusCode = 500;
        return context.Response.WriteAsync("An error occurred.");
    }));
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
