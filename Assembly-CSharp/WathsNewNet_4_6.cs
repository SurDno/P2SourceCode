// Decompiled with JetBrains decompiler
// Type: WathsNewNet_4_6
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
public class WathsNewNet_4_6
{
  private bool TestProperty { get; set; } = true;

  private void StringFormat() => string.Format("Строка с захватом переменных {0}", (object) 1);

  private event Action Event;

  private void NullOperator()
  {
    if (this.Event != null)
      this.Event();
    Action action = this.Event;
    if (action != null)
      action();
    WathsNewNet_4_6.TestClass testClass = (WathsNewNet_4_6.TestClass) null;
    object obj = testClass?.Value;
    obj = testClass?.Value;
  }

  private void NameofOperator()
  {
  }

  public class TestClass
  {
    public object Value;
  }
}
