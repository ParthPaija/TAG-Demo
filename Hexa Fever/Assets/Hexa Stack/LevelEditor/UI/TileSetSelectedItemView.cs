using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tag.HexaStack.Editor
{
    public class TileSetSelectedItemView : SerializedMonoBehaviour
    {
        #region PUBLIC_VARS

        #endregion

        #region PRIVATE_VARS

        [SerializeField] Transform parent;
        [SerializeField] GridGenerator gridGenerator;
        [SerializeField] GridGenerator gridGeneratorPrefab;

        #endregion

        #region UNITY_CALLBACKS

        #endregion

        #region PUBLIC_FUNCTIONS

        public void SetView(GameObject tileSet)
        {
            if (gridGenerator != null)
            {
                Destroy(gridGenerator.gameObject);
                gridGenerator = null;
            }

            gridGeneratorPrefab = tileSet.GetComponent<GridGenerator>();
            gridGenerator = Instantiate(tileSet, parent).GetComponent<GridGenerator>();
            gridGenerator.transform.localScale = new Vector3(50f, 50f, 50);
            gridGenerator.transform.localRotation = Quaternion.Euler(new Vector3(-90f, 90, -90));
            SetLayerForAllChildren(gridGenerator.gameObject,"UI");
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private void SetLayerForAllChildren(GameObject parent, string layerName)
        {
            // Set the layer for the parent GameObject
            parent.layer = LayerMask.NameToLayer(layerName);

            // Recursively set the layer for all child GameObjects
            foreach (Transform child in parent.transform)
            {
                SetLayerForAllChildren(child.gameObject, layerName);
            }
        }

        #endregion

        #region CO-ROUTINES

        #endregion

        #region EVENT_HANDLERS

        #endregion

        #region UI_CALLBACKS       

        public void OnClick()
        {
            LevelEditor.Instance.ChnageTileSet(gridGeneratorPrefab);
            LevelEditorUIManager.Instance.GetView<TileSetSelectedView>().Hide();
        }

        #endregion
    }
}
