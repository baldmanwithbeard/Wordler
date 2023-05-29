using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordler.Library.Commands
{
    public class DefaultCommand : Command
    {
        public override void Execute()
        {
            Console.WriteLine("Command not recognized. Please enter a valid command.");
        }
    }
}