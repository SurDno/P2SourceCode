namespace BehaviorDesigner.Runtime
{
  public enum TaskState
  {
    RootTaskDisabled = -6,
    BehaviorTreeReferenceTaskContainsNullExternalTree = -5,
    MultipleExternalBehaviorTreesAndParentTaskIsNullOrCannotHandleAsManyBehaviorTreesSpecified = -4,
    TaskIsNull = -3,
    ExternalTaskCannotBeFound = -2,
    ParentNotHaveAnyChildren = -1,
    Success = 0,
  }
}
