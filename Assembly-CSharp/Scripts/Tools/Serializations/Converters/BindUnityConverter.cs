using Cofe.Meta;
using Cofe.Serializations.Converters;
using System;
using UnityEngine;

namespace Scripts.Tools.Serializations.Converters
{
  [Initialisable]
  public static class BindUnityConverter
  {
    [Initialise]
    private static void Initialize()
    {
      ConvertService.AddConverter<Vector2>((ConvertService.TryParseHandle<Vector2>) ((string value, out Vector2 result) => UnityConverter.TryParseVector2(value, out result)), (Func<Vector2, string>) (value => UnityConverter.ToString(value)));
      ConvertService.AddConverter<Vector3>((ConvertService.TryParseHandle<Vector3>) ((string value, out Vector3 result) => UnityConverter.TryParseVector3(value, out result)), (Func<Vector3, string>) (value => UnityConverter.ToString(value)));
      ConvertService.AddConverter<Vector4>((ConvertService.TryParseHandle<Vector4>) ((string value, out Vector4 result) => UnityConverter.TryParseVector4(value, out result)), (Func<Vector4, string>) (value => UnityConverter.ToString(value)));
      ConvertService.AddConverter<Quaternion>((ConvertService.TryParseHandle<Quaternion>) ((string value, out Quaternion result) => UnityConverter.TryParseQuaternion(value, out result)), (Func<Quaternion, string>) (value => UnityConverter.ToString(value)));
      ConvertService.AddConverter<Color>((ConvertService.TryParseHandle<Color>) ((string value, out Color result) => UnityConverter.TryParseColor(value, out result)), (Func<Color, string>) (value => UnityConverter.ToString(value)));
    }
  }
}
