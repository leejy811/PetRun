using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISetting : MonoBehaviour
{
    void Awake()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        if (Screen.width / 16 == Screen.height / 9)
            return;

        int Ratio = Mathf.Min(Screen.width / 16, Screen.height / 9);

        if (Screen.width > Ratio * 16)
        {
            if (rectTransform.pivot.x == 0)
                rectTransform.anchoredPosition += new Vector2((Screen.width - Ratio * 16) / 2, 0);
            else if (rectTransform.pivot.x == 1)
                rectTransform.anchoredPosition += new Vector2((Screen.width - Ratio * 16) / -2, 0);
        }

        if (Screen.height > Ratio * 9)
        {
            if (rectTransform.pivot.y == 0)
                rectTransform.anchoredPosition += new Vector2(0, (Screen.height - Ratio * 9) / 2);
            else if (rectTransform.pivot.y == 1)
                rectTransform.anchoredPosition += new Vector2(0, (Screen.height - Ratio * 9) / -2);
        }

    }
}
