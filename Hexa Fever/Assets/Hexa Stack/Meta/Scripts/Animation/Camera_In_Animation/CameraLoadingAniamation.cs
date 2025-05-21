using System.Collections;
using UnityEngine;

public class CameraLoadingAniamation : MonoBehaviour
{
    #region PUBLIC_VARS
    #endregion

    #region PRIVATE_VARS

    [SerializeField] private Camera _metaCamera;
    [SerializeField] private float _cameraAnimationTime;
    [SerializeField] private Transform startPos;
    [SerializeField] private AnimationCurve cameraAnimationCurve;

    private float cameraStartOrthographicView = 0f;
    private float cameraEndOrthographicView;
    private Vector3 cameraEndPosition = Vector3.zero;

    private CameraSizeHandler CameraSizeHandler { get { return CameraSizeHandler.Instance; } }

    #endregion

    #region UNITY_CALLBACKS
    #endregion

    #region PUBLIC_FUNCTIONS

    public void PlayLoadingAnimation()
    {
        cameraEndOrthographicView = CameraSizeHandler.camera.orthographicSize;
        StartCoroutine(DoCameraLoadingAnimation());
    }

    #endregion

    #region PRIVATE_FUNCTIONS
    #endregion

    #region CO-ROUTINES

    private IEnumerator DoCameraLoadingAnimation()
    {
        float i = 0;
        float rate = 1 / _cameraAnimationTime;
        while (i < 1)
        {
            i += Time.deltaTime * rate;
            _metaCamera.transform.position = Vector3.LerpUnclamped(startPos.position, cameraEndPosition, cameraAnimationCurve.Evaluate(i));
            _metaCamera.orthographicSize = Mathf.LerpUnclamped(cameraStartOrthographicView, cameraEndOrthographicView, cameraAnimationCurve.Evaluate(i));
            yield return 0;
        }
    }

    #endregion

    #region EVENT_HANDLERS
    #endregion

    #region UI_CALLBACKS
    #endregion
}
