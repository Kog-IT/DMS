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
using DMS.Brands;
using DMS.ProductGroups;
using DMS.Warehouses;
using DMS.CustomerGroups;
using DMS.Suppliers;
using DMS.Salesmen;
using DMS.Dispatches;
using DMS.Transfers;
using DMS.Media;
using DMS.SalesmanRequests;
using Microsoft.EntityFrameworkCore;
using DMS.Returns;

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
    public virtual DbSet<ProductVariant> ProductVariants { get; set; }
    public virtual DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Governorate> Governorates { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<ProductGroup> ProductGroups { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<WarehouseProduct> WarehouseProducts { get; set; }
    public DbSet<CustomerGroup> CustomerGroups { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Salesman> Salesmen { get; set; }
    public DbSet<SalesmanWarehouse> SalesmanWarehouses { get; set; }
    public DbSet<PlannedDispatch> PlannedDispatches { get; set; }
    public DbSet<ActualDispatch> ActualDispatches { get; set; }

    public DbSet<WarehouseToWarehouseTransfer> WarehouseToWarehouseTransfers { get; set; }
    public DbSet<WarehouseToWarehouseTransferItem> WarehouseToWarehouseTransferItems { get; set; }
    public DbSet<WarehouseToSalesmanTransfer> WarehouseToSalesmanTransfers { get; set; }
    public DbSet<WarehouseToSalesmanTransferItem> WarehouseToSalesmanTransferItems { get; set; }
    public DbSet<SalesmanToWarehouseTransfer> SalesmanToWarehouseTransfers { get; set; }
    public DbSet<SalesmanToWarehouseTransferItem> SalesmanToWarehouseTransferItems { get; set; }
    public DbSet<SalesmanToSalesmanTransfer> SalesmanToSalesmanTransfers { get; set; }
    public DbSet<SalesmanToSalesmanTransferItem> SalesmanToSalesmanTransferItems { get; set; }


    public DbSet<SalesmanRequest> SalesmanRequests { get; set; }
    public DbSet<SalesmanRequestItem> SalesmanRequestItems { get; set; }
    public DbSet<MediaFile> MediaFiles { get; set; }

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
    public virtual DbSet<Return> Returns { get; set; }
    public virtual DbSet<ReturnLine> ReturnLines { get; set; }
    public DbSet<ReturnPhoto> ReturnPhotos { get; set; }

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
        modelBuilder.ApplyConfiguration(new ProductVariantConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new BrandConfiguration());
        modelBuilder.ApplyConfiguration(new ProductGroupConfiguration());
        modelBuilder.ApplyConfiguration(new WarehouseConfiguration());
        modelBuilder.ApplyConfiguration(new WarehouseProductConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerGroupConfiguration());
        modelBuilder.ApplyConfiguration(new SupplierConfiguration());
        modelBuilder.ApplyConfiguration(new SalesmanConfiguration());
        modelBuilder.ApplyConfiguration(new SalesmanWarehouseConfiguration());
        modelBuilder.ApplyConfiguration(new PlannedDispatchConfiguration());
        modelBuilder.ApplyConfiguration(new ActualDispatchConfiguration());
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

        modelBuilder.ApplyConfiguration(new WarehouseToWarehouseTransferConfiguration());
        modelBuilder.ApplyConfiguration(new WarehouseToWarehouseTransferItemConfiguration());
        modelBuilder.ApplyConfiguration(new WarehouseToSalesmanTransferConfiguration());
        modelBuilder.ApplyConfiguration(new WarehouseToSalesmanTransferItemConfiguration());
        modelBuilder.ApplyConfiguration(new SalesmanToWarehouseTransferConfiguration());
        modelBuilder.ApplyConfiguration(new SalesmanToWarehouseTransferItemConfiguration());
        modelBuilder.ApplyConfiguration(new SalesmanToSalesmanTransferConfiguration());
        modelBuilder.ApplyConfiguration(new SalesmanToSalesmanTransferItemConfiguration());

        modelBuilder.ApplyConfiguration(new SalesmanRequestConfiguration());
        modelBuilder.ApplyConfiguration(new SalesmanRequestItemConfiguration());
        modelBuilder.ApplyConfiguration(new MediaFileConfiguration());
    }
}
