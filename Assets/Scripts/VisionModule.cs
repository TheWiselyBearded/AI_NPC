using System;
using System.Collections;
using System.Collections.Generic;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Images;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Utilities.WebRequestRest;

public class VisionModule : MonoBehaviour
{
    public event Action<string> OnChatGPTVisionInputReceived;

    public Camera cameraToSample; // Assign the camera you want to sample from in the Unity Inspector
    public int textureWidth = 512; // Width of the output Texture2D
    public int textureHeight = 512; // Height of the output Texture2D

    public Texture2D sampledTexture;
    public RawImage debugImage;
    private RenderTexture eyesRT;

    public string ImageGeneratePrompt;
    private OpenAIClient openAI;
    private PersonalityType personalityType;

    public void SetClientID(OpenAIClient client) => openAI = client;
    public void SetPersonality(PersonalityType pt) => personalityType = pt;


    void Start()
    {
        // Create a new Texture2D to hold the sampled data
        sampledTexture = new Texture2D(textureWidth, textureHeight);
        if (debugImage != null) debugImage.gameObject.SetActive(false);
    }

    void Update()
    {
        // Check for the space bar key press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(SampleCameraTexture());
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            SubmitImageCreateRequest();
        }
    }


    IEnumerator SampleCameraTexture()
    {
        if (debugImage != null) debugImage.gameObject.SetActive(true);

        // Make sure the camera has a RenderTexture to render to
        eyesRT = new RenderTexture(textureWidth, textureHeight, 24);
        cameraToSample.targetTexture = eyesRT;

        // Activate the camera and render it
        cameraToSample.Render();

        // Yield to the end of the frame to ensure rendering is complete
        yield return new WaitForEndOfFrame();

        // Read the pixels from the RenderTexture into the Texture2D
        RenderTexture.active = eyesRT;
        sampledTexture.ReadPixels(new Rect(0, 0, textureWidth, textureHeight), 0, 0);
        sampledTexture.Apply();

        if (debugImage != null) debugImage.texture = sampledTexture;

        // Optionally, reset the camera's target texture
        cameraToSample.targetTexture = null;

        // You can now use 'sampledTexture' as needed
        SubmitVisionRequest(sampledTexture);
    }

    public async void SubmitVisionRequest(Texture2D texture)
    {        
        var messages = new List<Message>
        {
            new Message(Role.System, "You are a helpful assistant with the following personality:\t" + personalityType.Description),
            new Message(Role.User, new List<Content>
            {
                "What's in this image? Please use at most three sentences to describe teh scene.",
                texture
            })
        };
        var chatRequest = new ChatRequest(messages, model: "gpt-4-vision-preview", maxTokens: 200);
        var result = await openAI.ChatEndpoint.GetCompletionAsync(chatRequest);
        //Debug.Log($"{result.FirstChoice.Message.Role}: {result.FirstChoice} | Finish Reason: {result.FirstChoice.FinishDetails}");
        Debug.Log($"{result.FirstChoice}");
        OnChatGPTVisionInputReceived?.Invoke(result.FirstChoice.ToString());
    }

    public void ExternalSetImageGeneratePrompt(string userInput) => ImageGeneratePrompt = userInput;
    public void ExternalSubmitImageGenerateRequest() => SubmitImageCreateRequest(ImageGeneratePrompt);
    public void ExternalSubmitImageCaptureRequest() => StartCoroutine(SampleCameraTexture());


    public async void SubmitImageCreateRequest(string imagePromptReq = "A advanced looking space station full of ai robots, artists, and humans")
    {
        if (debugImage != null) debugImage.gameObject.SetActive(true);
        var request = new ImageGenerationRequest(imagePromptReq, OpenAI.Models.Model.DallE_3);
        var imageResults = await openAI.ImagesEndPoint.GenerateImageAsync(request);

        foreach (var imageResult in imageResults)
        {
            Debug.Log(imageResult.CachedPath);
            // path == file://path/to/image.png
            Assert.IsNotNull(imageResult.Texture);
            sampledTexture = imageResult.Texture;
            SubmitVisionRequest(sampledTexture);
            if (debugImage != null) debugImage.texture = sampledTexture;
            // texture == The preloaded Texture2D
        }
    }

}
