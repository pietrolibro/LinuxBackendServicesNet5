using MyShopOnLine.Backend.Records;
using MyShopOnLine.Backend.Services.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyShopOnLine.Backend.Services
{
    public interface IOrderService
    {
        Task<CreateResult<OrderRecord>> CreateAsync(OrderRecord order);
        Task<List<OrderRecord>> GetAsync();
        Task<OrderRecord> GetAsync(string number);
        Task<RemoveResult> RemoveAsync(string number);
        Task<UpdateResult> UpdateAsync(string number, OrderRecord orderIn);
    }
}