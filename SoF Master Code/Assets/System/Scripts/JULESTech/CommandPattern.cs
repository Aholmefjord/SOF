using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JULESTech
{
    public interface ICommand
    {
        void Execute();
    }

    public class CommandMap
    {
        private Dictionary<string, ICommand> cmdMap = new Dictionary<string, ICommand>();

        public CommandMap()
        {
        }

        public void AddCommand(string _key, ICommand _newCommand)
        {
            cmdMap[_key] = _newCommand;
        }

        public void ExecuteCommand(string _key)
        {
            if (!cmdMap.ContainsKey(_key))
                return;

            cmdMap[_key].Execute();
        }
    }
}
