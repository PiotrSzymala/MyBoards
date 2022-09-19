using System;
using System.Collections.Generic;

namespace Northwind.Entites
{
    public partial class Klienci
    {
        public string CustomerId { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
    }
}
