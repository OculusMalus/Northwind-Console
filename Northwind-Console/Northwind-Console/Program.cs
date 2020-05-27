using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
                    Console.WriteLine("3) Display Category and related products");
                    Console.WriteLine("4) Display all Categories and their related products");
                    Console.WriteLine("5) Add a new product");
                    Console.WriteLine("6) Edit a product");
                    Console.WriteLine("7) Display all products");
                    Console.WriteLine("8) Display a single product");
                    Console.WriteLine("\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Option {choice} selected");
                    if (choice == "1")
                    {
                        Category.ListCategories();
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
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        Console.WriteLine("Select the category whose products you want to display:");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");
                        Category category = db.Categories.FirstOrDefault(c => c.CategoryId == id);
                        Console.WriteLine($"{category.CategoryName} - {category.Description}");
                        foreach (Product p in category.Products)
                        {
                            Console.WriteLine(p.ProductName);
                        }
                    }
                    else if (choice == "4")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName}");
                            foreach (Product p in item.Products)
                            {
                                Console.WriteLine($"\t{p.ProductName}");
                            }
                        }
                    }
                    else if (choice == "5")
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
                                Console.WriteLine("Select a product Supplier");
                                foreach (var item in supplierQuery)
                                {
                                    Console.WriteLine($"{item.SupplierId}) {item.CompanyName}");
                                }

                                //check for valid supplier
                                if (int.TryParse(Console.ReadLine(), out int SupplierId))
                                {
                                    if (db.Suppliers.Any(s => s.SupplierId == SupplierId))
                                    {
                                        Product product = InputProduct(db);
                                        product.CategoryId = CategoryId;
                                        if (product != null)
                                        {
                                            product.SupplierId = SupplierId;
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
                    else if (choice == "6")
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
                                        //TODO call the edit Product Method
                                        Product.EditProduct(product, db);
                                    }
                                }

                            }
                        }                             

                        
                        
                    }
                    else if (choice == "7")
                    {
                        string displayChoice;
                        do
                        {
                            Console.Clear();
                            Console.WriteLine("1) Display ALL products");
                            Console.WriteLine("2) Display only DISCONTINUED products");
                            Console.WriteLine("3) Display only ACTIVE products");
                            displayChoice = Console.ReadLine();
                            
                            if (displayChoice == "1")
                            {
                            //TODO 
                            }

                            if (displayChoice == "2")
                            {
                                //TODO filter product list
                            }

                            if (displayChoice == "3")
                            {
                                //TODO
                            }
                        }
                        while (displayChoice.ToLower() != "q"); 
                        
                        

                    }

                    else if (choice == "8")
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