using UnityEngine;

public sealed class WorldHealthBar : MonoBehaviour
{
    private const string SortingLayerName = "GAME OBJECT";
    private const int SortingOrder = 400;

    private static Sprite barSprite;

    private Transform target;
    private Vector3 offset;
    private float width;
    private float height;

    private Transform fillTransform;

    public static WorldHealthBar Create(
        Transform target,
        Vector3 offset,
        Color fillColor,
        float width = 1.2f,
        float height = 0.12f)
    {
        GameObject barObject =
            new GameObject($"{target.name} HealthBar");

        barObject.transform.SetParent(
            target,
            false);

        WorldHealthBar healthBar =
            barObject.AddComponent<WorldHealthBar>();

        healthBar.Initialize(
            target,
            offset,
            fillColor,
            width,
            height);

        return healthBar;
    }

    public void SetValue(
        int current,
        int max)
    {
        float normalized =
            max <= 0
                ? 0f
                : Mathf.Clamp01((float)current / max);

        fillTransform.localScale =
            new Vector3(
                width * normalized,
                height,
                1f);

        fillTransform.localPosition =
            new Vector3(
                (-width * 0.5f) +
                ((width * normalized) * 0.5f),
                0f,
                -0.01f);
    }

    private void Initialize(
        Transform followTarget,
        Vector3 followOffset,
        Color fillColor,
        float barWidth,
        float barHeight)
    {
        target = followTarget;
        offset = followOffset;
        width = barWidth;
        height = barHeight;

        CreateBarSprite();

        transform.position =
            target.position + offset;

        SpriteRenderer background =
            CreatePart(
                "Background",
                new Color(0.08f, 0.04f, 0.03f, 0.9f),
                SortingOrder);

        background.transform.localScale =
            new Vector3(
                width + 0.08f,
                height + 0.06f,
                1f);

        SpriteRenderer fill =
            CreatePart(
                "Fill",
                fillColor,
                SortingOrder + 1);

        fillTransform =
            fill.transform;
    }

    private SpriteRenderer CreatePart(
        string partName,
        Color color,
        int sortingOrder)
    {
        GameObject part =
            new GameObject(partName);

        part.transform.SetParent(
            transform,
            false);

        SpriteRenderer renderer =
            part.AddComponent<SpriteRenderer>();

        renderer.sprite =
            barSprite;

        renderer.color =
            color;

        renderer.sortingLayerName =
            SortingLayerName;

        renderer.sortingOrder =
            sortingOrder;

        return renderer;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position =
            target.position + offset;
    }

    private static void CreateBarSprite()
    {
        if (barSprite != null)
        {
            return;
        }

        Texture2D texture =
            new Texture2D(
                8,
                8);

        texture.filterMode =
            FilterMode.Point;

        Color[] pixels =
            new Color[64];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] =
                Color.white;
        }

        texture.SetPixels(
            pixels);

        texture.Apply();

        barSprite =
            Sprite.Create(
                texture,
                new Rect(0f, 0f, 8f, 8f),
                new Vector2(0.5f, 0.5f),
                8f);
    }
}
