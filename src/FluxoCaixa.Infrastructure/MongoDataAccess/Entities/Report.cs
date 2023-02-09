using System;
using System.Collections.Generic;
using System.Text;

namespace FluxoCaixa.Infrastructure.MongoDataAccess.Entities
{
    public class Report
    {
        public double Amount { get; set; }
        public string Description { get; set; }
        public DateTime EntryDate { get; set; }
    }
}
