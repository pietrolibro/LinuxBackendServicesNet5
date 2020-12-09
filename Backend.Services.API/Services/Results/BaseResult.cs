using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyShopOnLine.Backend.Services.Results
{
    public record BaseResult
    {
        public bool Success { get; init; }
        public string ErrorMessage { get; init; }
    }
}
