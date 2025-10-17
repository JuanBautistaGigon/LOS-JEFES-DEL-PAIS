var builder = WebApplication.CreateBuilder(args);

// --- SECCIÓN 1: REGISTRO DE SERVICIOS ---

// 1. Definimos la política de CORS aquí, al registrar el servicio.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            // Para desarrollo, permitimos cualquier origen.
            // Para producción, deberías cambiarlo a: policy.WithOrigins("http://tu-dominio.com")
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// --- SECCIÓN 2: PIPELINE DE PETICIONES (MIDDLEWARE) ---

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2. Colocamos UseCors() en la posición correcta: ANTES de la autorización y los controladores.
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();