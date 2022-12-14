using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Andrius.Core.Debuging
{
public static class StringEx
{
    // this is a string Extension thus we can use it for GUI / textMeshPro or Text also
    public static bool DoLog { get; set; } = true;
    private const string Match = @"([^;:]*)\";
    private static readonly string pattern = $"{Match}:?{Match}:?{Match}:?{Match}:?{Match};";
    private const int MaxSize = 50, MinSize = 5;

    private static readonly Dictionary<string, string> styles = new Dictionary<string, string>
    {
            {"b","b"}, {"B","b"}, {"BOLD","b"}, {$"{FontStyle.Bold}","b"},

            {"i","i"} , {"I","i"} , {"ITALIC","i"}, {$"{FontStyle.Italic}","i"},

            {"bi",$"BI"}, {"ib",$"BI"}, {"Bi",$"BI"}, {"Ib",$"BI"}, {"BI",$"BI"}, {"IB",$"BI"}, {$"{FontStyle.BoldAndItalic}",$"BI"}, {"BoldItalic",$"BI"},
        };
    private static readonly Dictionary<string, string> variant = new Dictionary<string, string>
    {
        {"U","U"}, {"UP","U"}, {$"Upper","U"},{$"{Variant.UPPER}","U"},

        {"T","T"}, {"t","T"}, {$"TITLE","T"}, {$"{Variant.TitleCase}","T"},

        {"L","L"}, {"l","L"}, {"LO","L"}, {"Lo","L"}, {$"{Variant.lower}","L"},
    };
    private static readonly Dictionary<string, string> sizeList = Enumerable.Range(0, 99).ToList().ConvertAll(o => o.ToString()).ToDictionary(x => x, x => x);
    private static readonly Dictionary<string, string> colors = new Dictionary<string, string>
    {
        { C.red ,C.red}, { "RED" ,C.red}, { "Red" ,C.red}, { "r" ,C.red}, { "R" ,C.red}, {$"{Color.red}",C.red},

        {C.yellow,C.yellow}, {"YEllOW",C.yellow}, {"Yellow",C.yellow}, {"y",C.yellow}, {"Y",C.yellow}, {$"{Color.yellow}",C.yellow},

        {C.green,C.green}, {"GREEN",C.green}, {"Green",C.green}, {"g",C.green}, {"G",C.green}, {$"{Color.green}",C.green},

        {$"{C.magenta}",C.magenta}, {"MAGENTA",C.magenta}, {"Magenta",C.magenta}, {"m",C.magenta}, {"M",C.magenta}, {$"{Color.magenta}",C.magenta},

        {$"{C.white}",C.white}, {"White",C.white}, {"WHITE",C.white}, {"w",C.white}, {"W",C.white}, {$"{Color.white}",C.white},

        {$"{C.black}",C.black}, {"Black",C.black}, {"BLACK",C.black}, {"bla",C.black}, {"Bla",C.black}, {$"{Color.black}",C.black},

        {$"{C.blue}",C.blue}, {"Blue",C.blue}, {"bl",C.blue}, {"Bl",C.blue}, {"BL",C.blue}, {$"{Color.blue}",C.blue},

        {$"{C.gray}",C.gray}, {"Gray",C.gray}, {"GRAY",C.gray}, {"gr",C.gray}, {"GR",C.gray}, {$"{Color.gray}",C.gray},

        {$"{C.grey}",C.gray}, {"Grey",C.grey}, {"GREY",C.grey},

        {$"{C.cyan}",C.cyan}, {"Cyan",C.cyan}, {"CYAN",C.cyan}, {"c",C.cyan}, {"C",C.cyan}, {$"{Color.cyan}",C.cyan},

        {$"{C.aqua}",C.aqua},
        {$"{C.brown}",C.brown},
        {$"{C.darkBlue}",C.darkBlue},
        {$"{C.fuchsia}",C.fuchsia},
        {$"{C.lightBlue}",C.lightBlue},
        {$"{C.lime}",C.lime},
        {$"{C.maroon}",C.maroon},
        {$"{C.navy}",C.navy},
        {$"{C.olive}",C.olive},
        {$"{C.orange}",C.orange},
        {$"{C.purple}",C.purple},
        {$"{C.silver}",C.silver},
        {$"{C.teal}",C.teal},
        {$"{C.gold}",C.gold},

};
    private static string GetValue(string text, IEnumerable groups, IReadOnlyDictionary<string, string> list) => string.IsNullOrEmpty(Find(groups, out var key, list)) ? text : Convert(key, text);
    private static string Find(IEnumerable groups, out string value, IReadOnlyDictionary<string, string> list) => value = groups.Cast<Group>().ToList().Find(o => list.ContainsKey(o.Value))?.Value;
    private static string Convert(string key, string input) =>
        int.TryParse(key, out var value) ? $"<size={Mathf.Clamp(value, MinSize, MaxSize)}>{input}</size>" :
        colors.ContainsKey(key) ? $"<color={colors[key]}> {input} </color>" :
        styles.ContainsKey(key) ? StyleConvert(key, input) :
        variant.ContainsKey(key) ? VariantConvert(key, input) : $"<{key}>" + input + $"</{key}>";
    private static string StyleConvert(string key, string input) => styles[key] == $"BI" ? "<b><i>" + input + "</i></b>" : $"<{styles[key]}>" + input + $"</{styles[key]}>";
    private static string VariantConvert(string key, string input) => variant[key] == "U" ? input.ToUpper() : variant[key] == "T" ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower()) : input.ToLower();
    public static string Interpolate(this string input) => string.Concat(Regex.Matches(input, pattern, RegexOptions.Multiline).OfType<Match>().ToList()
            .ConvertAll(o => GetValue(GetValue(GetValue(GetValue(o.Groups[1].Value, o.Groups, variant), o.Groups, styles), o.Groups, sizeList), o.Groups, colors)));

    public static string Apply(string value) => $":{value};";
    public static string Apply(Color color) => $":{color};";
    public static string Apply(FontStyle fontStyle) => $":{fontStyle};";
    public static string Apply(int i) => $":{i};";
    public static string Apply(Variant caseVariant) => $":{caseVariant};";

    public static string Apply(string color, string fontStyle, string caseVariant, int i) => $":{color}:{fontStyle}:{caseVariant}:{i};";
    public static string Apply(string value, string value1, int i) => $":{value}:{value1}:{i};";
    public static string Apply(string value, string value1) => $":{value}:{value1};";
    public static string Apply(string value, int i) => $":{value}:{i};";

    public static string Apply(Color color, FontStyle fontStyle, Variant caseVariant, int i) => $":{color}:{fontStyle}:{caseVariant}:{i};";
    public static string Apply(Color color, FontStyle fontStyle, int i) => $":{color}:{fontStyle}:{i};";
    public static string Apply(Color color, FontStyle fontStyle) => $":{color}:{fontStyle};";
    public static string Apply(Color color, Variant caseVariant) => $":{color}:{caseVariant};";
    public static string Apply(Color color, int i) => $":{color}:{i};";
    public static string Apply(FontStyle fontStyle, int i) => $":{fontStyle}:{i};";
    public static string Apply(Variant caseVariant, int i) => $":{caseVariant}:{i};";
    public static string Apply(FontStyle fontStyle, Variant caseVariant) => $":{fontStyle}:{caseVariant};";

    public static void Action(Action action) { if (DoLog) action(); }

    public static void Log() => Action(() => Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, new string('_', 150)));
    public static void Log(int i) => Action(() => Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, new string('_', i)));
    public static void Log(char c) => Action(() => Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, new string(c, 150)));
    public static void Log(char c, int i) => Action(() => Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, new string(c, i)));
    public static void Log(Color color) => Action(() => Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, $"{new string('_', 150)}:{color};".Interpolate()));
    public static void Log(Color color, char c) => Action(() => Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, $"{new string(c, 150)}:{color};".Interpolate()));
    public static void Log(Color color, char c, int i) => Action(() => Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, $"{new string(c, i)}:{color};".Interpolate()));
    public static void Log(string @string, Object o = null) => Action(() => Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, o, @string.Interpolate()));
    public static void LogWarning(string @string, Object o = null) => Action(() => Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, o, @string.Interpolate()));
    public static void LogError(string @string, Object o = null) => Action(() => Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, o, @string.Interpolate()));
    public static void LogAssert(string @string, Object o = null) => Action(() => Debug.LogFormat(LogType.Assert, LogOption.NoStacktrace, o, @string.Interpolate()));
    public static void LogT(string @string) => Action(() => Debug.Log(@string.Interpolate()));
    public static void LogWarningT(string @string) => Action(() => Debug.LogWarningFormat(@string.Interpolate()));
    public static void LogErrorT(string @string) => Action(() => Debug.LogErrorFormat(@string.Interpolate()));
    public static void LogAssertT(string @string) => Action(() => Debug.LogAssertionFormat(@string.Interpolate()));

}

public enum Variant
{
    UPPER, TitleCase, lower
}

public struct C
{
    // install https://marketplace.visualstudio.com/items?itemName=NikolaMSFT.InlineColorPicker if you wish to see preview of color
    // feel free to add more color if you like i.e gold then add that to Dictionary {colors} Too like {$"{C.gold}",C.gold},

    public static string red = "red";
    public static string yellow = "yellow";
    public static string green = "green";
    public static string aqua = "aqua";
    public static string black = "black";
    public static string blue = "blue";
    public static string brown = "brown";
    public static string cyan = "cyan";
    public static string darkBlue = "darkblue";
    public static string fuchsia = "fuchsia";
    public static string grey = "#A0A0A0";
    public static string gray = "gray";
    public static string lightBlue = "lightblue";
    public static string lime = "lime";
    public static string magenta = "magenta";
    public static string maroon = "maroon";
    public static string navy = "navy";
    public static string olive = "olive";
    public static string orange = "orange";
    public static string purple = "purple";
    public static string silver = "silver";
    public static string teal = "teal";
    public static string white = "white";
    public static string gold = "#DC9A00";

}
}