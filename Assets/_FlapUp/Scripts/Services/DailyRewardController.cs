using UnityEngine;
using System.Collections;
using System;

namespace SgLib
{

    public class DailyRewardController : MonoBehaviour
    {
        public static DailyRewardController Instance { get; private set; }

        public DateTime NextRewardTime
        {
            get
            {
                return GetNextRewardTime();
            }
        }

        public TimeSpan TimeUntilReward
        { 
            get
            {
                return NextRewardTime.Subtract(DateTime.Now);
            }
        }

        [Header("Check to disable Daily Reward Feature")]
        public bool disable;

        [Header("Daily Reward Config")]
        [Tooltip("Number of hours between 2 rewards")]
        public int rewardIntervalHours = 6;
        [Tooltip("Number of minues between 2 rewards")]
        public int rewardIntervalMinutes = 0;
        [Tooltip("Number of seconds between 2 rewards")]
        public int rewardIntervalSeconds = 0;
        public int minRewardValue = 20;
        public int maxRewardValue = 50;

        private const string NextRewardTimePPK = "SGLIB_NEXT_DAILY_REWARD_TIME";

        void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        /// <summary>
        /// Determines whether the waiting time has passed and can reward now.
        /// </summary>
        /// <returns><c>true</c> if this instance can reward now; otherwise, <c>false</c>.</returns>
        public bool CanRewardNow()
        {
            return TimeUntilReward <= TimeSpan.Zero;
        }

        /// <summary>
        /// Gets the random reward.
        /// </summary>
        /// <returns>The random reward.</returns>
        public int GetRandomReward()
        {
            return UnityEngine.Random.Range(minRewardValue, maxRewardValue + 1);
        }

        /// <summary>
        /// Set the next reward time to some time in future determined by the predefined number of hours, minutes and seconds.
        /// </summary>
        public void ResetNextRewardTime()
        {
            DateTime next = DateTime.Now.Add(new TimeSpan(rewardIntervalHours, rewardIntervalMinutes, rewardIntervalSeconds));
            StoreNextRewardTime(next);
        }

        void StoreNextRewardTime(DateTime time)
        {
            PlayerPrefs.SetString(NextRewardTimePPK, time.ToBinary().ToString());
            PlayerPrefs.Save();
        }

        DateTime GetNextRewardTime()
        {
            string storedTime = PlayerPrefs.GetString(NextRewardTimePPK, string.Empty);

            if (!string.IsNullOrEmpty(storedTime))
                return DateTime.FromBinary(Convert.ToInt64(storedTime));
            else
                return DateTime.Now;
        }
    }
}
