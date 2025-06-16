// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.TaskState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace BehaviorDesigner.Runtime
{
  public enum TaskState
  {
    RootTaskDisabled = -6, // 0xFFFFFFFA
    BehaviorTreeReferenceTaskContainsNullExternalTree = -5, // 0xFFFFFFFB
    MultipleExternalBehaviorTreesAndParentTaskIsNullOrCannotHandleAsManyBehaviorTreesSpecified = -4, // 0xFFFFFFFC
    TaskIsNull = -3, // 0xFFFFFFFD
    ExternalTaskCannotBeFound = -2, // 0xFFFFFFFE
    ParentNotHaveAnyChildren = -1, // 0xFFFFFFFF
    Success = 0,
  }
}
