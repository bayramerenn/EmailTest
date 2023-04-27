using EmailTest;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<SmtpConfig>(builder.Configuration.GetSection(typeof(SmtpConfig).Name));

builder.Services.Configure<UserInterfaceConfig>(builder.Configuration.GetSection(typeof(UserInterfaceConfig).Name));

builder.Services.Configure<MailTemplateConfig>(builder.Configuration.GetSection(typeof(MailTemplateConfig).Name));

builder.Services.AddScoped<IEmailService, EmailService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
