using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamBot.Dal.Entities;

public class UserInfo
{


    public long UserInfoId { get; set; }
    public string FirstNamee { get; set; }
    public string LastNamee { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string DateOfBirth { get; set; }

    public long UserId { get; set; }
    public BotUser BotUserr { get; set; }


}
