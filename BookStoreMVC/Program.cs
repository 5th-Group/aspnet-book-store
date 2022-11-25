using BookStoreMVC.DataAccess;
using BookStoreMVC.Models;
using BookStoreMVC.Services;
using BookStoreMVC.Services.Implementation;
using MongoDbGenericRepository;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<BookStoreDataAccess>(
    builder.Configuration.GetSection("BookStoreDatabase"));
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IBookRepository, BookServices>();
builder.Services.AddScoped<IAuthorRepository, AuthorServices>();
builder.Services.AddScoped<IBookGenreRepository, BookGenreServices>();
builder.Services.AddScoped<IPublisherRepository, PublisherService>();
builder.Services.AddScoped<IOrderRepository, OrderService>();
builder.Services.AddScoped<IBookTypeRepository, BookTypeService>();
builder.Services.AddScoped<ICountryRepository, CountryServices>();
builder.Services.AddScoped<ILanguageRepository, LanguageServices>();

builder.Services.AddIdentity<User, Role>()
    .AddMongoDbStores<User, Role, Guid>(
        builder.Configuration.GetValue<string>("BookStoreDatabase:ConnectionString"),
        builder.Configuration.GetValue<string>("BookStoreDatabase:DatabaseName"));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();