using TMPro;
using UnityEngine;

public class ResourcesDisplayer : MonoBehaviour
{
    [SerializeField] private TMP_Text _countField;
    [SerializeField] private ResourceStorage _baseStorage;
    [SerializeField] private ResourceType _resourceType;

    private void Awake()
    {
        _countField.text = "0";
    }

    private void OnEnable()
    {
        _baseStorage.CountChanged += ChangeCount;
    }

    private void OnDisable()
    {
        _baseStorage.CountChanged -= ChangeCount;
    }

    private void ChangeCount(ResourceType resourceType, int count)
    {
        if (_resourceType == resourceType)
            _countField.text = $"{count}";
    }
}
