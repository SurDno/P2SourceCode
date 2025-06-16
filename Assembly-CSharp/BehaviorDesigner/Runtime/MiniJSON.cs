// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.MiniJSON
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime
{
  public static class MiniJSON
  {
    public static object Deserialize(string json)
    {
      return json == null ? (object) null : MiniJSON.Parser.Parse(json);
    }

    public static string Serialize(object obj) => MiniJSON.Serializer.Serialize(obj);

    private sealed class Parser : IDisposable
    {
      private const string WORD_BREAK = "{}[],:\"";
      private StringReader json;

      public static bool IsWordBreak(char c) => char.IsWhiteSpace(c) || "{}[],:\"".IndexOf(c) != -1;

      private Parser(string jsonString) => this.json = new StringReader(jsonString);

      public static object Parse(string jsonString)
      {
        using (MiniJSON.Parser parser = new MiniJSON.Parser(jsonString))
          return parser.ParseValue();
      }

      public void Dispose()
      {
        this.json.Dispose();
        this.json = (StringReader) null;
      }

      private Dictionary<string, object> ParseObject()
      {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        this.json.Read();
        while (true)
        {
          MiniJSON.Parser.TOKEN nextToken;
          do
          {
            nextToken = this.NextToken;
            if (nextToken != MiniJSON.Parser.TOKEN.NONE)
            {
              if (nextToken == MiniJSON.Parser.TOKEN.CURLY_CLOSE)
                goto label_4;
            }
            else
              goto label_3;
          }
          while (nextToken == MiniJSON.Parser.TOKEN.COMMA);
          string key = this.ParseString();
          if (key != null && this.NextToken == MiniJSON.Parser.TOKEN.COLON)
          {
            this.json.Read();
            dictionary[key] = this.ParseValue();
          }
          else
            goto label_6;
        }
label_3:
        return (Dictionary<string, object>) null;
label_4:
        return dictionary;
label_6:
        return (Dictionary<string, object>) null;
      }

      private List<object> ParseArray()
      {
        List<object> array = new List<object>();
        this.json.Read();
        bool flag = true;
        while (flag)
        {
          MiniJSON.Parser.TOKEN nextToken = this.NextToken;
          switch (nextToken)
          {
            case MiniJSON.Parser.TOKEN.NONE:
              return (List<object>) null;
            case MiniJSON.Parser.TOKEN.SQUARED_CLOSE:
              flag = false;
              break;
            case MiniJSON.Parser.TOKEN.COMMA:
              continue;
            default:
              object byToken = this.ParseByToken(nextToken);
              array.Add(byToken);
              break;
          }
        }
        return array;
      }

      private object ParseValue() => this.ParseByToken(this.NextToken);

      private object ParseByToken(MiniJSON.Parser.TOKEN token)
      {
        switch (token)
        {
          case MiniJSON.Parser.TOKEN.CURLY_OPEN:
            return (object) this.ParseObject();
          case MiniJSON.Parser.TOKEN.SQUARED_OPEN:
            return (object) this.ParseArray();
          case MiniJSON.Parser.TOKEN.STRING:
            return (object) this.ParseString();
          case MiniJSON.Parser.TOKEN.NUMBER:
            return this.ParseNumber();
          case MiniJSON.Parser.TOKEN.TRUE:
            return (object) true;
          case MiniJSON.Parser.TOKEN.FALSE:
            return (object) false;
          case MiniJSON.Parser.TOKEN.NULL:
            return (object) null;
          default:
            return (object) null;
        }
      }

      private string ParseString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        this.json.Read();
        bool flag = true;
        while (flag)
        {
          if (this.json.Peek() == -1)
            break;
          char nextChar1 = this.NextChar;
          switch (nextChar1)
          {
            case '"':
              flag = false;
              break;
            case '\\':
              if (this.json.Peek() == -1)
              {
                flag = false;
                break;
              }
              char nextChar2 = this.NextChar;
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
                    chArray[index] = this.NextChar;
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
        string nextWord = this.NextWord;
        if (nextWord.IndexOf('.') == -1)
        {
          long result;
          long.TryParse(nextWord, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result);
          return (object) result;
        }
        double result1;
        double.TryParse(nextWord, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result1);
        return (object) result1;
      }

      private void EatWhitespace()
      {
        while (char.IsWhiteSpace(this.PeekChar))
        {
          this.json.Read();
          if (this.json.Peek() == -1)
            break;
        }
      }

      private char PeekChar => Convert.ToChar(this.json.Peek());

      private char NextChar => Convert.ToChar(this.json.Read());

      private string NextWord
      {
        get
        {
          StringBuilder stringBuilder = new StringBuilder();
          while (!MiniJSON.Parser.IsWordBreak(this.PeekChar))
          {
            stringBuilder.Append(this.NextChar);
            if (this.json.Peek() == -1)
              break;
          }
          return stringBuilder.ToString();
        }
      }

      private MiniJSON.Parser.TOKEN NextToken
      {
        get
        {
          this.EatWhitespace();
          if (this.json.Peek() == -1)
            return MiniJSON.Parser.TOKEN.NONE;
          switch (this.PeekChar)
          {
            case '"':
              return MiniJSON.Parser.TOKEN.STRING;
            case ',':
              this.json.Read();
              return MiniJSON.Parser.TOKEN.COMMA;
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
              return MiniJSON.Parser.TOKEN.NUMBER;
            case ':':
              return MiniJSON.Parser.TOKEN.COLON;
            case '[':
              return MiniJSON.Parser.TOKEN.SQUARED_OPEN;
            case ']':
              this.json.Read();
              return MiniJSON.Parser.TOKEN.SQUARED_CLOSE;
            case '{':
              return MiniJSON.Parser.TOKEN.CURLY_OPEN;
            case '}':
              this.json.Read();
              return MiniJSON.Parser.TOKEN.CURLY_CLOSE;
            default:
              switch (this.NextWord)
              {
                case "false":
                  return MiniJSON.Parser.TOKEN.FALSE;
                case "true":
                  return MiniJSON.Parser.TOKEN.TRUE;
                case "null":
                  return MiniJSON.Parser.TOKEN.NULL;
                default:
                  return MiniJSON.Parser.TOKEN.NONE;
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

      private Serializer() => this.builder = new StringBuilder();

      public static string Serialize(object obj)
      {
        MiniJSON.Serializer serializer = new MiniJSON.Serializer();
        serializer.SerializeValue(obj, 1);
        return serializer.builder.ToString();
      }

      private void SerializeValue(object value, int indentationLevel)
      {
        switch (value)
        {
          case null:
            this.builder.Append("null");
            break;
          case string str:
            this.SerializeString(str);
            break;
          case bool flag:
            this.builder.Append(flag ? "true" : "false");
            break;
          case IList anArray:
            this.SerializeArray(anArray, indentationLevel);
            break;
          case IDictionary dictionary:
            this.SerializeObject(dictionary, indentationLevel);
            break;
          case char c:
            this.SerializeString(new string(c, 1));
            break;
          default:
            this.SerializeOther(value);
            break;
        }
      }

      private void SerializeObject(IDictionary obj, int indentationLevel)
      {
        bool flag = true;
        this.builder.Append('{');
        this.builder.Append('\n');
        for (int index = 0; index < indentationLevel; ++index)
          this.builder.Append('\t');
        foreach (object key in (IEnumerable) obj.Keys)
        {
          if (!flag)
          {
            this.builder.Append(',');
            this.builder.Append('\n');
            for (int index = 0; index < indentationLevel; ++index)
              this.builder.Append('\t');
          }
          this.SerializeString(key.ToString());
          this.builder.Append(':');
          ++indentationLevel;
          this.SerializeValue(obj[key], indentationLevel);
          --indentationLevel;
          flag = false;
        }
        this.builder.Append('\n');
        for (int index = 0; index < indentationLevel - 1; ++index)
          this.builder.Append('\t');
        this.builder.Append('}');
      }

      private void SerializeArray(IList anArray, int indentationLevel)
      {
        this.builder.Append('[');
        bool flag = true;
        for (int index = 0; index < anArray.Count; ++index)
        {
          object an = anArray[index];
          if (!flag)
            this.builder.Append(',');
          this.SerializeValue(an, indentationLevel);
          flag = false;
        }
        this.builder.Append(']');
      }

      private void SerializeString(string str)
      {
        this.builder.Append('"');
        foreach (char ch in str.ToCharArray())
        {
          switch (ch)
          {
            case '\b':
              this.builder.Append("\\b");
              break;
            case '\t':
              this.builder.Append("\\t");
              break;
            case '\n':
              this.builder.Append("\\n");
              break;
            case '\f':
              this.builder.Append("\\f");
              break;
            case '\r':
              this.builder.Append("\\r");
              break;
            case '"':
              this.builder.Append("\\\"");
              break;
            case '\\':
              this.builder.Append("\\\\");
              break;
            default:
              int int32 = Convert.ToInt32(ch);
              if (int32 >= 32 && int32 <= 126)
              {
                this.builder.Append(ch);
                break;
              }
              this.builder.Append("\\u");
              this.builder.Append(int32.ToString("x4"));
              break;
          }
        }
        this.builder.Append('"');
      }

      private void SerializeOther(object value)
      {
        int num1;
        switch (value)
        {
          case float num2:
            this.builder.Append(num2.ToString((IFormatProvider) CultureInfo.InvariantCulture));
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
          this.builder.Append(value);
        else if (value is double || value is Decimal)
        {
          this.builder.Append(Convert.ToDouble(value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
        }
        else
        {
          switch (value)
          {
            case Vector2 vector2:
              this.builder.Append("\"(" + vector2.x.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + vector2.y.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ")\"");
              break;
            case Vector3 vector3:
              this.builder.Append("\"(" + vector3.x.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + vector3.y.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + vector3.z.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ")\"");
              break;
            case Vector4 vector4:
              this.builder.Append("\"(" + vector4.x.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + vector4.y.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + vector4.z.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + vector4.w.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ")\"");
              break;
            case Quaternion quaternion:
              this.builder.Append("\"(" + quaternion.x.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + quaternion.y.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + quaternion.z.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "," + quaternion.w.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ")\"");
              break;
            default:
              this.SerializeString(value.ToString());
              break;
          }
        }
      }
    }
  }
}
