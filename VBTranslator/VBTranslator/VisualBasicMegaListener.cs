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
			OutLine(context.GetText());
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

