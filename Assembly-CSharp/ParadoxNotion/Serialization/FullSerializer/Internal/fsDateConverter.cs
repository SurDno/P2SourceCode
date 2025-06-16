using System;
using System.Globalization;

namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
  public class fsDateConverter : fsConverter
  {
    private const string DefaultDateTimeFormatString = "o";
    private const string DateTimeOffsetFormatString = "o";

    private string DateTimeFormatString => this.Serializer.Config.CustomDateTimeFormatString ?? "o";

    public override bool CanProcess(Type type)
    {
      return type == typeof (DateTime) || type == typeof (DateTimeOffset) || type == typeof (TimeSpan);
    }

    public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
    {
      switch (instance)
      {
        case DateTime dateTime:
          serialized = new fsData(dateTime.ToString(this.DateTimeFormatString));
          return fsResult.Success;
        case DateTimeOffset dateTimeOffset:
          serialized = new fsData(dateTimeOffset.ToString("o"));
          return fsResult.Success;
        case TimeSpan timeSpan:
          serialized = new fsData(timeSpan.ToString());
          return fsResult.Success;
        default:
          throw new InvalidOperationException("FullSerializer Internal Error -- Unexpected serialization type");
      }
    }

    public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
    {
      if (!data.IsString)
        return fsResult.Fail("Date deserialization requires a string, not " + (object) data.Type);
      if (storageType == typeof (DateTime))
      {
        DateTime result;
        if (DateTime.TryParse(data.AsString, (IFormatProvider) null, DateTimeStyles.RoundtripKind, out result))
        {
          instance = (object) result;
          return fsResult.Success;
        }
        if (!fsGlobalConfig.AllowInternalExceptions)
          return fsResult.Fail("Unable to parse " + data.AsString + " into a DateTime");
        try
        {
          instance = (object) Convert.ToDateTime(data.AsString);
          return fsResult.Success;
        }
        catch (Exception ex)
        {
          return fsResult.Fail("Unable to parse " + data.AsString + " into a DateTime; got exception " + (object) ex);
        }
      }
      else
      {
        if (storageType == typeof (DateTimeOffset))
        {
          DateTimeOffset result;
          if (!DateTimeOffset.TryParse(data.AsString, (IFormatProvider) null, DateTimeStyles.RoundtripKind, out result))
            return fsResult.Fail("Unable to parse " + data.AsString + " into a DateTimeOffset");
          instance = (object) result;
          return fsResult.Success;
        }
        if (!(storageType == typeof (TimeSpan)))
          throw new InvalidOperationException("FullSerializer Internal Error -- Unexpected deserialization type");
        TimeSpan result1;
        if (!TimeSpan.TryParse(data.AsString, out result1))
          return fsResult.Fail("Unable to parse " + data.AsString + " into a TimeSpan");
        instance = (object) result1;
        return fsResult.Success;
      }
    }
  }
}
