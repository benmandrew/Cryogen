using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EffectsController : MonoBehaviour
{
    public static EffectsController instance;

    public Volume vignetteVolume;
    public int vignetteIndex;
    public Volume colourVolume;
    public int colourIndex;

    internal Vignette vignetteEffect;
    internal ColorAdjustments colourEffect;
    
    // Start is called before the first frame update
    void Awake()
    {
        vignetteEffect = (Vignette)vignetteVolume.profile.components[vignetteIndex];
        colourEffect = (ColorAdjustments)colourVolume.profile.components[colourIndex];
        
        
        instance = this;
    }
}
