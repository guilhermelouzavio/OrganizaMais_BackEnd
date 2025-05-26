using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Dtos.Responses
{
    public class CustomerResponse
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Document { get; set; }
        public string Email { get; set; }
    }
}
