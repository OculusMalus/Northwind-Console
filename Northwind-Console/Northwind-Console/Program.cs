using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using NLog;
using NorthwindConsole.Models;

namespace NorthwindConsole
{
    class MainClass
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static void Main(string[] args)
        {
            logger.Info("Program started");
            try
            {
                string choice;
                do
                {
                    Console.WriteLine("1) Display Categories");
                    Console.WriteLine("2) Add Category");
                    Console.WriteLine("3) Edit Category");
                    Console.WriteLine("4) Display a specific category and related active products");
                    Console.WriteLine("5) Display all categories and related products");
                    Console.WriteLine("6) Add a new product");
                    Console.WriteLine("7) Edit a product");
                    Console.WriteLine("8) Display products");
                    Console.WriteLine("9) Display a single product");
                    Console.WriteLine("10) Delete a category");
                    Console.WriteLine("11) Delete a product");
                    Console.WriteLine("\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Option {choice} selected");
                    if (choice == "1")
                    {
                        Category.ListCategoriesWithDescription();
                    }
                    else if (choice == "2")
                    {
                        Category category = new Category();
                        Console.WriteLine("Enter Category Name:");
                        category.CategoryName = Console.ReadLine();
                        
                        ValidationContext context = new ValidationContext(category, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(category, context, results, true);
                        if (isValid)
                        {
                            var db = new NorthwindContext();
                            // check for unique name
                            if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                            }
                            else
                            {
                                Console.WriteLine("Enter the Category Description:");
                                category.Description = Console.ReadLine();
                                logger.Info("Validation passed");
                                db.AddCategory(category);
                                logger.Info("Category added - {title}", category.CategoryName);
                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }
                    }
                    else if (choice == "3")
                    {
                        Category.EditCategory();
                    }
                    else if (choice == "4")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        Console.WriteLine("Select the category whose products you want to display:");
                        
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");

                        Category category = db.Categories.FirstOrDefault(c => c.CategoryId == id);
                        Console.WriteLine($"{category.CategoryName} - {category.Description}");

                        foreach (Product p in category.Products.Where(p => p.Discontinued != true))
                        {
                            Console.WriteLine(p.ProductName);
                        }
                    }
                    else if (choice == "5")
                    {
                        //Display all Categories and their related active (not discontinued) product data (CategoryName, ProductName)
                        var db = new NorthwindContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName}");
                            foreach (Product p in item.Products)
                            {
                                if (p.Discontinued == false)
                                    Console.WriteLine($"\t{p.ProductName}");
                            }
                        }
                    }
                    else if (choice == "6")
                    {
                        var db = new NorthwindContext();
                        var categoryQuery = db.Categories.OrderBy(p => p.CategoryId);
                        var supplierQuery = db.Suppliers.OrderBy(p => p.SupplierId);

                        Console.WriteLine("Select a product Category");
                        foreach (var item in categoryQuery)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }

                        //check for valid category
                        if (int.TryParse(Console.ReadLine(), out int CategoryId))
                        {
                            if (db.Categories.Any(c => c.CategoryId == CategoryId))
                            {
                                
                                Console.Clear();
                                Console.WriteLine("Enter a product Supplier or \"add\" to enter a new supplier.");
                                foreach (var item in supplierQuery)
                                {
                                    Console.WriteLine($"{item.SupplierId}) {item.CompanyName}");
                                }
                                var supplier = Console.ReadLine();
                                //check for valid supplier
                                if (supplier.ToLower() == "add")
                                {
                                    Supplier.NewSupplier();                                    
                                }
                                else if (int.TryParse(supplier, out int supplierId))
                                {
                                    if (db.Suppliers.Any(s => s.SupplierId == supplierId))
                                    {
                                        Product product = InputProduct(db);
                                        product.CategoryId = CategoryId;
                                        if (product != null)
                                        {
                                            product.SupplierId = supplierId;
                                            db.AddProduct(product);
                                            logger.Info("Product added - {title}", product.ProductName);
                                        }
                                    }
                                    else logger.Error("There are no Suppliers with that Id"); 
                                }
                                else logger.Error("Invalid Supplier Id");
                            }
                            else logger.Error("There are no Categories with that Id");
                        }
                        else logger.Error("Invalid Category Id");                     

                    }
                    else if (choice == "7")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);

                        Console.WriteLine("Select the product category:");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        //check for valid category
                        if (int.TryParse(Console.ReadLine(), out int CategoryId))
                        {
                            if (db.Categories.Any(c => c.CategoryId == CategoryId))
                            {
                                logger.Info($"CategoryId {CategoryId} selected");
                                Console.Clear();
                                Console.WriteLine("Select the product number you wish to edit:");
                                logger.Info($"CategoryId {CategoryId} selected");
                                var productList = db.Products.Where(pl => pl.CategoryId == CategoryId).OrderBy(pl => pl.ProductId);

                                foreach (var item in productList)
                                {
                                    Console.WriteLine($"{item.ProductId}) {item.ProductName}");
                                }

                                //check for valid product id
                                if (int.TryParse(Console.ReadLine(), out int ProductId))
                                {
                                    if (db.Products.Any(p => p.ProductId == ProductId))
                                    {
                                        Product product = db.Products.FirstOrDefault(p => p.ProductId == ProductId);
                                        Product.EditProduct(product, db);
                                    }
                                }

                            }
                        }                             

                        
                        
                    }
                    else if (choice == "8")
                    {
                        string displayChoice;
                        var db = new NorthwindContext();
                        
                        Console.Clear();
                        Console.WriteLine("1) Display ALL products");
                        Console.WriteLine("2) Display only DISCONTINUED products");
                        Console.WriteLine("3) Display only ACTIVE products");
                        displayChoice = Console.ReadLine();
                        logger.Info($"Option {choice} selected");

                        if (displayChoice == "1")
                        {
                            logger.Info("{0} records returned", db.Products.Count());
                            Console.WriteLine("ALL Products (Discontinued products in Red)\n__________________________________________________");

                            foreach ( var item in db.Products)
                            {
                                Console.BackgroundColor = item.Discontinued ? ConsoleColor.Red : ConsoleColor.Black;
                                Console.WriteLine(item.ProductName);
                            }
                            Console.ResetColor();
                        }

                        if (displayChoice == "2")
                        {
                            var query = db.Products.Where(p => p.Discontinued == true);
                            logger.Info("{0} records returned", query.Count());
                            Console.WriteLine("ALL Discontinued Products\n___________________________");
                                                        
                            foreach (var item in query)
                            {
                                Console.WriteLine(item.ProductName);
                            }
                        }

                        if (displayChoice == "3")
                        {
                            var query = db.Products.Where(p => p.Discontinued == false);
                            logger.Info("{0} records returned", query.Count());
                            Console.WriteLine("ALL Active Products\n___________________________");

                            foreach (var item in query)
                            {
                                Console.WriteLine(item.ProductName);
                            }
                        }
                                               

                    }
                    else if (choice == "9")
                    {
                        string searchTerm;
                        string displayChoice;
                        var db = new NorthwindContext();

                        Console.Clear();
                        Console.WriteLine("Chose how you wish to select a product:");
                        Console.WriteLine("1) Enter Product Id");
                        Console.WriteLine("2) Search for product name");

                        displayChoice = Console.ReadLine();
                        if (displayChoice == "1")
                        {
                            Console.WriteLine("Enter the Id number of the product you wish to display");
                            if (int.TryParse(Console.ReadLine(), out int productId))
                            {
                                if (db.Products.Any(p => p.ProductId == productId))
                                {
                                    Console.Clear();
                                    Product.DisplayProduct(db.Products.FirstOrDefault(p => p.ProductId == productId));
                                }
                                else logger.Error("There are no Products with that Id");
                            }
                            else logger.Error("Invalid product Id");
                        }

                        if (displayChoice == "2")
                        {
                            Console.WriteLine("Enter all or part of the product name");
                            searchTerm = Console.ReadLine();

                            var query = db.Products.Where(p => p.ProductName.Contains(searchTerm)).OrderBy(p => p.ProductName);

                            Console.WriteLine($"{query.Count()} records returned");
                            

                            if (query.Count() == 1)
                            {
                                Console.Clear();
                                Product.DisplayProduct(query.FirstOrDefault());
                            }
                                

                            if (query.Count() > 1)
                            {
                                Console.WriteLine("{0,-8} {1,-50}", "ID", "ProductName");
                                foreach(var item in query)
                                {
                                    Console.WriteLine("{0,-8} {1,-50}", item.ProductId, item.ProductName);
                                }
                                
                                Console.WriteLine("\nEnter the Id number of the product you wish to display");
                                if (int.TryParse(Console.ReadLine(), out int productId))
                                {
                                    if (db.Products.Any(p => p.ProductId == productId))
                                    {
                                        Console.Clear();
                                        Product.DisplayProduct(db.Products.FirstOrDefault(p => p.ProductId == productId));
                                    }
                                    else logger.Error("There are no Products with that Id");
                                }
                                else logger.Error("Invalid product Id");
                                
                            }     
                                                   
                        }
                        

                    }
                    else if (choice == "10")
                    {
                        var db = new NorthwindContext();
                        Category.ListCategories();
                        Console.WriteLine("Enter the Category Id that you wish to delete");

                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");

                        Category category = db.Categories.FirstOrDefault(c => c.CategoryId == id);
                        if (db.DeleteCategory(category))
                            {
                            logger.Info("Category {0} successfully deleted", category.CategoryName);
                            }
                        else
                        {
                            logger.Error("Unable to delete a category with associated products.");
                            Console.WriteLine("You may not delete a category with associated products. In order to delete a category, all the associated products must first be placed in another category or deleted.");
                        }                           


                    }
                    else if (choice == "11")
                    {
                        //Select and display product
                        string searchTerm;
                        string displayChoice;
                        var db = new NorthwindContext();

                        Console.Clear();
                        Console.WriteLine("Chose how you wish to select a product:");
                        Console.WriteLine("1) Enter Product Id");
                        Console.WriteLine("2) Search for product name");

                        displayChoice = Console.ReadLine();
                        if (displayChoice == "1")
                        {
                            Console.WriteLine("Enter the Id number of the product you wish to delete");
                            if (int.TryParse(Console.ReadLine(), out int productId))
                            {
                                if (db.Products.Any(p => p.ProductId == productId))
                                {
                                    Console.Clear();
                                    //Product.DisplayProduct(db.Products.FirstOrDefault(p => p.ProductId == productId));
                                    db.DeleteProduct(db.Products.FirstOrDefault(p => p.ProductId == productId));
                                    logger.Info("Product Id {0} successfully deleted.", productId);
                                }
                                else logger.Error("There are no Products with that Id");
                            }
                            else logger.Error("Invalid product Id");
                        }
                        if (displayChoice == "2")
                        {
                            Console.WriteLine("Enter all or part of the product name");
                            searchTerm = Console.ReadLine();

                            var query = db.Products.Where(p => p.ProductName.Contains(searchTerm)).OrderBy(p => p.ProductName);

                            Console.WriteLine($"{query.Count()} records returned");


                            if (query.Count() == 1)
                            {
                                Console.Clear();
                                Product.DisplayProduct(query.FirstOrDefault());
                                db.DeleteProduct(query.FirstOrDefault());
                                logger.Info("Product id {0} successfully deleted", query.FirstOrDefault().ProductId);
                            }


                            if (query.Count() > 1)
                            {
                                Console.WriteLine("{0,-8} {1,-50}", "ID", "ProductName");
                                foreach (var item in query)
                                {
                                    Console.WriteLine("{0,-8} {1,-50}", item.ProductId, item.ProductName);
                                }

                                Console.WriteLine("\nEnter the Id number of the product you wish to delete");
                                if (int.TryParse(Console.ReadLine(), out int productId))
                                {
                                    if (db.Products.Any(p => p.ProductId == productId))
                                    {
                                        Console.Clear();
                                        Product.DisplayProduct(db.Products.FirstOrDefault(p => p.ProductId == productId));
                                    }
                                    else logger.Error("There are no Products with that Id");
                                }
                                else logger.Error("Invalid product Id");

                            }

                        }
                    }
                    Console.WriteLine();

                } while (choice.ToLower() != "q");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            logger.Info("Program ended");
        }

        public static Product InputProduct(NorthwindContext db)
        {
            Product product = new Product();
            Console.WriteLine("Enter the Product name");
            product.ProductName = Console.ReadLine();
            
            ValidationContext context = new ValidationContext(product, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(product, context, results, true);
            if (isValid)
            {
                //var db = new NorthwindContext();
                // check for unique name
                if (db.Products.Any(p => p.ProductName == product.ProductName))
                    results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                else
                {
                    logger.Info("Validation passed");
                    
                }
                return product;
            }
            else
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
            
            return null;
        }
    }
}