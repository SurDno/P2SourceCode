// Decompiled with JetBrains decompiler
// Type: Pathologic.Prototype.PlagueFaceTestController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace Pathologic.Prototype
{
  public class PlagueFaceTestController : MonoBehaviour
  {
    private bool _isInitialized;
    public PlagueFace face;
    public Transform graph;
    public Image overlay;
    public Camera playerCamera;
    public Transform playerCenter;

    private void Disable() => this.face.gameObject.SetActive(false);

    private void Enable()
    {
      if (!this._isInitialized || this.face.gameObject.activeSelf)
      {
        this.face.InitializeAt(this.graph.GetChild(Random.Range(0, this.graph.childCount)).GetComponent<PlagueFacePoint>());
        this.face.navigation.playerCamera = this.playerCamera;
        this._isInitialized = true;
      }
      this.face.gameObject.SetActive(true);
    }

    private void Update()
    {
      this.face.playerPosition = this.playerCenter.position;
      this.overlay.color = new Color(0.25f, 0.0f, 0.0f, this.face.attack * 0.9f);
      if (Input.GetKeyDown(KeyCode.E))
        this.Enable();
      if (!Input.GetKeyDown(KeyCode.Q))
        return;
      this.Disable();
    }
  }
}
