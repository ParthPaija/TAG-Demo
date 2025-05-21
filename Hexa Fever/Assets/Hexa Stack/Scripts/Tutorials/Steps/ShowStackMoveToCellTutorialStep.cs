using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Tag.HexaStack
{
    public class ShowStackMoveToCellTutorialStep : BaseTutorialStep
    {
        #region PUBLIC_VARS

        public UnityEvent OnTapAction;
        public UnityEvent OnStartStep;
        public UnityEvent OnEndStep;
        public TutorialHandAnimation tutorialHandAnimation;

        #endregion

        #region PRIVATE_VARS

        [SerializeField] private int itemSpawnerId;
        [SerializeField] private int cellId;
        [SerializeField] private Vector3 startPoint;
        [SerializeField] private Vector3 endPoint;
        [SerializeField] private Vector3 handOffset;

        [Header("Line Setting"), Space(30)]

        public CameraCacheType cameraCacheType;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private float curveHeight = 2.0f; // Height of the curve
        [SerializeField] private int curveResolution = 50; // Number of points for the curve
        [SerializeField] private float dashAnimationSpeed = 1.0f; // Speed of the dashes moving animation
        private Material lineMaterial;
        private float textureOffset = 0f;

        #endregion

        #region UNITY_CALLBACKS

        private void Start()
        {
            lineRenderer.enabled = false;
        }

        void Update()
        {
            if (!lineRenderer.enabled)
                return;

            Vector3[] curvePoints = GenerateCurvePoints(startPoint, endPoint);
            lineRenderer.SetPositions(curvePoints);

            textureOffset -= Time.deltaTime * dashAnimationSpeed;
            lineMaterial.mainTextureOffset = new Vector2(textureOffset, 0);
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        [Button]
        public override void OnStartStep1()
        {
            base.OnStartStep1();
            OnStartStep?.Invoke();

            ItemMovement.onItemPick += OnTap;
            ItemMovement.onItemDrop += OnDrop;

            if (ItemStackSpawnerManager.Instance.GetSpawnerByIendex(itemSpawnerId).ItemStack == null)
            {
                EndStep();
                return;
            }
            if (LevelManager.Instance.LoadedLevel.GetCellById(cellId).HasItem)
            {
                EndStep();
                return;
            }
            LevelManager.Instance.LoadedLevel.GridRotator.isRotationActive = false;
            startPoint = ItemStackSpawnerManager.Instance.GetSpawnerByIendex(itemSpawnerId).transform.position;
            endPoint = LevelManager.Instance.LoadedLevel.GetCellById(cellId).transform.position;

            CameraCache.TryFetchCamera(cameraCacheType, out Camera mainCamera);
            Vector3 screenPosStart = mainCamera.WorldToViewportPoint(ItemStackSpawnerManager.Instance.GetSpawnerByIendex(itemSpawnerId).transform.position);
            Vector3 screenPosEnd = mainCamera.WorldToViewportPoint(LevelManager.Instance.LoadedLevel.GetCellById(cellId).transform.position);

            lineRenderer.enabled = true;
            DrawLine();

            Debug.LogError("Start Step");

            tutorialHandAnimation.gameObject.SetActive(true);
            tutorialHandAnimation.startPosition.position = startPoint + handOffset;
            tutorialHandAnimation.endPosition.position = endPoint + handOffset;
            tutorialHandAnimation.PlayHandMove(true);
        }

        public override void EndStep()
        {
            OnEndStep?.Invoke();
            LevelManager.Instance.LoadedLevel.GridRotator.isRotationActive = true;
            ItemMovement.onItemPick -= OnTap;
            ItemMovement.onItemDrop -= OnDrop;

            lineRenderer.enabled = false;
            base.EndStep();

            Debug.LogError("End Step");
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void OnTap()
        {
            OnTapAction?.Invoke();
            tutorialHandAnimation.gameObject.SetActive(false);
            tutorialHandAnimation.endPosition.position = endPoint + handOffset;
            tutorialHandAnimation.startPosition.position = Vector3.zero;
        }

        private void OnDrop()
        {
            EndStep();
        }

        [Button]
        private void DrawLine()
        {
            lineMaterial = new Material(Shader.Find("Sprites/Default"));
            lineMaterial.mainTexture = CreateDashedTexture();
            lineMaterial.mainTexture.wrapMode = TextureWrapMode.Repeat;
            lineRenderer.material = lineMaterial;

            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.positionCount = curveResolution + 1;
            lineRenderer.textureMode = LineTextureMode.Tile;

            float lineLength = Vector3.Distance(startPoint, endPoint);
            lineMaterial.mainTextureScale = new Vector2(lineLength, 1);
        }

        Vector3[] GenerateCurvePoints(Vector3 start, Vector3 end)
        {
            Vector3[] points = new Vector3[curveResolution + 1];
            float distance = Vector3.Distance(start, end);

            Vector3 midPoint = (start + end) / 2;
            Vector3 controlPoint = midPoint + new Vector3(0, curveHeight * distance / 10, 0);

            for (int i = 0; i <= curveResolution; i++)
            {
                float t = i / (float)curveResolution;
                points[i] = CalculateBezierPoint(t, start, controlPoint, end);
            }
            return points;
        }

        Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            return (1 - t) * (1 - t) * p0 + 2 * (1 - t) * t * p1 + t * t * p2;
        }

        Texture2D CreateDashedTexture()
        {
            int textureWidth = 64;
            int textureHeight = 1;
            Texture2D texture = new Texture2D(textureWidth, textureHeight);
            for (int x = 0; x < textureWidth; x++)
            {
                Color color = (x % 16 < 8) ? Color.white : Color.clear; // Dashed pattern
                texture.SetPixel(x, 0, color);
            }
            texture.Apply();
            return texture;
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
