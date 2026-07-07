using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace lab2_2
{
    public partial class Form1 : Form
    {
        private string currentExpression = "";
        private bool isScientificMode = false;
        private bool isDegrees = true;
        private bool justCalculated = false;
        private bool isError = false;

        public Form1()
        {
            InitializeComponent();
            CreateCalculator();
        }

        private void CreateCalculator()
        {
            this.Text = "Калькулятор (Научный)";
            this.Size = new System.Drawing.Size(750, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Display
            TextBox display = new TextBox();
            display.Name = "display";
            display.Font = new System.Drawing.Font("Consolas", 16);
            display.Location = new System.Drawing.Point(12, 20);
            display.Size = new System.Drawing.Size(710, 50);
            display.TextAlign = HorizontalAlignment.Right;
            display.ReadOnly = true;
            display.Text = "0";
            this.Controls.Add(display);

            // Mode toggle
            Button toggleModeBtn = new Button();
            toggleModeBtn.Text = "⌵ Стандартный";
            toggleModeBtn.Font = new System.Drawing.Font("Segoe UI", 9);
            toggleModeBtn.Size = new System.Drawing.Size(130, 30);
            toggleModeBtn.Location = new System.Drawing.Point(12, 85);
            toggleModeBtn.Click += (s, e) =>
            {
                isScientificMode = !isScientificMode;
                toggleModeBtn.Text = isScientificMode ? "⌵ Стандартный" : "🔬 Научный";
                UpdateModeVisibility();
            };
            this.Controls.Add(toggleModeBtn);

            // Degrees/Radians toggle
            Button angleBtn = new Button();
            angleBtn.Text = "DEG";
            angleBtn.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);
            angleBtn.Size = new System.Drawing.Size(80, 30);
            angleBtn.Location = new System.Drawing.Point(150, 85);
            angleBtn.Click += (s, e) =>
            {
                isDegrees = !isDegrees;
                angleBtn.Text = isDegrees ? "DEG" : "RAD";
            };
            this.Controls.Add(angleBtn);

            CreateButtons();
            UpdateModeVisibility();
        }

        private void CreateButtons()
        {
            string[] stdButtons = {
                "7", "8", "9", "/",
                "4", "5", "6", "*",
                "1", "2", "3", "-",
                "0", ".", "+/-", "+",
                "C", "%", "=", ""
            };

            string[] sciButtons = {
                "sin", "cos", "tan", "√",
                "cot", "sec", "csc", "^",
                "log", "ln", "10^x", "e^x",
                "(", ")", "π", "e"
            };

            // Standard panel
            Panel stdPanel = new Panel();
            stdPanel.Name = "stdPanel";
            stdPanel.Location = new System.Drawing.Point(12, 125);
            stdPanel.Size = new System.Drawing.Size(710, 410);
            int x = 0, y = 0;
            foreach (string text in stdButtons)
            {
                if (string.IsNullOrEmpty(text)) continue;
                Button btn = new Button();
                btn.Text = text;
                btn.Font = new System.Drawing.Font("Segoe UI", 12);
                btn.Size = new System.Drawing.Size(85, 60);
                btn.Location = new System.Drawing.Point(x, y);
                btn.Click += (s, e) => ProcessInput(btn.Text);
                stdPanel.Controls.Add(btn);
                x += 90;
                if ((x / 90) % 4 == 0) { x = 0; y += 65; }
            }
            this.Controls.Add(stdPanel);

            // Scientific panel
            Panel sciPanel = new Panel();
            sciPanel.Name = "sciPanel";
            sciPanel.Location = new System.Drawing.Point(12, 125);
            sciPanel.Size = new System.Drawing.Size(710, 410);
            x = 0; y = 0;
            foreach (string text in stdButtons)
            {
                if (string.IsNullOrEmpty(text)) continue;
                Button btn = new Button();
                btn.Text = text;
                btn.Font = new System.Drawing.Font("Segoe UI", 10);
                btn.Size = new System.Drawing.Size(75, 55);
                btn.Location = new System.Drawing.Point(x, y);
                btn.Click += (s, e) => ProcessInput(btn.Text);
                sciPanel.Controls.Add(btn);
                x += 80;
                if ((x / 80) % 4 == 0) { x = 0; y += 60; }
            }

            // Add scientific buttons (4 columns)
            for (int i = 0; i < sciButtons.Length; i++)
            {
                int col = i / 4;
                int row = i % 4;
                Button btn = new Button();
                btn.Text = sciButtons[i];
                btn.Font = new System.Drawing.Font("Segoe UI", 9);
                btn.Size = new System.Drawing.Size(70, 55);
                btn.Location = new System.Drawing.Point(360 + col * 75, row * 60);
                btn.Click += (s, e) => ProcessInput(btn.Text);
                sciPanel.Controls.Add(btn);
            }
            this.Controls.Add(sciPanel);

            stdPanel.Visible = true;
            sciPanel.Visible = false;
        }

        private void UpdateModeVisibility()
        {
            Panel std = this.Controls["stdPanel"] as Panel;
            Panel sci = this.Controls["sciPanel"] as Panel;
            if (std != null) std.Visible = !isScientificMode;
            if (sci != null) sci.Visible = isScientificMode;
        }

        private void ProcessInput(string input)
        {
            TextBox display = this.Controls["display"] as TextBox;

            // Error state: only C works
            if (isError && input != "C") return;
            if (input == "C") isError = false;

            // After equals, start fresh unless continuing with an operator or function
            if (justCalculated && !IsOperator(input) && input != "=" && input != "C" && input != ")" && !IsFunction(input))
            {
                currentExpression = "";
                justCalculated = false;
            }

            // If expression empty, initial handling
            if (string.IsNullOrEmpty(currentExpression))
            {
                if (char.IsDigit(input[0]) || input == ".")
                {
                    currentExpression = input == "." ? "0." : input;
                    display.Text = currentExpression;
                    return;
                }
                else if (input == "C")
                {
                    currentExpression = "";
                    display.Text = "0";
                    return;
                }
                else if (input == "(")
                {
                    currentExpression = "(";
                    display.Text = currentExpression;
                    return;
                }
                else if (IsFunction(input))
                {
                    currentExpression = input + "(";
                    display.Text = currentExpression;
                    return;
                }
                else if (input == "-")
                {
                    currentExpression = "-";
                    display.Text = currentExpression;
                    return;
                }
            }

            switch (input)
            {
                case "C":
                    currentExpression = "";
                    justCalculated = false;
                    display.Text = "0";
                    break;

                case "=":
                    if (string.IsNullOrEmpty(currentExpression)) break;
                    try
                    {
                        double result = EvaluateExpression(currentExpression);
                        string displayExpr = BalanceParentheses(currentExpression);
                        display.Text = displayExpr + " = " + result.ToString();
                        currentExpression = result.ToString();
                        justCalculated = true;
                    }
                    catch (DivideByZeroException)
                    {
                        display.Text = "Ошибка: деление на 0";
                        currentExpression = "";
                        isError = true;
                    }
                    catch
                    {
                        display.Text = "Ошибка";
                        currentExpression = "";
                        isError = true;
                    }
                    break;

                case "+/-":
                    ToggleLastNumberSign();
                    display.Text = currentExpression;
                    break;

                case "%":
                    try
                    {
                        double val = EvaluateExpression(currentExpression);
                        val /= 100;
                        currentExpression = val.ToString();
                        display.Text = currentExpression;
                        justCalculated = false;
                    }
                    catch { }
                    break;

                // Scientific functions
                case "sin": case "cos": case "tan":
                case "cot": case "sec": case "csc":
                case "log": case "ln": case "√":
                    currentExpression += input + "(";
                    justCalculated = false;
                    display.Text = currentExpression;
                    break;

                case "^":
                    if (currentExpression.Length > 0 && IsOperator(currentExpression[currentExpression.Length - 1].ToString()))
                        currentExpression = currentExpression.Substring(0, currentExpression.Length - 1) + "^";
                    else
                        currentExpression += "^";
                    justCalculated = false;
                    display.Text = currentExpression;
                    break;

                case "10^x":
                    currentExpression += "10^(";
                    justCalculated = false;
                    display.Text = currentExpression;
                    break;

                case "e^x":
                    currentExpression += "e^(";
                    justCalculated = false;
                    display.Text = currentExpression;
                    break;

                case "π":
                    currentExpression += "π";
                    justCalculated = false;
                    display.Text = currentExpression;
                    break;

                case "e":
                    currentExpression += "e";
                    justCalculated = false;
                    display.Text = currentExpression;
                    break;

                case "(":
                    string lastToken = GetLastToken(currentExpression);
                    if (lastToken != "" && (char.IsDigit(lastToken[0]) || lastToken == ")" || lastToken == "π" || lastToken == "e"))
                    {
                        currentExpression += "*";
                    }
                    currentExpression += "(";
                    justCalculated = false;
                    display.Text = currentExpression;
                    break;

                case ")":
                    int openCount = CountOccurrences(currentExpression, '(');
                    int closeCount = CountOccurrences(currentExpression, ')');
                    if (openCount > closeCount)
                    {
                        currentExpression += ")";
                        justCalculated = false;
                        display.Text = currentExpression;
                    }
                    break;

                default: // digits, operators, decimal
                    if (input == ".")
                    {
                        string lastTok = GetLastToken(currentExpression);
                        if (lastTok == ")") return;

                        string lastNumber = GetLastNumber(currentExpression);
                        if (lastNumber == "")
                        {
                            if (currentExpression.Length > 0 && (IsOperator(currentExpression[currentExpression.Length - 1].ToString()) || currentExpression[currentExpression.Length - 1] == '('))
                                currentExpression += "0.";
                            else
                                currentExpression += "0.";
                        }
                        else if (!lastNumber.Contains("."))
                        {
                            currentExpression += ".";
                        }
                        justCalculated = false;
                    }
                    else if (IsOperator(input))
                    {
                        if (currentExpression.Length > 0 && currentExpression[currentExpression.Length - 1] == '(')
                        {
                            currentExpression += "0" + input;
                        }
                        else if (currentExpression.Length > 0 && IsOperator(currentExpression[currentExpression.Length - 1].ToString()))
                        {
                            currentExpression = currentExpression.Substring(0, currentExpression.Length - 1) + input;
                        }
                        else
                        {
                            currentExpression += input;
                        }
                        justCalculated = false;
                    }
                    else // digits
                    {
                        currentExpression += input;
                        justCalculated = false;
                    }
                    display.Text = currentExpression;
                    break;
            }
        }

        // --- Helper methods ---

        private string BalanceParentheses(string expr)
        {
            int open = 0;
            foreach (char c in expr)
            {
                if (c == '(') open++;
                else if (c == ')') open--;
            }
            for (int i = 0; i < open; i++)
                expr += ")";
            return expr;
        }

        private string GetLastNumber(string expr)
        {
            Match match = Regex.Match(expr, @"(-?[\d.]+)$");
            return match.Success ? match.Value : "";
        }

        private string GetLastToken(string expr)
        {
            Match match = Regex.Match(expr, @"([\d.]+|[+\-*/^()]|sin|cos|tan|cot|sec|csc|log|ln|√|π|e)$");
            return match.Success ? match.Value : "";
        }

        private void ToggleLastNumberSign()
        {
            if (string.IsNullOrEmpty(currentExpression)) return;

            Match match = Regex.Match(currentExpression, @"(-?[\d.]+)$");
            if (match.Success)
            {
                string oldNum = match.Value;
                string newNum = oldNum.StartsWith("-") ? oldNum.Substring(1) : "-" + oldNum;
                currentExpression = currentExpression.Substring(0, match.Index) + newNum;
            }
        }

        private int CountOccurrences(string str, char ch)
        {
            int count = 0;
            foreach (char c in str) if (c == ch) count++;
            return count;
        }

        private bool IsOperator(string s)
        {
            return s == "+" || s == "-" || s == "*" || s == "/" || s == "^";
        }

        private bool IsFunction(string s)
        {
            return s == "sin" || s == "cos" || s == "tan" || s == "cot" || s == "sec" || s == "csc" ||
                   s == "log" || s == "ln" || s == "√" || s == "10^x" || s == "e^x";
        }

        // ========== EXPRESSION EVALUATOR ==========

        private int pos = 0;
        private string expr = "";

        private double EvaluateExpression(string expression)
        {
            expr = expression.Replace("×", "*").Replace("÷", "/")
                             .Replace("π", Math.PI.ToString())
                             .Replace("e", Math.E.ToString());
            pos = 0;
            double result = ParseAddSub();
            if (pos != expr.Length) throw new Exception("Invalid expression");
            return result;
        }

        private double ParseAddSub()
        {
            double result = ParseMulDiv();
            while (pos < expr.Length)
            {
                char op = expr[pos];
                if (op == '+')
                {
                    pos++;
                    result += ParseMulDiv();
                }
                else if (op == '-')
                {
                    pos++;
                    result -= ParseMulDiv();
                }
                else break;
            }
            return result;
        }

        private double ParseMulDiv()
        {
            double result = ParsePower();
            while (pos < expr.Length)
            {
                char op = expr[pos];
                if (op == '*')
                {
                    pos++;
                    result *= ParsePower();
                }
                else if (op == '/')
                {
                    pos++;
                    double denom = ParsePower();
                    if (denom == 0) throw new DivideByZeroException();
                    result /= denom;
                }
                else break;
            }
            return result;
        }

        private double ParsePower()
        {
            double result = ParseUnary();
            if (pos < expr.Length && expr[pos] == '^')
            {
                pos++;
                double exponent = ParsePower(); // right-associative
                result = Math.Pow(result, exponent);
            }
            return result;
        }

        private double ParseUnary()
        {
            if (pos < expr.Length && expr[pos] == '-')
            {
                pos++;
                return -ParseUnary();
            }
            return ParseFunctionOrNumber();
        }

        private double ParseFunctionOrNumber()
        {
            if (pos >= expr.Length) return 0;

            if (char.IsDigit(expr[pos]) || expr[pos] == '.')
                return ParseNumber();

            string[] functions = { "sin", "cos", "tan", "cot", "sec", "csc", "log", "ln", "sqrt" };
            foreach (string func in functions)
            {
                if (expr.Substring(pos).StartsWith(func))
                {
                    pos += func.Length;
                    if (pos < expr.Length && expr[pos] == '(')
                    {
                        pos++;
                        double arg = ParseAddSub();
                        if (pos < expr.Length && expr[pos] == ')')
                            pos++;
                        return ApplyFunction(func, arg);
                    }
                }
            }

            if (expr.Substring(pos).StartsWith("√"))
            {
                pos++;
                if (pos < expr.Length && expr[pos] == '(')
                {
                    pos++;
                    double arg = ParseAddSub();
                    if (pos < expr.Length && expr[pos] == ')')
                        pos++;
                    return ApplyFunction("sqrt", arg);
                }
            }

            if (expr[pos] == '(')
            {
                pos++;
                double result = ParseAddSub();
                if (pos < expr.Length && expr[pos] == ')')
                    pos++;
                return result;
            }

            return 0;
        }

        private double ParseNumber()
        {
            int start = pos;
            while (pos < expr.Length && (char.IsDigit(expr[pos]) || expr[pos] == '.'))
                pos++;
            string numStr = expr.Substring(start, pos - start);
            return double.Parse(numStr, System.Globalization.CultureInfo.InvariantCulture);
        }

        private double ApplyFunction(string func, double arg)
        {
            if (isDegrees && (func == "sin" || func == "cos" || func == "tan" || func == "cot" || func == "sec" || func == "csc"))
                arg = arg * Math.PI / 180.0;

            switch (func)
            {
                case "sin": return Math.Sin(arg);
                case "cos": return Math.Cos(arg);
                case "tan": return Math.Tan(arg);
                case "cot":
                    double tan = Math.Tan(arg);
                    if (Math.Abs(tan) < 1e-12) throw new DivideByZeroException();
                    return 1.0 / tan;
                case "sec":
                    double cos = Math.Cos(arg);
                    if (Math.Abs(cos) < 1e-12) throw new DivideByZeroException();
                    return 1.0 / cos;
                case "csc":
                    double sin = Math.Sin(arg);
                    if (Math.Abs(sin) < 1e-12) throw new DivideByZeroException();
                    return 1.0 / sin;
                case "log": return Math.Log10(arg);
                case "ln": return Math.Log(arg);
                case "sqrt": return Math.Sqrt(arg);
                default: return arg;
            }
        }
    }
}