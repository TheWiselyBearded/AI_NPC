# AI NPC Framework for Unity

## Project Overview

The AI NPC Framework for Unity is a powerful, simple tool allowing  developers to effortlessly integrate AI NPC characters into their games. These NPCs can engage in dynamic, real-time conversations with players, demonstrating the ability to "listen," "think," and "speak" with response times of less than 10 seconds. Leveraging cutting-edge technologies, including the Meta Voice Dictation SDK, ChatGPT API, and Eleven Labs API, this framework provides a seamless and immersive experience for players.

ChatGPT API and Eleven Labs API provided by RageAgainstThePixel. Please check out and support their work here.

## Scenes

1. **NPC Demo**: A template scene allowing the user to speak with a NPC character.
2. **Multi NPC Demo**: A template scene allowing the user to observe multiple NPCs talking with each other.

## Roadmap

Infrequently, I will update the repository along with this roadmap. Please feel free to contirbute/make suggestions!
Here are some key milestones planned:
- **UPM Support**: Update repository/framework so that users can easily install and manage using Unity Package Manager (UPM).

- **Bug Fixes**: There are some bugs I've found during testing, such as intermittently freezing up when interfacing with Eleven Labs API. 

- **Improved Audio-Visual Synchronization**: Fine-tuning audio clips and animations to ensure smooth synchronization with AI states, possibly estimating API latency..

- **Voice Input Improvements**: Implement the ability to error check for complete sentences. Add more modular means of checking for keywords within user inputs.

- **Multi-NPC Dialogue**: Enabling NPCs to engage in conversations and facilitate user participation.

- **ChatGPT Vision Integration**: Incorporating upcoming ChatGPT vision capabilities from OpenAI for improved visual environment understanding.

- **Multiplayer Support**: Allowing multiple users to interact with NPC characters in a multiplayer setting.


## Setup

Setting up the AI NPC Framework in your Unity project is straightforward. Follow these steps to get started:

#### Running Samples

1. **Clone the Repository**: Clone this repository to your local machine using `git clone`.

2. **API Key Configuration**: Obtain API keys for the ChatGPT API and Eleven Labs API (provided by RageAgainstThePixel on GitHub) and configure them in your project.

3. **NPC Template Scene**: Run the template scenes, ensure that the components on the NPC prefab instance are not missing any properties in the inspector.

#### Create Custom Avatar
Presuming you have gone into Eleven Labs, created your voice clone, and signed into your account in Unity Eleven Labs dashboard.

1. Drag `PFB_AI_NPC` into scene from the Resources Folder. Expand to find the avatar instance.

2. Make your avatar model a child of the `PFB_AI_NPC` instance. In the avatar, make sure to assign the `CharacterAnimator` located in the Resources/Animations folder, or duplicate the animator and customize to your liking. 

3. Add an Audio Source to the character object, then proceed to add the Oculus Lip Sync components. Use existing prefabs for reference. 

4. Define the personality in the `ThinkerModule` component.

5. Select a voice in the `SpeakerModule` component. Adjust max response wait time as desired.

6. If adding/removing animations for the animator component of the character, specify the number of animations for each state.

7. Add necessary audio clips for introduction (if enabled), stalling (for thinking state), timeout (for failed conversation requests), and outro (if user says goodbye).

#### Other Settings and Notes
- If you wish to change the amount of time to listen for user input, look at the `AppDictationExperience` child object.
- Mouth movement is accomplished with Oculus Lip Sync, make sure to add to your avatar and correctly assign viseme blend shapes (Oculus Docs ref here).
- To speed up Eleven Labs API requests, the ChatGPT response is split up by sentence. You can disable this behavior in the `SpeakerModule` component, or edit the code to add your own desired regex operations to split.
- Animations are downloaded from Mixamo or created with NVIDIA Omniverse. Feel free to adjust `CharacterAnimator` in `Resources/Animations` to add additional animation clips or edit properties of animator (e.g., transition settings).
- Repository demonstrates using Avaturn Avatars and ReadyPlayerMe avatars. Any avatar with a facial rig will work.
- Avaturn models have a different weighting mechanism for blendshapes. Oculus Lip Sync presumes that each blend shape value ranges from 0-100. Avaturn works from 0-1, as such I made a separate component for Avaturn models.
- To make the characters blink, add the EyeAnimationHandler (from ReadyPlayerMe), appropriately assign the blend shape values for left and right eye closing.
- For multi-npc scene, each NPC must have its own unique Animator. Make sure the tags are assigned correctly and tracked in the `TriggerConvoStarter` child object. Additionally, make sure only one NPC character has its `IsTrigger` tick enabled on the attached collider.


## How it Works

The AI NPC Framework combines the power of the Meta Voice Dictation SDK, ChatGPT API, and Eleven Labs API to create a seamless conversation experience. Here's a high-level overview of how it works:

1. **Listening**: The NPC listens to the player's input using the Meta Voice Dictation SDK.

2. **Thinking**: The AI processes the player's input, generates responses, and evaluates the next course of action using the ChatGPT API.

3. **Speaking**: The NPC animates and speaks its response using the Eleven Labs API, providing a lifelike interaction with the player.

## NPC Controller

The `NPCController` is the central component responsible for managing the AI NPC's behavior and interactions. Here's how it works:

- It initializes and manages several modules, including the `ListenerModule`, `ThinkerModule`, `SpeakerModule`, and `MotionControllerModule`.

- The `Awake()` method sets up event listeners for handling user input and coordinating animations during conversations.

- The `IntroduceSelf()` function triggers the NPC's introduction if enabled, playing the introduction audio clip and setting the appropriate animation.

- When a new conversation begins, it calls `BeginListening()` to start listening for user input using the Meta Voice Dictation SDK.

- It also handles the cancellation of speaker requests by playing a timeout audio clip and resuming listening.

- User input is processed through the `HandleUserInput()` function, which checks for incomplete input and initiates the response generation process using the ThinkerModule.

## NPC Convo Manager

The `NPCConvoManager` is the central component responsible for managing dialogue between 2+ NPC characters. For now, the user cannot contribute to the conversation, only listen/observe. 

## Listener Module

The `ListenerModule` is responsible for listening to user input using the Meta Voice Dictation SDK. Here's how it works:

- It activates and deactivates the voice dictation service as needed, ensuring that the NPC can listen to the user's voice.

- It listens for full transcriptions of user input and invokes the `OnUserInputReceived` event when a complete transcription is received.

- The `ToggleDictation()` function enables or disables voice dictation based on the provided state.

## Thinker Module

The `ThinkerModule` manages the interaction with the ChatGPT API for generating responses. Here's how it works:

- It handles user input and initiates response generation using the ChatGPT API when the `GenerateResponse()` function is called.

- Depending on the selected personality, it sets up the chat conversation with appropriate prompts to guide the AI's responses.

- The module submits user input to the ChatGPT API and processes the AI's responses.

- It invokes the `OnChatGPTInputReceived` event with the AI's response for further processing.

## Speaker Module

The `SpeakerModule` handles the NPC's speech and audio playback. Here's how it works:

- It manages the selection of the voice for speech synthesis and initializes the Eleven Labs client.

- The module can split input text into sentences and request audio for each sentence separately or request audio for the full input text.

- It plays audio clips of NPC responses, triggering animations as needed to synchronize with speech.

- The `OnRequestCanceled` event is invoked when a request is canceled due to a timeout, and the `OnBeginSpeaking` event is invoked when speech begins.

## AudioManager

The `AudioManager` component controls audio playback during NPC interactions:

- Manages audio using an `AudioSource`, enabling voice responses and sound effects.
- Maintains a sequential queue of audio clips for a natural conversation flow.
- Provides specialized audio clips for introductions, timeouts, and stalls.
- Ensures synchronized audio and animations, triggering events upon completion (`OnTalkingComplete()`).

## Motion Controller Module

The `MotionControllerModule` handles animation and motion control for the NPC. Here's how it works:

- It controls NPC animations during different stages of conversation, such as listening, thinking, and speaking.

- The module randomizes animations to provide variety and realism to the NPC's expressions.

- It sets up animations for introduction and transitions between conversation stages.

These components and modules work together to create a lifelike and engaging AI NPC experience in your Unity project.

## License
<!-- Released under the [MIT license](LICENSE). -->

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

The MIT License (MIT)

Copyright © 2023 Alireza Bahremand

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.


<!-- ### License

Copyright © 2022, [Alireza Bahremand](https://github.com/TheWiselyBearded).
Released under the [MIT license](LICENSE). -->

