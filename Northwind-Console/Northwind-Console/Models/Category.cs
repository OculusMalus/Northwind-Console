using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace NorthwindConsole.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "YO - Enter the name!")]
        public string CategoryName { get; set; }
        public string Description { get; set; }

        public virtual List<Product> Products { get; set; }

        public static void ListCategories()
        {
            var db = new NorthwindContext();
            var query = db.Categories.OrderBy(p => p.CategoryName);
            Console.Clear();
            Console.WriteLine($"{query.Count()} records returned");
            Console.WriteLine("{0,-8} {1,-50}", "1) Category ID:", "Category Name");
            foreach (var item in query)
            {
                Console.WriteLine("{0,-8} {1,-50}", item.CategoryId, item.CategoryName);
            }
        }
    }
}
