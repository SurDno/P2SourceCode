// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.AnimatorStateService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Unity.Mecanim;
using Engine.Common;
using System;

#nullable disable
namespace Engine.Source.Services
{
  [GameService(new Type[] {typeof (AnimatorStateService)})]
  public class AnimatorStateService : IInitialisable
  {
    public void Initialise()
    {
    }

    public void Terminate() => PlayerAnimatorState.Clear();
  }
}
