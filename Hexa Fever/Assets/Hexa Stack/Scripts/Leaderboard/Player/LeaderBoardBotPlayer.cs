using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tag.HexaStack
{
    public class LeaderBoardBotPlayer : BaseLeaderBoardPlayer
    {
        #region PUBLIC_VARS
        #endregion

        #region PRIVATE_VARS
        protected int randomSeed;
        protected float targetMultiplier;
        #endregion

        #region KEY
        #endregion

        #region Propertices
        #endregion

        #region Overrided_Method
        public void InitSeedData(int randomSeed, float targetMultiplier)
        {
            this.randomSeed = randomSeed;
            this.targetMultiplier = targetMultiplier;
        }

        public override int GetCurrentPoints()
        {
            int targetScore = VIPLeaderboardManager.Instance.GetBotTargetScore();
            int botFinalScore = Mathf.FloorToInt(targetMultiplier * targetScore);

            DateTime currentTime = CustomTime.GetCurrentTime();

            var leaderBoardPlayerData = PlayerPersistantData.GetVIPLeaderboardPlayerData();
            CustomTime.TryParseDateTime(leaderBoardPlayerData.leaderboardStartTimeString, out DateTime leaderboardStartTime);
            DateTime eventStartTime = leaderboardStartTime;
            DateTime eventEndTime = eventStartTime.AddDays(VIPLeaderboardManager.Instance.LeaderBoardEventRunTimeInDays);

            Debug.LogError("Event Start Time :- " + eventStartTime);
            Debug.LogError("Event Current Time :- " + currentTime);
            Debug.LogError("Event End Time :- " + eventEndTime);

            // If event hasn't started
            if (currentTime < eventStartTime)
            {
                Debug.LogError("Event Not Started");
                return 0;
            }

            // If event has ended, return final score
            if (currentTime >= eventEndTime)
            {
                Debug.LogError("Final Bot Score");
                return botFinalScore;
            }

            // Initialize random with seed for this calculation
            Random.InitState(randomSeed);

            // Generate random update times (in hours from start)
            float totalEventHours = (float)(eventEndTime - eventStartTime).TotalHours;
            int numberOfUpdates = Random.Range(VIPLeaderboardManager.Instance.LeaderboardData.botUpdateCountRange.x, VIPLeaderboardManager.Instance.LeaderboardData.botUpdateCountRange.y); // x-y updates during the event
            
            List<float> updateHours = new List<float>();
            for (int i = 0; i < numberOfUpdates; i++)
            {
                updateHours.Add(Random.Range(0, totalEventHours));
            }
            updateHours.Sort();

            // Generate random scores for each update time
            List<int> updateScores = new List<int>();
            for (int i = 0; i < numberOfUpdates - 1; i++)
            {
                updateScores.Add(Random.Range(0, botFinalScore));
            }
            updateScores.Add(botFinalScore); // Ensure final update reaches target score
            updateScores.Sort();

            // Find current score based on elapsed time
            float elapsedHours = (float)(currentTime - eventStartTime).TotalHours;
            int currentScorePoints = 0;

            for (int i = 0; i < updateHours.Count; i++)
            {
                if (elapsedHours >= updateHours[i])
                {
                    currentScorePoints = updateScores[i];
                }
                else
                {
                    break;
                }
            }

            // Reset random seed
            Random.InitState(Utility.GetNewRandomSeed());

            return currentScorePoints;
        }
        #endregion

        #region UNITY_CALLBACKS
        #endregion

        #region PUBLIC_FUNCTIONS
        #endregion

        #region PRIVATE_FUNCTIONS
        #endregion

        #region CO-ROUTINES
        #endregion

        #region EVENT_HANDLERS
        #endregion

        #region UI_CALLBACKS
        #endregion
    }
}