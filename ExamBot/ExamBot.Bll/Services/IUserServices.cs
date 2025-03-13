using ExamBot.Dal.Entities;

namespace ExamBot.Bll.Services;

public interface IUserServices
{
    Task<long> AddUser(BotUser user);
    Task DeleteUser(long Id);
    Task<BotUser> GetUserByID(long ID);
    Task UpdateUser(BotUser user);
    Task<List<BotUser>> GetAllUser();
}