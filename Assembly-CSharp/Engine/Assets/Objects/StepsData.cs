namespace Engine.Assets.Objects
{
  public class StepsData : ScriptableObject
  {
    public StepsAction[] Actions;
    public StepsReaction[] Reactions;
    public PhysicMaterial[] DetailLayers;
    public PhysicMaterial[] TileLayers;
  }
}
