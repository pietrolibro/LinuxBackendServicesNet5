using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyShopOnLine.Backend.Services.Results
{
    public record UpdateResult : BaseResult
    {
        public bool NotFound { get; set; } = false;
    }
}
