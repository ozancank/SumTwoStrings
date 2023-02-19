// İki adet tamsayının toplanmının hesaplanması

using System.Text.RegularExpressions;

var valueA = "900000000000000000000000000000000000000000000000009";
var valueB = "900000000000000000000000000000000000000000000000008";
Console.WriteLine($"Value A\t: {valueA}");
Console.WriteLine($"Value B\t: {valueB}");
Console.WriteLine($"Result\t: {SumInteger(valueA, valueB)}");
Console.WriteLine(new string('-', 100));

var valueC = "-900000000000000000000000000000000000000000000000009";
var valueD = "-900000000000000000000000000000000000000000000000008";
Console.WriteLine($"Value C\t: {valueC}");
Console.WriteLine($"Value D\t: {valueD}");
Console.WriteLine($"Result\t: {SumInteger(valueC, valueD)}");
Console.WriteLine(new string('-', 100));

var valueE = "-900000000000000000000000000000000000000000000000009";
var valueF = "+900000000000000000000000000000000000000000000000008";
Console.WriteLine($"Value E\t: {valueE}");
Console.WriteLine($"Value F\t: {valueF}");
Console.WriteLine($"Result\t: {SumInteger(valueE, valueF)}");
Console.WriteLine(new string('-', 100));

Console.ReadKey();

static string SumInteger(string valueA, string valueB)
{
    ArgumentException.ThrowIfNullOrEmpty(valueA, nameof(valueA));
    ArgumentException.ThrowIfNullOrEmpty(valueB, nameof(valueB));

    ReplaceValues(ref valueA, ref valueB);

    var valueRegex = "^([+-]?[1-9][0-9]*)$";
    if (!Regex.IsMatch(valueA, valueRegex) || !Regex.IsMatch(valueB, valueRegex))
        throw new ArgumentException("\nValues can only contain integer, + and -.\nValue numbers cann't start zero.");

    SetOperator(ref valueA, ref valueB, out char operatorA, out char operatorB);

    if (operatorA == operatorB) return Addition(valueA, valueB);
    else return Subtraction(valueA, valueB);
}

static void ReplaceValues(ref string valueA, ref string valueB)
{
    valueA = valueA.Replace(" ", "");
    valueB = valueB.Replace(" ", "");
}

static void SetOperator(ref string valueA, ref string valueB, out char operatorA, out char operatorB)
{
    if (int.TryParse(valueA[0].ToString(), out _))
    {
        operatorA = '+';
        valueA = $"+{valueA}";
    }
    else operatorA = valueA[0];

    if (int.TryParse(valueB[0].ToString(), out _))
    {
        operatorB = '+';
        valueB = $"+{valueB}";
    }
    else operatorB = valueB[0];
}

static void CleanOperator(ref string valueA, ref string valueB)
{
    valueA = valueA[1..];
    valueB = valueB[1..];
}

static void SetSize(ref string valueA, ref string valueB)
{
    var size1 = valueA.Length;
    var size2 = valueB.Length;

    if (size1 > size2) valueB = valueB.PadLeft(size1, '0');
    else valueA = valueA.PadLeft(size2, '0');
}

static string Addition(string valueA, string valueB)
{
    var operatorResult = valueA[0];
    CleanOperator(ref valueA, ref valueB);
    SetSize(ref valueA, ref valueB);

    var result = "";
    var ten = false;
    for (int i = valueA.Length - 1; i >= 0; i--)
    {
        var temp = int.Parse(valueA[i].ToString()) + int.Parse(valueB[i].ToString());
        if (ten) temp += 1;
        if (temp >= 10)
        {
            temp -= 10;
            ten = true;
        }
        else ten = false;

        result = temp.ToString() + result;
    }

    if (ten) result = "1" + result;
    result = result.TrimStart('0');
    if (operatorResult == '-') result = "-" + result;
    return result;
}

static void SetAbsoluteValues(string valueA, string valueB, out char operatorResult, out string absoluteValueBig, out string absoluteValueSmall)
{
    operatorResult = '+';
    absoluteValueBig = valueA[1..];
    absoluteValueSmall = valueB[1..];

    static void BigValueIfValueA(string valueA, string valueB, out char operatorResult, out string absoluteValueBig, out string absoluteValueSmall)
    {
        operatorResult = valueA[0];
        absoluteValueBig = valueA;
        absoluteValueSmall = valueB;
    }

    static void BigValueIfValueB(string valueA, string valueB, out char operatorResult, out string absoluteValueBig, out string absoluteValueSmall)
    {
        operatorResult = valueB[0];
        absoluteValueBig = valueB;
        absoluteValueSmall = valueA;
    }

    if (valueA.Length > valueB.Length) BigValueIfValueA(valueA, valueB, out operatorResult, out absoluteValueBig, out absoluteValueSmall);
    else if (valueA.Length == valueB.Length)
    {
        for (int i = 1; i <= valueA.Length - 1; i++)
        {
            var numberA = int.Parse(valueA[i].ToString());
            var numberB = int.Parse(valueB[i].ToString());
            if (numberA == numberB) continue;
            else if (numberA > numberB) { BigValueIfValueA(valueA, valueB, out operatorResult, out absoluteValueBig, out absoluteValueSmall); break; }
            else { BigValueIfValueB(valueA, valueB, out operatorResult, out absoluteValueBig, out absoluteValueSmall); break; }
        }
    }
    else BigValueIfValueB(valueA, valueB, out operatorResult, out absoluteValueBig, out absoluteValueSmall);
}

static string Subtraction(string valueA, string valueB)
{
    SetAbsoluteValues(valueA, valueB, out char operatorResult, out string absoluteValueBig, out string absoluteValueSmall);
    CleanOperator(ref absoluteValueBig, ref absoluteValueSmall);
    SetSize(ref absoluteValueBig, ref absoluteValueSmall);

    static string DecreaseNumber(string absoluteValueBig, int index)
    {
        if (index >= 0)
        {
            var decreasingNumber = int.Parse(absoluteValueBig[index].ToString()) - 1;
            absoluteValueBig = absoluteValueBig.Remove(index, 1).Insert(index, decreasingNumber.ToString());
        }
        return absoluteValueBig;
    }

    static int FindIndexForGreatThanZero(string absoluteValueBig, int index)
    {
        if (index > 0 && absoluteValueBig[index] == '0')
        {
            index -= 1;
            return FindIndexForGreatThanZero(absoluteValueBig, index);
        }
        return index;
    }

    var result = "";
    for (int i = absoluteValueBig.Length - 1; i >= 0; i--)
    {
        var numberA = int.Parse(absoluteValueBig[i].ToString());
        var numberB = int.Parse(absoluteValueSmall[i].ToString());
        if (numberA >= numberB) result = (numberA - numberB).ToString() + result;
        else
        {
            result = (10 + numberA - numberB).ToString() + result;
            if (absoluteValueBig[i - 1] != '0') absoluteValueBig = DecreaseNumber(absoluteValueBig, i - 1);
            else
            {
                var numberIndex = FindIndexForGreatThanZero(absoluteValueBig, i - 1);
                absoluteValueBig = DecreaseNumber(absoluteValueBig, numberIndex);
                absoluteValueBig = absoluteValueBig.Remove(numberIndex + 1, i - numberIndex - 1).Insert(numberIndex + 1, new string('9', i - numberIndex - 1));
            }
        }
    }
    result = result.TrimStart('0');
    if (operatorResult == '-') result = "-" + result;
    return result;
}