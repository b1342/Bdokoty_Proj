using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkshowcaseApi.Common.Enums;
using WorkshowcaseApi.Domain.Users;

namespace WorkshowcaseApi.Features.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);

    Task<User?> GetByEmailAsync(string email);

    Task<IReadOnlyList<User>> GetAllAsync(UserType? userType, UserStatus? status, int page, int pageSize);

    Task<int> CountAsync(UserType? userType, UserStatus? status);

    Task AddAsync(User user);

    void Update(User user);

    Task SaveChangesAsync();
}
