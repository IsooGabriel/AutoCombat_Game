using System;
using System.Text;
using UnityEngine;

public static class GraphDataJsonUtility
{
    private const string ValueProperty = "\"value\"";

    public static GraphData FromJson(string json)
    {
        return JsonUtility.FromJson<GraphData>(NormalizeLegacyInputValues(json));
    }

    public static string NormalizeLegacyInputValues(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return json;
        }

        StringBuilder builder = null;
        int readIndex = 0;
        int searchIndex = 0;

        while (searchIndex < json.Length)
        {
            int valueIndex = json.IndexOf(ValueProperty, searchIndex, StringComparison.Ordinal);
            if (valueIndex < 0)
            {
                break;
            }

            int colonIndex = SkipWhiteSpace(json, valueIndex + ValueProperty.Length);
            if (colonIndex >= json.Length || json[colonIndex] != ':')
            {
                searchIndex = valueIndex + ValueProperty.Length;
                continue;
            }

            int tokenStart = SkipWhiteSpace(json, colonIndex + 1);
            if (tokenStart >= json.Length)
            {
                break;
            }

            char first = json[tokenStart];
            if (first == '{' || first == '[')
            {
                searchIndex = tokenStart + 1;
                continue;
            }

            if (!TryReadLegacyValue(json, tokenStart, out int tokenEnd, out string valueJson, out string typeName))
            {
                searchIndex = tokenStart + 1;
                continue;
            }

            builder ??= new StringBuilder(json.Length + 256);
            builder.Append(json, readIndex, colonIndex + 1 - readIndex);
            builder.Append(' ');
            builder.Append("{\"valueJson\":\"");
            builder.Append(EscapeJsonString(valueJson));
            builder.Append("\",\"typeName\":\"");
            builder.Append(EscapeJsonString(typeName));
            builder.Append("\"}");

            readIndex = tokenEnd;
            searchIndex = tokenEnd;
        }

        if (builder == null)
        {
            return json;
        }

        builder.Append(json, readIndex, json.Length - readIndex);
        return builder.ToString();
    }

    private static bool TryReadLegacyValue(string json, int start, out int end, out string valueJson, out string typeName)
    {
        end = start;
        valueJson = null;
        typeName = null;

        char first = json[start];
        if (first == '"')
        {
            if (!TryReadJsonString(json, start, out end, out valueJson))
            {
                return false;
            }

            typeName = typeof(string).AssemblyQualifiedName;
            return true;
        }

        if (StartsWith(json, start, "true"))
        {
            end = start + 4;
            valueJson = "true";
            typeName = typeof(bool).AssemblyQualifiedName;
            return true;
        }

        if (StartsWith(json, start, "false"))
        {
            end = start + 5;
            valueJson = "false";
            typeName = typeof(bool).AssemblyQualifiedName;
            return true;
        }

        if (StartsWith(json, start, "null"))
        {
            end = start + 4;
            valueJson = "";
            typeName = typeof(string).AssemblyQualifiedName;
            return true;
        }

        if (IsNumberStart(first))
        {
            end = start + 1;
            while (end < json.Length && IsNumberChar(json[end]))
            {
                end++;
            }

            valueJson = json.Substring(start, end - start);
            typeName = typeof(float).AssemblyQualifiedName;
            return true;
        }

        return false;
    }

    private static int SkipWhiteSpace(string text, int index)
    {
        while (index < text.Length && char.IsWhiteSpace(text[index]))
        {
            index++;
        }

        return index;
    }

    private static bool TryReadJsonString(string json, int start, out int end, out string value)
    {
        StringBuilder builder = new StringBuilder();
        end = start + 1;
        value = null;

        while (end < json.Length)
        {
            char c = json[end++];
            if (c == '"')
            {
                value = builder.ToString();
                return true;
            }

            if (c != '\\')
            {
                builder.Append(c);
                continue;
            }

            if (end >= json.Length)
            {
                return false;
            }

            char escaped = json[end++];
            switch (escaped)
            {
                case '"':
                case '\\':
                case '/':
                    builder.Append(escaped);
                    break;
                case 'b':
                    builder.Append('\b');
                    break;
                case 'f':
                    builder.Append('\f');
                    break;
                case 'n':
                    builder.Append('\n');
                    break;
                case 'r':
                    builder.Append('\r');
                    break;
                case 't':
                    builder.Append('\t');
                    break;
                case 'u':
                    if (end + 4 > json.Length)
                    {
                        return false;
                    }
                    string hex = json.Substring(end, 4);
                    builder.Append((char)Convert.ToInt32(hex, 16));
                    end += 4;
                    break;
                default:
                    return false;
            }
        }

        return false;
    }

    private static bool StartsWith(string text, int start, string value)
    {
        if (start + value.Length > text.Length)
        {
            return false;
        }

        for (int i = 0; i < value.Length; i++)
        {
            if (text[start + i] != value[i])
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsNumberStart(char c)
    {
        return c == '-' || c == '+' || char.IsDigit(c);
    }

    private static bool IsNumberChar(char c)
    {
        return char.IsDigit(c) || c == '-' || c == '+' || c == '.' || c == 'e' || c == 'E';
    }

    private static string EscapeJsonString(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }

        StringBuilder builder = new StringBuilder(value.Length);
        foreach (char c in value)
        {
            switch (c)
            {
                case '"':
                    builder.Append("\\\"");
                    break;
                case '\\':
                    builder.Append("\\\\");
                    break;
                case '\b':
                    builder.Append("\\b");
                    break;
                case '\f':
                    builder.Append("\\f");
                    break;
                case '\n':
                    builder.Append("\\n");
                    break;
                case '\r':
                    builder.Append("\\r");
                    break;
                case '\t':
                    builder.Append("\\t");
                    break;
                default:
                    builder.Append(c);
                    break;
            }
        }

        return builder.ToString();
    }
}
