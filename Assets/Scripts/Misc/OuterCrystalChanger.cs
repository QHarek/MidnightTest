using UnityEngine;

public class OuterCrystalChanger : MonoBehaviour
{
    [SerializeField] Light _pointLight;
    [SerializeField] Renderer _crystal;

    public void UpdateColor(Color color)
    {
        _pointLight.color = color;
        _crystal.material.color = color;
    }
}
