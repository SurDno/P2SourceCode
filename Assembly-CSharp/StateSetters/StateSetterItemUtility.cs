// Decompiled with JetBrains decompiler
// Type: StateSetters.StateSetterItemUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace StateSetters
{
  public static class StateSetterItemUtility
  {
    public static void Apply(this StateSetterItem[] states, bool on)
    {
      if (states == null)
        return;
      foreach (StateSetterItem state in states)
        state.Apply(on ? 1f : 0.0f);
    }

    public static void Apply(this StateSetterItem[] states, float value)
    {
      if (states == null)
        return;
      foreach (StateSetterItem state in states)
        state.Apply(value);
    }
  }
}
