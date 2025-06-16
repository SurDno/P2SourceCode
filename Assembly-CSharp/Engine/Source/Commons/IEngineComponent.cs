// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.IEngineComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;

#nullable disable
namespace Engine.Source.Commons
{
  public interface IEngineComponent
  {
    void OnChangeEnabled();

    void PrepareAdded();

    void PostRemoved();

    IEntity Owner { set; }
  }
}
