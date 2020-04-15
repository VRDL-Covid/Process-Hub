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

    //public float movementAmount = 0.01f;
    public float movementMagnitude= 0.5f;
    public float movementMagnitudeStep = 0.05f;
    public float movementMagnitudeResetValue = 0;

    public float rotateRatePerMinuteMagnitude = 60f;
    public float rotateRatePerMinuteMagnitudeStep = 5f;
    public float rotateRatePerMinuteMagnitudeResetValue = 0;

    [HideInInspector]
    public enum MagnitudeAmount
    {
        FREEZE = 0,
        INCREASE = 1,
        DECREASE = -1,
        RESET
    }

    [HideInInspector]
    public enum RotationAmount
    {
        FREEZE = 0,
        INCREASE = 1,
        DECREASE = -1,
        RESET
    }
    [HideInInspector]

    public enum RotateDirection
    {
        STOP=0,
        LEFT = -1,
        RIGHT=1,
        FREEZE
    }
    [HideInInspector]
    public enum VerticalDirection
    {
        STOP=0,
        UP=1,
        DOWN=-1
    }
    [HideInInspector]
    public enum LateralDirection
    {
        STOP=0,
        LEFT=-1,
        RIGHT=1
    }

    [HideInInspector]
    public enum SagialDirection
    {
        STOP=0,
        FWD=1,
        BACK=-1
    }

    public RotationAmount rotationAmount = RotationAmount.FREEZE;
    public MagnitudeAmount magnitudeAmount = MagnitudeAmount.FREEZE;
    public RotateDirection rotationDirection = RotateDirection.FREEZE;
    public VerticalDirection verticalDirection = VerticalDirection.STOP;
    public LateralDirection lateralDirection = LateralDirection.STOP;
    public SagialDirection sagialDirection = SagialDirection.STOP;

    int rotAmt = (int)RotationAmount.FREEZE;
    int magAmt = (int)MagnitudeAmount.FREEZE;
    int rotDir = (int)RotateDirection.STOP;
    int verDir = (int)VerticalDirection.STOP;
    int latDir = (int)LateralDirection.STOP;
    int sagDir = (int)SagialDirection.STOP;

    Vector3 posOffset = new Vector3(0, 0, 0);

    public void Start()
    {
        recognizer = new KeywordRecognizer(keywords);
        recognizer.OnPhraseRecognized += OnPhraseRecognized;
        recognizer.Start();

        // take a copy...
        movementMagnitudeResetValue = movementMagnitude;
        rotateRatePerMinuteMagnitudeResetValue = rotateRatePerMinuteMagnitude;

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
        Vector3 positionOffset = new Vector3(latDir, verDir, sagDir) * movementMagnitude;

        targetTransform.position = Vector3.Lerp(targetTransform.position, targetTransform.position + positionOffset , 0.5f * Time.deltaTime);
        
        //targetTransform.position
        if (isFollowSet)
        {
            // Define a target position above and behind the target transform
            Vector3 targetPosition = CameraCache.Main.transform.TransformPoint(followOffset);

            // Smoothly move towards that target position
            transform.position = Vector3.SmoothDamp(targetTransform.position, targetPosition, ref velocity, smoothTime);
        }

        if (rotationDirection != RotateDirection.FREEZE && rotationDirection != RotateDirection.STOP)
        {
            targetTransform.Rotate(0, rotateRatePerMinuteMagnitude * rotDir * Time.deltaTime, 0);
        }
    }
    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
       
        switch (args.text.ToLower())
        {

            //RATE...

            case "increase rate":
                rotationAmount = RotationAmount.INCREASE;
                break;
            case "decrease rate":
                rotationAmount = RotationAmount.DECREASE;
                break;
            case "reset rate":
                rotationAmount = RotationAmount.RESET;
                break;
            case "larger":
                OnScale();
                break;
            case "rotate left":
                rotationDirection = RotateDirection.LEFT;
                break;
            case "rotate right":
                rotationDirection = RotateDirection.RIGHT;
                break;
            case "stop rotate":
                rotationDirection = RotateDirection.STOP;
                break;
            case "stop":
                ResetMovement();
                break;
            case "follow me":
                OnStartFollow();
                break;
            case "follow stop":
                ResetMovement();
                break;
            case "back":
                sagialDirection = SagialDirection.BACK;
                break;
            case "forward":
                sagialDirection = SagialDirection.FWD;
                break;
            case "higher":
                verticalDirection = VerticalDirection.UP;
                break;
            case "lower":
                verticalDirection = VerticalDirection.DOWN;
                break;
            case "left":
                lateralDirection = LateralDirection.LEFT;
                break;
            case "right":
                lateralDirection = LateralDirection.RIGHT;
                break;
            case "slower":
                magnitudeAmount = MagnitudeAmount.DECREASE;
                break;
            case "faster":
                magnitudeAmount = MagnitudeAmount.INCREASE;
                break;
        }

        // get the current directions/amounts etc...
        rotAmt = (int)rotationAmount;
        magAmt = (int)magnitudeAmount;
        rotDir = (int)rotationDirection;
        verDir = (int)verticalDirection;
        latDir = (int)lateralDirection;
        sagDir = (int)sagialDirection;

        if (magnitudeAmount == MagnitudeAmount.INCREASE || magnitudeAmount == MagnitudeAmount.DECREASE)
        {
            movementMagnitude += magAmt * movementMagnitudeStep;
            if (movementMagnitude < 0)
                movementMagnitude = 0;
            magnitudeAmount = MagnitudeAmount.FREEZE;
        }
        else if (magnitudeAmount == MagnitudeAmount.RESET)
        {
            movementMagnitude = movementMagnitudeResetValue;
            magnitudeAmount = MagnitudeAmount.FREEZE;
        }

        if (rotationAmount == RotationAmount.INCREASE || rotationAmount == RotationAmount.DECREASE)
        {
            rotateRatePerMinuteMagnitude += rotAmt * rotateRatePerMinuteMagnitudeStep;
            if (rotateRatePerMinuteMagnitude < 0)
                rotateRatePerMinuteMagnitude = 0;
            rotationAmount = RotationAmount.FREEZE;
        }
        else if (rotationAmount == RotationAmount.RESET)
        {
            rotateRatePerMinuteMagnitude = rotateRatePerMinuteMagnitudeResetValue;
            rotationAmount = RotationAmount.FREEZE;
        }

        // tell the user the word was recognised...
        speechTextBox.text = string.Format("{0}", args.text);

    }

    public void ResetMovement()
    {
        posOffset = Vector3.zero;
        rotationDirection = RotateDirection.STOP;
        verticalDirection = VerticalDirection.STOP;
        lateralDirection = LateralDirection.STOP;
        sagialDirection = SagialDirection.STOP;
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
