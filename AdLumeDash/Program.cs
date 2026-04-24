using AdLumeDash.Interface;
using AdLumeDash.Models;
using AdLumeDash.Repository;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);


builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8666); // HTTP

    options.ListenAnyIP(8667, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped(_ => new SqlConnection(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IEquipamentoRepository, EquipamentoRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

Directory.CreateDirectory("storage");

app.UseHttpsRedirection();

app.MapControllers(); // <-- ESSENCIAL

//Midia xxxx = new Midia();
//xxxx.GeraRegistro(@"C:\Users\CBuosi\source\repos\AdLume\AdLumeDash\Storage\video_manha_1.mp4"); xxxx.Inserir();
//xxxx.GeraRegistro(@"C:\Users\CBuosi\source\repos\AdLume\AdLumeDash\Storage\video_manha_2.mp4"); xxxx.Inserir();
//xxxx.GeraRegistro(@"C:\Users\CBuosi\source\repos\AdLume\AdLumeDash\Storage\video_manha_3.mp4"); xxxx.Inserir();
//xxxx.GeraRegistro(@"C:\Users\CBuosi\source\repos\AdLume\AdLumeDash\Storage\video_noite_1.mp4"); xxxx.Inserir();
//xxxx.GeraRegistro(@"C:\Users\CBuosi\source\repos\AdLume\AdLumeDash\Storage\video_noite_2.mp4"); xxxx.Inserir();
//xxxx.GeraRegistro(@"C:\Users\CBuosi\source\repos\AdLume\AdLumeDash\Storage\video_promo_a.mp4"); xxxx.Inserir();
//xxxx.GeraRegistro(@"C:\Users\CBuosi\source\repos\AdLume\AdLumeDash\Storage\video_promo_b.mp4"); xxxx.Inserir();
//xxxx.GeraRegistro(@"C:\Users\CBuosi\source\repos\AdLume\AdLumeDash\Storage\video_tarde_1.mp4"); xxxx.Inserir();
//xxxx.GeraRegistro(@"C:\Users\CBuosi\source\repos\AdLume\AdLumeDash\Storage\video_tarde_2.mp4"); xxxx.Inserir();

app.Run();

