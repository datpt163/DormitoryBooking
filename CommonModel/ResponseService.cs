using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonModel
{
    public class ResponseService
    {
        public string ErrorMessage { get; set; } = string.Empty;
        public object? Data { get; set; }
    }
}
