using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCConvoManager : MonoBehaviour
{
    public NPCController starterNPC;
    private string starterResponse;
    public NPCController receiverNPC;
    private string receiverResponse;

    private void Awake() {        
        FormatReceiver();
    }

    protected void ToggleUserInput(bool status) {
        starterNPC.listenerModule.ToggleDictation(status);
        receiverNPC.listenerModule.ToggleDictation(status);
    }

    private void Update() {
        ToggleUserInput(false); // for now, no listening from user
    }

    // Start is called before the first frame update
    void Start()
    {
        starterResponse = $"Hello, my name is {starterNPC.thinkerModule.Personality.Name}. My bio is as follows {starterNPC.thinkerModule.Personality.Description}. What would you like to discuss? Ask a question or share a thought.";
        
        starterNPC.thinkerModule.OnChatGPTInputReceived += SaveStarterResponse;
        starterNPC.speakerModule.AudioManager.OnTalkingComplete += StarterFinishedSpeaking;

        receiverNPC.thinkerModule.OnChatGPTInputReceived += SaveReceiverResponse;
        receiverNPC.speakerModule.AudioManager.OnTalkingComplete += ReceiverFinishedSpeaking;
    }

    public void FormatReceiver() {
        receiverNPC.gameObject.GetComponentInChildren<Collider>().isTrigger = false;
    }

    private void StarterFinishedSpeaking() {
        receiverNPC.motionControllerModule.SetAnimatorThinking();
        receiverNPC.speakerModule.AudioManager.PlayStallClip();
        receiverNPC.thinkerModule.GenerateResponse(starterResponse);
    }

    private void ReceiverFinishedSpeaking() {
        starterNPC.motionControllerModule.SetAnimatorThinking();
        starterNPC.speakerModule.AudioManager.PlayStallClip();
        starterNPC.thinkerModule.GenerateResponse(receiverResponse);
    }

    private void OnDestroy() {
        starterNPC.thinkerModule.OnChatGPTInputReceived -= SaveStarterResponse;
        starterNPC.speakerModule.AudioManager.OnTalkingComplete -= StarterFinishedSpeaking;
        receiverNPC.thinkerModule.OnChatGPTInputReceived -= SaveReceiverResponse;
        receiverNPC.speakerModule.AudioManager.OnTalkingComplete -= ReceiverFinishedSpeaking;
    }

    public void SaveStarterResponse(string response) {
        starterResponse = response;
    }

    public void SaveReceiverResponse(string response) {
        receiverResponse = response;
    }

}
