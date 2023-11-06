using OpenAI.Chat;
using OpenAI;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Unity.Collections.LowLevel.Unsafe;
using System;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using OpenAI.Models;

[System.Serializable]
public class ThinkerModule : MonoBehaviour
{
    public event Action<string> OnChatGPTInputReceived;

    public PersonalityType Personality;
    public bool preprompt;

    // open ai api request variables
    private OpenAIClient openAI;
    private readonly List<Message> chatMessages = new List<Message>();
    private CancellationTokenSource lifetimeCancellationTokenSource;

    private static bool isChatPending;

    private void Awake() {
        Init();
    }
    private void OnDestroy() {
        this.Dipose();
    }

    
    private void Init() {
        lifetimeCancellationTokenSource = new CancellationTokenSource();
        openAI = new OpenAIClient();

        if (Personality == null) chatMessages.Add(new Message(Role.System, "You are a helpful assistant."));
        else chatMessages.Add(new Message(Role.System, $"Pretend you are {Personality.Name}. Please respond to all proceeding messages as this character in tune with this personality description {Personality.Description}. "));
    }

    /// <summary>
    /// entry point from other script to submit api request
    /// </summary>
    /// <param name="input"></param>
    public void GenerateResponse(string input) => SubmitChat(input);
    

    public string PrepromptQueryText(string text) {
        string complete = String.Empty;
        string preprompt = "Briefly and uniquely, as shortly as possible, please use no more than 4 sentences to respond to the following statement/question: ";
        complete = preprompt + text;
        return complete;
    }    


    public async void SubmitChat(string userInput) {
        if (isChatPending || string.IsNullOrWhiteSpace(userInput)) { return; }
        isChatPending = true;

        if (preprompt) userInput = PrepromptQueryText(userInput);
        var userMessage = new Message(Role.User, userInput);
        chatMessages.Add(userMessage);

        try {
            var chatRequest = new ChatRequest(chatMessages, Model.GPT3_5_Turbo);
            var result = await openAI.ChatEndpoint.GetCompletionAsync(chatRequest);
            var response = result.ToString();
            
            Debug.Log(response);
            OnChatGPTInputReceived?.Invoke(response);
        } catch (Exception e) {
            Debug.LogError(e);
        } finally {
            //if (lifetimeCancellationTokenSource != null) {}
            isChatPending = false;
        }
    }

    private void Dipose() {
        lifetimeCancellationTokenSource.Cancel();
        lifetimeCancellationTokenSource.Dispose();
        lifetimeCancellationTokenSource = null;
    }
}


[System.Serializable]
public class PersonalityType {
    public string Name;
    public string Description;

    public PersonalityType(string name, string description) {
        Name = name;
        Description = description;
    }
}