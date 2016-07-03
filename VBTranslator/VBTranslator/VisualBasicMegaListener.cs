using System;
using System.IO;
using System.Collections.Generic;

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

		Dictionary<string, string> DeclaredVars = new Dictionary<string, string>();

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

			OutLine("static double G(string s) { double res = 0; double.TryParse(s, out res); return res; }");
			OutLine("static double G(float f) { return (double)f; }");
			OutLine("static double G(double d) { return d; }");
			OutLine("static double G(decimal d) { return (double)d; }");
			OutLine("static double G(int i) { return (double)i; }");
			OutLine("");

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
				string varId = context.ID()[i].GetText();

				if(DeclaredVars.ContainsKey(varId)) {
					PrintErrorAndExit(1002, String.Format("Variable {0} was already declared", varId));
				}

				string typeDeclCS = "";

				string typeName = context.typeName()[i].GetText();

				switch(typeName.ToLower()) {
				case "integer":
					typeDeclCS = "int {0} = 0;";
					break;
				case "single":
					typeDeclCS = "float {0} = 0.0f;";
					break;
				case "decimal":
					typeDeclCS = "decimal {0} = 0m;";
					break;
				case "string":
					typeDeclCS = "string {0} = \"\";";
					break;
				default:
					// This never happens because parser doesn't allow any types not mentioned
					break;
				}

				DeclaredVars[varId] = typeName.ToLower();

				OutLine(String.Format(typeDeclCS, varId));
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
				result = String.Format("(G({0}) {1} G({2}))",
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
				result = String.Format("(G({0}) % G({1}))",
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
				result = String.Format("((int)(G({0}) + 0.5) / (int)(G({1}) + 0.5))",
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
				result = String.Format("(G({0}) {1} G({2}))",
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
				result = String.Format("(-G({0}))", result);
			}

			return result;
		}

		string PrintExpression(VisualBasicParser.ExponentiationExprContext context)
		{
			string result;

			// FIXME: Type problem with String

			if(context.exponentiationExpr() != null) {
				result = String.Format("Math.Pow(G({0}), G({1}))",
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
				result = context.FLOATING_POINT_CONSTANT().GetText() + "f";
			}
			else if(context.STRING_CONSTANT() != null) {
				result = context.STRING_CONSTANT().GetText();
			}

			return result;
		}

		public override void EnterAssignmentStmt(VisualBasicParser.AssignmentStmtContext context)
		{
			string varName = context.ID().GetText();

			Out(varName + " = ");

			if(DeclaredVars[varName] == "string") {
				Out("(");
			}
			else if(DeclaredVars[varName] == "integer") {
				Out("(int)("); 
			}
			else if(DeclaredVars[varName] == "decimal") {
				Out("(decimal)("); 
			}
		}

		public override void ExitAssignmentStmt(VisualBasicParser.AssignmentStmtContext context)
		{
			string varName = context.ID().GetText();

			if(DeclaredVars[varName] == "string") {
				Out(").ToString()");
			}
			else if(DeclaredVars[varName] == "integer") {
				Out(")"); 
			}
			else if(DeclaredVars[varName] == "decimal") {
				Out(")"); 
			}

			OutLine(";");
		}

		public override void EnterForLoopStmt(VisualBasicParser.ForLoopStmtContext context)
		{
			string varName = context.ID().GetText();

			string initStmtCS = String.Format("{0}{1} = (int)({2})",
				DeclaredVars.ContainsKey(varName) ? "" : "int ",
				varName,
				PrintExpression(context.expr()[0]));
			
			string testStmtCS, incrementStmtCS;

			if(context.STEP() != null) {
				testStmtCS = String.Format("{2} > 0 ? {0} <= {1} : {0} >= {1}",
					varName,
					PrintExpression(context.expr()[1]),
					PrintExpression(context.expr()[2]));
				incrementStmtCS = String.Format("{0} += {1}", varName, PrintExpression(context.expr()[2]));
			}
			else {
				testStmtCS = String.Format("{0} <= {1}", varName, PrintExpression(context.expr()[1]));
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
			Console.Write(String.Format("Error VB{0}:", error_code));
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

