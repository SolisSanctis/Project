
using System;
using System.Collections.Generic;



public class Parser {
	public const int _EOF = 0;
	public const int _ident = 1;
	public const int _number = 2;
	public const int _float = 3;
	public const int _string = 4;
	public const int _Next = 5;
	public const int maxT = 56;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;
    const int
        TuringMachine_ = 1, SetOfStates_ = 2, RuleSet_ = 3,
        Int_ = 4, String_ = 5, Float_ = 6; 
    public int my_type = 0;
    public struct turingMachine
    {
        public int alphabet;
        public int rule;
        public int condition;
    }
    public string[] alph = new string[10];
    public int[] condition = new int[10];
    public string name;
    public int num;
    public struct rul
    {
        public int cond1;
        public string alph;
        public int cond2;
        public string shift;
    }
    public rul[] r = new rul[10];
    public struct var
    {
        public string name;
        public int my_type;
        public turingMachine tm;
        public string[] al;
        public int[] con;
        public rul[] r;
    }
    List<var> variab = new List<var>();
    public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}
    
	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void TURINGMACHINE() {
		SetLine();
	}

	void SetLine() {
		Expect(6);
		SetOperator();
		Expect(7);
	}

	void SetOperator() {
		Operator();
		while (la.kind == 8) {
			Get();
			Operator();
		}
	}

	void Operator() {
		if (la.kind == 9) {
			Get();
			Label();
			Expect(10);
		}
		Operation();
	}

	void Label() {
		if (la.kind == 1) {
			Ident(ref name);
		} else if (la.kind == 2) {
			Number(ref num);
		} else SynErr(57);
	}

	void Operation() {
		switch (la.kind) {
		case 11: case 15: case 17: case 20: case 21: case 23: {
			TuringMachineOperations();
			break;
		}
		case 27: {
			If();
			break;
		}
		case 40: {
			While();
			break;
		}
		case 45: {
			NewIdent();
			break;
		}
		case 1: case 41: case 42: case 43: case 44: {
			Assign();
			break;
		}
		case 53: {
			GoTo();
			break;
		}
		case 54: {
			Write();
			break;
		}
		case 55: {
			Read();
			break;
		}
		default: SynErr(58); break;
		}
	}

	void Ident(ref string name) {
        name = la.val;
		Expect(1);
	}

	void Number(ref int num) {
        num = Convert.ToInt32(la.val);
		Expect(2);
	}

	void TuringMachineOperations() {
		switch (la.kind) {
		case 15: {
			ChangeState();
			break;
		}
		case 11: {
			ChangeRule();
			break;
		}
		case 17: {
			TuringMachine();
			break;
		}
		case 20: {
			ExternalAlphabet(ref num, ref alph);
			break;
		}
		case 21: {
			SetOfStates();
			break;
		}
		case 23: {
			RuleSet();
			break;
		}
		default: SynErr(59); break;
		}
	}

	void If() {
		Expect(27);
		Expect(12);
		BooleanValue();
		Expect(14);
		SetLine();
		if (la.kind == 28) {
			Get();
			SetLine();
		}
	}

	void While() {
		Expect(40);
		BooleanValue();
		Expect(14);
		SetLine();
	}

	void NewIdent() {
        var data = new var();
		Expect(45);
		Ident(ref data.name);

		Expect(46);
		Type(ref data.my_type);
        Console.Write("Create new variable ");
        Console.Write(data.name);
        Console.Write(". Her type ");
        if (data.my_type == 1) Console.WriteLine("TuringMachine");
        else if (data.my_type == 2) Console.WriteLine("SetOfStates");
        else if (data.my_type == 3) Console.WriteLine("RuleSet");
        else if (data.my_type == 4) Console.WriteLine("Integer");
        else if (data.my_type == 5) Console.WriteLine("String");
        else if (data.my_type == 6) Console.WriteLine("Float");
    }

	void Assign() {
		if (la.kind == 41) {
			Get();
			if (la.kind == 2) {
				Get();
			}
			Expect(16);
			TuringMachine();
		} else if (la.kind == 42) {
			Get();
			if (la.kind == 2) {
				Get();
			}
			Expect(16);
			ExternalAlphabet(ref num, ref alph);
		} else if (la.kind == 43) {
			Get();
			if (la.kind == 2) {
				Get();
			}
			Expect(16);
			SetOfStates();
		} else if (la.kind == 44) {
			Get();
			if (la.kind == 2) {
				Get();
			}
			Expect(16);
			RuleSet();
		} else if (la.kind == 1) {
			Ident(ref name);
			Expect(16);
			while (la.kind == 1 || la.kind == 2) {
				Expression();
			}
			while (StartOf(1)) {
				TuringMachineOperations();
			}
		} else SynErr(60);
	}

	void GoTo() {
		Expect(53);
		Label();
	}

	void Write() {
		Expect(54);
        string text = "";
		WhatWrite(ref text);
        Console.Write("Write ");
        Console.WriteLine(text);
	}

	void Read() {
		Expect(55);
        string text = la.val;
        Expect(1);
        Console.Write("Read ");
        Console.WriteLine(text);
    }

	void ChangeState() {
		Expect(15);
		SetOfStates();
		Expect(12);
		State();
		Expect(16);
		State();
		Expect(14);
	}

	void ChangeRule() {
		Expect(11);
		RuleSet();
		Expect(12);
		rule();
		Expect(13);
		rule();
		Expect(14);
	}

	void TuringMachine() {
		Expect(17);
		if (la.kind == 2) {
			Number(ref num);
		}
		if (la.kind == 18) {
			Get();
			ExternalAlphabet(ref num, ref alph);
			Expect(19);
			SetOfStates();
			Expect(19);
			RuleSet();
			Expect(14);
		}
	}

	void ExternalAlphabet(ref int num, ref string[] alph) {
		Expect(20);
		if (la.kind == 2) {
			Number(ref num);

        }
        alph = new string[10];
		if (la.kind == 18) {
            alph[0] = la.val;
            //Console.Write(" ");
            //Console.Write(num.ToString());
            //int index = 1;
            Get();
			String();
			while (la.kind == 19) {
                //alph[index] = la.val;
                //Console.Write(" ");
                //Console.Write(num.ToString());
                //index++;
                Get();
				String();
			}
			Expect(14);
		}
        
	}

	void SetOfStates() {
		Expect(21);
		if (la.kind == 2) {
			Number(ref num);
		}
		if (la.kind == 18) {
			Get();
			State();
			while (la.kind == 19) {
				Get();
				State();
			}
			Expect(14);
		}
	}

	void RuleSet() {
		Expect(23);
		if (la.kind == 2) {
			Number(ref num);
		}
		if (la.kind == 18) {
			Get();
			rule();
			while (la.kind == 19) {
				Get();
				rule();
			}
			Expect(14);
		}
	}

	void rule() {
		Expect(12);
		State();
		if (la.kind == 1) {
			Get();
		}
		Expect(5);
		State();
		if (la.kind == 1) {
			Get();
		}
		if (la.kind == 24 || la.kind == 25 || la.kind == 26) {
			shift();
		}
		Expect(14);
	}

	void State() {
		Expect(22);
		if (la.kind == 2) {
			Number(ref num);
		}
	}

	void String() {
		Expect(4);
	}

	void shift() {
		if (la.kind == 24) {
			Get();
		} else if (la.kind == 25) {
			Get();
		} else if (la.kind == 26) {
			Get();
		} else SynErr(61);
	}

	void BooleanValue() {
		Compare();
		while (la.kind == 38 || la.kind == 39) {
			LogicOperation();
			Compare();
		}
	}

	void Compare() {
		ExpressionS();
		while (StartOf(2)) {
			RelativeOperation();
			ExpressionS();
		}
	}

	void LogicOperation() {
		if (la.kind == 38) {
			Get();
		} else if (la.kind == 39) {
			Get();
		} else SynErr(62);
	}

	void ExpressionS() {
		if (la.kind == 12) {
			Get();
			Expression();
			Expect(14);
		} else if (la.kind == 1 || la.kind == 2) {
			Expression();
		} else SynErr(63);
	}

	void RelativeOperation() {
		if (la.kind == 34) {
			Get();
		} else if (la.kind == 35) {
			Get();
		} else if (la.kind == 36) {
			Get();
		} else if (la.kind == 37) {
			Get();
		} else SynErr(64);
	}

	void Expression() {
		if (la.kind == 1) {
			Ident(ref name);
		} else if (la.kind == 2) {
			Number(ref num);
		} else SynErr(65);
		while (StartOf(3)) {
			Oper();
			if (la.kind == 1) {
				Ident(ref name);
			} else if (la.kind == 2) {
				Number(ref num);
			} else SynErr(66);
		}
	}

	void Oper() {
		if (la.kind == 29) {
			Get();
		} else if (la.kind == 30) {
			Get();
		} else if (la.kind == 31) {
			Get();
		} else if (la.kind == 32) {
			Get();
		} else if (la.kind == 33) {
			Get();
		} else SynErr(67);
	}

	void Type(ref int my_type) {
		switch (la.kind) {
		case 47: {
            my_type = 1;
            Get();
			break;
		}
		case 48: {
            my_type = 2;
            Get();
			break;
		}
		case 49: {
            my_type = 3;
            Get();
			break;
		}
		case 50: {
            my_type = 4;
            Get();
			break;
		}
		case 51: {
            my_type = 5;
            Get();
			break;
		}
		case 52: {
            my_type = 6;
            Get();
			break;
		}
		default: SynErr(68); break;
		}
	}

	void WhatWrite(ref string text) {
		if (la.kind == 1) {
            text = la.val;
			Get();
		} else if (la.kind == 4) {
            text = la.val;
            Get();
		} else SynErr(69);
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		TURINGMACHINE();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _x,_x,_x,_T, _x,_T,_x,_x, _T,_T,_x,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "ident expected"; break;
			case 2: s = "number expected"; break;
			case 3: s = "float expected"; break;
			case 4: s = "string expected"; break;
			case 5: s = "Next expected"; break;
			case 6: s = "\"{\" expected"; break;
			case 7: s = "\"}\" expected"; break;
			case 8: s = "\";\" expected"; break;
			case 9: s = "\".\" expected"; break;
			case 10: s = "\":\" expected"; break;
			case 11: s = "\"changeR\" expected"; break;
			case 12: s = "\"(\" expected"; break;
			case 13: s = "\"is\" expected"; break;
			case 14: s = "\")\" expected"; break;
			case 15: s = "\"changeS\" expected"; break;
			case 16: s = "\"=\" expected"; break;
			case 17: s = "\"#T\" expected"; break;
			case 18: s = "\":(\" expected"; break;
			case 19: s = "\",\" expected"; break;
			case 20: s = "\"#A\" expected"; break;
			case 21: s = "\"#Q\" expected"; break;
			case 22: s = "\"q\" expected"; break;
			case 23: s = "\"#P\" expected"; break;
			case 24: s = "\"L\" expected"; break;
			case 25: s = "\"R\" expected"; break;
			case 26: s = "\"N\" expected"; break;
			case 27: s = "\"if\" expected"; break;
			case 28: s = "\"else\" expected"; break;
			case 29: s = "\"+\" expected"; break;
			case 30: s = "\"-\" expected"; break;
			case 31: s = "\"*\" expected"; break;
			case 32: s = "\"^\" expected"; break;
			case 33: s = "\"/\" expected"; break;
			case 34: s = "\"==\" expected"; break;
			case 35: s = "\"<\" expected"; break;
			case 36: s = "\">\" expected"; break;
			case 37: s = "\"!=\" expected"; break;
			case 38: s = "\"||\" expected"; break;
			case 39: s = "\"&&\" expected"; break;
			case 40: s = "\"while(\" expected"; break;
			case 41: s = "\"T\" expected"; break;
			case 42: s = "\"A\" expected"; break;
			case 43: s = "\"Q\" expected"; break;
			case 44: s = "\"P\" expected"; break;
			case 45: s = "\"new\" expected"; break;
			case 46: s = "\"as\" expected"; break;
			case 47: s = "\"TuringMachine\" expected"; break;
			case 48: s = "\"SetOfStates\" expected"; break;
			case 49: s = "\"RuleSet\" expected"; break;
			case 50: s = "\"Integer\" expected"; break;
			case 51: s = "\"String\" expected"; break;
			case 52: s = "\"Float\" expected"; break;
			case 53: s = "\"goto\" expected"; break;
			case 54: s = "\"write<<\" expected"; break;
			case 55: s = "\"read>>\" expected"; break;
			case 56: s = "??? expected"; break;
			case 57: s = "invalid Label"; break;
			case 58: s = "invalid Operation"; break;
			case 59: s = "invalid TuringMachineOperations"; break;
			case 60: s = "invalid Assign"; break;
			case 61: s = "invalid shift"; break;
			case 62: s = "invalid LogicOperation"; break;
			case 63: s = "invalid ExpressionS"; break;
			case 64: s = "invalid RelativeOperation"; break;
			case 65: s = "invalid Expression"; break;
			case 66: s = "invalid Expression"; break;
			case 67: s = "invalid Oper"; break;
			case 68: s = "invalid Type"; break;
			case 69: s = "invalid WhatWrite"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
