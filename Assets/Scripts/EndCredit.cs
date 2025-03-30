using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndCredit : MonoBehaviour
{
    public RectTransform textRect;

    public float speed = 50f;
    public float endY = 1000f;

    void Start()
    {
        StartCoroutine(ScrollCredits());
    }

    IEnumerator ScrollCredits()
    {
        while (textRect.anchoredPosition.y < endY)
        {
            textRect.anchoredPosition += new Vector2(0, speed * Time.deltaTime);
            yield return null;
        }
    }
}
