using UnityEngine;
using UnityEngine.UI;

public class GradientLegend : MonoBehaviour
{
    public Image gradientImage; // Assign this in the Inspector

    void Start()
    {
        Texture2D gradientTexture = CreateGradientTexture();
        ApplyTextureToImage(gradientTexture);
    }

    Texture2D CreateGradientTexture()
    {
        Texture2D texture = new Texture2D(256, 1);
        Gradient gradient = new Gradient();
        gradient.colorKeys = new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.red, 1.0f) };

        for (int i = 0; i < texture.width; i++)
        {
            Color color = gradient.Evaluate(i / (float)texture.width);
            texture.SetPixel(i, 0, color);
        }

        texture.Apply();
        return texture;
    }

    void ApplyTextureToImage(Texture2D texture)
    {
        if (gradientImage != null)
        {
            gradientImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
}
