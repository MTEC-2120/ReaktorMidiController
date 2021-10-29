using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public PlayerController playerController;
    public GameObject theGround;
    public float followUpSpeedFactor = 1;

    [HideInInspector]
    public bool isStopMoving = false;

    // How long the camera shaking.
    public float shakeDuration = 0.1f;
    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.3f;
    public float decreaseFactor = 0.3f;

    private Transform playerTransform;
    private float speedGoUp;
    private float speedGoDown;
    private float followDownspeedFactor = 2;
    private float initialPlayerDistance;
    private float currentShakeDuration;
    private Vector3 originalShakePos;
    private bool isShaking = false;

    // Use this for initialization
    void Start()
    {
        playerTransform = playerController.transform;
        initialPlayerDistance = transform.position.y - playerTransform.transform.position.y;
    }
	
    // Update is called once per frame
    void Update()
    {
        if (isShaking)
            return;

        if (!playerController.hasStarted)
            return;

        if (isStopMoving)
            return;

        float distance = transform.position.y - playerTransform.transform.position.y;

        speedGoUp = (initialPlayerDistance - distance) * followUpSpeedFactor;
        speedGoDown = Mathf.Min(-3f, (initialPlayerDistance - distance) * followDownspeedFactor);

        if (distance < initialPlayerDistance - 1f && !playerController.hitObstacle && !playerController.playerFallDown)
        {
            // Going up
            transform.position += new Vector3(0, speedGoUp * Time.deltaTime, 0);
        }
        else if (distance > initialPlayerDistance)
        {
            // Going down
            transform.position += new Vector3(0, speedGoDown * Time.deltaTime, 0);
        }
        else if (playerController.hasHitGround)
        {
            // Keep going down until the character is reveal
            transform.position += new Vector3(0, speedGoDown * Time.deltaTime, 0);
        }

        float finalDistance = transform.position.y - playerTransform.transform.position.y;
        bool shouldStop = playerController.hasHitGround && GameManager.Instance.GameState == GameState.GameOver && (finalDistance < initialPlayerDistance - 1f);

        if (shouldStop && !isStopMoving)
        {
            isStopMoving = true;
        }
    }

    public void ShakeCamera()
    {
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        if (isShaking)
            yield break;

        isShaking = true;
        originalShakePos = transform.position;
        currentShakeDuration = shakeDuration;

        while (currentShakeDuration > 0)
        {
            transform.position = originalShakePos + Random.insideUnitSphere * shakeAmount;
            currentShakeDuration -= Time.deltaTime * decreaseFactor;
            yield return null;
        }

        transform.position = originalShakePos;

        isShaking = false;
    }
}
