using Amazon.S3;
using dotnet.dll.version.api.Sevices;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<S3BucketService>();
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.MapGet("/message/reference", (MessageService service, string key, string? version) =>
{
    return service.GetMessageByKeyAndVersionReference(key, version);
})
.WithName("Message");

app.MapGet("/message/directory", async (MessageService service, string key, string? version) =>
{
    return await service.GetMessageByKeyAndVersionS3ToDirectory(key, version);
})
.WithName("Directory");

app.Run();