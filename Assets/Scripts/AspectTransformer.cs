using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.Windows.Speech;

public class AspectTransformer : MonoBehaviour//, IMixedRealitySpeechHandler
{
    [SerializeField]
    public string[] keywords;

    private KeywordRecognizer recognizer;

    [Tooltip("Scales object uniformly")]
    public Vector3 scale = new Vector3(0.1f, 0.1f, 0.1f);

    public Vector3 yRotation = new Vector3(0, 10, 0);

    public Vector3 followOffset = new Vector3(0, 0, 1.5f);

    [Tooltip("Speed and direction of object")]
    public float rotationsPerMinute = 10.0f;

    [Tooltip("Smoothing factor for following camera")]
    public float smoothTime = 0.3F;

    [Tooltip("Distance object is placed in front in z axis")]
    public float followDepth = 1.5F;

    [Tooltip("Set to cause transform to follow camera")]
    public bool isFollowSet = false;

    [Tooltip("Set to cause transform to rotate in direction (using sign of rotations per minute above)")]
    public bool isRotateYSet = false;

    [Tooltip("Object to transform (assumes this object if not set)")]
    public Transform targetTransform;

    [Tooltip("speech source")]
    public TMPro.TextMeshPro speechTextBox;

    private Vector3 velocity = Vector3.zero;

    public void Start()
    {
        recognizer = new KeywordRecognizer(keywords);
        recognizer.OnPhraseRecognized += OnPhraseRecognized;
        recognizer.Start();

        if (targetTransform == null)
            // assume self..
            targetTransform = gameObject.transform;
    }

    public void OnScale()
    {
        gameObject.transform.localScale += scale;
    }

    public void OnRotateleft()
    {
        gameObject.transform.Rotate(yRotation);
    }

    public void OnRotateRight()
    {
        gameObject.transform.Rotate(yRotation * -1);
    }

    public void OnStartFollow()
    {
        isFollowSet = true;
        Vector3 camPos = Camera.main.transform.position;
        followOffset = new Vector3(0, 0, followDepth);

    }

    public void OnStartRotateY()
    {
        isRotateYSet = true;
    }

    public void OnStopRotateY()
    {
        isRotateYSet = false;
    }

    public void OnStopFollow()
    {
        isFollowSet = false;
    }

    public void OnSnap()
    {
        isFollowSet = isRotateYSet = false;
    }

    void Update()
    {
        if (isFollowSet)
        {
            // Define a target position above and behind the target transform
            Vector3 targetPosition = CameraCache.Main.transform.TransformPoint(followOffset);

            // Smoothly move the camera towards that target position
            transform.position = Vector3.SmoothDamp(targetTransform.position, targetPosition, ref velocity, smoothTime);
        }

        if (isRotateYSet)
        {
            targetTransform.Rotate(0, 6.0f * rotationsPerMinute * Time.deltaTime, 0);
        }
    }
    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        switch (args.text.ToLower())
        {
            case "scale":
                OnScale();
                break;
            case "turn":
                OnStartRotateY();
                break;
            case "stop turn":
                OnStopRotateY();
                break;
            case "snap":
                OnSnap();
                break;
            case "follow":
                OnStartFollow();
                break;
            case "stop follow":
                OnStopFollow();
                break;
        }
        speechTextBox.text = string.Format("{0}", args.text);

    }

    public void DoCommand(string cmd)
    {
        // TBD...
    }

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        speechTextBox.text = string.Format("{0}", eventData.Command);
    }
}
