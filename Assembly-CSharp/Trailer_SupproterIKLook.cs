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
  private bool startToHeadLook = false;

  private void Start()
  {
    this.lookComponent = this.GetComponent<LookAtIK>();
    this.ResetWeight();
  }

  private void ResetWeight()
  {
    this.lookComponent.solver.bodyWeight = 0.0f;
    this.lookComponent.solver.headWeight = 0.0f;
    this.lookComponent.solver.eyesWeight = 0.0f;
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.L))
      this.startToHeadLook = !this.startToHeadLook;
    if (this.startToHeadLook)
    {
      this.lookComponent.solver.bodyWeight += Time.deltaTime / this.SpeedLowerBody;
      this.lookComponent.solver.headWeight += Time.deltaTime / this.SpeedLowerHead;
      this.lookComponent.solver.eyesWeight += Time.deltaTime / this.SpeedLowerHead;
      if ((double) this.lookComponent.solver.bodyWeight >= (double) this.BodyWeightMax)
        this.lookComponent.solver.bodyWeight = this.BodyWeightMax;
      if ((double) this.lookComponent.solver.headWeight >= (double) this.HeadWeightMax)
        this.lookComponent.solver.headWeight = this.HeadWeightMax;
      if ((double) this.lookComponent.solver.eyesWeight >= (double) this.EyesWeightMax)
        this.lookComponent.solver.eyesWeight = this.EyesWeightMax;
    }
    if (this.startToHeadLook || (double) this.lookComponent.solver.headWeight <= 0.0)
      return;
    this.ResetWeight();
  }
}
