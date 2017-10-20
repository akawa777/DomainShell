using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Xml;

namespace SharpMvt.Run
{
    public class RunCommand : TranspileOptions, Jsonel.Foundation.ICommand
    {
        public Jsonel.Foundation.IJsonelModel JsonelModel { get; set; }
        public override string BaseDir
        {
            get
            {
                return JsonelModel.JsonFileInfo.DirectoryName;
            }
            set
            {

            }
        }
    }

    public class RunCommandHandler : Jsonel.Foundation.ICommandHandler<RunCommand>
    {
        public void Handle(RunCommand command)
        {
            command.BaseDir = command.JsonelModel.JsonFileInfo.DirectoryName;

            TypeScriptTranspiler typeScriptTranspiler = new TypeScriptTranspiler();

            string code = typeScriptTranspiler.Transpile(command);

            string destFile = Path.GetFullPath(Path.Combine(command.BaseDir, command.DestFile));            

            Console.WriteLine("export " + destFile);

            using (StreamWriter writer = new StreamWriter(destFile, false, System.Text.Encoding.UTF8))
            {
                writer.Write(code);
            }
        }
    }
}
