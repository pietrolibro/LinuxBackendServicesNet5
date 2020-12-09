using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyShopOnLine.Backend.Records
{
    public record ProductRecord (string Code, string Description, decimal Cost, decimal Price, int Review, decimal Weight, int QuantityPerUnityPack);

}
