using UnityEngine;
using System.Collections;

public class GoldController : MonoBehaviour
{
    public float fluctuationRange;
    public float movingSpeed;

    void Start()
    {
        StartCoroutine(MoveGold());
    }


    IEnumerator MoveGold()
    {
        while (true)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos;
            if (transform.position.x < 0)
            {
                endPos = transform.position + new Vector3(fluctuationRange * 2, 0, 0);
            }
            else
            {
                endPos = transform.position + new Vector3(-fluctuationRange * 2, 0, 0);
            }

            float t = 0;
            while (t < movingSpeed)
            {
                t += Time.deltaTime;
                float fraction = t / movingSpeed;
                transform.position = Vector3.Lerp(startPos, endPos, fraction);
                yield return null;
            }

            if (GameManager.Instance.GameState == GameState.GameOver)
            {
                gameObject.SetActive(false);
                yield break;
            }
            yield return null;
        }
    }


}
