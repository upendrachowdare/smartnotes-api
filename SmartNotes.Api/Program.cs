using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using SmartNotes.Api.Data;
using SmartNotes.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HttpClient-backed OpenAI service (reads OPENAI_API_KEY or OpenAI:ApiKey)
builder.Services.AddHttpClient<IOpenAiService, OpenAiService>();
// LinkedIn HTTP client uses named client to include default headers if needed
builder.Services.AddHttpClient<ILinkedInService, LinkedInService>();

// Background service to create and post daily LinkedIn posts
builder.Services.AddHostedService<DailyLinkedInPoster>();
// Job polling services
builder.Services.AddHostedService<JobPoller>();
builder.Services.AddSingleton<IJobSource, MockJobSource>();
builder.Services.AddHttpClient<GreenhouseJobSource>();
builder.Services.AddSingleton<IJobSource>(sp => sp.GetRequiredService<GreenhouseJobSource>());
builder.Services.AddSingleton<IJobMatcher, SimpleJobMatcher>();
// Persistence
builder.Services.AddDbContext<SmartNotes.Api.Data.SmartNotesContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=smartnotes.db"));

// Email sender (for approval notifications)
builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();

// RSS connector
builder.Services.AddSingleton<IRssJobSource, RssJobSource>();
builder.Services.AddSingleton<IJobSource>(sp => sp.GetRequiredService<IRssJobSource>());

// Use approval apply service that saves attempts and sends email
builder.Services.AddSingleton<IJobApplyService, ApprovalJobApplyService>();

// Configure poll interval in appsettings: JobPollIntervalMinutes (default 20)

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Serve static dashboard
app.UseDefaultFiles();
app.UseStaticFiles();

// Apply EF Core migrations / ensure database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SmartNotesContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
