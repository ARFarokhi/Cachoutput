using Cachoutput;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(policy => policy
        .Expire(TimeSpan.FromSeconds(10)));

    options.AddPolicy("PeoplePolicy", policy => policy
        .Expire(TimeSpan.FromMinutes(10))
        .Tag("PeoplePolicy_Tag"));

    options.AddPolicy("ByIdCachePolicy", policy => policy
    .AddPolicy<ByIdCachePolicy>()
    .Expire(TimeSpan.FromMinutes(5)));
});

var app = builder.Build();
app.UseOutputCache();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
