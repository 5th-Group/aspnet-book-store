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

       );

       seoInfo.SetCommonInfo(
                  pageTitle: "Home",
                  description: "Create all SEO tags you need such as meta, link, twitter card (twitter:), open graph (og:), and ...Create all SEO tags you need such as meta, Create all SEO tags you need such as meta, link, twitter card (twitter:), open graph (og:), and ...link, twitter card (twitter:), open graph (og:), and ...Create all SEO tags you need such as meta, link, twitter card (twitter:), open graph (og:), and ...Create all SEO tags you need such as meta, Create all SEO tags you need such as meta, link, twitter card (twitter:), open graph (og:), and ...link, twitter card (twitter:), open graph (og:), and ...",
                  url: "swiftlib.site",
                  keywordTags: new[] { "Book", "Books", "Bookstore", "Switflib", "Ebook", "HardCover", "Paperback", });

       seoInfo.SetImageInfo(
             url: "https://site.com/uploads/image.jpg",
             width: 1200,
             height: 600,
             alt: "Image alt",
             mimeType: "image/jpeg", //optional: detect from url file extension
             cardType: SeoTags.TwitterCardType.SummaryLargeImage);


       seoInfo.AddDnsPrefetch("https://www.google-analytics.com");


       var organization = new OrganizationInfo
       {
           Url = "https://swiftlib.site/",
           Name = "Swiftlib",
           AlternateName = "Swiftlib Boostore",
           SocialMediaUrls = new[]
                          {
           "https://www.linkedin.com/company/MyCompanyId/"
           },
           Logo = new ImageInfo()
           {
               Url = "https://swiftlib.site/img/BoldDime.png",
               Caption = "Image Caption",
               InLanguage = "en-US",
               Width = 400,
               Height = 400
           },
           ContactPoints = new[]
                          {
           new ContactPointInfo()
           {
           Telephone = "+0123456789",
           ContactType = "sales",
           AvailableLanguage = new[] { "English" },
           AreaServed = new[] { "US" }
           },
           new ContactPointInfo()
           {
           Telephone = "+0123456789",
           ContactType = "customer service",
           AvailableLanguage = new[] { "English" },
           AreaServed = new[] { "US" }
           }
           }
       };

       var website = new WebSiteInfo
       {
           Url = "https://swiftlib.site/",
           Name = "Swiftlib",
           //    AlternateName = "WebSite AlternateName",
           Description = "lorem",
           InLanguage = "en-US",
           Publisher = organization.Id, //or OrganizationInfo.ReferTo(organization.Id) or OrganizationInfo.ReferTo(organization)
                                        //    SearchAction = new()
                                        //    {
                                        //        Target = "https://site.com/?s={search_term_string}",
                                        //        QueryInput = "required name=search_term_string"
                                        //    }
       };

       //        var breadcrumb = new BreadcrumbInfo
       //        {
       //            Url = "https://site.com/posts/post-url",
       //            Items = new[]
       //        {
       // ("https://site.com/", "Home"),
       // ("https://site.com/posts/", "Post List"),
       // ("https://site.com/posts/post-url", "Current Post"),
       // }
       //        };

       var image = new ImageInfo()
       {
           Url = "https://swiftlib.site/img/BoldDime.png",
           Caption = "Image Name",
           InLanguage = "en-US",
           Width = 1280,
           Height = 720
       };

       //        var author = new PersonInfo
       //        {
       //            Url = "https://site.com/author",
       //            Name = "Author Name",
       //            Description = "Author Description",
       //            SocialMediaUrls = new[]
       //        {
       // "https://twitter.com/AuthorId",
       // "https://www.linkedin.com/company/AuthorId/"
       // },
       //            Image = new()
       //            {
       //                Url = "https://site.com/uploads/author-image.jpg",
       //                Caption = "Author Name",
       //                InLanguage = "en-US",
       //                Width = 400,
       //                Height = 400
       //            }
       //        };

       var webpage = new SeoTags.PageInfo
       {
           Url = "https://swiftlib.site/",
           Title = "Swiftlib",
           Description = "Page Description",
           Keywords = new[] { "Swift", "Swiftlib", "Book", "Books", "book", "books", "Bookstore", "bookstore", "ebook", "Ebook", },
           Images = new[] { ImageInfo.ReferTo(image) },
           DatePublished = DateTimeOffset.Now,
           DateModified = DateTimeOffset.Now,
           InLanguage = "en-US",
           //    Author = author.Id, //or PersonInfo.ReferTo(author.Id) or PersonInfo.ReferTo(author)
           WebSite = website.Id, //or WebSiteInfo.ReferTo(website.Id) or WebSiteInfo.ReferTo(website)
                                 //    Breadcrumb = breadcrumb.Id //or BreadcrumbInfo.ReferTo(breadcrumb.Id) or BreadcrumbInfo.ReferTo(breadcrumb)
       };



       seoInfo.JsonLd.AddOrganization(organization);
       seoInfo.JsonLd.AddWebiste(website);
       //    seoInfo.JsonLd.AddBreadcrumb(breadcrumb);
       seoInfo.JsonLd.AddImage(image);
       //    seoInfo.JsonLd.AddPerson(author);
       seoInfo.JsonLd.AddPage(webpage);


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