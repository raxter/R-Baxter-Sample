using UnityEngine;
using System.Collections;

public class SplatShaderController : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer _targetMesh;

    private float _normalizationAmount = 0.5f;
    private float _showRedSplatAmount = 0;
    private float _showGreenSplatAmount = 0;
    private float _showBlueSplatAmount = 0;
    
    void OnGUI()
    {
        ShowSplatSlider(ref _normalizationAmount, "_NormalizeValues", "Normalize Splat Values");
        ShowSplatSlider(ref _showRedSplatAmount, "_ShowRedSplat", "Show Red Splat");
        ShowSplatSlider(ref _showGreenSplatAmount, "_ShowGreenSplat", "Show Green Splat");
        ShowSplatSlider(ref _showBlueSplatAmount, "_ShowBlueSplat", "Show Blue Splat");
    }

    void ShowSplatSlider(ref float splatAmount, string propertyName, string label)
    {
        GUILayout.Label(label);
        splatAmount = GUILayout.HorizontalSlider(splatAmount, 0, 1);

        _targetMesh.material.SetFloat(propertyName, splatAmount);
    }
}
