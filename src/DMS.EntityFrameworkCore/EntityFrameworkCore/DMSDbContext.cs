using Abp.Zero.EntityFrameworkCore;
using DMS.Authorization.Roles;
using DMS.Authorization.Users;
using DMS.Companies;
using DMS.Customers;
using DMS.EntityFrameworkCore.Configurations;
using DMS.Routes;
using DMS.Visits;
using DMS.MultiTenancy;
using DMS.Invoices;
using DMS.Orders;
using DMS.Payments;
using DMS.Products;
using DMS.Categories;
using DMS.Governorates;
using DMS.Cities;
using DMS.PriceLists;
using Microsoft.EntityFrameworkCore;

namespace DMS.EntityFrameworkCore;

public class DMSDbContext : AbpZeroDbContext<Tenant, Role, User, DMSDbContext>
{
    public DbSet<Company> Companies { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerContact> CustomerContacts { get; set; }
    public DbSet<Route> Routes { get; set; }
    public DbSet<RouteItem> RouteItems { get; set; }
    public DbSet<Visit> Visits { get; set; }
    public DbSet<VisitPhoto> VisitPhotos { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Governorate> Governorates { get; set; }
    public DbSet<City> Cities { get; set; }


    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderLine> OrderLines { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceLine> InvoiceLines { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentLine> PaymentLines { get; set; }
    public DbSet<PriceList> PriceLists { get; set; }
    public DbSet<PriceListItem> PriceListItems { get; set; }
    public DbSet<PriceListAssignment> PriceListAssignments { get; set; }

    public DMSDbContext(DbContextOptions<DMSDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new CompanyConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerContactConfiguration());
        modelBuilder.ApplyConfiguration(new RouteConfiguration());
        modelBuilder.ApplyConfiguration(new RouteItemConfiguration());
        modelBuilder.ApplyConfiguration(new VisitConfiguration());
        modelBuilder.ApplyConfiguration(new VisitPhotoConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderLineConfiguration());
        modelBuilder.ApplyConfiguration(new InvoiceConfiguration());
        modelBuilder.ApplyConfiguration(new InvoiceLineConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentMethodConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentLineConfiguration());
        modelBuilder.ApplyConfiguration(new PriceListConfiguration());
        modelBuilder.ApplyConfiguration(new PriceListItemConfiguration());
        modelBuilder.ApplyConfiguration(new PriceListAssignmentConfiguration());
    }
}
