using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace BehaviorDesigner.Runtime
{
  public static class MiniJSON
  {
    public static object Deserialize(string json)
    {
      return json == null ? null : Parser.Parse(json);
    }

    public static string Serialize(object obj) => Serializer.Serialize(obj);

    private sealed class Parser : IDisposable
    {
      private const string WORD_BREAK = "{}[],:\"";
      private StringReader json;

      public static bool IsWordBreak(char c) => char.IsWhiteSpace(c) || "{}[],:\"".IndexOf(c) != -1;

      private Parser(string jsonString) => json = new StringReader(jsonString);

      public static object Parse(string jsonString)
      {
        using (Parser parser = new Parser(jsonString))
          return parser.ParseValue();
      }

      public void Dispose()
      {
        json.Dispose();
        json = null;
      }

      private Dictionary<string, object> ParseObject()
      {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        json.Read();
        while (true)
        {
          TOKEN nextToken;
          do
          {
            nextToken = NextToken;
            if (nextToken != TOKEN.NONE)
            {
              if (nextToken == TOKEN.CURLY_CLOSE)
                goto label_4;
            }
            else
              goto label_3;
          }
          while (nextToken == TOKEN.COMMA);
          string key = ParseString();
          if (key != null && NextToken == TOKEN.COLON)
          {
            json.Read();
            dictionary[key] = ParseValue();
          }
          else
            goto label_6;
        }
label_3:
        return null;
label_4:
        return dictionary;
label_6:
        return null;
      }

      private List<object> ParseArray()
      {
        List<object> array = new List<object>();
        json.Read();
        bool flag = true;
        while (flag)
        {
          TOKEN nextToken = NextToken;
          switch (nextToken)
          {
            case TOKEN.NONE:
              return null;
            case TOKEN.SQUARED_CLOSE:
              flag = false;
              break;
            case TOKEN.COMMA:
              continue;
            default:
              object byToken = ParseByToken(nextToken);
              array.Add(byToken);
              break;
          }
        }
        return array;
      }

      private object ParseValue() => ParseByToken(NextToken);

      private object ParseByToken(TOKEN token)
      {
        switch (token)
        {
          case TOKEN.CURLY_OPEN:
            return ParseObject();
          case TOKEN.SQUARED_OPEN:
            return ParseArray();
          case TOKEN.STRING:
            return ParseString();
          case TOKEN.NUMBER:
            return ParseNumber();
          case TOKEN.TRUE:
            return true;
          case TOKEN.FALSE:
            return false;
          case TOKEN.NULL:
            return null;
          default:
            return null;
        }
      }

      private string ParseString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        json.Read();
        bool flag = true;
        while (flag)
        {
          if (json.Peek() == -1)
            break;
          char nextChar1 = NextChar;
          switch (nextChar1)
          {
            case '"':
              flag = false;
              break;
            case '\\':
              if (json.Peek() == -1)
              {
                flag = false;
                break;
              }
              char nextChar2 = NextChar;
              switch (nextChar2)
              {
                case '"':
                case '/':
                case '\\':
                  stringBuilder.Append(nextChar2);
                  break;
                case 'b':
                  stringBuilder.Append('\b');
                  break;
                case 'f':
                  stringBuilder.Append('\f');
                  break;
                case 'n':
                  stringBuilder.Append('\n');
                  break;
                case 'r':
                  stringBuilder.Append('\r');
                  break;
                case 't':
                  stringBuilder.Append('\t');
                  break;
                case 'u':
                  char[] chArray = new char[4];
                  for (int index = 0; index < 4; ++index)
                    chArray[index] = NextChar;
                  stringBuilder.Append((char) Convert.ToInt32(new string(chArray), 16));
                  break;
              }
              break;
            default:
              stringBuilder.Append(nextChar1);
              break;
          }
        }
        return stringBuilder.ToString();
      }

      private object ParseNumber()
      {
        string nextWord = NextWord;
        if (nextWord.IndexOf('.') == -1)
        {
          long result;
          long.TryParse(nextWord, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
          return result;
        }
        double result1;
        double.TryParse(nextWord, NumberStyles.Any, CultureInfo.InvariantCulture, out result1);
        return result1;
      }

      private void EatWhitespace()
      {
        while (char.IsWhiteSpace(PeekChar))
        {
          json.Read();
          if (json.Peek() == -1)
            break;
        }
      }

      private char PeekChar => Convert.ToChar(json.Peek());

      private char NextChar => Convert.ToChar(json.Read());

      private string NextWord
      {
        get
        {
          StringBuilder stringBuilder = new StringBuilder();
          while (!IsWordBreak(PeekChar))
          {
            stringBuilder.Append(NextChar);
            if (json.Peek() == -1)
              break;
          }
          return stringBuilder.ToString();
        }
      }

      private TOKEN NextToken
      {
        get
        {
          EatWhitespace();
          if (json.Peek() == -1)
            return TOKEN.NONE;
          switch (PeekChar)
          {
            case '"':
              return TOKEN.STRING;
            case ',':
              json.Read();
              return TOKEN.COMMA;
            case '-':
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
              return TOKEN.NUMBER;
            case ':':
              return TOKEN.COLON;
            case '[':
              return TOKEN.SQUARED_OPEN;
            case ']':
              json.Read();
              return TOKEN.SQUARED_CLOSE;
            case '{':
              return TOKEN.CURLY_OPEN;
            case '}':
              json.Read();
              return TOKEN.CURLY_CLOSE;
            default:
              switch (NextWord)
              {
                case "false":
                  return TOKEN.FALSE;
                case "true":
                  return TOKEN.TRUE;
                case "null":
                  return TOKEN.NULL;
                default:
                  return TOKEN.NONE;
              }
          }
        }
      }

      private enum TOKEN
      {
        NONE,
        CURLY_OPEN,
        CURLY_CLOSE,
        SQUARED_OPEN,
        SQUARED_CLOSE,
        COLON,
        COMMA,
        STRING,
        NUMBER,
        TRUE,
        FALSE,
        NULL,
      }
    }

    private sealed class Serializer
    {
      private StringBuilder builder;

      private Serializer() => builder = new StringBuilder();

      public static string Serialize(object obj)
      {
        Serializer serializer = new Serializer();
        serializer.SerializeValue(obj, 1);
        return serializer.builder.ToString();
      }

      private void SerializeValue(object value, int indentationLevel)
      {
        switch (value)
        {
          case null:
            builder.Append("null");
            break;
          case string str:
            SerializeString(str);
            break;
          case bool flag:
            builder.Append(flag ? "true" : "false");
            break;
          case IList anArray:
            SerializeArray(anArray, indentationLevel);
            break;
          case IDictionary dictionary:
            SerializeObject(dictionary, indentationLevel);
            break;
          case char c:
            SerializeString(new string(c, 1));
            break;
          default:
            SerializeOther(value);
            break;
        }
      }

      private void SerializeObject(IDictionary obj, int indentationLevel)
      {
        bool flag = true;
        builder.Append('{');
        builder.Append('\n');
        for (int index = 0; index < indentationLevel; ++index)
          builder.Append('\t');
        foreach (object key in obj.Keys)
        {
          if (!flag)
          {
            builder.Append(',');
            builder.Append('\n');
            for (int index = 0; index < indentationLevel; ++index)
              builder.Append('\t');
          }
          SerializeString(key.ToString());
          builder.Append(':');
          ++indentationLevel;
          SerializeValue(obj[key], indentationLevel);
          --indentationLevel;
          flag = false;
        }
        builder.Append('\n');
        for (int index = 0; index < indentationLevel - 1; ++index)
          builder.Append('\t');
        builder.Append('}');
      }

      private void SerializeArray(IList anArray, int indentationLevel)
      {
        builder.Append('[');
        bool flag = true;
        for (int index = 0; index < anArray.Count; ++index)
        {
          object an = anArray[index];
          if (!flag)
            builder.Append(',');
          SerializeValue(an, indentationLevel);
          flag = false;
        }
        builder.Append(']');
      }

      private void SerializeString(string str)
      {
        builder.Append('"');
        foreach (char ch in str)
        {
          switch (ch)
          {
            case '\b':
              builder.Append("\\b");
              break;
            case '\t':
              builder.Append("\\t");
              break;
            case '\n':
              builder.Append("\\n");
              break;
            case '\f':
              builder.Append("\\f");
              break;
            case '\r':
              builder.Append("\\r");
              break;
            case '"':
              builder.Append("\\\"");
              break;
            case '\\':
              builder.Append("\\\\");
              break;
            default:
              int int32 = Convert.ToInt32(ch);
              if (int32 >= 32 && int32 <= 126)
              {
                builder.Append(ch);
                break;
              }
              builder.Append("\\u");
              builder.Append(int32.ToString("x4"));
              break;
          }
        }
        builder.Append('"');
      }

      private void SerializeOther(object value)
      {
        int num1;
        switch (value)
        {
          case float num2:
            builder.Append(num2.ToString(CultureInfo.InvariantCulture));
            return;
          case int _:
          case uint _:
          case long _:
          case sbyte _:
          case byte _:
          case short _:
          case ushort _:
            num1 = 1;
            break;
          default:
            num1 = value is ulong ? 1 : 0;
            break;
        }
        if (num1 != 0)
          builder.Append(value);
        else if (value is double || value is Decimal)
        {
          builder.Append(Convert.ToDouble(value).ToString(CultureInfo.InvariantCulture));
        }
        else
        {
          switch (value)
          {
            case Vector2 vector2:
              builder.Append("\"(" + vector2.x.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + vector2.y.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ")\"");
              break;
            case Vector3 vector3:
              builder.Append("\"(" + vector3.x.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + vector3.y.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + vector3.z.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ")\"");
              break;
            case Vector4 vector4:
              builder.Append("\"(" + vector4.x.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + vector4.y.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + vector4.z.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + vector4.w.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ")\"");
              break;
            case Quaternion quaternion:
              builder.Append("\"(" + quaternion.x.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + quaternion.y.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + quaternion.z.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + quaternion.w.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ")\"");
              break;
            default:
              SerializeString(value.ToString());
              break;
          }
        }
      }
    }
  }
}
