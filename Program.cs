using Microsoft.Extensions.FileProviders;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// … boshqa service qo‘shimchalari …
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// wwwroot papkasini xizmatga qo‘yish (agar ishlatayotgan bo‘lsangiz)
app.UseDefaultFiles();
app.UseStaticFiles();

// UploadedImages papkasini ham statik qilib servis qilish:
var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "UploadedImages");
if (!Directory.Exists(uploadsPath))
    Directory.CreateDirectory(uploadsPath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/UploadedImages"
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
