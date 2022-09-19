using System;
using System.Collections.Generic;

namespace Northwind.Entites
{
    public partial class Pracownicy
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Title { get; set; }
        public DateTime? HireDate { get; set; }
    }
}
