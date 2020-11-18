using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SP.Web.ViewModels
{
    public class RequirementViewModel
    {
        public string FixedAmount { get; set; }
        public string Formula { get; set; }
        public long[] IdList { get; set; }
        public int UpdatedCount { get; set; }
    }
}
