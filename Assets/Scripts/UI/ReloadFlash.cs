using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadFlash : MonoBehaviour
{
    public Color[] shades;
    public float cycleTime = 0.5f;
    
    private Text _reloadText;
    private int _shadeIndex = 0;
    private float _lastUpdate;
    
    // Start is called before the first frame update
    void Start()
    {
        _reloadText = GetComponent<Text>();
        _lastUpdate = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        Color a = shades[_shadeIndex];
        Color b = shades[(_shadeIndex + 1) % shades.Length];

        float timepoint = (Time.time - _lastUpdate) / cycleTime;

        if (timepoint > 1.0f)
        {
            _lastUpdate = Time.time;
            _shadeIndex += 1;
            _shadeIndex %= shades.Length;
        }

        _reloadText.color = Color.Lerp(a, b, timepoint);
    }
}
