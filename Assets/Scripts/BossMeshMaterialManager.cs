using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMeshMaterialManager : MonoBehaviour
{
    private SkinnedMeshRenderer renderer;
    Material rendererMat;
    Color originalColor;
    [SerializeField] string colorPropertyName = "_EmissionColor";
    [SerializeField] bool testMeshChangeRenderer;
    [SerializeField] Color colorTest;
    private void Awake()
    {
        renderer = GetComponent<SkinnedMeshRenderer>();
        rendererMat = new Material(renderer.material);
        rendererMat.EnableKeyword(colorPropertyName);
        renderer.material = rendererMat;
        originalColor = rendererMat.GetColor(colorPropertyName);
    }

    private void Start()
    {
        if (testMeshChangeRenderer) LerpEmissionToColor(colorTest);
    }

    public void LerpEmissionToColor(Color color)
    {
        StartCoroutine(ChangeColor(color));
    }

    public void LerpToOriginalColor()
    {
        StartCoroutine(ChangeColor(originalColor));
    }

    IEnumerator ChangeColor(Color toColor)
    {
        float lerpAMT = 0.0f;
        float startTime = Time.time;
        float duration = 2.5f;
        Color startColor = GetMatColor();
        while(lerpAMT < 1.0f)
        {
            lerpAMT = (Time.time - startTime) / duration;
            SetMatColor(Color.Lerp(startColor, toColor, lerpAMT));
            yield return null;
        }
        SetMatColor(toColor);
        yield return null;
    }

    private Color GetMatColor()
    {
        return rendererMat.GetColor(colorPropertyName);
    }

    private void SetMatColor(Color color)
    {
        rendererMat.SetColor(colorPropertyName, color);
    }
}
