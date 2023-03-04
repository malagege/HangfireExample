using Hangfire;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Hangfire services.
builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseInMemoryStorage());

 // Add the processing server as IHostedService
builder.Services.AddHangfireServer();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.UseHangfireDashboard();

// 假如有使用  UseEndpoints 就要用下面方法。
// app.UseEndpoints(endpoints =>
// {
//     endpoints.MapControllers();
//     endpoints.MapHangfireDashboard();
// });


// 簡單執行 Hangfire 程式
BackgroundJob.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));

app.MapControllers();

app.Run();
