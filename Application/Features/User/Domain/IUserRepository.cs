using UserEntity = Application.Features.User.Domain.Entities.User;

namespace Application.Features.User.Domain;

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);
    Task AddAsync(UserEntity user, CancellationToken cancellationToken);
    Task<UserEntity?> GetByVerificationTokenAsync(string token, CancellationToken cancellationToken);
    Task UpdateAsync(UserEntity user, CancellationToken cancellationToken);
}
