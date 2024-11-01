using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StockMarketUI.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options => {
    options.Filters.Add<UserInfoActionFilter>();
});

builder.Services.AddScoped<UserInfoActionFilter>();

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpClient("StockMarketAPI", client => {
    client.BaseAddress = new Uri("http://localhost:5280");
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "http://localhost:5000",
        ValidAudience = "http://localhost:5000",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("WMDPFnnHcJK/7jjwW36YB0mQWoOJzG1ugA/r2FNdCYo="))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.HttpContext.Session.GetString("Token");
            
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            context.HttpContext.Response.Redirect("/auth/login");
            return Task.CompletedTask;
        },
        OnForbidden = context =>
        {
            context.HttpContext.Response.Redirect("/auth/login");
            return Task.CompletedTask;
        },
    };
});

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

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
