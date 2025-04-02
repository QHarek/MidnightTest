public enum BuildingAction
{
    Build,
    Destroy
}

public interface IBuildingManagerObserver
{
    void OnBuildingAction(BuildingAction action, Machine building);
}