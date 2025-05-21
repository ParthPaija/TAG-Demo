using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
namespace Tag.HexaStack
{
    public class GridRotator : MonoBehaviour
    {
        public bool isRotationActive = true;
        public float rotationSpeed = -1f;
        public float smoothSpeed = 17f;
        public Camera mainCamera;


        private float targetRotation = 0f;
        [ShowInInspector] private float currentRotation = 0f;
        private Vector3 lastMousePosition;
        private bool isRotating = false;
        private Vector3 centerPoint;
        private float lastRotationDelta = 0f;


        public SpriteRenderer blackBackground;
        public float smoothSpeedScale = 8f;
        public float scalePadding = 0.85f; // Padding inside black background

        [SerializeField] private Vector3 baseScale;
        [SerializeField] private List<Renderer> childRenderers;
        [SerializeField] private Bounds combinedBounds;

        private RaycastHit hit;

        public static bool RaycastBlock
        {
            get { return BaseView.blockView.Count > 0; }
        }

        public float CurrentRotation { get => currentRotation; set => currentRotation = value; }

        void Start()
        {
            mainCamera = Camera.main;
            centerPoint = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            currentRotation = transform.eulerAngles.y;
            targetRotation = currentRotation;

            baseScale = transform.localScale;

            List<BaseCell> cells = GetComponent<Level>().BaseCells;

            for (int i = 0; i < cells.Count; i++)
            {
                childRenderers.Add(cells[i].GetComponentInChildren<Renderer>());
            }

            blackBackground = GameplayManager.Instance?.bg;

            if (blackBackground == null)
            {
                Debug.LogWarning("Black background reference is missing!");
            }
            ItemStackSpawnerManager.Instance.ChanageScale(LevelManager.Instance.LoadedLevel.BaseCells[0].transform.lossyScale);
        }

        public void Init(float rotation)
        {
            currentRotation = rotation;
            targetRotation = currentRotation;
            transform.Rotate(0, rotation, 0);
        }

        void Update()
        {
            if (ItemMovementHelper.Instance == null)
                return;

            if (ItemMovementHelper.Instance.IsAnyThingPick || RaycastBlock)
                return;

            HandleRotation();
        }

        private void LateUpdate()
        {
            if (blackBackground == null) return;
            UpdateScale();
        }

        private void HandleRotation()
        {
            if (!isRotationActive)
                return;

            if (Input.GetMouseButtonDown(0) && GetRayHit(mainCamera.ScreenToWorldPoint(Input.mousePosition), out hit, LayerMask.GetMask("GridArea")))
            {
                InputManager.StopInteraction = true;
                isRotating = true;
                lastMousePosition = Input.mousePosition;
                lastRotationDelta = 0f;
            }

            if (Input.GetMouseButtonUp(0))
            {
                InputManager.StopInteraction = false;
                targetRotation = Mathf.Round(targetRotation / 60f) * 60f;
                transform.Rotate(0, targetRotation, 0);
                isRotating = false;
                ItemStackSpawnerManager.Instance.ChanageScale(LevelManager.Instance.LoadedLevel.BaseCells[0].transform.lossyScale);
            }

            if (isRotating)
            {
                float lastAngle = GetAngleFromCenter(lastMousePosition);
                float currentAngle = GetAngleFromCenter(Input.mousePosition);
                float rotationDelta = Mathf.DeltaAngle(lastAngle, currentAngle);

                if (Mathf.Abs(rotationDelta - lastRotationDelta) > 15f)
                {
                    rotationDelta = lastRotationDelta + Mathf.Sign(rotationDelta - lastRotationDelta) * 15f;
                }

                rotationDelta *= rotationSpeed * Time.deltaTime * 60f;
                targetRotation += rotationDelta;
                lastRotationDelta = rotationDelta;
                lastMousePosition = Input.mousePosition;
            }

            currentRotation = Mathf.Lerp(currentRotation, targetRotation, Time.deltaTime * smoothSpeed);
            transform.localRotation = Quaternion.Euler(0f, currentRotation, 0f);
        }

        private void UpdateScale()
        {
            if (childRenderers == null || childRenderers.Count == 0) return;

            // Get combined bounds of all hex pieces
            combinedBounds = new Bounds(transform.position, Vector3.zero);
            foreach (Renderer renderer in childRenderers)
            {
                if (renderer != null && renderer.enabled)
                {
                    combinedBounds.Encapsulate(renderer.bounds);
                }
            }

            if (blackBackground == null) return;

            // Get screen space dimensions of the background
            float backgroundWorldHeight = blackBackground.transform.localScale.y;
            float backgroundWorldWidth = blackBackground.transform.localScale.x;

            // Get the grid's dimensions in world space
            float gridWorldWidth = combinedBounds.size.x;
            float gridWorldHeight = combinedBounds.size.y;  // Using z instead of y for top-down view

            // Calculate scale factors
            float widthScale = (backgroundWorldWidth / gridWorldWidth) * scalePadding;
            float heightScale = (backgroundWorldHeight / gridWorldHeight) * scalePadding;

            // Use the smaller scale to ensure grid fits within background
            float targetScale = Mathf.Min(widthScale, heightScale);

            // Apply smooth scaling
            Vector3 currentScale = transform.localScale;
            float newScale = Mathf.Lerp(currentScale.x, targetScale * baseScale.x, Time.deltaTime * smoothSpeedScale);

            ItemStackSpawnerManager.Instance.ChanageScale(LevelManager.Instance.LoadedLevel.BaseCells[0].transform.lossyScale);
            // Safety check for valid scale values
            if (!float.IsInfinity(newScale) && !float.IsNaN(newScale) && newScale > 0)
            {
                transform.localScale = new Vector3(newScale, newScale, newScale);
            }
        }

        private bool GetRayHit(Vector3 pos, out RaycastHit hit, LayerMask layerMask)
        {
            return Physics.Raycast(pos, InputManager.eventTranform.forward, out hit, Mathf.Infinity, layerMask);
        }

        private float GetAngleFromCenter(Vector3 mousePos)
        {
            Vector2 direction = mousePos - centerPoint;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;
            return angle;
        }
    }
}