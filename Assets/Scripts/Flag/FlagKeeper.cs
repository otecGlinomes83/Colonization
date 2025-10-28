using UnityEngine;

[RequireComponent(typeof(ClickDetector))]
public class FlagKeeper : MonoBehaviour
{
   [SerializeField] private Flag _flagPrefab;
   [SerializeField] private FlagBlueprint _flagBlueprintPrefab;

    public void StartPlacement()
    {
        
    }
}
