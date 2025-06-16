// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Crowds.ICrowdContextComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components.Parameters;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Components.Crowds
{
  public interface ICrowdContextComponent : IComponent
  {
    void RestoreState(List<IParameter> states, bool indoor);

    void StoreState(List<IParameter> states, bool indoor);
  }
}
