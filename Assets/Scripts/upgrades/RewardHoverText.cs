using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class RewardHoverText : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private Vector3 screenOffset = new Vector3(0f, 140f, 0f);

    [Header("Layout")]
    [SerializeField] private RectTransform panelRoot;

    [SerializeField] private Image iconImage;

    [SerializeField] private TMP_Text titleText;

    [SerializeField] private TMP_Text descriptionText;

    [Header("Optional")]
    [SerializeField] private Camera targetCamera;

    private Transform target;

    public void Bind(
        Transform followTarget,
        Vector3 worldOffset)
    {
        target = followTarget;
        targetCamera = Camera.main;
        screenOffset =
            new Vector3(
                worldOffset.x * 100f,
                worldOffset.y * 100f,
                worldOffset.z);

        if (panelRoot == null)
        {
            panelRoot = GetComponent<RectTransform>();
        }

        if (panelRoot != null)
        {
            panelRoot.localScale = Vector3.one;
        }
    }

    public void SetText(
        string title,
        string description,
        Sprite icon)
    {
        if (titleText != null)
        {
            titleText.text = title;
        }

        if (descriptionText != null)
        {
            descriptionText.text = description;
        }

        if (iconImage != null)
        {
            iconImage.sprite = icon;
            iconImage.overrideSprite = icon;
            iconImage.enabled = icon != null;
            iconImage.preserveAspect = true;

            if (icon != null)
            {
                iconImage.SetNativeSize();
            }
        }
    }

    private void Awake()
    {
        AutoWire();
    }

    private void OnValidate()
    {
        AutoWire();
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        if (targetCamera == null)
        {
            targetCamera = Camera.main;

            if (targetCamera == null)
            {
                return;
            }
        }

        Vector3 screenPosition =
            targetCamera.WorldToScreenPoint(target.position);

        if (screenPosition.z < 0f)
        {
            if (panelRoot != null)
            {
                panelRoot.gameObject.SetActive(false);
            }

            return;
        }

        if (panelRoot != null)
        {
            panelRoot.gameObject.SetActive(true);
            panelRoot.position = screenPosition + screenOffset;
        }
    }

    private void AutoWire()
    {
        if (panelRoot == null)
        {
            panelRoot = GetComponent<RectTransform>();
        }

        if (titleText == null)
        {
            titleText = FindText("Title");
        }

        if (descriptionText == null)
        {
            descriptionText = FindText("Description");
        }

        if (iconImage == null)
        {
            iconImage = FindImage("Icon");
        }
    }

    private TMP_Text FindText(
        string childName)
    {
        Transform child =
            transform.Find(childName);

        if (child == null)
        {
            return null;
        }

        return child.GetComponent<TMP_Text>();
    }

    private Image FindImage(
        string childName)
    {
        Transform child =
            transform.Find(childName);

        if (child == null)
        {
            return null;
        }

        return child.GetComponent<Image>();
    }
}
