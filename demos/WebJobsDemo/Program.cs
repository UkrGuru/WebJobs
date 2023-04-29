using System.Reflection;
using UkrGuru.Extensions;
using UkrGuru.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var initDb = builder.Configuration.GetValue<bool>("WJbSettings:InitDb");

builder.Services.AddWebJobs(builder.Configuration.GetConnectionString("DefaultConnection"),
    logLevel: builder.Configuration.GetValue<DbLogLevel>("Logging:LogLevel:UkrGuru.SqlJson"),
    nThreads: builder.Configuration.GetValue<int>("WJbSettings:NThreads"),
    initDb: initDb);

var app = builder.Build();

if (initDb) Assembly.GetExecutingAssembly().InitDb();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
