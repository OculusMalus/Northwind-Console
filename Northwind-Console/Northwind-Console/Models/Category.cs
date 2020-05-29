using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace NorthwindConsole.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Name field is required")]
        public string CategoryName { get; set; }
        public string Description { get; set; }

        public virtual List<Product> Products { get; set; }

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void ListCategories()
        {
            var db = new NorthwindContext();
            var query = db.Categories.OrderBy(p => p.CategoryId);
            Console.Clear();
            Console.WriteLine($"{query.Count()} records returned");
            Console.WriteLine("{0,-8} {1,-50}", "ID:", "Category Name");
            foreach (var item in query)
            {
                Console.WriteLine("{0,-8} {1,-50}", item.CategoryId, item.CategoryName);
            }
        }

        public static void ListCategoriesWithDescription()
        {
            var db = new NorthwindContext();
            var query = db.Categories.OrderBy(p => p.CategoryName);
            Console.Clear();
            Console.WriteLine($"{query.Count()} records returned");
            Console.WriteLine("{0,-15} {1,-50}", "Category Name", "Description");
            foreach (var item in query)
            {
                Console.WriteLine("{0,-15} {1,-50}", item.CategoryName, item.Description);
            }
        }

        public static void EditCategory( )
        {
            var db = new NorthwindContext();
            string choice= "";
            Console.Clear();
            do
            {
                ListCategories();
                Console.WriteLine("Enter the Id number of the categoy that you wish to edit or q to finish:");
                if (int.TryParse(Console.ReadLine(), out int categoryId))
                {
                    if (db.Categories.Any(c => c.CategoryId == categoryId))
                    {
                        logger.Info($"Category ID {categoryId} selected");
                        var selectedCategory = db.Categories.Where(c => c.CategoryId == categoryId).FirstOrDefault();
                                                
                        {
                            do
                            {
                                Console.WriteLine("Category name: {0}", selectedCategory.CategoryName);
                                Console.WriteLine("Category description: {0}", selectedCategory.Description);
                                Console.WriteLine("\nEnter 1 to edit Category Name OR Enter 2 to edit Category Description OR q to quit:");
                                choice = Console.ReadLine();
                                if (choice == "1")
                                {
                                    var oldCategoryName = selectedCategory.CategoryName;
                                    Console.WriteLine("Enter new Category name:");
                                    var newCategoryName = Console.ReadLine();
                                    selectedCategory.CategoryName = newCategoryName;
                                    db.UpdateCategory(selectedCategory);
                                    logger.Info("User changed Category name from {0} to {1}", oldCategoryName, newCategoryName);


                                }
                                if (choice == "2")
                                {
                                    var oldCategoryDescription = selectedCategory.Description;
                                    Console.WriteLine("Enter new Category description:");
                                    var newCategoryDescription = Console.ReadLine();
                                    selectedCategory.Description = newCategoryDescription;
                                    db.UpdateCategory(selectedCategory);
                                    logger.Info("User changed Category description from: {0}\n to\n {1}", oldCategoryDescription, newCategoryDescription);
                                }
                            }
                            while (choice.ToLower() != "q");                       
                            
                        }
                    }
                    else logger.Info("No such category Id exists");
                }
                else logger.Error("Invalid Input");                                         
                           
            }
            while (choice.ToLower() != "q");
        }
    }
}
