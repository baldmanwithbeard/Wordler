using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordler.Library.Commands
{
    public class ExitCommand : Command
    {
        public override void Execute()
        {
            Console.WriteLine("Exiting game...");
            Environment.Exit(0);
        }
    }
}