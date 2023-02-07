using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISetting : MonoBehaviour
{
    void Awake()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        
        if (Screen.width > 1920)
        {
            if (rectTransform.pivot.x == 0)
                rectTransform.anchoredPosition += new Vector2((Screen.width - 1920) / 2, 0);
            else if (rectTransform.pivot.x == 1)
                rectTransform.anchoredPosition += new Vector2((Screen.width - 1920) / -2, 0);
        }

        if (Screen.height > 1080)
        {
            if (rectTransform.pivot.y == 0)
                rectTransform.anchoredPosition += new Vector2(0, (Screen.height - 1080) / 2);
            else if (rectTransform.pivot.y == 1)
                rectTransform.anchoredPosition += new Vector2(0, (Screen.height - 1080) / -2);
        }

    }
}
