﻿using System.Collections.Generic;
using WebStore.Entities.ViewModels.Page;

namespace WebStore.Entities.ViewModels
{
    public class CatalogViewModel
    {
        public int? BrandId { get; set; }
        public int? SectionId { get; set; }
        public IEnumerable<ProductViewModel> Products { get; set; }
        public ProductsPageViewModel PageViewModel { get; set; }
    }
}
