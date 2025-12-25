using YemekSepeti.Data;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddSession();

builder.Services.AddScoped<YemekSepeti.Data.RestaurantRepository>();
builder.Services.AddScoped<YemekSepeti.Data.UserRepository>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<MenuItemRepository>();


builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Home/Login"; 
    });

builder.Services.AddAuthorization();
builder.Services.AddSession();


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();   // 🔴 REQUIRED
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=index}/{id?}");

app.Run();
