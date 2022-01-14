using Sunduk.WebApi;
using System.Net.Mail;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("https://sunduk.one",
                                              "http://www.sunduk.one",
                                              "http://test.sunduk.one",
                                              "http://www.test.sunduk.one",
                                              "https://localhost:5001"
                                              );
                      });
});

// Add services to the container.

builder.Services.AddSingleton<SendMessageService>(new SendMessageService());
var feedbackFrom = builder.Configuration["FeedbackFrom"];
var feedbackTo = builder.Configuration["FeedbackTo"];
var feedbackPass = builder.Configuration["FeedbackPass"];

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);

app.MapGet("/sendmessage|{Name}|{Message}", (string Name, string Message) =>
{
    return SendMessageService.Send(Name, Message, feedbackFrom, feedbackTo, feedbackPass);
});

app.MapGet("/", () =>
{
    return "Ok";
});

app.Run();
