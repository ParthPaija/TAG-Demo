using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tag.HexaStack
{
    public class InputManager : Manager<InputManager>
    {
        #region PUBLIC_VARS

        public static Transform eventTranform
        {
            get { return Instance.eventCamera.transform; }
        }

        public static Camera EventCamera
        {
            get { return Instance.eventCamera; }
        }

        public static bool RaycastBlock
        {
            get { return BaseView.blockView.Count > 0; }
        }

        [ShowInInspector]
        public static bool StopInteraction
        {
            get;
            set;
        }

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private Camera eventCamera;
        [SerializeField] private bool convertToWorldSpace;

        private List<Action<Vector3>> mouseButtonDownEvent = new List<Action<Vector3>>();
        private List<Action<Vector3>> mouseButtonUpEvent = new List<Action<Vector3>>();
        private List<Action<Vector3>> mouseButtonMoveEvent = new List<Action<Vector3>>();
        private List<Action<Vector3>> onUIClick = new List<Action<Vector3>>();
        private List<Action<Vector3>> onUIPointerUp = new List<Action<Vector3>>();

        private bool isMouseDown;
        private Touch touch;

        #endregion

        #region UNITY_CALLBACKS

        private void Start()
        {
            isMouseDown = false;
            StopInteraction = false;
        }

        private void Update()
        {
            if (StopInteraction)
            {
                return;
            }

            if (EventSystem.current != null && (EventSystem.current.currentSelectedGameObject != null || RaycastBlock))
            {
                InvokeOnUIClick(Input.mousePosition);
#if UNITY_EDITOR
                if (Input.GetMouseButtonUp(0))
                {
                    InvokeOnUIPointerUp(Input.mousePosition);
                }
#endif
#if !UNITY_EDITOR
                if (Input.touchCount > 0)
                {
                    touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
                    {
                        InvokeOnUIPointerUp(touch.position);
                    }
                }
#endif
                return;
            }

            //#if UNITY_EDITOR
            EditorInput();
            //#endif
            //#if !UNITY_EDITOR
            //MobileInput();
            //#endif
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void AddListenerMouseButtonDown(Action<Vector3> action)
        {
            if (!mouseButtonDownEvent.Contains(action))
                mouseButtonDownEvent.Add(action);
        }

        public void RemoveListenerMouseButtonDown(Action<Vector3> action)
        {
            if (mouseButtonDownEvent.Contains(action))
                mouseButtonDownEvent.Remove(action);
        }

        public void AddListenerMouseButtonUp(Action<Vector3> action)
        {
            if (!mouseButtonUpEvent.Contains(action))
                mouseButtonUpEvent.Add(action);
        }

        public void RemoveListenerMouseButtonUp(Action<Vector3> action)
        {
            if (mouseButtonUpEvent.Contains(action))
                mouseButtonUpEvent.Remove(action);
        }

        public void AddListenerMouseButtonMove(Action<Vector3> action)
        {
            if (!mouseButtonMoveEvent.Contains(action))
                mouseButtonMoveEvent.Add(action);
        }

        public void RemoveListenerMouseButtonMove(Action<Vector3> action)
        {
            if (mouseButtonMoveEvent.Contains(action))
                mouseButtonMoveEvent.Remove(action);
        }

        public void AddListenerUIClick(Action<Vector3> action)
        {
            if (!onUIClick.Contains(action))
                onUIClick.Add(action);
        }

        public void RemoveListenerUIClick(Action<Vector3> action)
        {
            if (onUIClick.Contains(action))
                onUIClick.Remove(action);
        }

        public void AddListenerUIPointerUp(Action<Vector3> action)
        {
            if (!onUIPointerUp.Contains(action))
                onUIPointerUp.Add(action);
        }

        public void RemoveListenerUIPointerUp(Action<Vector3> action)
        {
            if (onUIPointerUp.Contains(action))
                onUIPointerUp.Remove(action);
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void EditorInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                InvakeMouseButtonDown(Input.mousePosition);
                isMouseDown = true;
            }
            else if (isMouseDown)
            {
                InvokeMouseMove(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                InvakeMouseButtonUp(Input.mousePosition);
                isMouseDown = false;
            }
        }

        private void MobileInput()
        {
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    InvakeMouseButtonDown(touch.position);
                    isMouseDown = true;
                }
                else if (isMouseDown)
                {
                    InvokeMouseMove(touch.position);
                }

                if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
                {
                    InvakeMouseButtonUp(touch.position);
                    isMouseDown = false;
                }
            }
        }

        private void InvakeMouseButtonDown(Vector3 pos)
        {
            if (convertToWorldSpace)
                pos = eventCamera.ScreenToWorldPoint(pos);
            foreach (var ev in mouseButtonDownEvent)
            {
                ev?.Invoke(pos);
            }
        }

        private void InvakeMouseButtonUp(Vector3 pos)
        {
            if (convertToWorldSpace)
                pos = eventCamera.ScreenToWorldPoint(pos);
            foreach (var ev in mouseButtonUpEvent)
            {
                ev?.Invoke(pos);
            }
        }

        private void InvokeMouseMove(Vector3 pos)
        {
            if (convertToWorldSpace)
                pos = eventCamera.ScreenToWorldPoint(pos);

            foreach (var ev in mouseButtonMoveEvent)
            {
                ev?.Invoke(pos);
            }
        }

        private void InvokeOnUIClick(Vector3 pos)
        {
            if (convertToWorldSpace)
                pos = eventCamera.ScreenToWorldPoint(pos);

            foreach (var ev in onUIClick)
            {
                ev?.Invoke(pos);
            }
        }

        private void InvokeOnUIPointerUp(Vector3 pos)
        {
            if (convertToWorldSpace)
                pos = eventCamera.ScreenToWorldPoint(pos);
            foreach (var ev in onUIPointerUp)
            {
                ev?.Invoke(pos);
            }
        }

        #endregion
    }
}
