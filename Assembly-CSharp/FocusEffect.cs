using UnityEngine;

public class FocusEffect : MonoBehaviour
{
  private static MaterialPropertyBlock propertyBlock;
  [SerializeField]
  private Renderer[] renderers;
  private bool initialized;
  private DialogIndicationView externalEffect;
  private Coroutine disablingCoroutine;

  private void Disable() => enabled = false;

  private void OnEnable()
  {
    if (!initialized)
    {
      if (renderers == null || renderers.Length == 0)
        renderers = GetComponentsInChildren<Renderer>();
      initialized = true;
    }
    if (propertyBlock == null)
    {
      propertyBlock = new MaterialPropertyBlock();
      propertyBlock.SetInt("_FocusEffect", 1);
    }
    for (int index = 0; index < renderers.Length; ++index)
      renderers[index].SetPropertyBlock(propertyBlock);
    if (externalEffect == null)
    {
      externalEffect = DialogIndicationView.Create(transform);
      if (externalEffect != null)
      {
        Renderer renderer1 = null;
        float num1 = float.MinValue;
        for (int index = 0; index < renderers.Length; ++index)
        {
          Renderer renderer2 = renderers[index];
          Vector3 extents = renderer2.bounds.extents;
          float num2 = extents.x * extents.y * extents.z;
          if (num2 > (double) num1)
          {
            renderer1 = renderer2;
            num1 = num2;
          }
        }
        if (renderer1 is MeshRenderer)
          externalEffect.SetShape((MeshRenderer) renderer1);
        else if (renderer1 is SkinnedMeshRenderer)
          externalEffect.SetShape((SkinnedMeshRenderer) renderer1);
      }
    }
    if (!(externalEffect != null))
      return;
    externalEffect.SetVisibility(true);
  }

  private void OnDisable()
  {
    for (int index = 0; index < renderers.Length; ++index)
    {
      Renderer renderer = renderers[index];
      if (renderer != null)
        renderer.SetPropertyBlock(null);
      else
        Debug.LogError("render == null, разобраться");
    }
    if (!(externalEffect != null))
      return;
    externalEffect.SetVisibility(false);
  }

  public void SetActive(bool value)
  {
    if (value)
    {
      if (enabled)
        CancelInvoke("Disable");
      else
        enabled = true;
    }
    else if (enabled)
      Invoke("Disable", 1f);
  }
}
