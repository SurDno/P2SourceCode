using RootMotion.FinalIK;
using UnityEngine;

public class Trailer_SupproterIKLook : MonoBehaviour
{
  private LookAtIK lookComponent;
  public float SpeedLowerHead = 2f;
  public float SpeedLowerBody = 3f;
  public float BodyWeightMax = 0.5f;
  public float HeadWeightMax = 1f;
  public float EyesWeightMax = 1f;
  private bool startToHeadLook;

  private void Start()
  {
    lookComponent = GetComponent<LookAtIK>();
    ResetWeight();
  }

  private void ResetWeight()
  {
    lookComponent.solver.bodyWeight = 0.0f;
    lookComponent.solver.headWeight = 0.0f;
    lookComponent.solver.eyesWeight = 0.0f;
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.L))
      startToHeadLook = !startToHeadLook;
    if (startToHeadLook)
    {
      lookComponent.solver.bodyWeight += Time.deltaTime / SpeedLowerBody;
      lookComponent.solver.headWeight += Time.deltaTime / SpeedLowerHead;
      lookComponent.solver.eyesWeight += Time.deltaTime / SpeedLowerHead;
      if (lookComponent.solver.bodyWeight >= (double) BodyWeightMax)
        lookComponent.solver.bodyWeight = BodyWeightMax;
      if (lookComponent.solver.headWeight >= (double) HeadWeightMax)
        lookComponent.solver.headWeight = HeadWeightMax;
      if (lookComponent.solver.eyesWeight >= (double) EyesWeightMax)
        lookComponent.solver.eyesWeight = EyesWeightMax;
    }
    if (startToHeadLook || lookComponent.solver.headWeight <= 0.0)
      return;
    ResetWeight();
  }
}
