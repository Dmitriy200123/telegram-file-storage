using DataBaseFiller.Actions;

namespace DataBaseFiller
{
    public class ActionProvider
    {
        private readonly IAction _infoAction = new ShowDataBaseInfoAction();
        private readonly IAction _addAction = new AddDataBaseInfoAction();
        public IAction? ChooseAction(string command)
        {
            return command switch
            {
                "info" => _infoAction,
                "add" => _addAction,
                _ => null
            };
        }
    }
}