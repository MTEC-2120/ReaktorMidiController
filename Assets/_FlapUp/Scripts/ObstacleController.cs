using UnityEngine;
using System.Collections;

public class ObstacleController : MonoBehaviour
{
    public GameObject leftOb;
    public GameObject rightOb;
    public float movingSpeed;
    public float fluctuationRange;

    private float firstRightObPosition;
    private float maxLengthMove;
    private int leftTurn = 1;
    private int rightTurn = 1;

    // Use this for initialization
    void Start()
    {
        firstRightObPosition = Mathf.Abs(rightOb.transform.position.x);
        maxLengthMove = GameManager.Instance.maxObstacleFluctuationRange - 0.5f;


        StartCoroutine(MovingLeftOb());
        StartCoroutine(MovingRightOb());
        StartCoroutine(MovingObWhenGameOver());
    }

    IEnumerator MovingLeftOb()
    {
        while (true)
        {
            leftTurn = leftTurn * (-1);

            Vector3 startPos = leftOb.transform.position;
            Vector3 endPos;
            if (leftTurn < 0)
            {
                endPos = leftOb.transform.position + new Vector3(-fluctuationRange, 0, 0);
            }
            else
            {
                endPos = leftOb.transform.position + new Vector3(fluctuationRange, 0, 0);
            }

            float t = 0;
            while (t < movingSpeed && GameManager.Instance.GameState != GameState.GameOver)
            {
                t += Time.deltaTime;
                float fraction = t / movingSpeed;
                leftOb.transform.position = Vector3.Lerp(startPos, endPos, fraction);
                yield return null;
            }

            if (GameManager.Instance.GameState == GameState.GameOver)
            {
                yield break;
            }
        }
    }


    IEnumerator MovingRightOb()
    {
        while (true)
        {
            rightTurn = rightTurn * (-1);

            Vector3 startPos = rightOb.transform.position;
            Vector3 endPos;
            if (rightTurn < 0)
            {
                endPos = rightOb.transform.position + new Vector3(fluctuationRange, 0, 0);
            }
            else
            {
                endPos = rightOb.transform.position + new Vector3(-fluctuationRange, 0, 0);
            }

            float t = 0;
            while (t < movingSpeed && GameManager.Instance.GameState != GameState.GameOver)
            {
                t += Time.deltaTime;
                float fraction = t / movingSpeed;
                rightOb.transform.position = Vector3.Lerp(startPos, endPos, fraction);
                yield return null;
            }

            if (GameManager.Instance.GameState == GameState.GameOver)
            {
                yield break;
            }
        }
    }


    IEnumerator MovingObWhenGameOver()
    {
        while (true)
        {
            if (GameManager.Instance.GameState == GameState.GameOver)
            {
                yield return new WaitForSeconds(0.3f);

                float currentRightObXPosition = Mathf.Abs(rightOb.transform.position.x);

                if (currentRightObXPosition < firstRightObPosition + maxLengthMove)
                {
                    float addedDistance = (firstRightObPosition + maxLengthMove) - currentRightObXPosition;

                    Vector3 startPosLeftOb = leftOb.transform.position;                    
                    Vector3 endPosLeftOb = leftOb.transform.position + new Vector3(-addedDistance, 0, 0);
     

                    Vector3 startPosRightOb = rightOb.transform.position;
                    Vector3 endPosRightOb = rightOb.transform.position + new Vector3(addedDistance, 0, 0);
         

                    float t = 0;
                    while (t < GameManager.Instance.minObstacleSpeedFactor / 2)
                    {
                        t += Time.deltaTime;
                        float fraction = t / (GameManager.Instance.minObstacleSpeedFactor / 2);
                        leftOb.transform.position = Vector3.Lerp(startPosLeftOb, endPosLeftOb, fraction);
                        rightOb.transform.position = Vector3.Lerp(startPosRightOb, endPosRightOb, fraction);
                        yield return null;
                    }

                    yield break;

                }
                else
                {
                    yield break;
                }               
            }
            yield return null;
        }
    }

}
