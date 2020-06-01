using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace NorthwindConsole.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }
        [Required(ErrorMessage = "Company Name is required")]
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }

        public virtual List<Product> Products { get; set; }
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void DisplaySupplier(Supplier supplier)
        {
            Console.WriteLine("{0,-22} {1,-50}", "1) Supplier ID:", supplier.SupplierId);
            Console.WriteLine("{0,-22} {1,-50}", "2) Company Name:", supplier.CompanyName);
            Console.WriteLine("{0,-22} {1,-50}", "3) Contact Name:", supplier.ContactName);
            Console.WriteLine("{0,-22} {1,-50}", "4) Contact Title:", supplier.ContactTitle);
            Console.WriteLine("{0,-22} {1,-50}", "5) Address:", supplier.Address);
            Console.WriteLine("{0,-22} {1,-50}", "6) City:", supplier.City);
            Console.WriteLine("{0,-22} {1,-50}", "7) Region:", supplier.Region);
            Console.WriteLine("{0,-22} {1,-50}", "8) Postal Code:", supplier.PostalCode);
            Console.WriteLine("{0,-22} {1,-50}", "9) Country:", supplier.Country);
            Console.WriteLine("{0,-22} {1,-50}", "10) Phone:", supplier.Phone);
            Console.WriteLine("{0,-22} {1,-50}", "10) Fax:", supplier.Fax);

        }

        public static void NewSupplier()
        {
            Console.Clear();
            Supplier supplier = new Supplier();
            Console.WriteLine("Enter Company Name:");
            supplier.CompanyName = Console.ReadLine();
             
            ValidationContext context = new ValidationContext(supplier, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(supplier, context, results, true);
            if (isValid)
            {
                var db = new NorthwindContext();
                // check for unique name
                if (db.Suppliers.Any(s => s.CompanyName == supplier.CompanyName))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Supplier exists", new string[] { "CompanyName" }));
                }
                else
                {
                    logger.Info("Validation passed");
                    db.AddSupplier(supplier);
                    logger.Info("Supplier added - {title}", supplier.CompanyName);
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
    }
}