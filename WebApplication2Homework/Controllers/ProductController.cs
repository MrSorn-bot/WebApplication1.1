﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication2Homework.Service;

namespace WebApplication2Homework._1.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService ns;
        private readonly ICategoryService categoryService;

        public ProductController(IProductService ns, ICategoryService categoryService)
        {
            this.ns = ns;
            this.categoryService = categoryService; // เพิ่มบริการจัดการหมวดหมู่
        }

        public IActionResult Index(string keyword)
        {
            var products = ns.GetAll(keyword);

            // Grouping โดยใช้ GroupBy
            var groupedProducts = products
                .GroupBy(p => p.category != null ? p.category.Name : "ไม่มีหมวดหมู่")
                .Select(g => new
                {
                    CategoryName = g.Key,
                    Products = g.ToList()
                })
                .ToList();

            return View(groupedProducts);
        }

        public IActionResult UpCreate(int? id)
        {
            var model = new ProductViewModel
            {
                Categories = categoryService.GetAll(null)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList(),
                Product = id.HasValue ? ns.SearchData(id.Value) : new Product()
            };

            return model.Product != null ? View(model) : RedirectToAction("Index");
        }

        [HttpPost]
        [HttpPost]
        public IActionResult UpCreate(Product product, IFormFileCollection files)
        {
            if (product.Id > 0)
            {
                // Logic for updating the product if necessary
            }
            else
            {
                ns.AddData(product, files); // ส่งไฟล์หลายไฟล์ไปยัง AddData
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            ns.DeleteData(id);
            return RedirectToAction("Index");
        }
    }
}
