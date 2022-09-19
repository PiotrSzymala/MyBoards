using System;
using System.Collections.Generic;

namespace Northwind.Entites
{
    public partial class Produkty
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int? SupplierId { get; set; }
        public string? QuantityPerUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
        public string? Kategoria { get; set; }
        public decimal? Wartosc { get; set; }
    }
}
