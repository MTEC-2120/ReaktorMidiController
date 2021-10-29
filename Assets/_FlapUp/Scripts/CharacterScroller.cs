using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SgLib;

public class CharacterScroller : MonoBehaviour
{
    [Header("Scroller config")]
    public float minScale = 1f;
    public float maxScale = 1.5f;
    public float characterSpace = 3f;
    public float moveForwardAmount = 2f;
    public float swipeThresholdX = 5f;
    public float swipeThresholdY = 30f;
    public float rotateSpeed = 30f;
    public float snapTime = 0.3f;
    public float resetRotateSpeed = 180f;
    [Range(0.1f, 1f)]
    public float scrollSpeedFactor = 0.25f;
    // adjust this tweak scrolling speed
    public Vector3 centerPoint;
    public Vector3 originalScale = Vector3.one;
    public Vector3 originalRotation = Vector3.zero;

    [Header("UI stuff")]
    public Text totalGold;
    public Text characterName;
    public Text characterPrice;
    public Button selectButon;
    public Button unlockButton;
    public Button lockButton;
    public Color lockColor = Color.black;

    List<GameObject> listCharacter = new List<GameObject>();
    GameObject currentCharacter;
    GameObject lastCurrentCharacter;
    IEnumerator rotateCoroutine;
    Vector3 startPos;
    Vector3 endPos;
    float startTime;
    float endTime;
    bool isCurrentCharacterRotating = false;
    bool hasMoved = false;

    // Use this for initialization
    void Start()
    {

        lockColor.a = 0;    // need this for later setting material colors to work

        int currentCharacterIndex = CharacterManager.Instance.CurrentCharacterIndex;
        currentCharacterIndex = Mathf.Clamp(currentCharacterIndex, 0, CharacterManager.Instance.characters.Length - 1); 

        for (int i = 0; i < CharacterManager.Instance.characters.Length; i++)
        {
            int deltaIndex = i - currentCharacterIndex;

            GameObject character = (GameObject)Instantiate(CharacterManager.Instance.characters[i], centerPoint, Quaternion.Euler(originalRotation.x, originalRotation.y, originalRotation.z));
            Character charData = character.GetComponent<Character>();
            charData.characterSequenceNumber = i;
            listCharacter.Add(character);
            character.transform.localScale = originalScale;
            character.transform.position = centerPoint + new Vector3(deltaIndex * characterSpace, 0, 0);

            // Set color based on locking status
            Renderer[] charRdr = character.GetComponentsInChildren<Renderer>();

            if (charRdr.Length > 0) //This character has childs
            {
                for (int j = 0; j < charRdr.Length; j++)
                {
                    if (charData.IsUnlocked)
                        charRdr[j].material.SetColor("_Color", Color.white);
                    else
                        charRdr[j].material.SetColor("_Color", lockColor);

                }
            }
            else //This character not has child
            {
                if (charData.IsUnlocked)
                    charRdr[0].material.SetColor("_Color", Color.white);
                else
                    charRdr[0].material.SetColor("_Color", lockColor);
            }
            
            // set as child of this object
            character.transform.parent = transform;
        }

        // Highlight current character
        currentCharacter = listCharacter[currentCharacterIndex];
        currentCharacter.transform.localScale = maxScale * originalScale;
        currentCharacter.transform.position += moveForwardAmount * Vector3.forward;
        lastCurrentCharacter = null;
        StartRotateCurrentCharacter();
    }

    // Update is called once per frame
    void Update()
    {
        #region Scrolling
        // Do the scrolling stuff
        if (Input.GetMouseButtonDown(0))    // first touch
        {
            startPos = Input.mousePosition;
            startTime = Time.time;
            hasMoved = false;
        }
        else if (Input.GetMouseButton(0))   // touch stays
        {
            endPos = Input.mousePosition;
            endTime = Time.time;

            float deltaX = Mathf.Abs(startPos.x - endPos.x);
            float deltaY = Mathf.Abs(startPos.y - endPos.y);

            if (deltaX >= swipeThresholdX && deltaY <= swipeThresholdY)
            {
                hasMoved = true;

                if (isCurrentCharacterRotating)
                    StopRotateCurrentCharacter(true);
                
                float speed = deltaX / (endTime - startTime);
                Vector3 dir = (startPos.x - endPos.x < 0) ? Vector3.right : Vector3.left;
                Vector3 moveVector = dir * (speed / 10) * scrollSpeedFactor * Time.deltaTime;

                // Move and scale the children
                for (int i = 0; i < listCharacter.Count; i++)
                {
                    MoveAndScale(listCharacter[i].transform, moveVector);
                }

                // Update for next step
                startPos = endPos;
                startTime = endTime;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (hasMoved)
            {
                // Store the last currentCharacter
                lastCurrentCharacter = currentCharacter;

                // Update current character to the one nearest to center point
                currentCharacter = FindCharacterNearestToCenter();

                // Snap
                float snapDistance = centerPoint.x - currentCharacter.transform.position.x;
                StartCoroutine(SnapAndRotate(snapDistance));
            }
        }

        #endregion

        // Update UI
        totalGold.text = CoinManager.Instance.Coins.ToString();
        Character charData = currentCharacter.GetComponent<Character>();
        characterName.text = charData.characterName;
        characterPrice.text = charData.price.ToString();

        if (currentCharacter != lastCurrentCharacter)
        {
            if (charData.IsUnlocked)
            { 
                unlockButton.gameObject.SetActive(false);
                lockButton.gameObject.SetActive(false);
                selectButon.gameObject.SetActive(true);
            }
            else
            {   
                selectButon.gameObject.SetActive(false);
                if (CoinManager.Instance.Coins >= charData.price)
                {
                    unlockButton.gameObject.SetActive(true);
                    lockButton.gameObject.SetActive(false);
                }
                else
                {
                    unlockButton.gameObject.SetActive(false);
                    lockButton.gameObject.SetActive(true);
                }    
            }
        }
    }

    void MoveAndScale(Transform tf, Vector3 moveVector)
    {
        // Move
        tf.position += moveVector;

        // Scale and move forward according to distance from current position to center position
        float d = Mathf.Abs(tf.position.x - centerPoint.x);
        if (d < (characterSpace / 2))
        {
            float factor = 1 - d / (characterSpace / 2);
            float scaleFactor = Mathf.Lerp(minScale, maxScale, factor);
            tf.localScale = scaleFactor * originalScale;

            float fd = Mathf.Lerp(0, moveForwardAmount, factor);
            Vector3 pos = tf.position;
            pos.z = centerPoint.z + fd;
            tf.position = pos;
        }
        else
        {
            tf.localScale = minScale * originalScale;
            Vector3 pos = tf.position;
            pos.z = centerPoint.z;
            tf.position = pos;
        }
    }

    GameObject FindCharacterNearestToCenter()
    {
        float min = -1;
        GameObject nearestObj = null;

        for (int i = 0; i < listCharacter.Count; i++)
        {
            float d = Mathf.Abs(listCharacter[i].transform.position.x - centerPoint.x);
            if (d < min || min < 0)
            {
                min = d;
                nearestObj = listCharacter[i];
            }
        }

        return nearestObj;
    }

    IEnumerator SnapAndRotate(float snapDistance)
    {       
        float snapDistanceAbs = Mathf.Abs(snapDistance);
        float snapSpeed = snapDistanceAbs / snapTime;
        float sign = snapDistance / snapDistanceAbs; 
        float movedDistance = 0;

        SoundManager.Instance.PlaySound(SoundManager.Instance.tick);

        while (Mathf.Abs(movedDistance) < snapDistanceAbs)
        {
            float d = sign * snapSpeed * Time.deltaTime;
            float remainedDistance = Mathf.Abs(snapDistanceAbs - Mathf.Abs(movedDistance));
            d = Mathf.Clamp(d, -remainedDistance, remainedDistance);

            Vector3 moveVector = new Vector3(d, 0, 0);
            for (int i = 0; i < listCharacter.Count; i++)
            {
                MoveAndScale(listCharacter[i].transform, moveVector);
            }

            movedDistance += d;
            yield return null;
        } 
            
        if (currentCharacter != lastCurrentCharacter || !isCurrentCharacterRotating)
        {
            // Stop rotating the last current character
            StopRotateCurrentCharacter();

            // Now rotate the new current character
            StartRotateCurrentCharacter();
        }
    }

    void StartRotateCurrentCharacter()
    {
        StopRotateCurrentCharacter(false);   // stop previous rotation if any
        rotateCoroutine = CRRotateCharacter(currentCharacter.transform);
        StartCoroutine(rotateCoroutine);
        isCurrentCharacterRotating = true;
    }

    void StopRotateCurrentCharacter(bool resetRotation = false)
    {
        if (rotateCoroutine != null)
        {
            StopCoroutine(rotateCoroutine);
        }

        isCurrentCharacterRotating = false;

        if (resetRotation)
            StartCoroutine(CRResetCharacterRotation(currentCharacter.transform));        
    }

    IEnumerator CRRotateCharacter(Transform charTf)
    {
        while (true)
        {
            charTf.Rotate(new Vector3(0, rotateSpeed * Time.deltaTime, 0));
            yield return null;
        }
    }

    IEnumerator CRResetCharacterRotation(Transform charTf)
    {
        Vector3 startRotation = charTf.rotation.eulerAngles;
        Vector3 endRotation = originalRotation;
        float timePast = 0;
        float rotateAngle = Mathf.Abs(endRotation.y - startRotation.y);
        float rotateTime = rotateAngle / resetRotateSpeed;

        while (timePast < rotateTime)
        {
            timePast += Time.deltaTime;
            Vector3 rotation = Vector3.Lerp(startRotation, endRotation, timePast / rotateTime);
            charTf.rotation = Quaternion.Euler(rotation);
            yield return null;
        }
    }

    public void UnlockButton()
    {
        bool unlockSucceeded = currentCharacter.GetComponent<Character>().Unlock();
        if (unlockSucceeded)
        {
            Renderer[] charRdr = currentCharacter.GetComponentsInChildren<Renderer>();

            if (charRdr.Length > 0)
            {
                for (int i = 0; i < charRdr.Length; i++)
                {
                    charRdr[i].material.SetColor("_Color", Color.white);
                }
            }
            else
            {
                currentCharacter.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            }
                        
            unlockButton.gameObject.SetActive(false);
            selectButon.gameObject.SetActive(true);

            SoundManager.Instance.PlaySound(SoundManager.Instance.unlock);
        }
    }

    public void SelectButton()
    {
        CharacterManager.Instance.CurrentCharacterIndex = currentCharacter.GetComponent<Character>().characterSequenceNumber;
        BackToMain();
    }

    public void BackToMain()
    {
        SceneManager.LoadScene("Main");
    }
}
