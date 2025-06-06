using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/SignIn"; 
        options.AccessDeniedPath = "/Account/SignInAdmin"; 

        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        options.SlidingExpiration = true;

        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                var requestedPath = context.Request.Path;

                if (requestedPath.StartsWithSegments("/Admin"))
                {
                    context.Response.Redirect("/Account/SignInAdmin");
                }
                else if (requestedPath.StartsWithSegments("/User"))
                {
                    context.Response.Redirect("/Account/SignIn");
                }
                else
                {
                    context.Response.Redirect(options.LoginPath);
                }

                return Task.CompletedTask;
            }
        };
    });


var app = builder.Build();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Anon}/{action=Index}/{id?}");

app.Run();
