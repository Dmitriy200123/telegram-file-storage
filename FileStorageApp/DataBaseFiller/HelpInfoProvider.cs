using System.Text;

namespace DataBaseFiller
{
    public class HelpInfoProvider
    {
        public string GetHelpInfo()
        {
            var sb = new StringBuilder();
            sb.Append("1)To get tables info, enter \"info\"\n");
            sb.Append("2)To add info enter \"add\"");
            return sb.ToString();
        }
    }
}