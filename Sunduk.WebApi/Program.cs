using Sunduk.WebApi;
using System.Net.Mail;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<SendMessageService>(new SendMessageService());
var feedbackFrom = builder.Configuration["FeedbackFrom"];
var feedbackTo = builder.Configuration["FeedbackTo"];
var feedbackPass = builder.Configuration["FeedbackPass"];

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();


app.MapGet("/sendmessage|{Name}|{Message}", (string Name, string Message) =>
{
    SendMessageService.Send(Name, Message, feedbackFrom, feedbackTo, feedbackPass);
});

app.MapGet("/", () =>
{
    return "Ok";
});

app.Run();
