using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonModel.Message
{
    public class ResponseMessage<T> where T : new()
    {
        public string ErrorMessage { get; set; } = string.Empty;
        public T Data { get; set; } = new T();
    }
}
