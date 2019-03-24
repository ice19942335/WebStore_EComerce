﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartBreadcrumbs;
using WebStore.Entities.Dto.Order;
using WebStore.Entities.Entities;
using WebStore.Entities.ViewModels;
using WebStore.Entities.ViewModels.Cart;
using WebStore.Entities.ViewModels.Order;
using WebStore.Interfaces;
using WebStore.Interfaces.services;

namespace WebStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrdersService _ordersService;

        public CartController(ICartService cartService, IOrdersService ordersService)
        {
            _cartService = cartService;
            _ordersService = ordersService;
        }

        [Breadcrumb("Cart")]
        public IActionResult Details()
        {
            var model = new DetailsViewModel()
            {
                CartViewModel = _cartService.TransformCart(),
                OrderViewModel = new OrderViewModel()
            };
            return View(model);

        }

        public IActionResult DecrementFromCart(int id)
        {
            _cartService.DecrementFromCart(id);
            return Json(new {id, message = "Quantity of item reduced by 1" });
            //return RedirectToAction("Details");
        }

        public IActionResult RemoveFromCart(int id)
        {
            _cartService.RemoveFromCart(id);
            return Json(new { id, message = $"Item ID:{id} removed from cart" });
            //return RedirectToAction("Details");
        }

        public IActionResult RemoveAll()
        {
            _cartService.RemoveAll();
            return RedirectToAction("Details");
        }

        public IActionResult AddToCart(int id)
        {
            _cartService.AddToCart(id);
            return Json(new { id, message = "Product added to cart" });
        }

        public IActionResult GetCartView() => ViewComponent("Cart");

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CheckOut(OrderViewModel model)
        {
            if (ModelState.IsValid)
            {

                Dictionary<ProductViewModel, int> productViewModelList = _cartService.TransformCart().Items;
                List<OrderItemDto> productDtoList = new List<OrderItemDto>();

                foreach (var productViewModel in productViewModelList)
                {
                    productDtoList.Add(new OrderItemDto
                    {
                        Id = productViewModel.Key.Id,
                        Price = productViewModel.Key.Price,
                        Quantity = productViewModel.Value
                    });
                }

                CreateOrderModel orderModel = new CreateOrderModel()
                {
                    OrderViewModel = model,
                    OrderItems = productDtoList
                };

                var orderResult = _ordersService.CreateOrder(orderModel, User.Identity.Name);
                _cartService.RemoveAll();

                if (orderResult.Id.Equals(0)) //Will be true IF CreateOrder method will return a (New Order)
                    return View("YouHaveToBeRegistredUser");

                return RedirectToAction("OrderConfirmed", new { id = orderResult.Id });
            }

            var detailsModel = new DetailsViewModel
            {
                CartViewModel = _cartService.TransformCart(),
                OrderViewModel = model
            };
            return View("Details", detailsModel);
        }

        public IActionResult OrderConfirmed(int id)
        {
            ViewBag.OrderId = id;
            return View();
        }
    }
}