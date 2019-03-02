﻿using System.Collections.Generic;
using WebStore.Entities.Entities;

namespace WebStore.Models.Admin
{
    public class ProductListViewModel
    {
        public PageViewModel PageViewModel { get; set; }
        public Product Product { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
