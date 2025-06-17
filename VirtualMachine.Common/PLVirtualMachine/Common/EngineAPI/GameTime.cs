using System;
using System.Globalization;
using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.Serialization;

namespace PLVirtualMachine.Common.EngineAPI
{
  [VMType("GameTime")]
  public class GameTime : ISerializeStateSave, IDynamicLoadSerializable, IVMStringSerializable
  {
    protected double totalValue;
    public static GameTime Zero = new();
    private static readonly int DaySeconds = 86400;
    private static readonly int HourSeconds = 3600;
    private static readonly int MinuteSeconds = 60;

    public GameTime() => totalValue = 0.0;

    public GameTime(ushort days, byte hours, byte minutes, byte seconds)
    {
      Init(days, hours, minutes, seconds);
    }

    public GameTime(ulong totalSeconds) => totalValue = totalSeconds;

    public GameTime(ulong totalSeconds, double lastSecond)
    {
      totalValue = totalSeconds + lastSecond;
    }

    public GameTime(GameTime copyGameTime)
    {
      if (copyGameTime == null)
        Logger.AddError(string.Format("Invalid game time assignment at {0}", EngineAPIManager.Instance.CurrentFSMStateInfo));
      else
        totalValue = copyGameTime.totalValue;
    }

    public void CopyFrom(object data) => totalValue = ((GameTime) data).totalValue;

    public void Init() => totalValue = 0.0;

    public void Init(ushort days, byte hours, byte minutes, byte seconds, double lastPart = 0.0)
    {
      totalValue = days * 24 * 3600 + hours * 3600 + minutes * 60 + seconds;
    }

    public void Process(double elapsedTimeVal) => totalValue += elapsedTimeVal;

    public ushort Days
    {
      get => (ushort) ((uint) (int) Math.Floor(totalValue) / (uint) DaySeconds);
      set
      {
        byte hours = Hours;
        byte minutes = Minutes;
        byte seconds = Seconds;
        double lastSecond = LastSecond;
        Init(value, hours, minutes, seconds, lastSecond);
      }
    }

    public byte Hours
    {
      get => (byte) ((int) Math.Floor(totalValue) % DaySeconds / HourSeconds);
      set
      {
        ushort days = Days;
        byte minutes = Minutes;
        byte seconds = Seconds;
        double lastSecond = LastSecond;
        Init(days, value, minutes, seconds, lastSecond);
      }
    }

    public byte Minutes
    {
      get => (byte) ((int) Math.Floor(totalValue) % HourSeconds / MinuteSeconds);
      set
      {
        ushort days = Days;
        byte hours = Hours;
        byte seconds = Seconds;
        double lastSecond = LastSecond;
        Init(days, hours, value, seconds, lastSecond);
      }
    }

    public byte Seconds
    {
      get => (byte) ((uint) (int) Math.Floor(totalValue) % (uint) MinuteSeconds);
      set
      {
        ushort days = Days;
        byte hours = Hours;
        byte minutes = Minutes;
        double lastSecond = LastSecond;
        Init(days, hours, minutes, value, lastSecond);
      }
    }

    public ulong TotalSeconds
    {
      get => (ulong) Math.Floor(totalValue);
      set
      {
        double num = totalValue - Math.Floor(totalValue);
        totalValue = value + num;
      }
    }

    public double TotalValue => totalValue;

    public double LastSecond => totalValue - Math.Floor(totalValue);

    public string Write()
    {
      Logger.AddError("Not allowed serialization data struct in virtual machine!");
      return string.Empty;
    }

    public void Read(string data)
    {
      switch (data)
      {
        case null:
          Logger.AddError(string.Format("Attempt to read null game time data at {0}", EngineAPIManager.Instance.CurrentFSMStateInfo));
          break;
        case "":
          break;
        default:
          string[] strArray = data.Split(':');
          if (strArray.Length < 4)
          {
            Logger.AddError(string.Format("Cannot convert {0} to GameTime at {1}", data, EngineAPIManager.Instance.CurrentFSMStateInfo));
            break;
          }

          double lastPart = 0.0;
          if (!ushort.TryParse(strArray[0], NumberStyles.Number, CultureInfo.InvariantCulture, out ushort result1))
            Logger.AddError(string.Format("Cannot convert {0} to GameTime at {1}", data, EngineAPIManager.Instance.CurrentFSMStateInfo));
          if (!byte.TryParse(strArray[1], NumberStyles.Number, CultureInfo.InvariantCulture, out byte result2))
            Logger.AddError(string.Format("Cannot convert {0} to GameTime at {1}", data, EngineAPIManager.Instance.CurrentFSMStateInfo));
          if (!byte.TryParse(strArray[2], NumberStyles.Number, CultureInfo.InvariantCulture, out byte result3))
            Logger.AddError(string.Format("Cannot convert {0} to GameTime at {1}", data, EngineAPIManager.Instance.CurrentFSMStateInfo));
          if (!byte.TryParse(strArray[3], NumberStyles.Number, CultureInfo.InvariantCulture, out byte result4))
            Logger.AddError(string.Format("Cannot convert {0} to GameTime at {1}", data, EngineAPIManager.Instance.CurrentFSMStateInfo));
          if (strArray.Length > 4)
          {
            if (!uint.TryParse(strArray[4], NumberStyles.Number, CultureInfo.InvariantCulture, out uint result5))
              Logger.AddError(string.Format("Cannot convert {0} to GameTime at {1}", data, EngineAPIManager.Instance.CurrentFSMStateInfo));
            lastPart = 9.9999999747524271E-07 * result5;
          }
          Init(result1, result2, result3, result4, lastPart);
          break;
      }
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.Save(writer, "Days", Days);
      SaveManagerUtility.Save(writer, "Hours", Hours);
      SaveManagerUtility.Save(writer, "Minutes", Minutes);
      SaveManagerUtility.Save(writer, "Seconds", Seconds);
      SaveManagerUtility.Save(writer, "LastMicroseconds", LastSecond);
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      ushort days = 0;
      byte hours = 0;
      byte minutes = 0;
      byte seconds = 0;
      double lastPart = 0.0;
      foreach (XmlElement childNode in xmlNode.ChildNodes)
      {
        if (childNode.Name == "Days")
          days = StringUtility.ToByte(childNode.InnerText);
        else if (childNode.Name == "Hours")
          hours = StringUtility.ToByte(childNode.InnerText);
        else if (childNode.Name == "Minutes")
          minutes = StringUtility.ToByte(childNode.InnerText);
        else if (childNode.Name == "Seconds")
          seconds = StringUtility.ToByte(childNode.InnerText);
        else if (childNode.Name == "LastMicroseconds")
          lastPart = StringUtility.ToDouble(childNode.InnerText);
      }
      Init(days, hours, minutes, seconds, lastPart);
    }

    public static implicit operator TimeSpan(GameTime time)
    {
      return TimeSpan.FromSeconds(time.TotalSeconds);
    }

    public static implicit operator GameTime(TimeSpan time)
    {
      return new GameTime((ulong) time.TotalSeconds);
    }
  }
}
