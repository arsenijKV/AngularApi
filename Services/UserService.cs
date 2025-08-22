namespace WebApi.Services;
using WebApi.Models;
using WebApi.Entities;

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
    // users hardcoded for simplicity, store in a db with hashed passwords in production applications
    private readonly List<User> _users = new();


    public async Task<User> Authenticate(AuthenticateModel model)
    {
        // wrapped in "await Task.Run" to mimic fetching user from a db
        var user = _users.SingleOrDefault(x =>
            x.Username == model.Username && x.Password == model.Password);

        if (user == null)
            throw new Exception("Неверный логин или пароль");

        return await Task.FromResult(user);
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        // wrapped in "await Task.Run" to mimic fetching users from a db
        return await Task.Run(() => _users);
    }
    public User GetById(int id)
    {
        var user = _users.FirstOrDefault(x => x.Id == id);
        if (user == null) throw new Exception("Пользователь не найден");
        return user;
    }

    public async Task<User> Create(CreateUserModel model)
    {
        if (_users.Any(x => x.Username == model.Username))
            throw new Exception("Пользователь с таким логином уже существует");

        var user = new User
        {
            Id = _users.Count + 1,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Username = model.Username,
            Password = model.Password
        };

        _users.Add(user);

        return await Task.FromResult(user);


    }
    public async Task<User> Update(int id, UpdateUserModel model)
    {
        var user = GetById(id);

        if (user == null)
            return null;


        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Username = model.Username;
        if (!string.IsNullOrEmpty(model.Password))
            user.Password = model.Password;


        return await Task.FromResult(user);
    }

    public async Task<User> Delete(int id)
    {
        var user = GetById(id);

        if (user == null)
            return null;

        _users.Remove(user);

        return await Task.FromResult(user);

    }


}
