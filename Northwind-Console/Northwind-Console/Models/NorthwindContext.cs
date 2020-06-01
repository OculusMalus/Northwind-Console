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

        public bool DeleteCategory(Category category)
        {
            if (category.Products.Count == 0)
            {
                Categories.Remove(category);
                SaveChanges();
                return true;
            }
            else return false;
               
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

        public void DeleteProduct(Product product)
        {
            Products.Remove(product);
            SaveChanges();                
        }

        public void AddSupplier(Supplier supplier)
        {
            this.Suppliers.Add(supplier);
            this.SaveChanges();
        }


    }
    
}