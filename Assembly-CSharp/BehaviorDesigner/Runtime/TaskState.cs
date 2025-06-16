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
