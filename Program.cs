using GroupProject_Ecommerce.Data;
using GroupProject_Ecommerce.Models;
using GroupProject_Ecommerce.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webapi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MyDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddIdentity<User, IdentityRole>(opt =>
{
    opt.Password.RequiredUniqueChars = 1;
    opt.Password.RequireUppercase = true;
    opt.Password.RequiredLength = 6;
    opt.Password.RequireNonAlphanumeric = true;
    opt.Password.RequireLowercase = false;

}).AddEntityFrameworkStores<MyDbContext>();

//VnPay
builder.Services.AddSingleton<IVnPayService, VnPayService>();

builder.Services.AddTransient<ISendMailService, SendMailService>();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
