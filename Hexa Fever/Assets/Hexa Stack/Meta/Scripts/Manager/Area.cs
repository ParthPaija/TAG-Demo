using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Tag.TaskSystem;
using UnityEngine;

namespace Tag.MetaGame
{
    public class Area : MonoBehaviour
    {
        #region PUBLIC_VARS

        public string id;
        public TaskAreaData taskAreaData;
        public bool isRequiredNewResolution;
        #endregion

        #region PRIVATE_VARS
        [SerializeField] private BaseDecoreItem[] _baseDecoreItems;
        [SerializeField] private RevealSequenceData[] _revealSequenceDatas;
        private Dictionary<string, BaseDecoreItem> _decoreMapDict;
        private Dictionary<string, string> _areaProgressDataDict = new Dictionary<string, string>();

        private event Action OnDecoreIconsShow;
        private event Action OnDecoreIconsHide;
        private AreaManager AreaManager { get { return AreaManager.Instance; } }
        private AreaEditMode AreaEditMode { get { return AreaEditMode.Instance; } }
        private MetaCameraSizeController MetaCameraSizeController { get { return MetaCameraSizeController.Instance; } }
        #endregion

        #region UNITY_CALLBACKS
        private void OnEnable()
        {
            MetaCameraSizeController.SetCameraSizeAccordingAreaNo(isRequiredNewResolution);
        }
        #endregion

        #region PUBLIC_FUNCTIONS

        public void Init(string data)
        {
            IniDict();
            LoadData(data);
        }

        public void LoadArea()
        {
            for (int i = 0; i < _baseDecoreItems.Length; i++)
            {
                _baseDecoreItems[i].LoadItem();
            }
        }

        public void SetDecoreItemData(string taskId, string taskData)
        {
            _areaProgressDataDict[taskId] = taskData;
            AreaManager.SetAreaData(id, JsonConvert.SerializeObject(_areaProgressDataDict));
        }

        public void StartTask(string taskId)
        {
            _decoreMapDict[taskId].StartDecor();
        }
        [Button]
        public string GetData()
        {
            _areaProgressDataDict = new Dictionary<string, string>();
            for (int i = 0; i < _baseDecoreItems.Length; i++)
            {
                _areaProgressDataDict.Add(_baseDecoreItems[i].id, _baseDecoreItems[i].GetData());
            }
            return JsonConvert.SerializeObject(_areaProgressDataDict);
        }

        public void SaveData()
        {
            AreaManager.SaveData();
        }

        public void RegisterIconAction(Action showIconAction, Action hideIconAction)
        {
            OnDecoreIconsShow += showIconAction;
            OnDecoreIconsHide += hideIconAction;
        }

        public void PlayRevealSequence(Action exitAction)
        {
            AreaEditMode areaEditMode = AreaEditMode;
            areaEditMode.AreaEditModeOn(exitAction);
            areaEditMode.HideIcons();
            HideAllObjects();
            PlayAnimationSequence(SequenceEndAction);

            void SequenceEndAction()
            {
                areaEditMode.ShowIcons();
            }
        }

        public void AreaCompleteRevealSequence(Action sequenceEndAction)
        {
            AreaEditMode areaEditMode = AreaEditMode;
            areaEditMode.AreaEditModeOn();
            areaEditMode.HideIcons();
            HideAllObjects();
            PlayAnimationSequence(sequenceEndAction);
        }

        public List<string> GetCompleteTaskIds()
        {
            List<string> taskIds = new List<string>();
            for (int i = 0; i < _baseDecoreItems.Length; i++)
            {
                if (_baseDecoreItems[i].IsItemDecore())
                    taskIds.Add(_baseDecoreItems[i].id);
            }
            return taskIds;
        }
        #endregion

        #region PRIVATE_FUNCTIONS

        private void IniDict()
        {
            _decoreMapDict = new Dictionary<string, BaseDecoreItem>();
            for (int i = 0; i < _baseDecoreItems.Length; i++)
            {
                _decoreMapDict.Add(_baseDecoreItems[i].id, _baseDecoreItems[i]);
            }
        }

        private void LoadData(string data)
        {
            _areaProgressDataDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

            for (int i = 0; i < _baseDecoreItems.Length; i++)
            {
                BaseDecoreItem baseDecoreItem = _baseDecoreItems[i];
                string prefsData = _areaProgressDataDict[baseDecoreItem.id];
                baseDecoreItem.LoadData(prefsData, SaveData);
            }
        }

        private void PlayAnimationSequence(Action sequenceEndAction)
        {
            float waitTime = 0;
            string lastRevealTaskId = null;
            for (int i = 0; i < _revealSequenceDatas.Length; i++)
            {
                RevealSequenceData revealSequenceData = _revealSequenceDatas[i];
                if (waitTime < revealSequenceData.delay)
                {
                    waitTime = revealSequenceData.delay;
                    lastRevealTaskId = revealSequenceData.taskId;
                }
                StartCoroutine(PlayAnimation(revealSequenceData));
            }
            StartCoroutine(PlayAllParticle(_decoreMapDict[lastRevealTaskId].GetAnimationTime() + waitTime, sequenceEndAction));
        }

        private void PlayAllTaskParticles()
        {
            for (int i = 0; i < _revealSequenceDatas.Length; i++)
            {
                _decoreMapDict[_revealSequenceDatas[i].taskId].PlayCompleteParticle();
            }
        }

        private void HideAllObjects()
        {
            for (int i = 0; i < _revealSequenceDatas.Length; i++)
            {
                _decoreMapDict[_revealSequenceDatas[i].taskId].HideItem();
            }
        }

        #endregion

        #region CO-ROUTINES

        private IEnumerator PlayAnimation(RevealSequenceData revealSequenceData)
        {
            yield return new WaitForSeconds(revealSequenceData.delay);
            _decoreMapDict[revealSequenceData.taskId].PlayRevealAnimation();
        }

        private IEnumerator PlayAllParticle(float waitTime, Action endAction)
        {
            yield return new WaitForSeconds(waitTime);
            PlayAllTaskParticles();
            yield return new WaitForSeconds(2f);
            endAction?.Invoke();
        }

        #endregion

        #region EVENT_HANDLERS

        public void RaiseOnDecoreIconsShow()
        {
            OnDecoreIconsShow?.Invoke();
        }

        public void RaiseOnDecoreIconsHide()
        {
            OnDecoreIconsHide?.Invoke();
        }

        #endregion

        #region UI_CALLBACKS

        #endregion

#if UNITY_EDITOR
        [ContextMenu("Play Reveal Sequence")]
        public void PlayRevealSequence()
        {
            PlayRevealSequence(null);
        }
        [Button]
        public void SetTaskID()
        {
            for (int i = 0; i < _baseDecoreItems.Length; i++)
            {
                _baseDecoreItems[i].id = "T" + (i + 1);
                _revealSequenceDatas[i].taskId = "T" + (i + 1);
                _revealSequenceDatas[i].taskId = "T" + (i + 1);
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
        [Button]
        public void SetSimpleTaskScript()
        {
            for (int i = 0; i < _baseDecoreItems.Length; i++)
            {
                if (_baseDecoreItems[i].GetComponent<SimpleDecoreItem>() == null)
                {
                    _baseDecoreItems[i].gameObject.AddComponent<SimpleDecoreItem>();
                    _baseDecoreItems[i].GetComponent<SimpleDecoreItem>().id = _baseDecoreItems[i].id;
                    _baseDecoreItems[i] = _baseDecoreItems[i].GetComponent<SimpleDecoreItem>();
                    DestroyImmediate(_baseDecoreItems[i].GetComponent<DecoreItemWithOption>());
                }
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}