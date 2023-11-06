using UnityEngine;
using System.Collections;

public class EyeAnimationHandler : MonoBehaviour {
    [SerializeField, Range(0, 1)] private float blinkSpeed = 0.1f;
    private WaitForSeconds blinkDelay;

    private const int VerticalMargin = 15;
    private const int HorizontalMargin = 5;

    public SkinnedMeshRenderer headMesh;
    private const string EyeBlinkLeftBlendshapeName = "eyeBlinkLeft1";
    private const string EyeBlinkRightBlendshapeName = "eyeBlinkRight1";
    public int eyeBlinkLeftBlendshapeIndex = -1;
    public int eyeBlinkRightBlendshapeIndex = -1;

    public Transform leftEyeBone;
    private const string HalfbodyLeftEyeBoneName = "Armature/spine/spine.001/spine.002/spine.003/spine.004/spine.005/spine.006/face/Eyes.001/Left_Eye.001";
    private const string FullbodyLeftEyeBoneName = "Armature/spine/spine.001/spine.002/spine.003/spine.004/spine.005/spine.006/face/Eyes.001/Left_Eye.001";

    public Transform rightEyeBone;
    private const string HalfbodyRightEyeBoneName = "Armature/spine/spine.001/spine.002/spine.003/spine.004/spine.005/spine.006/face/Eyes.001/Right_Eye.001";
    private const string FullbodyRightEyeBoneName = "Armature/spine/spine.001/spine.002/spine.003/spine.004/spine.005/spine.006/face/Eyes.001/Right_Eye.001";
    private const string ArmatureHipsLeftUpLegBoneName = "Armature/spine/thigh.L";
    public float EyeBlinkValue = 70f;

    private bool isFullbody;
    private bool hasEyeBlendshapes;

    private void Start() {
        if (headMesh == null) headMesh = gameObject.GetComponent<SkinnedMeshRenderer>();// (MeshType.HeadMesh);

        if (eyeBlinkLeftBlendshapeIndex == -1) eyeBlinkLeftBlendshapeIndex = headMesh.sharedMesh.GetBlendShapeIndex(EyeBlinkLeftBlendshapeName);
        if (eyeBlinkRightBlendshapeIndex == -1) eyeBlinkRightBlendshapeIndex = headMesh.sharedMesh.GetBlendShapeIndex(EyeBlinkRightBlendshapeName);
        blinkDelay = new WaitForSeconds(blinkSpeed);
        hasEyeBlendshapes = (eyeBlinkLeftBlendshapeIndex > -1 && eyeBlinkRightBlendshapeIndex > -1);

        if (leftEyeBone == null) {
            isFullbody = transform.Find(ArmatureHipsLeftUpLegBoneName);
            leftEyeBone = transform.Find(isFullbody ? FullbodyLeftEyeBoneName : HalfbodyLeftEyeBoneName);
            rightEyeBone = transform.Find(isFullbody ? FullbodyRightEyeBoneName : HalfbodyRightEyeBoneName);
        }


        InvokeRepeating(nameof(AnimateEyes), 1, 3);
    }

    private void AnimateEyes() {
        //RotateEyes();

        if (hasEyeBlendshapes) {
            StartCoroutine(BlinkEyes());
            //BlinkEyes().Run();
        }
    }

    private void RotateEyes() {
        float vertical = Random.Range(-VerticalMargin, VerticalMargin);
        float horizontal = Random.Range(-HorizontalMargin, HorizontalMargin);

        Quaternion rotation = isFullbody ?
            Quaternion.Euler(horizontal, vertical, 0) :
            Quaternion.Euler(horizontal - 90, 0, vertical + 180);

        leftEyeBone.localRotation = rotation;
        rightEyeBone.localRotation = rotation;
    }

    private IEnumerator BlinkEyes() {
        headMesh.SetBlendShapeWeight(eyeBlinkLeftBlendshapeIndex, EyeBlinkValue);
        headMesh.SetBlendShapeWeight(eyeBlinkRightBlendshapeIndex, EyeBlinkValue);

        yield return blinkDelay;

        headMesh.SetBlendShapeWeight(eyeBlinkLeftBlendshapeIndex, 0);
        headMesh.SetBlendShapeWeight(eyeBlinkRightBlendshapeIndex, 0);
    }
}