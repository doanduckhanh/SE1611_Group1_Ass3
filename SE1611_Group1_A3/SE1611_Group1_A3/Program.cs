using SE1611_Group1_A3.FileUploadService;
using SE1611_Group1_A3.Middleware;
using SE1611_Group1_A3.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<IFileUploadService, LocalFileUploadService>();
builder.Services.AddDbContext<MusicStoreContext>();
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseMiddleware<ViewDataMiddleware>();
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.   
    app.UseHsts();
    app.UseMvc();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.UseSession();
app.Use(async (context, next) =>
{
    await new ViewDataMiddleware(next).InvokeAsync(context);
});

app.Run();
