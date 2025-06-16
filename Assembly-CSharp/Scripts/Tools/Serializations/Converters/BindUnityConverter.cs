using Cofe.Meta;
using Cofe.Serializations.Converters;
using UnityEngine;

namespace Scripts.Tools.Serializations.Converters
{
  [Initialisable]
  public static class BindUnityConverter
  {
    [Initialise]
    private static void Initialize()
    {
      ConvertService.AddConverter((string value, out Vector2 result) => UnityConverter.TryParseVector2(value, out result), value => UnityConverter.ToString(value));
      ConvertService.AddConverter((string value, out Vector3 result) => UnityConverter.TryParseVector3(value, out result), value => UnityConverter.ToString(value));
      ConvertService.AddConverter((string value, out Vector4 result) => UnityConverter.TryParseVector4(value, out result), value => UnityConverter.ToString(value));
      ConvertService.AddConverter((string value, out Quaternion result) => UnityConverter.TryParseQuaternion(value, out result), value => UnityConverter.ToString(value));
      ConvertService.AddConverter((string value, out Color result) => UnityConverter.TryParseColor(value, out result), value => UnityConverter.ToString(value));
    }
  }
}
