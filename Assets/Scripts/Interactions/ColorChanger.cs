using System.Xml.Serialization;
using UnityEngine;

public class ColorChanger
{
    private Color _defaultColor;

    public void SetDefaultColor(Color color)
    {
        _defaultColor = color;
    }

    public void ChangeColor(MeshRenderer renderer, Color newColor)
    {
        renderer.material.color = newColor;
    }

    public void TryChangeColorToDefault(MeshRenderer renderer)
    {
        if (_defaultColor != null)
            ChangeColor(renderer, _defaultColor);
    }
}
