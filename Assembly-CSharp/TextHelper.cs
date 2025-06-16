using System.Text;
using Cofe.Serializations.Converters;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Services.Inputs;
using Engine.Source.Utility;
using InputServices;

public class TextHelper
{
  public static string FormatString(string message, int fontSize, bool smallCaps, bool allCaps)
  {
    message = Localize(message);
    message = ReplaceTags(message);
    if (allCaps)
      message = FormatStringToUpper(message);
    else if (smallCaps)
      message = FormatStringUpperCharsToCapital(message, fontSize);
    return message;
  }

  private static string Localize(string message)
  {
    if (string.IsNullOrEmpty(message))
      return "";
    string str;
    for (int index = message.IndexOf('{'); index != -1; index = message.IndexOf('{', index + str.Length))
    {
      int num = message.IndexOf('}', index + 1);
      if (num != -1)
      {
        int startIndex = num + 1;
        string tag = message.Substring(index, startIndex - index);
        LocalizationService service = ServiceLocator.GetService<LocalizationService>();
        str = service != null ? service.GetText(tag) : tag;
        message = message.Substring(0, index) + str + message.Substring(startIndex);
      }
      else
        break;
    }
    return message;
  }

  public static string FormatStringToUpper(string message)
  {
    if (string.IsNullOrEmpty(message))
      return null;
    StringBuilder stringBuilder = new StringBuilder();
    for (int index = 0; index < message.Length; ++index)
    {
      char ch = message[index];
      if (CharIsEditable(message, index))
        stringBuilder.Append(ch.ToString().ToUpper());
      else
        stringBuilder.Append(ch);
    }
    message = stringBuilder.ToString();
    return message.ToUpper();
  }

  public static string FormatStringUpperCharsToCapital(string message, int fontSize)
  {
    if (string.IsNullOrEmpty(message))
      return null;
    string str = "<size=" + (object) Mathf.RoundToInt(fontSize * 1.25f) + ">";
    StringBuilder stringBuilder = new StringBuilder();
    bool flag = false;
    for (int index = 0; index < message.Length; ++index)
    {
      char c = message[index];
      if (CharIsEditable(message, index) && !char.IsLower(c))
      {
        if (!flag)
        {
          stringBuilder.Append(str);
          flag = true;
        }
        stringBuilder.Append(c);
      }
      else
      {
        if (flag)
        {
          stringBuilder.Append("</size>");
          flag = false;
        }
        stringBuilder.Append(char.ToUpperInvariant(c));
      }
    }
    if (flag)
      stringBuilder.Append("</size>");
    message = stringBuilder.ToString();
    return message;
  }

  private static bool CharIsEditable(string message, int charNum)
  {
    bool flag = false;
    for (int index = 0; index < message.Length; ++index)
    {
      switch (message[index])
      {
        case ' ':
          continue;
        case '<':
          flag = true;
          continue;
        case '>':
          flag = false;
          continue;
        default:
          if (!flag && charNum == index)
            return true;
          continue;
      }
    }
    return false;
  }

  public static string ReplaceTags(string text, string hotkeyPrefix, string hotkeySuffix)
  {
    if (string.IsNullOrEmpty(text))
      return text;
    string str1 = "<hotkey>";
    string str2 = "</hotkey>";
    while (true)
    {
      int length = text.IndexOf(str1);
      if (length != -1)
      {
        int num = text.IndexOf(str2);
        if (num != -1 && num > length)
        {
          string str3 = text.Substring(0, length);
          string str4 = text.Substring(num + str2.Length);
          string str5 = text.Substring(length + str1.Length, num - length - str1.Length);
          GameActionType result;
          if (DefaultConverter.TryParseEnum(str5, out result))
            str5 = InputUtility.GetHotKeyNameByAction(result, InputService.Instance.JoystickUsed);
          text = str3 + hotkeyPrefix + str5 + hotkeySuffix + str4;
        }
        else
          goto label_6;
      }
      else
        break;
    }
    return text;
label_6:
    return text;
  }

  public static string ReplaceTags(string text)
  {
    return ReplaceTags(text, string.Empty, string.Empty);
  }
}
