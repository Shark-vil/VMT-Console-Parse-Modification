using System;
using VmtConsoleParseModification.Modules;
using VmtConsoleParseModification.Templates;

namespace VmtConsoleParseModification
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Write to vmt file path:");
            string VmtFilePath = Console.ReadLine();

            VmtTemplate VmtObject = VmtParser.ParseVmtFile(VmtFilePath);

            Console.Read();
        }
    }
}
