using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SgLib
{
    public class RewardUIController : MonoBehaviour
    {
        public Transform animatedGiftBox;
        public GameObject congratsText;
        public GameObject sunburst;
        public GameObject reward;
        public Text rewardText;

        bool isRewarding = false;

        public void Reward(int rewardValue)
        {
            if (!isRewarding)
            {
                StartCoroutine(CRPlayRewardAnim(rewardValue));
            }
        }

        public void Close()
        {
            if (!isRewarding)
            {
                gameObject.SetActive(false);
            }
        }

        IEnumerator CRPlayRewardAnim(int rewardValue)
        {
            isRewarding = true;

            congratsText.SetActive(false);
            reward.SetActive(false);
            sunburst.SetActive(false);

            animatedGiftBox.gameObject.SetActive(true);
            float start = Time.time;

            while (Time.time - start < 2f)
            {
                animatedGiftBox.eulerAngles = new Vector3(0, 0, Random.Range(-10f, 10f));
                animatedGiftBox.localScale = new Vector3(Random.Range(0.9f, 1.3f), Random.Range(0.9f, 1.3f), Random.Range(0.9f, 1.3f));
                yield return new WaitForSeconds(0.07f);
            }

            start = Time.time;
            Vector3 startScale = animatedGiftBox.localScale;

            while (Time.time - start < 0.15f)
            {
                animatedGiftBox.localScale = Vector3.Lerp(startScale, Vector3.one * 20f, (Time.time - start) / 0.2f);
                yield return null;
            }

            animatedGiftBox.gameObject.SetActive(false);  


            // Show reward
            reward.SetActive(true);

            for (int i = 1; i <= rewardValue; i++)
            {
                rewardText.text = i.ToString();
                SoundManager.Instance.PlaySound(SoundManager.Instance.tick);
                yield return new WaitForSeconds(0.03f);
            }

            // Actually store the rewards.
            CoinManager.Instance.AddCoins(rewardValue);

            yield return new WaitForSeconds(0.2f);

            SoundManager.Instance.PlaySound(SoundManager.Instance.rewarded);
            congratsText.SetActive(true);
            sunburst.SetActive(true);

            reward.GetComponent<Animator>().SetTrigger("Reward");

            yield return new WaitForSeconds(3f);

            isRewarding = false;
        }
    }
}
