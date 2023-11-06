using Meta.Voice.Samples.Dictation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardDictationTrigger : MonoBehaviour
{
    public DictationActivation dictation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) dictation.ToggleActivation();
    }
}
