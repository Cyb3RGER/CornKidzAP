using System.Collections;
using UnityEngine;

namespace CornKidzAP.Archipelago;

public static class UIUtils
{
    public static IEnumerator SlideRect(RectTransform rectTransform, Vector3 startPosition, Vector3 targetPosition, float duration)
    {
        var elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            var t = elapsed / duration;
            rectTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, Mathf.SmoothStep(0.0f, 1.0f, t));
            yield return null;
        }

        rectTransform.localPosition = targetPosition; // Ensure it ends exactly at target
    }
}