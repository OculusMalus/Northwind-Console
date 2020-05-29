using System.Data.Entity;

namespace NorthwindConsole.Models
{
    public class NorthwindContext : DbContext
    {
        public NorthwindContext() : base("name=NorthwindContext") { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        public void AddCategory(Category category)
        {
            this.Categories.Add(category);
            this.SaveChanges();
        }

        public void UpdateCategory(Category UpdatedCategory)
        {
            Category category = this.Categories.Find(UpdatedCategory.CategoryId);
            category.CategoryName = UpdatedCategory.CategoryName;
            category.Description = UpdatedCategory.Description;
            this.SaveChanges();
        }

        public void AddProduct(Product product)
        {
            this.Products.Add(product);
            this.SaveChanges();
        }

        public void UpdateProduct(Product UpdatedProduct)
        {
            Product product = this.Products.Find(UpdatedProduct.ProductId);
            product.ProductName = UpdatedProduct.ProductName;
            product.CategoryId = UpdatedProduct.CategoryId;
            product.SupplierId = UpdatedProduct.SupplierId;
            product.Discontinued = UpdatedProduct.Discontinued;
            SaveChanges();
        }

        
    }
    
}