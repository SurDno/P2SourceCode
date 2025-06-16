// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.EventData`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace ParadoxNotion
{
  public class EventData<T> : EventData
  {
    public T value { get; private set; }

    protected override object GetValue() => (object) this.value;

    public EventData(string name, T value)
      : base(name)
    {
      this.value = value;
    }
  }
}
