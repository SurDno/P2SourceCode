using UnityEngine;

[CreateAssetMenu(menuName = "Data/Dialog Lighting Profile")]
public class DialogLightingProfile : ScriptableObject
{
  [SerializeField]
  private Vector2 keyLightRotation = new Vector2(15f, -120f);
  [SerializeField]
  private Vector2 backLightRotation = new Vector2(0.0f, 30f);
  [SerializeField]
  private Vector2 fillLightRotation = new Vector2(-15f, 90f);

  public Vector2 KeyLightRotation => this.keyLightRotation;

  public Vector2 BackLightRotation => this.backLightRotation;

  public Vector2 FillLightRotation => this.fillLightRotation;
}
