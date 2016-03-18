using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Adelante.Payments.Api.Models
{
    public class Summary
    {
        public DateTime Date { get; set; }
        public string LedgerCode { get; set; }
        public int Transactions { get; set; }
        public decimal Total { get; set; }
        public decimal Average { get; set; }
        public string Method { get; set; }
        public string Channel { get; set; }
    }
}