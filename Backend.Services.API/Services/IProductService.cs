using System.Collections.Generic;
using System.Threading.Tasks;

using MyShopOnLine.Backend.Records;
using MyShopOnLine.Backend.Services.Results;

namespace MyShopOnLine.Backend.Services
{
    public interface IProductService
    {
        Task<CreateResult<ProductRecord>> CreateAsync(ProductRecord product);
        Task<List<ProductRecord>> GetAsync();
        Task<ProductRecord> GetAsync(string code);
        Task<RemoveResult> RemoveAsync(string code);
        Task<UpdateResult> UpdateAsync(string code, ProductRecord productIn);
    }
}