using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkshowcaseApi.Common.Enums;
using WorkshowcaseApi.Data;
using WorkshowcaseApi.Domain.Users;

namespace WorkshowcaseApi.Features.Users.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var normalizedEmail = NormalizeEmail(email);

        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == normalizedEmail);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(
        UserType? userType,
        UserStatus? status,
        int page,
        int pageSize)
    {
        var skip = (page - 1) * pageSize;

        var query = BuildFilteredQuery(userType, status);

        return await query
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ThenBy(x => x.Id)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync(UserType? userType, UserStatus? status)
    {
        return await BuildFilteredQuery(userType, status).CountAsync();
    }

    public async Task AddAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
    }

    public void Update(User user)
    {
        _dbContext.Users.Update(user);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    private IQueryable<User> BuildFilteredQuery(UserType? userType, UserStatus? status)
    {
        IQueryable<User> query = _dbContext.Users;

        if (userType.HasValue)
        {
            query = query.Where(x => x.UserType == userType.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        return query;
    }

    private static string NormalizeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return string.Empty;
        }

        return email.Trim().ToLowerInvariant();
    }
}
