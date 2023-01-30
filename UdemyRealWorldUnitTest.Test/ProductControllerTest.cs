using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UdemyRealWordUnitTest.Web.Models;

namespace UdemyRealWorldUnitTest.Test
{
    public class ProductControllerTest
    {
        protected DbContextOptions<UdemyUnitTestDBContext> _contextOptions { get; private set; }

        public void SetContextOptions(DbContextOptions<UdemyUnitTestDBContext> contextOptions)
        {
            _contextOptions = contextOptions;
            Seed();
        }

        public void Seed()
        {
            using (UdemyUnitTestDBContext context= new UdemyUnitTestDBContext(_contextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Category.Add(new Category() { Name = "Kalemler" });
                context.Category.Add(new Category() { Name = "Defterler" });
                context.SaveChanges();


                context.Product.Add(new Product()
                    { CategoryId = 1, Name = "Kalem 1", Price = 50, Stock = 100, Color = "red" });
                context.Product.Add(new Product()
                    { CategoryId = 2, Name = "Kalem 2", Price = 20, Stock = 200, Color = "mavi" });

                context.SaveChanges();
            }


        }
    }
}
