using TMPro;
using UnityEngine;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _countField;
    [SerializeField] private ResourceStorage _resourceStorage;
    [SerializeField] private ResourceType _resourceType;

    private void Awake()
    {
        _countField.text = "0";
    }

    private void OnEnable()
    {
        _resourceStorage.CountChanged += ChangeCount;
    }

    private void OnDisable()
    {
        _resourceStorage.CountChanged -= ChangeCount;
    }

    private void ChangeCount(ResourceType resourceType, int count)
    {
        if (_resourceType == resourceType)
            _countField.text = $"{count}";
    }
}
