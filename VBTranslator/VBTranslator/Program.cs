using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace VBTranslator
{

	class MainClass
	{
		public static void Main(string[] args)
		{
			string input = File.ReadAllText(args[0]);

			var ms = new MemoryStream(Encoding.UTF8.GetBytes(input));
			var lexer = new VisualBasicLexer(new AntlrInputStream(ms));

			var tokens = new CommonTokenStream(lexer);

			var parser = new VisualBasicParser(tokens);

			var tree = parser.file();

			var pastwk = new ParseTreeWalker();

			pastwk.Walk(new VisualBasicMegaListener(), tree);
		}
	}
}
