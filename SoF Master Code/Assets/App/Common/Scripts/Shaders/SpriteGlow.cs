using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SpriteGlow : MonoBehaviour
{
	[ColorUsage(true, true, 0f, 8f, 0.125f, 3f)]
	public Color GlowColor = Color.white;

	[Range(0, 16)]
	public int OutlineWidth = 1;

	private SpriteRenderer spriteRenderer;
    private Image img;

	private void OnEnable ()
	{
        if(GetComponent<SpriteRenderer>() != null)
    		spriteRenderer = GetComponent<SpriteRenderer>();
        if (GetComponent<Image>() != null)
            img = GetComponent<Image>();

		UpdateOutline(true);
	}

	private void OnDisable ()
	{
		UpdateOutline(false);
	}

	private void Update ()
	{
		UpdateOutline(true);
	}

    private void UpdateOutline(bool outline)
    {
        if (spriteRenderer != null)
        {
            var mpb = new MaterialPropertyBlock();
            spriteRenderer.GetPropertyBlock(mpb);
            mpb.SetFloat("_Outline", outline ? 1f : 0);
            mpb.SetColor("_OutlineColor", GlowColor);
            mpb.SetFloat("_OutlineSize", OutlineWidth);
            spriteRenderer.SetPropertyBlock(mpb);
        }
        if (img != null)
        {
            img.material.SetFloat("_Outline", outline ? 1f : 0);
            img.material.SetColor("_OutlineColor", GlowColor);
            img.material.SetFloat("_OutlineSize", OutlineWidth);
        }
    }
}
