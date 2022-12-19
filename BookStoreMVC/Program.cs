using BookStoreMVC.DataAccess;
using BookStoreMVC.Models;
using BookStoreMVC.Services;
using BookStoreMVC.Services.Implementation;
using MongoDB.Bson;
using SeoTags;

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
builder.Services.AddScoped<IBookRepository, BookRepositoryService>();
builder.Services.AddScoped<IProductRepository, ProductRepositoryService>();
builder.Services.AddScoped<IOrderRepository, OrderService>();
builder.Services.AddScoped<IAuthorRepository, AuthorServices>();
builder.Services.AddScoped<IBookGenreRepository, BookGenreRepositoryService>();
builder.Services.AddScoped<IPublisherRepository, PublisherRepositoryService>();
builder.Services.AddScoped<IOrderRepository, OrderService>();
builder.Services.AddScoped<IBookTypeRepository, BookTypeService>();
builder.Services.AddScoped<ICountryRepository, CountryServices>();
builder.Services.AddScoped<ILanguageRepository, LanguageServices>();
builder.Services.AddScoped<IPaymentStrategy, PaymentStrategy>();
builder.Services.AddScoped<IPaymentService, VNPPayment>();
builder.Services.AddScoped<IPaymentService, MomoPayment>();
builder.Services.AddScoped<IHelpers, HelperService>();

builder.Services.AddSeoTags(seoInfo =>
   {
       seoInfo.SetSiteInfo(
           siteTitle: "Swiftlib, place you can buy anybook!",
           openSearchUrl: "https://site.com/open-search.xml",  //optional
           robots: "index, follow"
       //optional
       );
       seoInfo.SetCommonInfo(
                  pageTitle: "Home",
                  description: "Create all SEO tags you need such as meta, link, twitter card (twitter:), open graph (og:), and ...",
                  url: "https://aspnet-book-store-production-067c.up.railway.app/",
                  keywordTags: new[] { "Book", "Books", "Bookstore", "Switflib", "Ebook", "HardCover", "Paperback", });

       seoInfo.SetImageInfo(
             url: "https://opengraph.githubassets.com/faf6ca93025794067b4f4c0beeff874561fa5b581ac92bd7a6ca1ec7d7e14c42/definux/Seo",
             width: 1200,
             height: 600,
             alt: "Image alt",
             mimeType: "image/jpeg", //optional: detect from url file extension
             cardType: SeoTags.TwitterCardType.SummaryLargeImage);


       seoInfo.AddDnsPrefetch("https://www.google-analytics.com");


   });

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