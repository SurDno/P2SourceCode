// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.ConditionalReevaluate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using BehaviorDesigner.Runtime.Tasks;

#nullable disable
namespace BehaviorDesigner.Runtime
{
  public class ConditionalReevaluate
  {
    public int index;
    public TaskStatus taskStatus;
    public int compositeIndex = -1;
    public int stackIndex = -1;

    public void Initialize(int i, TaskStatus status, int stack, int composite)
    {
      this.index = i;
      this.taskStatus = status;
      this.stackIndex = stack;
      this.compositeIndex = composite;
    }
  }
}
