using Application;
using Application.Interfaces;
using Infrastructure;
using Infrastructure.AI;
using Infrastructure.Pdf;
using Infrastructure.Qdrant_VectorDB;
using Microsoft.EntityFrameworkCore;
using OpenAI.Embeddings;
using Qdrant.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

using (var scope = app.Services.CreateScope()) 
{
    var vectorStore = scope.ServiceProvider.GetRequiredService<IVectorStore>();

    await vectorStore.InitializeAsync();
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();

