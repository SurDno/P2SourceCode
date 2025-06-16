// Decompiled with JetBrains decompiler
// Type: GroupActivityObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class GroupActivityObject : MonoBehaviour
{
  public List<POIBase> childPOIs;
  public bool IsDialogActivity;
  private bool isBusy = false;
  public static List<GroupActivityObject> ActiveGroupObjects = new List<GroupActivityObject>();

  public bool IsBusy
  {
    get => this.isBusy;
    set => this.isBusy = value;
  }

  public void Awake()
  {
    this.IsBusy = false;
    POIBase component = this.GetComponent<POIBase>();
    if ((Object) component != (Object) null && !this.childPOIs.Contains(component))
      this.childPOIs.Add(component);
    foreach (POIBase childPoI in this.childPOIs)
      childPoI.IsChildPOI = true;
  }

  private void OnEnable()
  {
    if (GroupActivityObject.ActiveGroupObjects.Contains(this))
      return;
    GroupActivityObject.ActiveGroupObjects.Add(this);
  }

  private void OnDisable()
  {
    if (!GroupActivityObject.ActiveGroupObjects.Contains(this))
      return;
    GroupActivityObject.ActiveGroupObjects.Remove(this);
  }
}
