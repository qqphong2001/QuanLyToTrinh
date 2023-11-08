using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class ChartRequests
    {
        public string[] labels { get; set; }
        public DatasetsChart datasets { get; set; }
    }
}
