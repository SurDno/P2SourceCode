using System;
using UnityEngine;

namespace Engine.Source.Services.Consoles.Binds
{
  public class ThrowScript : MonoBehaviour
  {
    private void Start() => throw new Exception();

    private void Update()
    {
      if (!((UnityEngine.Object) this.gameObject != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }
  }
}
