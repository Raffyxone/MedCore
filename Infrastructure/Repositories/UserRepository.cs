using Application.Features.User.Domain;
using Application.Features.User.Domain.ValueObjects;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using UserEntity = Application.Features.User.Domain.Entities.User;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        var emailVo = Email.Create(email).Value;
        return await _context.Users.AnyAsync(u => u.Email == emailVo, cancellationToken);
    }

    public async Task AddAsync(UserEntity user, CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserEntity?> GetByVerificationTokenAsync(string token, CancellationToken cancellationToken)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token, cancellationToken);
    }

    public async Task UpdateAsync(UserEntity user, CancellationToken cancellationToken)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
