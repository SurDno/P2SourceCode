using System;

public static class SafeTypeUtility
{
  public static Type GetSafeType(object o)
  {
    Type safeType = o.GetType();
    while (safeType.Namespace != null && safeType.Namespace.StartsWith("UnityEditor.") && safeType != typeof (object))
      safeType = safeType.BaseType;
    return safeType;
  }
}
