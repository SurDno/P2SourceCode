using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime
{
  public class BehaviorGameGUI : MonoBehaviour
  {
    private BehaviorTreeManager behaviorManager;
    private Camera mainCamera;

    public void Start() => mainCamera = Camera.main;

    public void OnGUI()
    {
      if ((Object) behaviorManager == (Object) null)
        behaviorManager = MonoBehaviourInstance<BehaviorTreeManager>.Instance;
      if ((Object) behaviorManager == (Object) null || (Object) mainCamera == (Object) null)
        return;
      List<BehaviorTreeClient> behaviorTrees = behaviorManager.BehaviorTrees;
      for (int index1 = 0; index1 < behaviorTrees.Count; ++index1)
      {
        BehaviorTreeClient behaviorTreeClient = behaviorTrees[index1];
        string text = "";
        for (int index2 = 0; index2 < behaviorTreeClient.activeStack.Count; ++index2)
        {
          Stack<int> active = behaviorTreeClient.activeStack[index2];
          if (active.Count != 0 && behaviorTreeClient.taskList[active.Peek()] is Action)
            text = text + behaviorTreeClient.taskList[behaviorTreeClient.activeStack[index2].Peek()].FriendlyName + (index2 < behaviorTreeClient.activeStack.Count - 1 ? "\n" : "");
        }
        Vector2 guiPoint = GUIUtility.ScreenToGUIPoint((Vector2) Camera.main.WorldToScreenPoint(behaviorTreeClient.behavior.transform.position));
        GUIContent content = new GUIContent(text);
        Vector2 vector2 = GUI.skin.label.CalcSize(content);
        vector2.x += 14f;
        vector2.y += 5f;
        GUI.Box(new Rect(guiPoint.x - vector2.x / 2f, (float) ((double) Screen.height - (double) guiPoint.y + (double) vector2.y / 2.0), vector2.x, vector2.y), content);
      }
    }
  }
}
