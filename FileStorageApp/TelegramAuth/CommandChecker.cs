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
            var splitText = text.Split(' ');
            if (splitText.Length != 2)
                return false;
            if (splitText[0] != "/start")
                return false;
            if (splitText[1].Length != 64)
                return false;
            token = splitText[1];
            return true;
        }
    }
}