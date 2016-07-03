using System;

namespace VBTranslator
{

	public class VisualBasicMegaListener : VisualBasicBaseListener
	{
		readonly string OutputFilename;

		public VisualBasicMegaListener(string outputFilename)
		{
			OutputFilename = outputFilename;
		}
	}
}

