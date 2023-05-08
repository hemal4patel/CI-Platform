
using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Repositories.Interface;
using CiPlatformWeb.Repositories.Repository;
using CiPlatformWeb.UserAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews()
.AddNewtonsoftJson(options =>
options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

builder.Services.AddScoped<CountryCityActionFilter>();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.AddService<CountryCityActionFilter>();
});

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
builder.Services.AddScoped<IAdminBanner, AdminBanner>();
builder.Services.AddScoped<IAdminComment, AdminComment>();
builder.Services.AddScoped<IAdminTimesheet, AdminTimesheet>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSetting:Issuer"],
        ValidAudience = builder.Configuration["JwtSetting:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSetting:Key"]))
    };
});

builder.Services.AddMvc().AddSessionStateTempDataProvider();
builder.Services.AddSession();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseSession();
app.Use(async (context, next) =>
{
    var token = context.Session.GetString("Token");
    if (!string.IsNullOrWhiteSpace(token))
    {
        context.Request.Headers.Add("Authorization", "Bearer " + token);
    }
    context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
    context.Response.Headers.Add("Expires", "-1");
    context.Response.Headers.Add("Pragma", "no-cache");
    await next();
});

app.UseStatusCodePages(async context =>
{
    var request = context.HttpContext.Request;
    var response = context.HttpContext.Response;

    if (response.StatusCode == (int) HttpStatusCode.Unauthorized || response.StatusCode == (int) HttpStatusCode.Forbidden)
    {
        response.Redirect("/");
        await response.CompleteAsync();
    }
    else if (response.StatusCode == (int) HttpStatusCode.NotFound)
    {
        response.Redirect("/Mission/PageNotFound");
        await response.CompleteAsync();
    }
});


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
