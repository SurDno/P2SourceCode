// Decompiled with JetBrains decompiler
// Type: Engine.Common.Types.LocalizedText
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

#nullable disable
namespace Engine.Common.Types
{
  public struct LocalizedText
  {
    public static readonly LocalizedText Empty;
    private readonly ulong id;

    public LocalizedText(ulong id) => this.id = id;

    public ulong Id => this.id;

    public override int GetHashCode() => this.Id.GetHashCode();

    public override bool Equals(object a)
    {
      return a is LocalizedText localizedText && this == localizedText;
    }

    public static bool operator ==(LocalizedText a, LocalizedText b) => (long) a.Id == (long) b.Id;

    public static bool operator !=(LocalizedText a, LocalizedText b) => !(a == b);
  }
}
