using FMOD;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Tag.HexaStack
{
    public class GoalItemView : SerializedMonoBehaviour
    {
        #region PUBLIC_VARS

        public Transform GoalTransfrom { get { return itemImage.transform; } }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private bool isTextRemoveAfterGoalFinish = false;
        [SerializeField] private bool isReverseGoal = false;
        [SerializeField] private Image itemImage;
        [SerializeField] private GameObject rightTickObj;
        [SerializeField] private Text counterText;
        [SerializeField] RectFillBar fillBar;

        [SerializeField] private BaseLevelGoal baseLevelGoal;

        #endregion

        #region UNITY_CALLBACKS

        private void OnDestroy()
        {
            MainSceneUIManager.Instance.GetView<VFXView>().DeRagisterOnItemAnimationComplete(PlayAnimation);
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void SetView(BaseLevelGoal baseLevelGoal)
        {
            this.baseLevelGoal = baseLevelGoal;
            itemImage.sprite = baseLevelGoal.GetRender();

            if (isReverseGoal)
                counterText.text = $"{baseLevelGoal.GoalCount}";
            else
                counterText.text = $"{baseLevelGoal.CurrentCount}/{baseLevelGoal.GoalCount}";
            FillGoal();
            MainSceneUIManager.Instance.GetView<VFXView>().RagisterOnItemAnimationComplete(PlayAnimation);
        }

        public Transform GetItemEndPosition(GoalType goalType, int itemId)
        {
            if (baseLevelGoal.IsPlayAnimation(goalType, itemId))
                return GoalTransfrom;
            return null;
        }

        public void SetGoalItem()
        {
            if (isReverseGoal)
                counterText.text = $"{baseLevelGoal.GoalCount - baseLevelGoal.CurrentCount}";
            else
                counterText.text = $"{baseLevelGoal.CurrentCount}/{baseLevelGoal.GoalCount}";
            rightTickObj.SetActive(baseLevelGoal.IsGoalFullFilled());
            if (isTextRemoveAfterGoalFinish && LevelEditorManager.Instance == null)
                counterText.gameObject.SetActive(!baseLevelGoal.IsGoalFullFilled());
            FillGoal();
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void PlayAnimation(int itemType, int size, bool isLastObject)
        {
            //if (baseLevelGoal.IsPlayAnimation(itemType))
            {
                //baseLevelGoal.UpdateGoal(itemType, size);
                SetGoalItem();
            }
        }

        private void FillGoal()
        {
            if (fillBar != null)
            {
                float fillAmount = (float)baseLevelGoal.CurrentCount / baseLevelGoal.GoalCount;
                fillBar.Fill(fillAmount, 0.2f, false, null);
            }
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}
