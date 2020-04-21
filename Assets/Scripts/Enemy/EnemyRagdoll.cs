using System.Collections;
using System.Collections.Generic;
//using Packages.Rider.Editor;
using UnityEngine;

public class EnemyRagdoll : MonoBehaviour
{
    public float ragdollTime = 1f;
    public float despawnTime = 10f;
    private const float fadeTime = 2f;

    public Material[] fadeVariants;
    
    private Animator _animator;
    private List<Rigidbody> _bodies;

    public SkinnedMeshRenderer ragdollRenderer;
    public AudioClip deathClip;
    private AudioSource source;
    
    // Start is called before the first frame update
    void Start() {
        _bodies = new List<Rigidbody>();
        foreach (Rigidbody r in GetComponentsInChildren<Rigidbody>())
        {
            _bodies.Add(r);
            r.isKinematic = true;
        }
        _animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        source.PlayOneShot(deathClip);
        StartCoroutine(SwapRagdoll());
    }

    IEnumerator SwapRagdoll() {
        yield return new WaitForSeconds(ragdollTime);

        _animator.enabled = false;
        
        foreach (Rigidbody r in _bodies)
        {
            r.isKinematic = false;
        }
        
        yield return new WaitForSeconds(despawnTime);

        
        Color[] startColours = new Color[fadeVariants.Length];
        Color[] endColours = new Color[fadeVariants.Length];

        for (int i = 0; i < fadeVariants.Length; i++)
        {

            startColours[i] = fadeVariants[i].color;
            endColours[i] = fadeVariants[i].color;
            endColours[i].a = 0;
        }

        ragdollRenderer.materials = fadeVariants;
        
        float elapsed = 0f;
        
        while (elapsed < fadeTime)
        {
            for (int i = 0; i < fadeVariants.Length; i++)
            {
                ragdollRenderer.materials[i].color = Color.Lerp(startColours[i], endColours[i], elapsed / fadeTime);
            }
            elapsed += Time.deltaTime;

            yield return null;
        }
        
        Destroy(gameObject);
    }
}
