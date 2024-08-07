using Jay_Mail_Mvc.Dto;
using Jay_Mail_Mvc.IRepository;
using Jay_Mail_Mvc.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//configure the EmailsettingsDto to inherint from the app settings emailsettings field
builder.Services.Configure<EmailSettingsDto>(builder.Configuration.GetSection("EmailSettings"));

//adding Email service as Dipendency Injection transient
builder.Services.AddTransient<IEmailService, EmailService>();

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
