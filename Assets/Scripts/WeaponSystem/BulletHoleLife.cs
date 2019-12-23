using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BulletHoleLife : MonoBehaviour
{
    public float destoryTimer = 10.0f;
    
    public AudioClip[] impactSounds;
    private AudioSource _audioSource;
    
    
    public Transform bulletHoleSprite;


    public float bulletHoleMinSize = 0.01f;
    public float bulletHoleMaxSize = 0.025f;
    float _randomSize;

    void Awake () {
        _randomSize = (Random.Range 
                (bulletHoleMinSize, bulletHoleMaxSize));
        bulletHoleSprite.transform.localScale = 
                new Vector3 (_randomSize, _randomSize, _randomSize);
    }

    void Start () {
        StartCoroutine (DespawnTimer ());
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = impactSounds
            [Random.Range(0, impactSounds.Length)];
        _audioSource.Play();
    }
	
    IEnumerator DespawnTimer() {
        yield return new WaitForSeconds (destoryTimer);
        Destroy (gameObject);
    }
}
