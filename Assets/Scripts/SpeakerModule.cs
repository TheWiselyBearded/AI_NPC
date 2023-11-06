using ElevenLabs;
using ElevenLabs.Voices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using Debug = UnityEngine.Debug;

[System.Serializable]
public class SpeakerModule : MonoBehaviour
{
    public event Action OnRequestCanceled;
    public event Action OnBeginSpeaking;

    public AudioManager AudioManager;
    public bool SplitInputTextToggle;
    [Tooltip("Acceptable time to wait for Eleven Labs to return voice clip. If exceeded, NPC will ask user to repeat.")] 
    public int maxResponseWaitTime = 30;

    // Voice selection
    public ElevenLabsClient client;
    public Voice voice;
    private List<string> audioOutputPaths = new List<string>();
    private ElevenLabs.Models.Model m = new ElevenLabs.Models.Model("eleven_multilingual_v2");
    
    // Start is called before the first frame update
    async void Start()
    {
        Init();
    }

    public void Init() {
        client = new ElevenLabsClient(ElevenLabsAuthentication.Default.LoadFromEnvironment());
        //_ = GetAllVoices();
    }

    protected async Task GetAllVoices() {
        var allVoices = await client.VoicesEndpoint.GetAllVoicesAsync();

        foreach (var voice in allVoices) {
            Debug.Log($"{voice.Id} | {voice.Name} | similarity boost: {voice.Settings?.SimilarityBoost} | stability: {voice.Settings?.Stability} | style: {voice.Settings?.Style}");
        }
    }

    public void SubmitVoiceRequest(string inputText) {
        if (SplitInputTextToggle) RequestAudioSplitBurst(inputText);
        else RequestAudioFull(inputText);
    }

    async Task RequestAudioFull(string result) {
        //var voice = (await client.VoicesEndpoint.GetVoiceAsync(selectedID)); // (await client.VoicesEndpoint.GetAllVoicesAsync()).FirstOrDefault();
        var _vs = await client.VoicesEndpoint.GetVoiceSettingsAsync(voice.Id);
        var (clipPath, audioClip) = await client.TextToSpeechEndpoint.TextToSpeechAsync(result, voice, _vs);
        AudioManager.AddAudioClip(audioClip);
        OnBeginSpeaking?.Invoke();
    }

    
    async Task RequestAudioSplitBurst(string result) {
        var stopwatch = new Stopwatch();
        var sentences = SplitInputText(result).ToList();
        var _vs = await client.VoicesEndpoint.GetVoiceSettingsAsync(voice.Id);
        var tasks = new List<Task<System.Tuple<string, AudioClip>>>();
        foreach (string sentence in sentences) {
            stopwatch.Start();
            var task = client.TextToSpeechEndpoint.TextToSpeechAsync(sentence, voice, _vs, m)
                .ContinueWith(t => {
                    stopwatch.Stop();
                    TimeSpan elapsedTime = stopwatch.Elapsed;
                    UnityEngine.Debug.Log($"Time taken for sentence: {elapsedTime.TotalMilliseconds} ms");
                    stopwatch.Reset();

                    if (t.IsFaulted || t.IsCanceled) {
                        return null;
                    }

                    return t.Result;
                });
            tasks.Add(task);
        }
        var timeout = Task.Delay(TimeSpan.FromSeconds(maxResponseWaitTime));
        var completedTask = await Task.WhenAny(Task.WhenAll(tasks), timeout);
        if (completedTask == timeout) {
            Debug.Log("Operation timed out");
            OnRequestCanceled?.Invoke();
            return;
        }

        var results = await Task.WhenAll(tasks);

        foreach (var (clipPath, audioClip) in results) {
            audioOutputPaths.Add(clipPath);
            AudioManager.AddAudioClip(audioClip);
            //Debug.Log($"CLIP {clipPath} length {audioClip.length}");
        }
        OnBeginSpeaking?.Invoke();
    }




    // Split the input text into sentences using the pattern
    public string[] SplitInputText(string inputText) {
        string sentencePattern = @"(?<=[.!?])\s+";
        string[] sentences = Regex.Split(inputText, sentencePattern);
        for (int i = 0; i < sentences.Length; i++) {
            string sentence = sentences[i].Trim(); // Trim to remove leading/trailing spaces
        }
        return sentences;
    }

}
