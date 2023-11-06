using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConversationTrigger : MonoBehaviour {
    public NPCController controller;
    public string[] collisionTags;

    private GameObject lastObject;  // keep track of object until leaving collision bounds

    private void Awake() {
        if (controller == null) controller = GetComponent<NPCController>();
    }

    private bool CheckAgainstTags(string collisionObjectTag) {
        if (collisionTags.Contains(collisionObjectTag)) return true;
        else return false;
    }

    private void OnTriggerEnter(Collider other) {
        if (collisionTags.Length == 0 || !controller.introduceSelf) return;
        if (CheckAgainstTags(other.gameObject.tag) && lastObject == null && lastObject != other.gameObject) {
            Debug.Log("Triggered Intro");
            controller.IntroduceSelf();
            lastObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (collisionTags.Length == 0 || !controller.introduceSelf) return;
        if (other.gameObject == lastObject) lastObject = null;
    }
}
