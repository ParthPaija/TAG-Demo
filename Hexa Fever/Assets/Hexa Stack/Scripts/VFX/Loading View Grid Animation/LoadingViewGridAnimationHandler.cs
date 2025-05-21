using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hexa_Stack
{
    public class LoadingViewGridAnimationHandler : MonoBehaviour
    {
        #region PUBLIC_VARS

        public List<LoadingViewGridCell> cells;

        public float moveDuration = 0.5f; // Speed of animation
        public float delayBetweenStackMove = 0.3f; // Delay between each stack animation
        public float jumpHeight = 1f;
        public float delayBetweenTileMove = 0.2f; // Delay between each stack animation
        #endregion

        #region PRIVATE_VARS
        private bool isAnimating = false;
        private LoadingViewGridStack recentStack = null;
        #endregion

        #region UNITY_CALLBACKS

        private void Awake()
        {
            SetInitialData();
        }

        #endregion

        #region PUBLIC_FUNCTIONS

        public void PlayAnimation()
        {
            ResetData();
            SetCellData();
            StartCoroutine(PlayGridIdleAnimation());
        }

        public void ResetData()
        {
            for (int i = 0; i < cells.Count; i++)
            {
                cells[i].ResetData();
            }
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetInitialData()
        {
            for(int i = 0;i < cells.Count;i++)
            {
                cells[i].SetInitialData();
            }
        }

        private void SetCellData()
        {
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].stack != null)
                    cells[i].IsOccupied = true;
                else
                    cells[i].IsOccupied = false;
            }
        }

        private LoadingViewGridCell GetRandomCell()
        {
            return cells[Random.Range(0, cells.Count)];
        }

        public bool AlmostEqual(Vector3 v1, Vector3 v2, float tolerance)
        {
            Debug.Log(v1 + "---" + v2 + "---" + tolerance);
            return Mathf.Abs(Vector3.Distance(v1, v2)) <= tolerance;
        }

        private Vector3 IsDirectionMatch(Vector3 direction)
        {
            Vector3 FromCenterToBottom = new Vector3(0.00f, 0.00f, -1.00f);
            Vector3 FromCenterToTop = new Vector3(0.00f, 0.00f, 1.00f);
            Vector3 FromCenterToTopRight = new Vector3(0.86f, 0.00f, 0.50f);
            Vector3 FromCenterToBottomRight = new Vector3(0.86f, 0.00f, -0.50f);
            Vector3 FromCenterToTopLeft = new Vector3(-0.86f, 0.00f, 0.50f);
            Vector3 FromCenterToBottomLeft = new Vector3(-0.86f, 0.00f, -0.50f);

            if (AlmostEqual(direction, FromCenterToBottom, 0.01f))
                return new Vector3(-180, 0, 0);
            else if (AlmostEqual(direction, FromCenterToTop, 0.01f))
                return new Vector3(180, 0, 0);
            else if (AlmostEqual(direction, FromCenterToTopRight, 0.01f))
                return new Vector3(0, 0, 0);
            else if (AlmostEqual(direction, FromCenterToBottomRight, 0.01f))
                return new Vector3(0, 0, 0);
            else if (AlmostEqual(direction, FromCenterToTopLeft, 0.01f))
                return new Vector3(0, 0, 0);
            else if (AlmostEqual(direction, FromCenterToBottomLeft, 0.01f))
                return new Vector3(0, 0, 0);

            return new Vector3(0, 0, 0);
        }

        #endregion

        #region CO-ROUTINES

        private IEnumerator PlayGridIdleAnimation()
        {
            isAnimating = true;

            LoadingViewGridCell tempCell;
            LoadingViewGridCell nearCell;

            while (true) // Continuous loop
            {

                tempCell = GetRandomCell();
                if (tempCell.stack != null)
                {
                    nearCell = tempCell.nearCells[Random.Range(0, tempCell.nearCells.Count - 1)];
                    if (!nearCell.IsOccupied) // Check if empty adjacent cell available
                    {
                        if (recentStack == null || recentStack != tempCell.stack)
                        {
                            yield return StartCoroutine(MoveStackToCell(tempCell, nearCell));
                            yield return new WaitForSeconds(delayBetweenStackMove); // Wait before checking next move
                        }
                    }
                }
            }
        }

        private IEnumerator MoveStackToCell(LoadingViewGridCell fromCell, LoadingViewGridCell toCell)
        {
            LoadingViewGridStack stack = fromCell.stack;
            recentStack = stack;

            if (stack == null || stack.tiles.Count == 0)
                yield break; // No tiles to move

            List<LoadingViewGridTile> tiles = new List<LoadingViewGridTile>(stack.tiles); // Copy tiles for order

            stack.transform.position = toCell.transform.position;
            for (int j = 0; j < tiles.Count; j++)
            {
                tiles[j].transform.position = fromCell.transform.position;
                tiles[j].transform.localPosition += new Vector3(0, (j + 1) * 1.8f, 0); ;
            }

            // Move each tile from top to bottom
            for (int i = tiles.Count - 1; i >= 0; i--)
            {
                StartCoroutine(DoTileMoveAnimation(tiles[i], fromCell, toCell, i, tiles.Count));

                yield return new WaitForSeconds(delayBetweenTileMove); // Small delay per tile
            }

            // ✅ Update cell occupation
            fromCell.stack = null;
            fromCell.IsOccupied = false;

            toCell.stack = stack;
            toCell.IsOccupied = true;
        }

        IEnumerator DoTileMoveAnimation(LoadingViewGridTile tile, LoadingViewGridCell fromCell, LoadingViewGridCell toCell, int currentTileCount, int totalTileCount)
        {

            //Vector3 distance = toCell.transform.localPosition - fromCell.transform.localPosition;
            //distance = Vector3.Normalize(distance);

            Vector3 direction = (toCell.transform.localPosition - fromCell.transform.localPosition).normalized;

            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            if (angle > 0 && angle <= 60)
                angle = 120;
            else if (angle > 60 && angle <= 120)
                angle = 180f;
            else if (angle > 120 && angle <= 180)
                angle = 240f;
            else if (angle > 180 && angle <= 240)
                angle = 300;
            else if (angle > 240 && angle <= 360)
                angle = 360;
            else if (angle > -60 && angle <= 0)
                angle = -60f;
            else
                angle = -120f;
            //else
            //    angle = 0;
            //angle -= 60f;

            float zRotation = /*direction.x > 0 ? -180 : */180;
            tile.transform.localEulerAngles = new Vector3(0, angle, 0);
            //Debug.Log(IsDirectionMatch(distance));
            Vector3 startRotation = new Vector3(0, angle, 0);
            Vector3 endRotation = new Vector3(0, angle, zRotation);

            float i = 0;
            float rate = 1 / moveDuration;
            Vector3 midPoint, mid1, mid2;
            while (i < 1)
            {
                i += Time.deltaTime * rate;

                midPoint = Vector3.Lerp(fromCell.transform.position + new Vector3(0, (currentTileCount + 1) * 0.35f, 0), toCell.transform.position, 0.75f);
                midPoint += new Vector3(0, jumpHeight, 0);
                mid1 = Vector3.Lerp(fromCell.transform.position + new Vector3(0, (currentTileCount + 1) * 0.35f, 0), midPoint, i);
                mid2 = Vector3.Lerp(midPoint, toCell.transform.position, i);

                tile.transform.position = Vector3.Lerp(mid1, mid2, i);
                tile.transform.localPosition += new Vector3(0, (totalTileCount - currentTileCount) * 1.8f, 0);
                tile.transform.localEulerAngles = Vector3.Lerp(startRotation, endRotation, i);
                yield return null;
            }
            tile.transform.localRotation = Quaternion.identity;
            tile.transform.position = toCell.transform.position;
            tile.transform.localPosition = new Vector3(0, (totalTileCount - currentTileCount) * 1.8f, 0);

        }

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        #endregion
    }
}
