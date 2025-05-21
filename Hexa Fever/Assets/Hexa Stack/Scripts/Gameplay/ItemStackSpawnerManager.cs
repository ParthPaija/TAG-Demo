using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Tag.HexaStack
{
    public class ItemStackSpawnerManager : SerializedManager<ItemStackSpawnerManager>
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private ItemStackSpawner[] itemStackSpawners;
        private Coroutine spawnCO;
        Sequence sequence;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void OnGameStart()
        {
            LevelProgressData levelProgressData = DataManager.LevelProgressData;
            int setIndex = levelProgressData.setIndex;
            GameplayManager.Instance.CurrentLevel.SetIndex = setIndex;

            SpwnItem(true);
            spawnCO = StartCoroutine(SpawnItemsContinuously());
        }

        [Button]
        public void SpwnItem(bool isSaveData = false)
        {
            var playerData = DataManager.PlayerData;
            LevelProgressData levelProgressData = DataManager.LevelProgressData;

            if (levelProgressData.spawnerData.Count >= 3 && isSaveData)
            {
                for (int i = 0; i < itemStackSpawners.Length; i++)
                {
                    itemStackSpawners[i].SpawnStack(levelProgressData.GetSpwanerData(i));
                }
            }
            else
            {
                for (int i = 0; i < itemStackSpawners.Length; i++)
                {
                    itemStackSpawners[i].SpawnStack(GameplayManager.Instance.CurrentLevel.GetItem()[i]);
                }
            }

            CoroutineRunner.Instance.Wait(0.2f, () =>
            {
                StakInAnimation();
            });
        }

        public void SpwnItemForBooster(bool isSaveData = false)
        {
            var playerData = DataManager.PlayerData;
            LevelProgressData levelProgressData = DataManager.LevelProgressData;

            if (levelProgressData.spawnerData.Count >= 3 && isSaveData)
            {
                for (int i = 0; i < itemStackSpawners.Length; i++)
                {
                    itemStackSpawners[i].SpawnStack(levelProgressData.GetSpwanerData(i));
                }
            }
            else
            {
                for (int i = 0; i < itemStackSpawners.Length; i++)
                {
                    itemStackSpawners[i].SpawnStack(GameplayManager.Instance.CurrentLevel.GetItemForBooster()[i]);
                }
                //SaveData(levelProgressData);
                //DataManager.Instance.SaveLevelProgressData(levelProgressData);
            }

            CoroutineRunner.Instance.Wait(0.2f, () =>
            {
                StakInAnimation();
            });
        }

        public void SaveData(LevelProgressData levelProgressData)
        {
            for (int i = 0; i < itemStackSpawners.Length; i++)
            {
                itemStackSpawners[i].SavaData(levelProgressData);
            }
            levelProgressData.setIndex = GameplayManager.Instance.CurrentLevel.SetIndex;
        }

        public void SetUndoBoosterData(LevelProgressData levelProgressData)
        {
            for (int i = 0; i < itemStackSpawners.Length; i++)
            {
                itemStackSpawners[i].SpawnStack(levelProgressData.GetSpwanerData(i));
            }

            CoroutineRunner.Instance.Wait(0.2f, () =>
            {
                StakInAnimation();
            });
        }

        public void ClearStack()
        {
            for (int i = 0; i < itemStackSpawners.Length; i++)
            {
                itemStackSpawners[i].ClearStack();
            }
            if (spawnCO != null)
                StopCoroutine(spawnCO);
        }

        public void StakInAnimation()
        {
            sequence = DOTween.Sequence();

            for (int i = 0; i < itemStackSpawners.Length; i++)
            {
                int index = i; // Capture the index for the callback
                sequence.AppendCallback(() =>
                {
                    itemStackSpawners[index].gameObject.SetActive(true);
                    itemStackSpawners[index].StakInAnimation();
                });
                sequence.AppendInterval(0.2f); // Wait time between each stack animation
            }
        }

        public void StakOutAnimation()
        {
            if (sequence != null)
                sequence.Kill();

            for (int i = 0; i < itemStackSpawners.Length; i++)
            {
                itemStackSpawners[i].gameObject.SetActive(false);
                //itemStackSpawners[i].StakOutAnimation();
            }
        }

        public void ChanageScale(Vector3 scale)
        {
            for (int i = 0; i < itemStackSpawners.Length; i++)
            {
                itemStackSpawners[i].transform.localScale = scale;
            }
        }

        public ItemStackSpawner GetSpawnerByIendex(int index)
        {
            return itemStackSpawners[index];
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        #endregion

        #region CO-ROUTINES

        private IEnumerator SpawnItemsContinuously()
        {
            yield return new WaitForSeconds(1);
            while (true)
            {
                bool allEmpty = true;

                for (int i = 0; i < itemStackSpawners.Length; i++)
                {
                    if (itemStackSpawners[i].ItemStack != null)
                    {
                        allEmpty = false;
                    }
                    yield return null;
                }

                yield return new WaitForSeconds(0.1f);
                yield return new WaitForEndOfFrame();

                if (allEmpty)
                {
                    GameplayManager.Instance.CurrentLevel.SetIndex++;
                    for (int i = 0; i < itemStackSpawners.Length; i++)
                    {
                        itemStackSpawners[i].SpawnStack(GameplayManager.Instance.CurrentLevel.GetItem()[i]);
                    }
                    StakInAnimation();
                }
            }
        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}
