using LunchWeb.ViewModels.CustomerViewModels;

namespace LunchWeb.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerViewModel>> GetAllUsersAsync();
        Task<CustomerViewModel?> GetUserByIdAsync(string id);
        Task CreateUserAsync(CustomerCreateViewModel user);
        Task UpdateUserAsync(CustomerUpdateViewModel user);
        Task DeleteUserAsync(string id);
    }
}
