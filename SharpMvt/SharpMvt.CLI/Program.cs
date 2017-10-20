using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace SharpMvt.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                while (true)
                {
                    Console.Write(">");
                    string line = Console.ReadLine();

                    try
                    {
                        Execute(line.Split(' '));
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            else
            {
                Execute(args);
            }
        }

        private static void Execute(string[] args)
        {
            Jsonel.Foundation.JsonelModel model = new Jsonel.Foundation.JsonelModel();

            model.Set(args);
            model.Handle(typeof(SharpMvt.Run.RunCommand), typeof(SharpMvt.Run.RunCommandHandler));
        }
    }
}
