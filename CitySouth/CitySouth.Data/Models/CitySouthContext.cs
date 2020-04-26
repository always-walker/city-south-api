using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using CitySouth.Data.Models.Mapping;

namespace CitySouth.Data.Models
{
    public partial class CitySouthContext : DbContext
    {
        static CitySouthContext()
        {
            Database.SetInitializer<CitySouthContext>(null);
        }

        public CitySouthContext()
            : base("Name=CitySouthContext")
        {
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<CostConfig> CostConfigs { get; set; }
        public DbSet<ElseCost> ElseCosts { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeePostHistory> EmployeePostHistories { get; set; }
        public DbSet<Estate> Estates { get; set; }
        public DbSet<Good> Goods { get; set; }
        public DbSet<GoodsCategory> GoodsCategories { get; set; }
        public DbSet<GoodsOut> GoodsOuts { get; set; }
        public DbSet<GoodsOutDetail> GoodsOutDetails { get; set; }
        public DbSet<GoodsReceipt> GoodsReceipts { get; set; }
        public DbSet<GoodsReceiptDetail> GoodsReceiptDetails { get; set; }
        public DbSet<GoodsStorage> GoodsStorages { get; set; }
        public DbSet<HandHouse> HandHouses { get; set; }
        public DbSet<House> Houses { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<OwnerCar> OwnerCars { get; set; }
        public DbSet<OwnerFamily> OwnerFamilies { get; set; }
        public DbSet<Parking> Parkings { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<sysAuthor> sysAuthors { get; set; }
        public DbSet<sysAuthorInRole> sysAuthorInRoles { get; set; }
        public DbSet<sysConfig> sysConfigs { get; set; }
        public DbSet<sysRole> sysRoles { get; set; }
        public DbSet<sysUser> sysUsers { get; set; }
        public DbSet<sysUserInEstate> sysUserInEstates { get; set; }
        public DbSet<sysUserInRole> sysUserInRoles { get; set; }
        public DbSet<sysUserLog> sysUserLogs { get; set; }
        public DbSet<WaterAndElectricity> WaterAndElectricities { get; set; }
        public DbSet<WorkPlan> WorkPlans { get; set; }
        public DbSet<WorkPlanTime> WorkPlanTimes { get; set; }
        public DbSet<WorkTimeConfig> WorkTimeConfigs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ArticleMap());
            modelBuilder.Configurations.Add(new CostConfigMap());
            modelBuilder.Configurations.Add(new ElseCostMap());
            modelBuilder.Configurations.Add(new EmployeeMap());
            modelBuilder.Configurations.Add(new EmployeePostHistoryMap());
            modelBuilder.Configurations.Add(new EstateMap());
            modelBuilder.Configurations.Add(new GoodMap());
            modelBuilder.Configurations.Add(new GoodsCategoryMap());
            modelBuilder.Configurations.Add(new GoodsOutMap());
            modelBuilder.Configurations.Add(new GoodsOutDetailMap());
            modelBuilder.Configurations.Add(new GoodsReceiptMap());
            modelBuilder.Configurations.Add(new GoodsReceiptDetailMap());
            modelBuilder.Configurations.Add(new GoodsStorageMap());
            modelBuilder.Configurations.Add(new HandHouseMap());
            modelBuilder.Configurations.Add(new HouseMap());
            modelBuilder.Configurations.Add(new OwnerMap());
            modelBuilder.Configurations.Add(new OwnerCarMap());
            modelBuilder.Configurations.Add(new OwnerFamilyMap());
            modelBuilder.Configurations.Add(new ParkingMap());
            modelBuilder.Configurations.Add(new PostMap());
            modelBuilder.Configurations.Add(new PropertyMap());
            modelBuilder.Configurations.Add(new sysAuthorMap());
            modelBuilder.Configurations.Add(new sysAuthorInRoleMap());
            modelBuilder.Configurations.Add(new sysConfigMap());
            modelBuilder.Configurations.Add(new sysRoleMap());
            modelBuilder.Configurations.Add(new sysUserMap());
            modelBuilder.Configurations.Add(new sysUserInEstateMap());
            modelBuilder.Configurations.Add(new sysUserInRoleMap());
            modelBuilder.Configurations.Add(new sysUserLogMap());
            modelBuilder.Configurations.Add(new WaterAndElectricityMap());
            modelBuilder.Configurations.Add(new WorkPlanMap());
            modelBuilder.Configurations.Add(new WorkPlanTimeMap());
            modelBuilder.Configurations.Add(new WorkTimeConfigMap());
        }
    }
}
