using EmailClient.Common.DataContext.SqlServer;
using EmailClient.EmailService;
using EmailClient.WebApi.DAL;
using Microsoft.AspNetCore.Http.Features;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

////Extension method in order to use port 5003 for https (localhost) while MVC app running on 5001
builder.WebHost.UseUrls("https://localhost:5003/");

builder.Services.AddEmailClientContext();

builder.Services.AddControllers();

//adding email smtp config
var emailConfig = builder.Configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    { Title = "EmailClient Email Service API", Version = "v1" });
});

//configure the form options
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
    options.MemoryBufferThreshold = int.MaxValue;
});

//add the repo 
builder.Services.AddScoped<EmailRepo>();

//add custom email sender service 
builder.Services.AddScoped<IEmailSender, EmailSender>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json",
            "EmailClient Email Service API Version 1");

        //not allow Put methods, so Email records can't be edited
        c.SupportedSubmitMethods(new[]
        {
            SubmitMethod.Get, SubmitMethod.Post,
            SubmitMethod.Delete
        });
    });
}

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
