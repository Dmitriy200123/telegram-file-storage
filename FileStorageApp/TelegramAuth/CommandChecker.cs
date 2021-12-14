using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramAuth
{
    public static class CommandChecker
    {
        public static bool IsCommandValid(Update update, out string? token)
        {
            token = null;
            if (update.Type != UpdateType.Message || update.Message!.Type != MessageType.Text)
                return false;
            var text = update.Message.Text;
            if (text is null)
                return false;
            var splittedText = text.Split(' ');
            if (splittedText.Length != 2)
                return false;
            if (splittedText[0] != "/start")
                return false;
            token = splittedText[1];
            return true;
        }
    }
}