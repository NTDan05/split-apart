using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grow : MonoBehaviour
{
    [SerializeField] float growDuration = 0.35f;   // how fast they expand
    [SerializeField] float targetSize = 1f; // final size multiplier
    [SerializeField] float startSize = 0.3f;  // initial size multiplier
    private Vector3 targetScale;
    private Vector3 startScale;

    void Awake()
    {
        targetScale = Vector3.one * targetSize;
        startScale = Vector3.one * startSize;
    }

    void OnEnable()
    {
        // start tiny and grow back to normal
        transform.localScale = startScale;
        StartCoroutine(GrowToFullSize());
    }

    IEnumerator GrowToFullSize()
    {
        float elapsed = 0f;
        while (elapsed < growDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / growDuration;

            // Smooth grow (ease-out curve feels nice)
            transform.localScale = Vector3.Lerp(startScale, targetScale, Mathf.SmoothStep(0, 1, t));
            
            yield return null;
        }

        transform.localScale = targetScale; // snap to final
    }
}
