using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wordler.Library.Commands;

namespace Wordler.Library
{
    public class CommandFactory
    {
        public Command CreateCommand(string userInput) =>
            userInput switch
            {
                "help" => new HelpCommand(),
                "exit" => new ExitCommand(),
                _ => new DefaultCommand()
            };
    }
}