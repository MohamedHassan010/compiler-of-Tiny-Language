using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASON_Compiler
{

    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        public bool flag1 = false, isFoundReturn=false;
        int InputPointer = 0;

        List<Token> TokenStream;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");          
            root.Children.Add(Program());
            return root;
        }
        Node FunctionDeclaration()
        {
            Node declaration = new Node("Function_declaration");
            declaration.Children.Add(DataType());
            declaration.Children.Add(FnName());
            declaration.Children.Add(match(Token_Class.LParanthesis));
            declaration.Children.Add(Parameters());
            declaration.Children.Add(match(Token_Class.RParanthesis));

            return declaration;
        }
        Node RepeatState()
        {
            Node Repeat_stat = new Node("Repeat Statement");
            Repeat_stat.Children.Add(match(Token_Class.Repeat));
            Repeat_stat.Children.Add(Statements(false));
            Repeat_stat.Children.Add(match(Token_Class.Until));
            Repeat_stat.Children.Add(Condition_Statement());
            return Repeat_stat;
        }
        Node ReturnState()
        {

            Node Return_stat = new Node("Return_Statement");

            Return_stat.Children.Add(match(Token_Class.Return));
            Return_stat.Children.Add(Expression());
            Return_stat.Children.Add(match(Token_Class.Semicolon));
           // isFoundReturn = true;
            return Return_stat;
        }
        Node Condition_Statement()
        {
            Node condition_stat = new Node("Condition_Statement");
            condition_stat.Children.Add(Condition());
            condition_stat.Children.Add(Condition_State());
            return condition_stat;
        }
        Node Condition()
        {
            Node condition = new Node("Condition");
            condition.Children.Add(match(Token_Class.Identifier));
            condition.Children.Add(CondOp());
            condition.Children.Add(Term());
            return condition;
        }
        Node Condition_State()
        {
            Node condition_state = new Node("Condition_State");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.AndOperator || TokenStream[InputPointer].token_type == Token_Class.OrOperator))
            {
                condition_state.Children.Add(Bool_op());
                condition_state.Children.Add(Condition());
                condition_state.Children.Add(Condition_State());
            }
            else return null;
            return condition_state;
        }
        Node Bool_op()
        {
            Node bool_op = new Node("Boolean_Operator");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.AndOperator)
                bool_op.Children.Add(match(Token_Class.AndOperator));
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.OrOperator)
                bool_op.Children.Add(match(Token_Class.OrOperator));

            return bool_op;
        }
        Node CondOp()
        {
            Node condOp = new Node("CondOp");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
            {
                condOp.Children.Add(match(Token_Class.LessThanOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
            {
                condOp.Children.Add(match(Token_Class.GreaterThanOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.EqualConOp)
            {
                condOp.Children.Add(match(Token_Class.EqualConOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.NotEqualOp)
            {
                condOp.Children.Add(match(Token_Class.NotEqualOp));
            }

            return condOp;
        }
        Node Term()
        {
            Node term = new Node("Term");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Number)
                term.Children.Add(match(Token_Class.Number));
            else if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Identifier && InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis))
                term.Children.Add(funcationCall());
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Identifier)
                term.Children.Add(match(Token_Class.Identifier));


            return term;
        }
        Node ID()
        {
            Node ID = new Node("ID");

            if (TokenStream[InputPointer].token_type == Token_Class.Identifier && InputPointer < TokenStream.Count)
            {
                ID.Children.Add(match(Token_Class.Identifier));
                ID.Children.Add(Commadash());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Number && InputPointer < TokenStream.Count)
            {
                ID.Children.Add(match(Token_Class.Number));
                ID.Children.Add(Commadash());
            }


            return ID;
        }        
        Node Commadash()
        {
            Node commadash = new Node("Commadash");

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                commadash.Children.Add(match(Token_Class.Comma));
                if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Identifier
                && InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
                {
                    commadash.Children.Add(funcationCall());
                }
                else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Identifier)
                    commadash.Children.Add(match(Token_Class.Identifier));
                else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Number)
                    commadash.Children.Add(match(Token_Class.Number));
                commadash.Children.Add(Commadash());
            }
            else return null;

            return commadash;
        
  
        }
        Node ArthematicOperation()
        {
            Node A_op = new Node("ArthematicOperation");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.PlusOp)
                A_op.Children.Add(match(Token_Class.PlusOp));
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.MinusOp)
                A_op.Children.Add(match(Token_Class.MinusOp));
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.DivideOp)
                A_op.Children.Add(match(Token_Class.DivideOp));
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
                A_op.Children.Add(match(Token_Class.MultiplyOp));
            
            return A_op;
        }
        Node Equation()
        {
            Node equation = new Node("Equation");

            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Identifier) && (TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis))
            {
                equation.Children.Add(funcationCall());
                equation.Children.Add(Equationdash());
            }
            else if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Identifier))
            {
                equation.Children.Add(match(Token_Class.Identifier));
                equation.Children.Add(Equationdash());
            }

            else if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Number))
            {
                equation.Children.Add(match(Token_Class.Number));
                equation.Children.Add(Equationdash());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
            {
                equation.Children.Add(match(Token_Class.LParanthesis));
                equation.Children.Add(Equation());
                equation.Children.Add(match(Token_Class.RParanthesis));
                equation.Children.Add(Equationdash());


            }
            else return null;
            
                     
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type != Token_Class.Semicolon))
                equation.Children.Add(Equation());
            return equation;
        }
        Node Equationdash()
        {
            Node equationdash = new Node("Equationdash");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.PlusOp || TokenStream[InputPointer].token_type == Token_Class.MinusOp
            || TokenStream[InputPointer].token_type == Token_Class.DivideOp || TokenStream[InputPointer].token_type == Token_Class.MultiplyOp))
            {
                equationdash.Children.Add(ArthematicOperation());
                equationdash.Children.Add(Equationdash());
            }
            else
                return null;           
            return equationdash;
        }
        Node Expression()
        {
            Node expression = new Node("Expression");
            
                if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.String))
                {
                    expression.Children.Add(match(Token_Class.String));
                }
                else if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Identifier || TokenStream[InputPointer].token_type == Token_Class.Number))
                {
                    if (InputPointer + 1 < TokenStream.Count && (TokenStream[InputPointer + 1].token_type == Token_Class.PlusOp
                    || TokenStream[InputPointer + 1].token_type == Token_Class.MinusOp
                    || TokenStream[InputPointer + 1].token_type == Token_Class.DivideOp
                    || TokenStream[InputPointer + 1].token_type == Token_Class.MultiplyOp))
                        expression.Children.Add(Equation());
                    else
                        expression.Children.Add(Term());

                }
                else if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.LParanthesis))
                {
                    expression.Children.Add(Equation());
                }
               
            return expression;

        }
        Node DataType()
        {
            Node datatype = new Node("DataType");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Data_Type_INT)
                datatype.Children.Add(match(Token_Class.Data_Type_INT));
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.String)
                datatype.Children.Add(match(Token_Class.String));
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Data_Type_Float)
                datatype.Children.Add(match(Token_Class.Data_Type_Float));
          
            return datatype;
        }
        Node AssigementState()
        {
            Node assigementstate = new Node("AssigementState");            
            assigementstate.Children.Add(match(Token_Class.Identifier));           
            assigementstate.Children.Add(match(Token_Class.Assign));
            assigementstate.Children.Add(Expression());
         
            return assigementstate;
        }
        Node WriteStatement()
        {
            Node write = new Node("write");
            write.Children.Add(match(Token_Class.Write));
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.EndL))
            {
                write.Children.Add(match(Token_Class.EndL));
            }
            else
                write.Children.Add(Expression());

             write.Children.Add(match(Token_Class.Semicolon));

            return write;

        }
        Node declare_statement()
        {
            Node declare_statement = new Node("declare_statement");
            declare_statement.Children.Add(DataType());
            if (InputPointer < TokenStream.Count && Token_Class.Identifier == TokenStream[InputPointer].token_type &&
                InputPointer + 1 < TokenStream.Count && Token_Class.Assign == TokenStream[InputPointer + 1].token_type)
            {
                declare_statement.Children.Add(AssigementState());
                
            }
            else if (InputPointer < TokenStream.Count && Token_Class.Identifier == TokenStream[InputPointer].token_type)
            {
                declare_statement.Children.Add(match(Token_Class.Identifier));
            }
            declare_statement.Children.Add(Declartion());

            declare_statement.Children.Add(match(Token_Class.Semicolon));
            return declare_statement;
        }      
        Node Declartion()
        {
            Node declartion = new Node("declartion");
            if (InputPointer < TokenStream.Count && Token_Class.Comma == TokenStream[InputPointer].token_type)
            {
                declartion.Children.Add(match(Token_Class.Comma));
                if (InputPointer < TokenStream.Count && Token_Class.Identifier == TokenStream[InputPointer].token_type &&
               InputPointer + 1 < TokenStream.Count && Token_Class.Assign == TokenStream[InputPointer + 1].token_type)
                {
                    declartion.Children.Add(AssigementState());

                }
                else if (InputPointer < TokenStream.Count && Token_Class.Identifier == TokenStream[InputPointer].token_type)
                {
                    declartion.Children.Add(match(Token_Class.Identifier));
                }
                declartion.Children.Add(Declartion());
            }
            else return null;
            return declartion;
            
        }
        Node ReadState()
        {
            Node Read = new Node("ReadState");
            Read.Children.Add(match(Token_Class.Read));
            Read.Children.Add(match(Token_Class.Identifier));
            Read.Children.Add(match(Token_Class.Semicolon));
            return Read;
        }
        Node FnName()
        {
            Node fnName = new Node("FnName");
            fnName.Children.Add(match(Token_Class.Identifier));
            return fnName;
        }
        Node IfState()
        {
            Node if_stat = new Node("IF Statement");
            if_stat.Children.Add(match(Token_Class.If));
            if_stat.Children.Add(Condition_Statement());
            if_stat.Children.Add(match(Token_Class.Then));
            if_stat.Children.Add(Statements(false));
            if_stat.Children.Add(ElseIfClosure());
            return if_stat;
        }
        Node ElseIfState()
        {
            Node elseIfState = new Node("ElseIfState");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Else_If)
            {
                elseIfState.Children.Add(match(Token_Class.Else_If));
                elseIfState.Children.Add(Condition_Statement());
                elseIfState.Children.Add(match(Token_Class.Then));
                elseIfState.Children.Add(Statements(false));
                elseIfState.Children.Add(ElseIfClosure());
            }

            return elseIfState;
        }
        Node ElseState()
        {
            Node elseState = new Node("ElseState");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Else)
            {
                elseState.Children.Add(match(Token_Class.Else));
                elseState.Children.Add(Statements(false));
                elseState.Children.Add(match(Token_Class.End));
            }

            return elseState;

        }
        Node ElseIfClosure()
        {
            Node else_if_stat = new Node("Else IF Closure");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Else_If)
                else_if_stat.Children.Add(ElseIfState());
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Else)
                else_if_stat.Children.Add(ElseState());
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.End)
                else_if_stat.Children.Add(match(Token_Class.End));
            return else_if_stat;
        }
        Node Parameters()
        {
            Node parameters = new Node("parameters");

            parameters.Children.Add(Parameter());
            parameters.Children.Add(DashParameter());

            return parameters;
        }
        Node Parameter()
        {
            Node Parameter = new Node("parameter");

            Parameter.Children.Add(DataType());
            Parameter.Children.Add(match(Token_Class.Identifier));

            return Parameter;
        }
        Node DashParameter()
        {
            Node dashparameter = new Node("dashparameter");
            if (InputPointer < TokenStream.Count && Token_Class.Comma == TokenStream[InputPointer].token_type)
            {
                dashparameter.Children.Add(match(Token_Class.Comma));
                dashparameter.Children.Add(Parameter());
                dashparameter.Children.Add(DashParameter());
            }
            else 
                return null;

            return dashparameter;
        }
        Node FunctionBody()
        {
            Node functionbody = new Node("functionbody");
            functionbody.Children.Add(match(Token_Class.Left_CurlyBracket));
            functionbody.Children.Add(Statements(false));
            if(!isFoundReturn)
            functionbody.Children.Add(ReturnState());// check again
            functionbody.Children.Add(match(Token_Class.Right_CurlyBracket));
            return functionbody;
        }        
        Node Statements(bool flag)
        {
            Node statements = new Node("Statements");                       
            statements.Children.Add(Statement(ref flag));
            if(flag==true)
            statements.Children.Add(Statements(false));
          else  
                return null;
            return statements;
        }     
        Node Statement(ref bool flag)
        {
            Node statement = new Node("Statement");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Read)
            {
                statement.Children.Add(ReadState());
                flag = true;
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Write)
            {
                flag = true;
                statement.Children.Add(WriteStatement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Identifier && InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.Assign)
            {
                flag = true;
                statement.Children.Add(AssigementState());
                statement.Children.Add(match(Token_Class.Semicolon));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Identifier
                && InputPointer+1 < TokenStream.Count && TokenStream[InputPointer+1].token_type == Token_Class.LParanthesis)
            {
                flag = true;
                statement.Children.Add(funcationCall());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comment)
            {
                flag = true;
                statement.Children.Add(match(Token_Class.Comment));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Repeat)
            {
                flag = true;
                statement.Children.Add(RepeatState());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Return)
            {
                flag = true;
                statement.Children.Add(ReturnState());
                if (InputPointer  < TokenStream.Count && TokenStream[InputPointer ].token_type == Token_Class.Right_CurlyBracket)
                    isFoundReturn = true;
            }
          
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.If)
            {
                flag = true;
                statement.Children.Add(IfState());
            }
            else if (InputPointer < TokenStream.Count && (
                TokenStream[InputPointer].token_type == Token_Class.Data_Type_INT 
                || TokenStream[InputPointer].token_type == Token_Class.Data_Type_Float
                || TokenStream[InputPointer].token_type == Token_Class.String))
            {
                flag = true;
                statement.Children.Add(declare_statement());
            }
           else
            {
                return null;
            }
            return statement;
        }
        Node FunctionStatment()
        {
            Node functionstatment = new Node("functionstatment");
            functionstatment.Children.Add(FunctionDeclaration());
            functionstatment.Children.Add(FunctionBody());
            //if (functionstatment.Children.Count != 0) RepeatFunStatment = true;
            return functionstatment;
        }
        Node funcationCall()
        {
            Node fn_Call = new Node("Fn_Call");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                fn_Call.Children.Add(match(Token_Class.Identifier));
                fn_Call.Children.Add(match(Token_Class.LParanthesis));
                fn_Call.Children.Add(ID());
                fn_Call.Children.Add(match(Token_Class.RParanthesis));
            }
            return fn_Call;
        }
        Node MainFunction()
        {
            Node mainfunction = new Node("mainfunction");
            mainfunction.Children.Add(DataType());
            mainfunction.Children.Add(match(Token_Class.main));
            mainfunction.Children.Add(match(Token_Class.LParanthesis));
            mainfunction.Children.Add(match(Token_Class.RParanthesis));
            mainfunction.Children.Add(FunctionBody());
            
                return mainfunction;
        }
        Node Program()
        {
            Node program = new Node("program");
            while(TokenStream[InputPointer].token_type == Token_Class.Comment)
            {
                program.Children.Add(match(Token_Class.Comment));
            }
            if (TokenStream[InputPointer + 1].token_type != Token_Class.main)
            {
                program.Children.Add(FunStatment());

            }
            if(InputPointer<TokenStream.Count)
            {
                while (TokenStream[InputPointer].token_type == Token_Class.Comment)
                {
                    program.Children.Add(match(Token_Class.Comment));
                }
                program.Children.Add(MainFunction());
            }
          
            MessageBox.Show("Success");
            return program;
        }
        Node FunStatment()
        {
            Node funstatment = new Node("funstatment");
           
            funstatment.Children.Add(FunctionStatment());
            if (InputPointer+1<TokenStream.Count&&TokenStream[InputPointer + 1].token_type != Token_Class.main)
                funstatment.Children.Add(FunStatment());

            return funstatment;
        }      
        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }
        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
