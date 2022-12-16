using BookStoreMVC.DataAccess;
using BookStoreMVC.Models;
using BookStoreMVC.Services;
using BookStoreMVC.Services.Implementation;
using MongoDB.Bson;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<BookStoreDataAccess>(
    builder.Configuration.GetSection("BookStoreDatabase"));
builder.Services.Configure<BookStoreCloudStorage>(
    builder.Configuration.GetSection("GoogleCloudStorage"));

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "SwiftLib";
    options.IdleTimeout = new TimeSpan(0, 30, 0);
});


// Singleton DI Resolver
builder.Services.AddSingleton<ICloudStorage, GoogleStorageServices>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Scoped DI Resolver
builder.Services.AddScoped<IBookRepository, BookServices>();
builder.Services.AddScoped<IAuthorRepository, AuthorServices>();
builder.Services.AddScoped<IBookGenreRepository, BookGenreServices>();
builder.Services.AddScoped<IPublisherRepository, PublisherService>();
builder.Services.AddScoped<IOrderRepository, OrderService>();
builder.Services.AddScoped<IBookTypeRepository, BookTypeService>();
builder.Services.AddScoped<ICountryRepository, CountryServices>();
builder.Services.AddScoped<ILanguageRepository, LanguageServices>();
builder.Services.AddScoped<IPaymentStrategy, PaymentStrategy>();
builder.Services.AddScoped<IPaymentService, VNPPayment>();
builder.Services.AddScoped<IPaymentService, MomoPayment>();
// builder.Services.AddTransient<Func<PaymentServiceEnum, IPaymentService>>(provider => key =>
// {
//     switch (key)
//     {
//         case PaymentServiceEnum.MomoPayment:
//             return provider.GetService<MomoPayment>()!;
//         case PaymentServiceEnum.VNPPayment:
//             return provider.GetService<VNPPayment>()!;
//         default:
//             return null;
//     }
// });
// builder.Services.AddScoped<IPaymentService[]>(_ => new []{builder.Services.AddScoped<PaymentService<>>()});
builder.Services.AddScoped<IHelpers, HelperService>();


// Authorization Policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireUserRole",
        policyBuilder => policyBuilder.RequireRole("User"));
    options.AddPolicy("RequireAdminRole",
        policyBuilder => policyBuilder.RequireRole("Admin"));
});


// Inject Httpclient
builder.Services.AddHttpClient("momo-payment", client => client.BaseAddress = new Uri("https://test-payment.momo.vn"));
builder.Services.AddHttpClient("vnp-payment", client => client.BaseAddress = new Uri("https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"));



builder.Services.AddIdentity<User, Role>(opts =>
    {
        opts.Password.RequiredLength = 8;
        opts.Password.RequiredUniqueChars = 1;
        opts.Password.RequireNonAlphanumeric = false;
    })
    .AddMongoDbStores<User, Role, ObjectId>(
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

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();