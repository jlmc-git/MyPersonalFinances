using FinanceCore.Application.Transactions;
using FinanceCore.Application.Transactions.Commands.CreateTransaction;
using FinanceCore.Infrastructure.Transactions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMediatR(configuration =>
    configuration.RegisterServicesFromAssembly(typeof(CreateTransactionCommand).Assembly));
builder.Services.AddSingleton<ITransactionRepository, InMemoryTransactionRepository>();

builder.Services.Configure<ClassificationRuleOptions>(
    builder.Configuration.GetSection(ClassificationRuleOptions.SectionName));

builder.Services.AddSingleton<ILlmClient, StubLlmClient>();
builder.Services.AddSingleton<RuleBasedTransactionClassifier>();
builder.Services.AddSingleton<LlmClassifier>();

builder.Services.AddSingleton<ITransactionClassifier>(sp =>
{
    string strategy = sp.GetRequiredService<IConfiguration>()
        .GetValue<string>("ClassifierStrategy") ?? "rules";

    return strategy switch
    {
        "llm" => sp.GetRequiredService<LlmClassifier>(),
        _ => sp.GetRequiredService<RuleBasedTransactionClassifier>()
    };
});

builder.Services.AddOpenApi();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
