using MyShopOnLine.Backend.Records;
using MyShopOnLine.Backend.Services.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyShopOnLine.Backend.Services
{
    public interface ICustomerService
    {
        Task<CreateResult<CustomerRecord>> CreateAsync(CustomerRecord customer);
        Task<List<CustomerRecord>> GetAsync();
        Task<CustomerRecord> GetAsync(string email);
        Task<RemoveResult> RemoveAsync(string email);
        Task<UpdateResult> UpdateAsync(string email, CustomerRecord customerIn);
    }
}