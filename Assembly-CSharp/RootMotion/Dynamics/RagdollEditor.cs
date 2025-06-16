// Decompiled with JetBrains decompiler
// Type: RootMotion.Dynamics.RagdollEditor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.Dynamics
{
  [HelpURL("https://www.youtube.com/watch?v=y-luLRVmL7E&index=1&list=PLVxSIA1OaTOuE2SB9NUbckQ9r2hTg4mvL")]
  [AddComponentMenu("Scripts/RootMotion.Dynamics/Ragdoll Manager/Ragdoll Editor")]
  public class RagdollEditor : MonoBehaviour
  {
    [HideInInspector]
    public Rigidbody selectedRigidbody;
    [HideInInspector]
    public Collider selectedCollider;
    [HideInInspector]
    public bool symmetry = true;
    [HideInInspector]
    public RagdollEditor.Mode mode;

    [ContextMenu("User Manual")]
    private void OpenUserManual()
    {
      Application.OpenURL("http://root-motion.com/puppetmasterdox/html/page2.html");
    }

    [ContextMenu("Scrpt Reference")]
    private void OpenScriptReference()
    {
      Application.OpenURL("http://root-motion.com/puppetmasterdox/html/class_root_motion_1_1_dynamics_1_1_ragdoll_editor.html");
    }

    [ContextMenu("TUTORIAL VIDEO")]
    private void OpenTutorial()
    {
      Application.OpenURL("https://www.youtube.com/watch?v=y-luLRVmL7E&index=1&list=PLVxSIA1OaTOuE2SB9NUbckQ9r2hTg4mvL");
    }

    [Serializable]
    public enum Mode
    {
      Colliders,
      Joints,
    }
  }
}
