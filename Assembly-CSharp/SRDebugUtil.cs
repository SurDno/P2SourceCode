using SRF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class SRDebugUtil
{
  public const int LineBufferCount = 512;

  public static bool IsFixedUpdate { get; set; }

  [DebuggerNonUserCode]
  [DebuggerStepThrough]
  public static void AssertNotNull(object value, string message = null, MonoBehaviour instance = null)
  {
    if (EqualityComparer<object>.Default.Equals(value, (object) null))
    {
      string str;
      if (message == null)
        str = "Assert Failed";
      else
        str = "NotNullAssert Failed: {0}".Fmt((object) message);
      message = str;
      UnityEngine.Debug.LogError((object) message, (UnityEngine.Object) instance);
      if ((UnityEngine.Object) instance != (UnityEngine.Object) null)
        instance.enabled = false;
      throw new NullReferenceException(message);
    }
  }

  [DebuggerNonUserCode]
  [DebuggerStepThrough]
  public static void Assert(bool condition, string message = null, MonoBehaviour instance = null)
  {
    if (!condition)
    {
      string str;
      if (message == null)
        str = "Assert Failed";
      else
        str = "Assert Failed: {0}".Fmt((object) message);
      message = str;
      UnityEngine.Debug.LogError((object) message, (UnityEngine.Object) instance);
      throw new Exception(message);
    }
  }

  [Conditional("UNITY_EDITOR")]
  [DebuggerNonUserCode]
  [DebuggerStepThrough]
  public static void EditorAssertNotNull(object value, string message = null, MonoBehaviour instance = null)
  {
    SRDebugUtil.AssertNotNull(value, message, instance);
  }

  [Conditional("UNITY_EDITOR")]
  [DebuggerNonUserCode]
  [DebuggerStepThrough]
  public static void EditorAssert(bool condition, string message = null, MonoBehaviour instance = null)
  {
    SRDebugUtil.Assert(condition, message, instance);
  }
}
