using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioManager : MonoBehaviour {

    public AudioSource audioSource;
    private Queue<AudioClip> audioClips;

    public AudioClip[] stallClips;
    [SerializeField] public AudioClip introClip, timeoutClip, outroClip; 

    protected bool conversing;

    public delegate void TalkingComplete();
    public event TalkingComplete OnTalkingComplete;

    private System.Random randomGenerator = new System.Random();

    private void Awake() {
        audioClips = new Queue<AudioClip>();
        if (audioSource == null) audioSource = GetComponentInChildren<AudioSource>();
    }

    // Play the next audio clip in the array
    private void PlayNextAudioClip() {
        if (audioClips.Count > 0) {
            audioSource.clip = audioClips.Dequeue();
            audioSource.Play();
        }
    }

    protected void Update() {
        // Check if there are more audio clips to play
        if (audioClips.Count > 0 && !audioSource.isPlaying) {
            conversing = true;
            PlayNextAudioClip();
        }
        if (conversing && audioClips.Count == 0 && !audioSource.isPlaying) {
            conversing = false;
            OnTalkingComplete?.Invoke();
        }
    }

    // Set the audio clips for responses
    public void AddAudioClip(AudioClip clip) => audioClips.Enqueue(clip);

    public void PlayTimeoutClip() {
        audioSource.clip = timeoutClip;
        audioSource.Play();
    }

    private void PlayClip(AudioClip clip) {
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void PlayStallClip() => PlayClip(stallClips[randomGenerator.Next(0, stallClips.Length)]);
    public void PlayOutroClip() => PlayClip(outroClip);
    public void PlayIntroClip() => AddAudioClip(introClip);
}
