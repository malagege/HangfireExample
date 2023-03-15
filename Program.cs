using Hangfire;
using Hangfire.SqlServer;

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
        .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
        {
//            SchemaName = "Test",  //假如有多個專案使用同一個 Database的話，可以用這個控制
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        }));

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

// 立即執行
var jobId = BackgroundJob.Enqueue(
    () => Console.WriteLine("Fire-and-forget!"));

// 重複性工作
RecurringJob.AddOrUpdate(
    "myrecurringjob",
    () => Console.WriteLine("Recurring!"),
    Cron.Daily);

// 延遲執行
var jobId3 = BackgroundJob.Schedule(
    () => Console.WriteLine("Delayed!"),
    TimeSpan.FromSeconds(30));

// 某排程執行後執行
BackgroundJob.ContinueJobWith(
    jobId,
    () => Console.WriteLine("Continuation!"));



app.Run();
