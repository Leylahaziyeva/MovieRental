using MovieRental.BLL.ViewModels.UserList;

namespace MovieRental.BLL.Services.Contracts
{
    public interface IUserListService
    {
        Task<UserListsViewModel> GetUserListsAsync(string userId, int page = 1, int pageSize = 12);
        Task<UserListViewModel?> GetUserListByIdAsync(int listId, string userId);
        Task<int> CreateUserListAsync(string userId, CreateUserListViewModel model);
        Task<bool> UpdateUserListAsync(int listId, string userId, CreateUserListViewModel model);
        Task<bool> DeleteUserListAsync(int listId, string userId);
        Task<bool> AddMovieToListAsync(int listId, int movieId, string userId);
        Task<bool> RemoveMovieFromListAsync(int listId, int movieId, string userId);
        Task<bool> IsMovieInListAsync(int listId, int movieId, string userId);
    }
}