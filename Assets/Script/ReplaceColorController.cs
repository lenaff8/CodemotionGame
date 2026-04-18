using UnityEngine;

public class ReplaceColorController : MonoBehaviour
{
    [Header("Renderer con el material")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Material runtimeMaterial;

    private void Awake()
    {
        // Importante: instanciamos el material para no modificar el asset global
        runtimeMaterial = spriteRenderer.material;
    }

    /// <summary>
    /// Cambia el color del shader (_ReplaceColor)
    /// </summary>
    public void SetReplaceColor(Color newColor)
    {
        if (runtimeMaterial != null)
        {
            runtimeMaterial.SetColor("_ReplaceColor", newColor);
        }
    }
}