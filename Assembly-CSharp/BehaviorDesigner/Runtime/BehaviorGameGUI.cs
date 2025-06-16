// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.BehaviorGameGUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime
{
  public class BehaviorGameGUI : MonoBehaviour
  {
    private BehaviorTreeManager behaviorManager;
    private Camera mainCamera;

    public void Start() => this.mainCamera = Camera.main;

    public void OnGUI()
    {
      if ((Object) this.behaviorManager == (Object) null)
        this.behaviorManager = MonoBehaviourInstance<BehaviorTreeManager>.Instance;
      if ((Object) this.behaviorManager == (Object) null || (Object) this.mainCamera == (Object) null)
        return;
      List<BehaviorTreeClient> behaviorTrees = this.behaviorManager.BehaviorTrees;
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
