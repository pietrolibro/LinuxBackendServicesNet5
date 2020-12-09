using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MyShopOnLine.Backend.Records;

namespace MyShopOnLine.Backend.Services.Results
{
    public record CreateResult<T> : BaseResult
    {
        public bool AlreadyExists { get; init; } = false;
        public T NewRecord { get; init; }
    }
}
