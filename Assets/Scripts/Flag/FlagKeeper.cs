using System;
using UnityEngine;

[RequireComponent(typeof(FlagPlacer))]
[RequireComponent(typeof(ClickDetector))]
public class FlagKeeper : MonoBehaviour
{
    private FlagPlacer _flagPlacer;
    private StorageChecker _storageChecker;
    private MeshRenderer _meshRenderer;
    private ColorChanger _colorChanger = new ColorChanger();

    public event Action FlagPlaced;
    public event Action FlagDestroyed;

    private void Awake()
    {
        _flagPlacer = GetComponent<FlagPlacer>();
    }

    public void Initialize(StorageChecker storageChecker, MeshRenderer meshRenderer)
    {
        _storageChecker = storageChecker;
        _meshRenderer = meshRenderer;
        _colorChanger.SetDefaultColor(_meshRenderer.material.color);
    }

    public void TryStartPlacement()
    {
        if (_storageChecker.IsAbleToCreateBase() == false)
            return;

        _colorChanger.ChangeColor(_meshRenderer, Color.red);

        _flagPlacer.StartPlacement();
        _flagPlacer.Placed += OnFlagPlaced;
    }

    public void DestroyFlag()
    {
        _flagPlacer.DestroyFlag();
        FlagDestroyed?.Invoke();
    }

    private void OnFlagPlaced()
    {
        _flagPlacer.Placed -= OnFlagPlaced;
        _colorChanger.TryChangeColorToDefault(_meshRenderer);

        if (_storageChecker.IsAbleToCreateBase() == false)
        {
            _flagPlacer.DestroyFlag();

            return;
        }

        FlagPlaced?.Invoke();
    }

    public bool TryGetFlagPosition(out Transform flagPosition)
    {
        if (_flagPlacer.TryGetFlagPosition(out flagPosition))
            return true;

        return false;
    }
}
