using BehaviorDesigner.Runtime;

public static class BehaviorSubtreeUtility
{
  public static BehaviorTree GetCharacterSubtree(GameObject character)
  {
    if ((Object) character == (Object) null)
    {
      Debug.LogError((object) "character == null");
      return null;
    }
    BehaviorTree[] components = character.GetComponents<BehaviorTree>();
    if (components.Length < 2)
    {
      PrepareCharacter(character);
      components = character.GetComponents<BehaviorTree>();
    }
    return components.Length > 1 ? components[1] : null;
  }

  public static void SetCharacterSubtree(BehaviorTree tree, ExternalBehaviorTree newTree)
  {
    if ((Object) tree == (Object) null)
      return;
    string name = tree.ExternalBehaviorTree?.name;
    if ((Object) newTree != (Object) null)
      tree.enabled = true;
    bool startWhenEnabled = tree.StartWhenEnabled;
    if ((Object) newTree == (Object) null)
      tree.StartWhenEnabled = false;
    tree.ExternalBehaviorTree = newTree;
    tree.StartWhenEnabled = startWhenEnabled;
    if (!((Object) newTree == (Object) null))
      return;
    tree.enabled = false;
  }

  private static void PrepareCharacter(GameObject character)
  {
    BehaviorTree[] components = character.GetComponents<BehaviorTree>();
    if (components.Length != 1)
      return;
    BehaviorTree behaviorTree1 = components[0];
    BehaviorTree behaviorTree2 = character.AddComponent<BehaviorTree>();
    behaviorTree2.StartWhenEnabled = behaviorTree1.StartWhenEnabled;
    behaviorTree2.PauseWhenDisabled = behaviorTree1.PauseWhenDisabled;
    behaviorTree2.RestartWhenComplete = behaviorTree1.RestartWhenComplete;
    behaviorTree2.ResetValuesOnRestart = behaviorTree1.ResetValuesOnRestart;
  }
}
