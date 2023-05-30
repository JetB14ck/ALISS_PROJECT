using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALISS.Data
{
    public class DowloadFileResult
    {
        public string ErrorMessage { get; set; }
        public string ErrorName { get; set; }
        public bool Succeeded { get; set; }
    }
}
