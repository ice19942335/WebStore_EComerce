﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebStore.Entities.Dto.Order;
using WebStore.Entities.Entities;
using WebStore.Interfaces.services;

namespace WebStore.ServiceHosting.Controllers
{

    [Produces("application/json")]
    [Route("api/orders")]
    public class OrdersController : Controller, IOrdersService
    {
        private readonly IOrdersService _ordersService;
        public OrdersController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        [HttpGet("user/{userName}")]
        public IEnumerable<OrderDto> GetUserOrders(string userName)
        {
            return _ordersService.GetUserOrders(userName);
        }

        [HttpGet("{id}"), ActionName("Get")]
        public OrderDto GetOrderById(int id)
        {
            return _ordersService.GetOrderById(id);
        }

        [HttpPost("{userName?}")]
        public OrderDto CreateOrder([FromBody]CreateOrderModel orderModel, string userName)
        {
            return _ordersService.CreateOrder(orderModel, userName);
        }

        [HttpGet, ActionName("Get")]
        public List<OrderDto> GetAllOrdersList()
        {
            return _ordersService.GetAllOrdersList();
        }

        public bool DeleteOdrerById(int id)
        {
            throw new NotImplementedException();
        }
    }
}

