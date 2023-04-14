
using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Repositories.Interface;
using CiPlatformWeb.Repositories.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews()
.AddNewtonsoftJson(options =>
options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailGeneration, EmailGeneration>();
builder.Services.AddScoped<IMissionList, MissionList>();
builder.Services.AddScoped<IMissionDetail, MissionDetail>();
builder.Services.AddScoped<IStoryList, StoryList>();
builder.Services.AddScoped<IUserProfile, UserProfile>();
builder.Services.AddScoped<IVolunteeringTimesheet, VolunteeringTimesheet>();
builder.Services.AddScoped<IAdminUser, AdminUser>();
builder.Services.AddScoped<IAdminCms, AdminCms>();
builder.Services.AddScoped<IAdminMission, AdminMission>();
builder.Services.AddScoped<IAdminTheme, AdminTheme>();
builder.Services.AddScoped<IAdminSkill, AdminSkill>();
builder.Services.AddScoped<IAdminApplication, AdminApplication>();
builder.Services.AddScoped<IAdminStory, AdminStory>();

builder.Services.AddSession();
builder.Services.AddMemoryCache();

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

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=AdminUser}/{id?}");

app.Run();
