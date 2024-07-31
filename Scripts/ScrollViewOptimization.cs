using UnityEngine;
using UnityEngine.UI;

public class ScrollViewOptimization : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform[] elements;
    public float padding = 50f;

    private RectTransform viewport;

    private void Start()
    {
        viewport = scrollRect.viewport;
    }

    private void Update()
    {
        foreach (Transform child in elements)
            ToggleVisibility(child.gameObject);
    }

    private void ToggleVisibility(GameObject obj)
    {
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        if (rectTransform == null) return;

        // Получаем координаты углов объекта в мировых координатах
        Vector3[] worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);

        // Преобразуем мировые координаты в локальные координаты viewport
        for (int i = 0; i < worldCorners.Length; i++)
        {
            worldCorners[i] = viewport.InverseTransformPoint(worldCorners[i]);
        }

        // Создаем прямоугольник для viewport
        Rect viewportRect = new Rect(
            -viewport.rect.width * viewport.pivot.x,
            -viewport.rect.height * viewport.pivot.y,
            viewport.rect.width,
            viewport.rect.height
        );

        // Добавляем padding
        viewportRect.xMin -= padding;
        viewportRect.xMax += padding;
        viewportRect.yMin -= padding;
        viewportRect.yMax += padding;

        // Проверяем видимость углов объекта
        bool isVisible = false;
        foreach (Vector3 corner in worldCorners)
        {
            if (viewportRect.Contains(corner))
            {
                isVisible = true;
                break;
            }
        }

        obj.SetActive(isVisible);
        // Debug.Log(obj.name + " " + isVisible);
    }
}