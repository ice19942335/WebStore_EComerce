﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebStore.Areas.Admin.Models;
using WebStore.DAL.Context;
using WebStore.DomainNew.Entities;
using WebStore.Entities.Entities;
using WebStore.Infrastructure.Interfaces;
using WebStore.Models;

namespace WebStore.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly IProductData _productData;
        private readonly WebStoreContext _webStoreContext;
        private readonly IProductDataAdmin _productDataAdmin;

        public HomeController(IProductData productData, WebStoreContext webStoreContext, IProductDataAdmin productDataAdmin)
        {
            _productData = productData;
            _webStoreContext = webStoreContext;
            _productDataAdmin = productDataAdmin;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ProductList(int page = 1)
        {
            int pageSize = 5;
            IQueryable<Product> productsList = _webStoreContext.Products;

            var count = await productsList.CountAsync();
            var items = await productsList.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);

            ProductListViewModel viewModel = new ProductListViewModel
            {
                PageViewModel = pageViewModel,
                Products = items
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int? id)
        {
            ProductViewModel model;
            if (id.HasValue)
            {
                model = _productData.GetProductById(id.Value);
                if (ReferenceEquals(model, null))
                    return NotFound();
            }
            else
            {
                model = new ProductViewModel();
            }

            return View("Edit", model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(ProductViewModel model)
        {
            if (model.Id > 0)
            {
                if (_productDataAdmin.Edit(model))
                    return RedirectToAction(nameof(ProductList));
            }
            else
            {
                if (ModelState.IsValid)
                    AddNewProduct(model);
            }

            if (ModelState.IsValid)
                return RedirectToAction(nameof(ProductList));
            else
                return View("Edit", model);
        }

        public void AddNewProduct(ProductViewModel dbItemProduct)
        {
            _productDataAdmin.Create(dbItemProduct);
        }
    }

}