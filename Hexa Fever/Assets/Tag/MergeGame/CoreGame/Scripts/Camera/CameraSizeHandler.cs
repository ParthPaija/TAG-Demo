using Sirenix.OdinInspector;
using System;
using Tag.AssetManagement;
using Tag.HexaStack;
using UnityEngine;

public class CameraSizeHandler : Manager<CameraSizeHandler>
{
    #region PUBLIC_VARIABLES

    public new Camera camera;

    [Header("Bounds Settings")]
    public SpriteRenderer requiredGameplayBounds;
    public SpriteRenderer maximumGameplayBounds;
    public float bannerAdHeight = 50f;
    public bool isBannerAdActive;
    #endregion

    #region PRIVATE_VARIABLES

    #endregion

    #region UNITY_CALLBACKS
    private void Awake()
    {
        InitializeSize();
    }
    #endregion

    #region PUBLIC_FUNCTIONS

    #endregion

    #region PRIVATE_FUNCTIONS

    [Button]
    public void InitializeSize()
    {
        Camera myCam = camera;

        float screenWidth = GetGameViewSize().x;
        float screenHight = GetGameViewSize().y;


        myCam.transform.position = requiredGameplayBounds.transform.position;

        Vector2 currentWidthLimits = new Vector2(myCam.ScreenToWorldPoint(new Vector3(0, 0, 0)).x, myCam.ScreenToWorldPoint(new Vector3(screenWidth, 0, 0)).x);

        Vector2 maxWidthLimits = new Vector2(maximumGameplayBounds.transform.position.x - maximumGameplayBounds.transform.localScale.x / 2, maximumGameplayBounds.transform.position.x + maximumGameplayBounds.transform.localScale.x / 2);
        Vector2 requiredGameplayWidthLimits = new Vector2(requiredGameplayBounds.transform.position.x - requiredGameplayBounds.transform.localScale.x / 2, requiredGameplayBounds.transform.position.x + requiredGameplayBounds.transform.localScale.x / 2);

        float camWidthInWorldScale = myCam.transform.InverseTransformPoint(myCam.ScreenToWorldPoint(new Vector3(screenWidth, 0, 0))).x * 2;
        float camHeightInWorldScale = myCam.transform.InverseTransformPoint(myCam.ScreenToWorldPoint(new Vector3(0, screenHight, 0))).y * 2;

        //Check For Horizontal Inbound With Required Gameplay
        if (camWidthInWorldScale < (requiredGameplayWidthLimits.y - requiredGameplayWidthLimits.x))
        {
            // Set Width According to Required Gameplay Increasing Height
            myCam.orthographicSize = ((requiredGameplayWidthLimits.y - requiredGameplayWidthLimits.x) / myCam.aspect) / 2;
            Vector2 currentHeightLimits = new Vector2(myCam.ScreenToWorldPoint(new Vector3(0, 0, 0)).y, myCam.ScreenToWorldPoint(new Vector3(0, screenHight, 0)).y);
            Vector2 maxHeightLimits = new Vector2(maximumGameplayBounds.transform.position.y - maximumGameplayBounds.transform.localScale.y / 2, maximumGameplayBounds.transform.position.y + maximumGameplayBounds.transform.localScale.y / 2);

            if (currentHeightLimits.x < maxHeightLimits.x || currentHeightLimits.y > maxHeightLimits.y)
            {
                Debug.LogError("2");
                // Debug.Log("Out Of Bound Height With Max Area");
                if (camHeightInWorldScale < (maxHeightLimits.y - maxHeightLimits.x))
                {
                    Debug.LogError("3");
                    // Debug.Log("Setting Camera Position...");
                    Vector3 newPos = myCam.transform.position;
                    if (currentHeightLimits.x < maxHeightLimits.x)
                    {
                        newPos.y += (maxHeightLimits.x - currentHeightLimits.x);
                    }
                    else if (currentHeightLimits.y > maxHeightLimits.y)
                    {
                        newPos.y += (maxHeightLimits.y - currentHeightLimits.y);
                    }
                    myCam.transform.position = newPos;
                }
                else
                {
                    Debug.LogError("Camera Position Cannot be Set to Cover Max Area In Height.. Please Increase Max Area.");
                }
            }
        }
        else if (currentWidthLimits.x < maxWidthLimits.x || currentWidthLimits.y > maxWidthLimits.y)
        {
            Debug.LogError("4");
            // Debug.Log("Out Of Bound Width With Max Area");
            if (camWidthInWorldScale < (maxWidthLimits.y - maxWidthLimits.x))
            {
                Debug.LogError("5");
                //Debug.Log("Setting Camera Position...");
                Vector3 newPos = myCam.transform.position;
                if (currentWidthLimits.x < maxWidthLimits.x)
                {
                    newPos.x += (maxWidthLimits.x - currentWidthLimits.x);
                }
                else if (currentWidthLimits.y > maxWidthLimits.y)
                {
                    newPos.x += (maxWidthLimits.y - currentWidthLimits.y);
                }
                myCam.transform.position = newPos;
            }
            else
            {
                Debug.LogError("6");

                Debug.LogError("Camera Position Cannot be Set to Cover Max Area In Width.. Please Increase Max Area.");
                myCam.orthographicSize = ((maxWidthLimits.y - maxWidthLimits.x) / myCam.aspect) / 2;
            }
        }
    }


    public Vector2 GetGameViewSize()
    {
        return new Vector2(Screen.safeArea.size.x, Screen.safeArea.size.y);
        //#if UNITY_EDITOR
        //        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        //        var getSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        //        var res = getSizeOfMainGameView.Invoke(null, null);
        //        return (Vector2)res;
        //#else
        //#endif
    }
    #endregion

    #region EVENT_HANDLERS
    #endregion

    #region COROUTINES
    #endregion

    #region UI_CALLBACKS
    #endregion
}
