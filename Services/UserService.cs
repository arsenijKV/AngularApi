namespace WebApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WebApi.Data;
using WebApi.Entities;
using WebApi.Models;

public interface IUserService
{
    Task<User> Authenticate(AuthenticateModel model);
    Task<IEnumerable<User>> GetAll();
    Task<User> Create(CreateUserModel model);
    Task<User> Update(int id, UpdateUserModel model);
    Task<User> Delete(int id);
    
}

public class UserService : IUserService
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher<User> _hasher;

    public UserService(AppDbContext db, PasswordHasher<User> hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    public async Task<User> Authenticate(AuthenticateModel model)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == model.Username);
        if (user == null) return null;

        var res = _hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
        return res == PasswordVerificationResult.Success ? user : null;
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        return await _db.Users
            .AsNoTracking()
            .Select(u => new User { Id = u.Id, FirstName = u.FirstName, LastName = u.LastName, Username = u.Username })
            .ToListAsync();
    }

    public async Task<User> Create(CreateUserModel model)
    {
        if (await _db.Users.AnyAsync(x => x.Username == model.Username))
            throw new Exception("Пользователь с таким логином уже существует");

        var user = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Username = model.Username
        };

        user.PasswordHash = _hasher.HashPassword(user, model.Password);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        user.PasswordHash = string.Empty;
        return user;
    }

    public async Task<User> Update(int id, UpdateUserModel model)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return null;

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Username = model.Username;
        if (!string.IsNullOrEmpty(model.Password))
            user.PasswordHash = _hasher.HashPassword(user, model.Password);

        await _db.SaveChangesAsync();
        user.PasswordHash = string.Empty;
        return user;
    }

    public async Task<User> Delete(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return null;

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return user;
    }
}
