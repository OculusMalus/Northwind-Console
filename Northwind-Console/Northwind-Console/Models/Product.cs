using System;
using System.Linq;

namespace NorthwindConsole.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string QuantityPerUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        public Int16? UnitsInStock { get; set; }
        public Int16? UnitsOnOrder { get; set; }
        public Int16? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }

        public int? CategoryId { get; set; }
        public int? SupplierId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Supplier Supplier { get; set; }

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        

        public static void EditProduct(Product product, NorthwindContext db)
        {                             
            string choice;
            Console.Clear();
            do
            {                
                DisplayProduct(product);
                Console.WriteLine("Enter the field number that you wish to edit or q to finish:");
                choice = Console.ReadLine();
                
                logger.Info($"Option {choice} selected");
                if (choice == "1")
                {
                    logger.Error($"Product Id cannot be changed");
                }
                if (choice == "2")
                {
                    Console.WriteLine("Current product name: {0}", product.ProductName);
                    Console.WriteLine("Enter new product name:");
                    var oldName = product.ProductName;
                    var newName = Console.ReadLine();
                    if (newName != null && newName.Length > 2)
                    {
                        product.ProductName = newName;
                        logger.Info("User changed product name from {0} to {1}", oldName, newName);
                    }
                    else
                    {
                        logger.Error("Product Name should not be null and must be longer than 2 characters");
                    }                                            
                }
                if (choice == "3")
                {                    
                    var query = db.Categories.OrderBy(p => p.CategoryId);

                    Console.WriteLine("Categories\n");
                    foreach (var item in query)
                    {
                        Console.WriteLine($"{item.CategoryId} - {item.CategoryName}");
                    }

                    Console.WriteLine("Enter the new product category number:");
                    var oldCategoryName = product.Category.CategoryName;
                    //check for valid category
                    if (int.TryParse(Console.ReadLine(), out int newCategoryId))
                    {
                        if (db.Categories.Any(c => c.CategoryId == newCategoryId))
                        {
                            product.CategoryId = newCategoryId;
                            var newCategory = db.Categories.Where(c => c.CategoryId == newCategoryId).FirstOrDefault();
                            logger.Info("User changed product category from {0} to {1}", oldCategoryName, newCategory.CategoryName);
                        }
                        else
                            logger.Error("No such category Id exists");
                    }
                    else
                        logger.Error("Invalid input");
                }

                if (choice == "4")
                {
                    var query = db.Suppliers.OrderBy(s => s.SupplierId);
                    var oldSupplier = product.Supplier.CompanyName;
                    Console.WriteLine("Suppliers\n");
                    foreach (var item in query)
                    {
                        Console.WriteLine($"{item.SupplierId} - {item.CompanyName}");
                    }

                    Console.WriteLine("Enter the new supplier number:");
                    //check for valid supplier
                    if (int.TryParse(Console.ReadLine(), out int newSupplierId))
                    {
                        if (db.Suppliers.Any(s => s.SupplierId == newSupplierId))
                        {
                            product.SupplierId = newSupplierId;
                            var newSupplier = db.Suppliers.FirstOrDefault(s => s.SupplierId == newSupplierId);
                            logger.Info("User changed Supplier from {0} to {1}", oldSupplier, newSupplier.CompanyName);
                        }
                        else
                            logger.Error("No such supplier exists");
                    }
                    else
                        logger.Error("Invalid input");                                                           
                }
                if (choice == "5")
                {
                    Console.WriteLine("Current quantity per unit: {0}", product.QuantityPerUnit);
                    Console.WriteLine("Enter new quantity per unit:");
                    var oldQtyPerUnit = product.QuantityPerUnit;
                    var newQtyPerUnit = Console.ReadLine();
                    if (newQtyPerUnit != null && newQtyPerUnit.Length > 2)
                    {
                        product.QuantityPerUnit = newQtyPerUnit;
                        logger.Info("User changed quantity per unit from {0} to {1}", oldQtyPerUnit, newQtyPerUnit);
                    }
                    else
                    {
                        logger.Error("Quantity Per Unit should not be null and must be longer than 2 characters");
                    }
                }
                if (choice == "6")
                {
                    Console.WriteLine("Current unit price: {0:C2}", product.UnitPrice);
                    Console.WriteLine("Enter new unit price:");
                    
                    var oldUnitPrice = product.UnitPrice;
                    //check for valid format
                    if (decimal.TryParse(Console.ReadLine(), out decimal newUnitPrice))
                    {                        
                        product.UnitPrice = newUnitPrice;
                        logger.Info("User changed unit price from {0:C2} to {1:C2}", oldUnitPrice, newUnitPrice);                                           
                           
                    }
                    else
                        logger.Error("Invalid input");
                }
                if (choice == "7")
                {
                    Console.WriteLine("Current number of units in stock: {0}", product.UnitsInStock);
                    Console.WriteLine("Enter new number of units in stock:");
                    var oldUnitsInStock = product.UnitsInStock;
                    //check for valid input
                    if (short.TryParse(Console.ReadLine(), out short newUnitsInStock))
                    {
                        product.UnitsInStock = newUnitsInStock;
                        logger.Info("User changed units in stock from {0} to {1}", oldUnitsInStock, newUnitsInStock);
                    }
                    else
                        logger.Error("Invalid Input");
                }
                if (choice == "8")
                {
                    Console.WriteLine("Current number of units in stockon order: {0}", product.UnitsOnOrder);
                    Console.WriteLine("Enter new number of units on order:");
                    var oldUnitsOnOrder = product.UnitsOnOrder;
                    //check for valid input
                    if (short.TryParse(Console.ReadLine(), out short newUnitsOnOrder))
                    {
                        product.UnitsOnOrder = newUnitsOnOrder;
                        logger.Info("User changed units on order from {0} to {1}", oldUnitsOnOrder, newUnitsOnOrder);
                    }
                    else
                        logger.Error("Invalid Input");
                }
                if (choice == "9")
                {
                    Console.WriteLine("Current reorder level: {0}", product.ReorderLevel);
                    Console.WriteLine("Enter new reorder level:");
                    var oldReorderLevel = product.ReorderLevel;
                    //check for valid input
                    if (short.TryParse(Console.ReadLine(), out short newReorderLevel))
                    {
                        product.ReorderLevel = newReorderLevel;
                        logger.Info("User changed reorder level from {0} to {1}", oldReorderLevel, newReorderLevel);
                    }
                    else
                        logger.Error("Invalid Input");
                }
                if (choice == "10")
                {
                    //display current status TODO ask if want change status?
                    Console.WriteLine("Current product status: " + ((product.Discontinued) ? "Discontinued" : "Active"));
                   
                }
                db.UpdateProduct(product);
            }
            while (choice.ToLower() != "q") ;
            
        }

        public static void DisplayProduct(Product product)
        {
            //Console.WriteLine($"{p.ProductId}) {p.ProductName}");
            Console.WriteLine("{0,-22} {1,-50}", "1) Product ID:", product.ProductId);
            Console.WriteLine("{0,-22} {1,-50}", "2) Product Name:", product.ProductName);
            Console.WriteLine("{0,-22} {1,-50}", "3) Category:", product.Category.CategoryName);
            Console.WriteLine("{0,-22} {1,-50}", "4) Supplier:", product.Supplier.CompanyName);
            Console.WriteLine("{0,-22} {1,-50}", "5) Quantity Per Unit:", product.QuantityPerUnit);
            Console.WriteLine("{0,-22} {1,-50:C2}", "6) Unit Price:", product.UnitPrice);
            Console.WriteLine("{0,-22} {1,-50}", "7) Units in Stock:", product.UnitsInStock);
            Console.WriteLine("{0,-22} {1,-50}", "8) Units on Order:", product.UnitsOnOrder);
            Console.WriteLine("{0,-22} {1,-50}", "9) Reorder Level:", product.ReorderLevel);
            Console.WriteLine("{0,-22} {1,-50}", "10) Status:", (product.Discontinued) ? "Discontinued" : "Active");
            
        }

       
    }
}