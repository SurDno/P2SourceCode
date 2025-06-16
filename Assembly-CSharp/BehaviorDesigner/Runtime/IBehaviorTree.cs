// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.IBehaviorTree
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime
{
  public interface IBehaviorTree
  {
    string GetOwnerName();

    int GetInstanceID();

    BehaviorSource BehaviorSource { get; }

    Object GetObject();

    SharedVariable GetVariable(string name);

    void SetVariable(string name, SharedVariable item);

    void SetVariableValue(string name, object value);
  }
}
