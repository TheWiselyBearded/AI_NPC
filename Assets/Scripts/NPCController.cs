using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Tooltip("Should avatar introduce self when others comes in proxmity?")]
    public bool introduceSelf;
    // References to modules
    [SerializeField] public ListenerModule listenerModule;
    [SerializeField] public ThinkerModule thinkerModule;
    [SerializeField] public SpeakerModule speakerModule;
    [SerializeField] public MotionControllerModule motionControllerModule;

    // Initialize modules and other variables
    void Awake() {
        listenerModule = GetComponent<ListenerModule>();
        thinkerModule = GetComponent<ThinkerModule>();
        speakerModule = GetComponent<SpeakerModule>();
        motionControllerModule = GetComponent<MotionControllerModule>();

        // Set up event listeners for handling user input
        listenerModule.OnUserInputReceived += HandleUserInput;
        thinkerModule.OnChatGPTInputReceived += HandleChatResponse;
        speakerModule.OnRequestCanceled += OnSpeakerModuleCancel;
        speakerModule.AudioManager.OnTalkingComplete += BeginListening;

        listenerModule.OnUserInputReceived += (string _) => motionControllerModule.SetAnimatorThinking();
        speakerModule.OnBeginSpeaking += () => motionControllerModule.SetAnimatorSpeaking();
        speakerModule.AudioManager.OnTalkingComplete += () => motionControllerModule.SetAnimatorListening();
    }

    private void OnDestroy() {
        listenerModule.OnUserInputReceived -= HandleUserInput;
        thinkerModule.OnChatGPTInputReceived -= HandleChatResponse;
        speakerModule.OnRequestCanceled -= OnSpeakerModuleCancel;
        speakerModule.AudioManager.OnTalkingComplete -= BeginListening;
    }

    public void IntroduceSelf() {
        if (introduceSelf && speakerModule.AudioManager.introClip != null) {
            speakerModule.AudioManager.PlayIntroClip();
            motionControllerModule.SetAnimatorIntro();
        }
    }

    /// <summary>
    /// restart conversational pipeline for new entry
    /// </summary>
    private void BeginListening() => listenerModule.ToggleDictation(true);
    

    private void OnSpeakerModuleCancel() {
        if (speakerModule.AudioManager.timeoutClip != null) 
            speakerModule.AudioManager.PlayTimeoutClip();
        
        BeginListening();
    }

    /// <summary>
    /// Feed to eleven labs api
    /// </summary>
    /// <param name="obj"></param>
    private void HandleChatResponse(string chatResponse) => speakerModule.SubmitVoiceRequest(chatResponse);

    // Handle user input
    private void HandleUserInput(string userInput) {
        // Check for incomplete input (voice dictation error handling)
        if (IsInputIncomplete(userInput)) {
            // Trigger the appropriate response, e.g., request user to repeat
            RequestUserToRepeat();
            return;
        }
        if (IsKeyword(userInput)) {
            speakerModule.AudioManager.PlayOutroClip();
            return;
        }

        speakerModule.AudioManager.PlayStallClip();
        thinkerModule.GenerateResponse(userInput);  // pass to chat gpt
    }

    // Check for incomplete input (voice dictation error handling)
    // You can use punctuation or word count thresholds
    private bool IsInputIncomplete(string input) {
        return false;
    }

    /// simple demonstration of keyword parsing. 
    /// TODO: add submodule for searching against list of keywords and mapping to audio clips/
    private bool IsKeyword(string input) {
        if (input.Contains("goodbye")) return true;
        else return false;
    }


    // Request the user to repeat their question or thought
    // Play predefined audio clip and animation
    private void RequestUserToRepeat() {
        speakerModule.AudioManager.PlayTimeoutClip();
    }
}
