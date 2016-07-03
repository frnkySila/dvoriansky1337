using System;
using System.IO;

namespace VBTranslator
{

	public class VisualBasicMegaListener : VisualBasicBaseListener
	{
		readonly string OutputFilename;
		StreamWriter OutputWriter;

		public VisualBasicMegaListener(string outputFilename)
		{
			OutputFilename = outputFilename;

			OutputWriter = new StreamWriter(File.Open(OutputFilename, FileMode.Create));
			OutputWriter.AutoFlush = true;
		}

		public override void EnterFile(VisualBasicParser.FileContext context)
		{
			OutLine("using System;");
			OutLine("using System.IO;");
			OutLine("using System.Windows.Forms;");
			OutLine("");
		}

		public override void EnterDeclarationStmt(VisualBasicParser.DeclarationStmtContext context)
		{
			for(int i = 0; i < context.typeName().Length; i++) {
				string typeNameCS = "";

				string typeName = context.typeName()[i].GetText();

				switch(typeName.ToLower()) {
				case "integer":
					typeNameCS = "int";
					break;
				case "single":
					typeNameCS = "float";
					break;
				case "decimal":
					typeNameCS = "decimal";
					break;
				case "string":
					typeNameCS = "string";
					break;
				default:
					// This never happens because parser doesn't allow any types not mentioned
					break;
				}

				OutLine(typeNameCS + " " + context.ID()[i].GetText() + ";");
			}
		}

		void OutLine(string s)
		{
			OutputWriter.Write(s + "\n");
		}

		void Out(string s)
		{
			OutputWriter.Write(s);
		}
	}
}

