using TMPro;
using UnityEngine;

public sealed class DamageNumber : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;

    [SerializeField]
    private float lifetime = 1f;

    [SerializeField]
    private float floatSpeed = 1.8f;

    [SerializeField]
    private Vector3 randomOffsetRange =
        new Vector3(0.35f, 0.15f, 0f);

    private Color startColor =
        new Color(1f, 0.32f, 0.05f, 1f);

    private float timer;

    private void Awake()
    {
        if (text == null)
        {
            text = GetComponent<TMP_Text>();
        }

        if (text == null)
        {
            text = gameObject.AddComponent<TextMeshPro>();
        }

        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = 6f;
        text.color = startColor;

        MeshRenderer meshRenderer =
            text.GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            meshRenderer.sortingLayerName = "GAME OBJECT";
            meshRenderer.sortingOrder = 500;
        }

        transform.position +=
            new Vector3(
                Random.Range(-randomOffsetRange.x, randomOffsetRange.x),
                Random.Range(0f, randomOffsetRange.y),
                randomOffsetRange.z);
    }

    public void Show(
        int damage)
    {
        text.text =
            damage.ToString();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        transform.position +=
            Vector3.up *
            floatSpeed *
            Time.deltaTime;

        float t =
            Mathf.Clamp01(timer / lifetime);

        Color color =
            startColor;

        color.a =
            1f - t;

        text.color =
            color;

        transform.localScale =
            Vector3.one *
            Mathf.Lerp(1.1f, 0.85f, t);

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
