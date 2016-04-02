using UnityEngine;
using System.Collections;

public class DestroyAfterSoundEffect : MonoBehaviour {
    public AudioClip audioClip;
    private AudioSource _audioSource;

	void Awake() {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.PlayOneShot(audioClip);
        Destroy(gameObject, audioClip.length);
	}
}
