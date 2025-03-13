using ExamBot.Dal;
using ExamBot.Dal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamBot.Bll.Services;

public class UserInfoServices : IUserInfoServices
{
    private readonly MainContext _mainContext;
    public UserInfoServices(MainContext maincontext)
    {
        _mainContext = maincontext;
    }
    public async Task<long> AddUserInfo(UserInfo userInfo)
    {
        _mainContext.UserInfos.Add(userInfo);
        _mainContext.SaveChanges();
        return userInfo.UserInfoId;
    }

    public async Task DeleteUserInfo(long Id)
    {
        var userInfo = await GetUserInfByID(Id);
        _mainContext.UserInfos.Remove(userInfo);
        _mainContext.SaveChanges();
    }

    public async Task<List<UserInfo>> GetAllUserInfos()
    {
        return _mainContext.UserInfos.ToList();
    }

    public async Task<UserInfo> GetUserInfByID(long ID)
    {
        var userInfo = _mainContext.UserInfos.FirstOrDefault(ui => ui.UserId == ID);
        if (userInfo == null)
        {
            throw new Exception("UserInfo Not Found");
        }
        return userInfo;
    }

    public async Task UpdateUserInfo(UserInfo userInfo)
    {
        var oldUserInfo = await GetUserInfByID(userInfo.UserId);
        oldUserInfo.UserInfoId = userInfo.UserId;
        oldUserInfo.Address = userInfo.Address;
        oldUserInfo.PhoneNumber = userInfo.PhoneNumber;
        oldUserInfo.DateOfBirth = userInfo.DateOfBirth;
        oldUserInfo.FirstNamee = userInfo.FirstNamee;
        oldUserInfo.LastNamee = userInfo.LastNamee;
        oldUserInfo.Email = userInfo.Email;
        _mainContext.SaveChanges();
    }
}
