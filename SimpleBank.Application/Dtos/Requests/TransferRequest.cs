using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBank.Application.Dtos.Requests
{
    public record TransferRequest(Guid FromAccountId, Guid ToAccountId, decimal Amount, string Description);

}
