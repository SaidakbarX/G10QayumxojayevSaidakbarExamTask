using ExamBot.Dal.Entities;

namespace ExamBot.Bll.Services;

public interface IUserInfoServices
{
    Task<long> AddUserInfo(UserInfo userInfo);
    Task DeleteUserInfo(long Id);
    Task<UserInfo> GetUserInfByID(long ID);
    Task UpdateUserInfo(UserInfo userInfo);
    Task<List<UserInfo>> GetAllUserInfos();
}