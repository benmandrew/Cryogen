using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedCryopodFade : MonoBehaviour
{
    public Renderer casingRenderer;
    public int casingGlowIndex;
    public Renderer heartRenderer;
    public int heartGlowIndex;
    
    public Color finalColour;

    public float fadeTime = 2f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float elapsed = 0f;

        Material casingMat = casingRenderer.materials[casingGlowIndex];
        Material heartMat = heartRenderer.materials[heartGlowIndex];
        
        casingMat.EnableKeyword("_EMISSION");
        heartMat.EnableKeyword("_EMISSION");
        ;
        
        Color startCasing = casingMat.GetColor("_EmissionColor");
        Color startHeart = heartMat.GetColor("_EmissionColor");


        while (elapsed < fadeTime)
        {
            casingMat.SetColor("_EmissionColor", Color.Lerp(startCasing, finalColour, elapsed / fadeTime));
            heartMat.SetColor("_EmissionColor", Color.Lerp(startHeart, finalColour, elapsed / fadeTime));

            elapsed += Time.deltaTime;

            yield return null;
        }

        casingMat.SetColor("_EmissionColor", finalColour);
        heartMat.SetColor("_EmissionColor", finalColour);

        yield return null;
    }
}
