using System;
using System.IO;

namespace VBTranslator
{

	public class VisualBasicMegaListener : VisualBasicBaseListener
	{
		readonly string OutputFilename;
		StreamWriter OutputWriter;

		int IgnoreNextExpressions = 0;
		int CurrentLoopDepth = 0;

		bool AtLineBeginning = true;
		int CurrentIndentLevel = 0;


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
			OutLine("class Program {");
			Indent();
			OutLine("public static void Main() {");
			Indent();
		}

		public override void ExitFile(VisualBasicParser.FileContext context)
		{
			Unindent();
			OutLine("}");
			Unindent();
			OutLine("}");
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

		public override void EnterExpr(VisualBasicParser.ExprContext context)
		{
			if(IgnoreNextExpressions == 0) {
				Out(PrintExpression(context));
			}
			else {
				IgnoreNextExpressions -= 1;
			}
		}

		string PrintExpression(VisualBasicParser.ExprContext context)
		{
			return PrintExpression(context.concatExpr());
		}

		string PrintExpression(VisualBasicParser.ConcatExprContext context)
		{
			string result;

			if(context.concatExpr() != null) {
				result = String.Format("(({0}).ToString() + ({1}).ToString())",
					PrintExpression(context.concatExpr()),
					PrintExpression(context.additiveExpr())
				);
			}
			else {
				result = PrintExpression(context.additiveExpr());
			}

			return result;
		}

		string PrintExpression(VisualBasicParser.AdditiveExprContext context)
		{
			string result;

			if(context.additiveExpr() != null) {
				result = String.Format("({0} {1} {2})",
					PrintExpression(context.additiveExpr()),
					context.children[2].GetText(),
					PrintExpression(context.moduloExpr())
				);
			}
			else {
				result = String.Format("{0}", PrintExpression(context.moduloExpr()));
			}

			return result;
		}

		string PrintExpression(VisualBasicParser.ModuloExprContext context)
		{
			string result;

			// FIXME: Type problem with String

			if(context.moduloExpr() != null) {
				result = String.Format("({0} % {1})",
					PrintExpression(context.moduloExpr()),
					PrintExpression(context.integerDivExpr())
				);
			}
			else {
				result = PrintExpression(context.integerDivExpr());
			}

			return result;
		}

		string PrintExpression(VisualBasicParser.IntegerDivExprContext context)
		{
			string result;

			// FIXME: Type problem with String

			if(context.integerDivExpr() != null) {
				result = String.Format("((int)({0} + 0.5) / (int)({1} + 0.5))",
					PrintExpression(context.integerDivExpr()),
					PrintExpression(context.multiplicativeExpr())
				);
			}
			else {
				result = PrintExpression(context.multiplicativeExpr());
			}

			return result;
		}

		string PrintExpression(VisualBasicParser.MultiplicativeExprContext context)
		{
			string result;

			// FIXME: Type problem with String

			if(context.multiplicativeExpr() != null) {
				result = String.Format("({0} {1} {2})",
					PrintExpression(context.multiplicativeExpr()),
					context.children[2].GetText(),
					PrintExpression(context.negationExpr())
				);
			}
			else {
				result = String.Format("{0}", PrintExpression(context.negationExpr()));
			}

			return result;
		}

		string PrintExpression(VisualBasicParser.NegationExprContext context)
		{
			string result = PrintExpression(context.exponentiationExpr());

			// FIXME: Type problem with String

			if(context.children[0].GetText() == "-") {
				result = String.Format("(-{0})", result);
			}

			return result;
		}

		string PrintExpression(VisualBasicParser.ExponentiationExprContext context)
		{
			string result;

			// FIXME: Type problem with String

			if(context.exponentiationExpr() != null) {
				result = String.Format("Math.Pow({0}, {1})",
					PrintExpression(context.exponentiationExpr()),
					PrintExpression(context.primaryExpr())
				);
			}
			else {
				result = PrintExpression(context.primaryExpr());
			}

			return result;
		}

		string PrintExpression(VisualBasicParser.PrimaryExprContext context)
		{
			string result = String.Empty;

			if(context.children.Count >= 3) { // Rule 1: ( expr )
				result = String.Format("({0})", PrintExpression(context.concatExpr()));
			}
			else if(context.ID() != null) { // ...
				result = context.ID().GetText();
			}
			else if(context.INTEGER_CONSTANT() != null) {
				result = context.INTEGER_CONSTANT().GetText();
			}
			else if(context.FLOATING_POINT_CONSTANT() != null) {
				result = context.FLOATING_POINT_CONSTANT().GetText();
			}
			else if(context.STRING_CONSTANT() != null) {
				result = context.STRING_CONSTANT().GetText();
			}

			return result;
		}

		public override void EnterAssignmentStmt(VisualBasicParser.AssignmentStmtContext context)
		{
			Out(context.ID().GetText() + " = ");
		}

		public override void ExitAssignmentStmt(VisualBasicParser.AssignmentStmtContext context)
		{
			OutLine(";");
		}

		public override void EnterForLoopStmt(VisualBasicParser.ForLoopStmtContext context)
		{
			string varName = context.ID().GetText();

			string initStmtCS = String.Format("int {0} = {1}", varName, PrintExpression(context.expr()[0]));
			string testStmtCS = String.Format("{0} < {1}", varName, PrintExpression(context.expr()[1]));

			string incrementStmtCS;

			if(context.STEP() != null) {
				incrementStmtCS = String.Format("{0} += {1}", varName, PrintExpression(context.expr()[2]));
			}
			else {
				incrementStmtCS = String.Format("{0}++", varName);
			}

			Out(String.Format("for({0}; {1}; {2})", initStmtCS, testStmtCS, incrementStmtCS));

			OutLine(" {");
			Indent();
			CurrentLoopDepth += 1;

			if(context.STEP() != null) {
				IgnoreNextExpressions += 3;
			}
			else {
				IgnoreNextExpressions += 2;
			}
		}

		public override void ExitForLoopStmt(VisualBasicParser.ForLoopStmtContext context)
		{
			Unindent();
			OutLine("}");
			CurrentLoopDepth -= 1;
		}

		public override void EnterMsgBoxStmt(VisualBasicParser.MsgBoxStmtContext context)
		{
			Out("MessageBox.Show((");
		}

		public override void ExitMsgBoxStmt(VisualBasicParser.MsgBoxStmtContext context)
		{
			OutLine(").ToString());");
		}

		public override void EnterExitStmt(VisualBasicParser.ExitStmtContext context)
		{
			if(CurrentLoopDepth != 0)
				OutLine("break;");
			else
				PrintErrorAndExit(1001, "Exit For statement outside of loop");
		}

		void PrintErrorAndExit(int error_code, string message)
		{
			Console.WriteLine(String.Format("Error %s:", error_code));
			Console.WriteLine("\t" + message);

			OutputWriter.Close();
			File.Delete(OutputFilename);

			Environment.Exit(error_code);
		}

		void Indent()
		{
			CurrentIndentLevel += 4;
		}

		void Unindent()
		{
			CurrentIndentLevel -= 4;
		}

		string CurrentIndent()
		{
			return new string(' ', CurrentIndentLevel);
		}

		void OutLine(string s)
		{
			if(AtLineBeginning) {
				OutputWriter.Write(CurrentIndent());
			}

			OutputWriter.Write(s + "\n");

			AtLineBeginning = true;
		}

		void Out(string s)
		{
			if(AtLineBeginning) {
				OutputWriter.Write(CurrentIndent());
				AtLineBeginning = false;
			}

			OutputWriter.Write(s);
		}
	}
}

