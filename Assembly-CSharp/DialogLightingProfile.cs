using UnityEngine;

[CreateAssetMenu(menuName = "Data/Dialog Lighting Profile")]
public class DialogLightingProfile : ScriptableObject
{
  [SerializeField]
  private Vector2 keyLightRotation = new(15f, -120f);
  [SerializeField]
  private Vector2 backLightRotation = new(0.0f, 30f);
  [SerializeField]
  private Vector2 fillLightRotation = new(-15f, 90f);

  public Vector2 KeyLightRotation => keyLightRotation;

  public Vector2 BackLightRotation => backLightRotation;

  public Vector2 FillLightRotation => fillLightRotation;
}
