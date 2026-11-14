using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        Console.WriteLine("Integral calculator\n");

        string inputRaw = "ing{-6x^11+3+-2+x^11+4+(x+2)^-1+sin(3x)+cos(2x+1)+(2x+1)^3}";
        string input = inputRaw.Replace("ing", "")
                               .Replace("{", "")
                               .Replace("}", "")
                               .Replace(" ", "");

        Console.WriteLine("Input: " + input);

        string result = CalculateIntegral(input);
        Console.WriteLine("Antiderivative: " + result + " + C");
    }

    static string CalculateIntegral(string expr)
    {
        var terms = new List<string>();
        int sign = 1;

        for (int i = 0; i < expr.Length; i++)
        {
            char c = expr[i];

            if ((c == '+' || c == '-') && (i == 0 || "+-*/".Contains(expr[i - 1])))
            {
                sign = (c == '+') ? 1 : -1;
                continue;
            }

            if (char.IsDigit(c))
            {
                int j = i;
                string number = "";

                while (j < expr.Length && char.IsDigit(expr[j]))
                    number += expr[j++];

                i = j - 1;

                if (sign == -1)
                    number = "-" + number;

                sign = 1;

                if (j < expr.Length && expr[j] == 'x')
                {
                    int exp = 1;

                    if (j + 1 < expr.Length && expr[j + 1] == '^')
                    {
                        int k = j + 2;
                        bool neg = false;

                        if (expr[k] == '-')
                        {
                            neg = true;
                            k++;
                        }

                        string expStr = "";
                        while (k < expr.Length && char.IsDigit(expr[k]))
                            expStr += expr[k++];

                        exp = int.Parse(expStr);
                        if (neg) exp = -exp;

                        i = k - 1;
                    }

                    terms.Add(IntegratePower(number, exp));
                }
                else
                {
                    terms.Add(number);
                }

                continue;
            }

            if (c == 'x')
            {
                string coeff = (sign == -1) ? "-1" : "1";
                sign = 1;

                int exp = 1;

                if (i + 1 < expr.Length && expr[i + 1] == '^')
                {
                    int k = i + 2;
                    bool neg = false;

                    if (expr[k] == '-')
                    {
                        neg = true;
                        k++;
                    }

                    string expStr = "";
                    while (k < expr.Length && char.IsDigit(expr[k]))
                        expStr += expr[k++];

                    exp = int.Parse(expStr);
                    if (neg) exp = -exp;

                    i = k - 1;
                }

                terms.Add(IntegratePower(coeff, exp));
                continue;
            }

            if (expr.Substring(i).StartsWith("sin("))
            {
                int end = expr.IndexOf(')', i);
                string inner = expr.Substring(i + 4, end - i - 4);

                terms.Add(IntegrateTrig(inner, sign, true));
                sign = 1;
                i = end;
                continue;
            }

            if (expr.Substring(i).StartsWith("cos("))
            {
                int end = expr.IndexOf(')', i);
                string inner = expr.Substring(i + 4, end - i - 4);

                terms.Add(IntegrateTrig(inner, sign, false));
                sign = 1;
                i = end;
                continue;
            }
        }

        return Clean(terms);
    }

    static string IntegratePower(string coeffStr, int exp)
    {
        if (exp == -1)
            return coeffStr + "*ln|x|";

        int coeff = int.Parse(coeffStr);
        int newExp = exp + 1;

        return $"{coeff}x^{newExp}/{newExp}";
    }

    static string IntegrateTrig(string inner, int sign, bool isSin)
    {
        string factor = GetCoeff(inner);

        if (isSin)
            return (sign == -1 ? "-" : "") + $"-cos({inner})/{factor}";
        else
            return (sign == -1 ? "-" : "") + $"sin({inner})/{factor}";
    }

    static string GetCoeff(string expr)
    {
        string cleaned = expr.Replace("x", "");
        if (cleaned == "" || cleaned == "+") return "1";
        if (cleaned == "-") return "1";
        return cleaned;
    }

    static string Clean(List<string> parts)
    {
        string result = string.Join(" + ", parts);
        return result.Replace("+ -", "- ");
    }
}
