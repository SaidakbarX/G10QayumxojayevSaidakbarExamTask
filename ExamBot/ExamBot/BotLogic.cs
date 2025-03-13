using ExamBot.Dal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using ExamBot.Bll.Services;

namespace ExamBot;

public class BotLogic
{
    private static string BotToken = "7568090052:AAFRknLsHJ9WvqhkOaCoC5GOI3vV-1zrNcU";
    private long AdminID = 1151741183;

    private List<long> allChat = new List<long>();

    private TelegramBotClient BotClient = new TelegramBotClient(BotToken);

    private Dictionary<long, string> UserForUserInfo = new Dictionary<long, string>();

    private Dictionary<long, UserInfo> UserInfos = new Dictionary<long, UserInfo>();

    private readonly IUserServices _userService;
    private readonly IUserInfoServices _userInfoService;

    public BotLogic(IUserInfoServices userInfoService, IUserServices userService)
    {
        _userInfoService = userInfoService;
        _userService = userService;
    }
    // shirift uchun 
    public string EscapeMarkdownV2(string text)
    {
        string[] specialChars = { "[", "]", "(", ")", ">", "#", "+", "-", "=", "|", "{", "}", ".", "!" };
        foreach (var ch in specialChars)
        {
            text = text.Replace(ch, "\\" + ch);
        }
        return text;
    }

    // validatsiya
    private bool ValidateFNameAndLName(string name)
    {
        foreach (var l in name)
        {
            if (!char.IsLetter(l) || l == ' ')
            {
                return false;
            }
        }
        return !string.IsNullOrEmpty(name) && name.Length <= 50;
    }
    private bool ValidatePhone(string phone)
    {
        foreach (var l in phone)
        {
            if (!char.IsDigit(l) || l == ' ')
            {
                return false;
            }
        }
        return phone.Length == 9;
    }
    private bool ValidateEmail(string email)
    {
        email.ToLower();

        return email.EndsWith("@gmail.com") && !string.IsNullOrEmpty(email) && email.Length <= 200 && email.Length > 10;
    }
    public async Task StartBot()
    {
        var receiverOptions = new ReceiverOptions { AllowedUpdates = new[] { UpdateType.Message, UpdateType.InlineQuery } };

        Console.WriteLine("Your bot is starting");

        BotClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions
            );

        Console.ReadKey();
    }



    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {

        if (update.Type == UpdateType.Message)
        {

            var message = update.Message;
            var user = message.Chat;
            BotUser userObject;
            try
            {
                userObject = await _userService.GetUserByID(user.Id);
            }
            catch (Exception ex)
            {
                userObject = null;
            }

            Console.WriteLine($"{user.Id},  {user.FirstName}, {message.Text}");
            // bu notificatsion
            if (message.Text == "AllChat")
            {
                if (userObject.TelegramUserId == AdminID)
                {
                    await bot.SendTextMessageAsync(user.Id, "So'zni Kiriting : ", cancellationToken: cancellationToken);
                    allChat.Add(AdminID);
                }
            }
            else if (allChat.Contains(AdminID))
            {
                var users = await _userService.GetAllUser();
                foreach (var u in users)
                {
                    await bot.SendTextMessageAsync(u.TelegramUserId, message.Text, cancellationToken: cancellationToken);
                }
            }

            if (message.Text == "Fill data")
            {
                if (userObject.UserInfo is null)
                {
                    try
                    {
                        UserInfos.Add(user.Id, new UserInfo());
                        UserForUserInfo.Add(user.Id, "Fir");
                    }
                    catch (Exception ex)
                    {
                        UserInfos.Remove(user.Id);
                        UserInfos.Add(user.Id, new UserInfo());

                        UserForUserInfo.Remove(user.Id);
                        UserForUserInfo.Add(user.Id, "Fir");
                    }

                    await bot.SendTextMessageAsync(user.Id, "Enter first name : ", cancellationToken: cancellationToken);
                }
                else if (userObject.UserInfo is not null)
                {
                    var userInformation = await _userInfoService.GetUserInfByID(userObject.BotUserId);//userID
                    var userInfo = $"~You Has Already Have Informations~\n\n*Fir* : _{userInformation.FirstNamee}_\n" +
                        $"*LastName* : _{userInformation.LastNamee}_\n" +
                        $"*Email* : {userInformation.Email}\n" +
                        $"*PhoneNumber* : {userInformation.PhoneNumber}\n" +
                        $"*Adress* : `{userInformation.Address}`\n" +
                        $"*DateOfBirth* : *{userInformation.DateOfBirth}*";

                    await bot.SendTextMessageAsync(user.Id, EscapeMarkdownV2(userInfo), cancellationToken: cancellationToken, parseMode: ParseMode.MarkdownV2);
                    return;
                }
            }
            else if (UserForUserInfo.ContainsKey(user.Id) && UserForUserInfo[user.Id] == "Fir")
            {
                var validate = ValidateFNameAndLName(message.Text);
                if (!validate)
                {
                    await bot.SendTextMessageAsync(user.Id, "Enter your first name correctly !!!", cancellationToken: cancellationToken);
                    return;
                }
                var info = UserInfos[user.Id];
                info.FirstNamee = message.Text;
                var ch = info.FirstNamee[0];
                info.FirstNamee = info.FirstNamee.Remove(0, 1);
                info.FirstNamee = char.ToUpper(ch) + info.FirstNamee;
                UserForUserInfo[user.Id] = "Las";
                await bot.SendTextMessageAsync(user.Id, "Enter your last name : ", cancellationToken: cancellationToken);
            }

            else if (UserForUserInfo.ContainsKey(user.Id) && UserForUserInfo[user.Id] == "Las")
            {
                var validate = ValidateFNameAndLName(message.Text);
                if (!validate)
                {
                    await bot.SendTextMessageAsync(user.Id, "Please enter your last name correctly!!!", cancellationToken: cancellationToken);
                    return;
                }
                var info = UserInfos[user.Id];
                info.LastNamee = message.Text;
                var ch = info.LastNamee[0];
                info.LastNamee = info.LastNamee.Remove(0, 1);
                info.LastNamee = char.ToUpper(ch) + info.LastNamee;
                UserForUserInfo[user.Id] = "Ema";
                await bot.SendTextMessageAsync(user.Id, "Enter your email : ", cancellationToken: cancellationToken);
            }

            else if (UserForUserInfo.ContainsKey(user.Id) && UserForUserInfo[user.Id] == "Ema")
            {
                var validate = ValidateEmail(message.Text);
                if (!validate)
                {
                    await bot.SendTextMessageAsync(user.Id, "Please enter your email correctly!!", cancellationToken: cancellationToken);
                    return;
                }
                var info = UserInfos[user.Id];
                info.Email = message.Text;
                info.Email.ToLower();
                UserForUserInfo[user.Id] = "Pho";
                await bot.SendTextMessageAsync(user.Id, "Enter telephone number (in 909009090 format):", cancellationToken: cancellationToken);
            }

            else if (UserForUserInfo.ContainsKey(user.Id) && UserForUserInfo[user.Id] == "Pho")
            {
                var validate = ValidatePhone(message.Text);
                if (!validate)
                {
                    await bot.SendTextMessageAsync(user.Id, "Please enter your phone number correctly !!", cancellationToken: cancellationToken);
                    return;
                }
                var info = UserInfos[user.Id];
                info.PhoneNumber = message.Text;
                info.PhoneNumber = "+998" + info.PhoneNumber;
                UserForUserInfo[user.Id] = "Adr";
                await bot.SendTextMessageAsync(user.Id, "Enter your address : ", cancellationToken: cancellationToken);
            }

            else if (UserForUserInfo.ContainsKey(user.Id) && UserForUserInfo[user.Id] == "Adr")
            {
                if (message.Text.Length > 200 && !string.IsNullOrEmpty(message.Text))
                {
                    await bot.SendTextMessageAsync(user.Id, "Please enter the address correctly !!", cancellationToken: cancellationToken);
                    return;
                }
                var info = UserInfos[user.Id];
                info.Address = message.Text;
                UserForUserInfo[user.Id] = "Dat";
                await bot.SendTextMessageAsync(user.Id, "Enter date of birth : ", cancellationToken: cancellationToken);
            }

            else if (UserForUserInfo.ContainsKey(user.Id) && UserForUserInfo[user.Id] == "Dat")
            {
                var info = UserInfos[user.Id];
                info.UserId = userObject.BotUserId;
                info.DateOfBirth = message.Text;

                await _userInfoService.AddUserInfo(info);

                UserInfos.Remove(user.Id);
                UserForUserInfo.Remove(user.Id);
                await bot.SendTextMessageAsync(user.Id, "Information Added", cancellationToken: cancellationToken);
            }




            if (message.Text == "Get data")
            {
                UserInfo userInformation;
                try
                {
                    userInformation = await _userInfoService.GetUserInfByID(userObject.BotUserId);//userID
                }
                catch (Exception ex)
                {
                    await bot.SendTextMessageAsync(user.Id, "User info not found", cancellationToken: cancellationToken);
                    return;
                }

                var userInfo = $"*Ism* : _{userInformation.FirstNamee}_\n" +
                    $"*Familiya* : _{userInformation.LastNamee}_\n" +
                    $"*Email* : {userInformation.Email}\n" +
                    $"*PhoneNumber* : {userInformation.PhoneNumber}\n" +
                    $"*Adress* : `{userInformation.Address}`\n" +
                    $"*Summary* : *{userInformation.DateOfBirth}*";

                await bot.SendTextMessageAsync(user.Id, EscapeMarkdownV2(userInfo), cancellationToken: cancellationToken, parseMode: ParseMode.MarkdownV2);
            }


            if (message.Text == "Delete data")
            {
                var userInformation = await _userService.GetUserByID(user.Id);//tguid
                if (userInformation.UserInfo is null)
                {
                    await bot.SendTextMessageAsync(user.Id, "To turn off informationn\nAdd information first", cancellationToken: cancellationToken);
                    return;
                }
                else
                {
                    await _userInfoService.DeleteUserInfo(userInformation.UserInfo.UserId);

                    await bot.SendTextMessageAsync(user.Id, "exempt disabled", cancellationToken: cancellationToken);
                }
            }

            if (message.Text == "/start")
            {

                if (userObject == null)
                {
                    userObject = new BotUser()
                    {
                        CreatedAt = DateTime.UtcNow,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        IsBlocked = false,
                        PhoneNumberr = null,
                        TelegramUserId = user.Id,
                        UpdatedAt = DateTime.UtcNow,
                        Username = user.Username
                    };


                    await _userService.AddUser(userObject);
                }
                else
                {
                    if (user.FirstName != userObject.FirstName || user.LastName != userObject.LastName || user.Username != userObject.Username)
                    {
                        userObject.UpdatedAt = DateTime.UtcNow;
                    }
                    ;
                    userObject.FirstName = user.FirstName;
                    userObject.LastName = user.LastName;
                    userObject.Username = user.Username;
                    await _userService.UpdateUser(userObject);
                }

                var keyboard = new ReplyKeyboardMarkup(new[]
            {
                    new[]
                    {
                        new KeyboardButton("Fill data"),
                        new KeyboardButton("Get data"),
                    },
                    new[]
                    {
                        new KeyboardButton("Delete data"),
                    },
                })
                { ResizeKeyboard = true };

                await bot.SendTextMessageAsync(user.Id, "Hello 👋", replyMarkup: keyboard);
                return;
            }
        }
        else if (update.Type == UpdateType.CallbackQuery)
        {
            var id = update.CallbackQuery.From.Id;

            var text = update.CallbackQuery.Data;

            CallbackQuery res = update.CallbackQuery;
        }
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(exception.Message);
    }
}
