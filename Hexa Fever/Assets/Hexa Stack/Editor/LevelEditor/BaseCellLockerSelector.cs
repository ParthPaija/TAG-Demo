using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Tag.HexaStack.Editor
{
    public abstract class BaseCellLockerSelector
    {
        public BaseCellLockerSelector()
        {

        }

        public virtual void SelecteInCell(BaseCell baseCell)
        {
        }
    }

    [LabelText("Ad Cell Locker")]
    [HideReferenceObjectPicker]
    public class AdCellLockerSelector : BaseCellLockerSelector
    {
        string path = "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/AdCellLocker.prefab";

        public AdCellLockerSelector()
        {

        }

        public override void SelecteInCell(BaseCell baseCell)
        {
            FreeCellUnlocker freeCellUnlocker = (FreeCellUnlocker)AssetDatabase.LoadAssetAtPath<BaseCellUnlocker>(path);

            FreeCellUnlocker temp = (FreeCellUnlocker)PrefabUtility.InstantiatePrefab(freeCellUnlocker, baseCell.transform);
            baseCell.BaseCellUnlocker = temp;
        }
    }

    [LabelText("Grass Cell Locker")]
    [HideReferenceObjectPicker]
    public class GrassCellLockerSelector : BaseCellLockerSelector
    {
        string path = "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/GrassCellUnlocker.prefab";

        public override void SelecteInCell(BaseCell baseCell)
        {
            GrassCellUnlocker grassCellUnlocker = (GrassCellUnlocker)AssetDatabase.LoadAssetAtPath<BaseCellUnlocker>(path);

            GrassCellUnlocker temp = (GrassCellUnlocker)PrefabUtility.InstantiatePrefab(grassCellUnlocker, baseCell.transform);
            baseCell.BaseCellUnlocker = temp;
        }
    }

    [LabelText("Point Cell Locker")]
    [HideReferenceObjectPicker]
    public class PointCellLockerSelector : BaseCellLockerSelector
    {
        string path = "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/GoalPointCellLocker.prefab";
        public BaseLevelGoal baseLevelGoal;

        public PointCellLockerSelector()
        {
            baseLevelGoal = new AllItemCollectGoal();
        }

        public override void SelecteInCell(BaseCell baseCell)
        {
            GoalPointCellUnlocker goalPointCellUnlocker = (GoalPointCellUnlocker)AssetDatabase.LoadAssetAtPath<BaseCellUnlocker>(path);

            GameObject temp = GameObject.Instantiate(goalPointCellUnlocker.gameObject, baseCell.transform);
            GoalPointCellUnlocker goalPointCellUnlocker1 = temp.GetComponent<GoalPointCellUnlocker>();
            goalPointCellUnlocker1.LevelGoal = baseLevelGoal;
            goalPointCellUnlocker1.SetText();
            baseCell.BaseCellUnlocker = goalPointCellUnlocker1;
            baseLevelGoal = new AllItemCollectGoal();
        }
    }

    [LabelText("Specific Item Point Locker")]
    [HideReferenceObjectPicker]
    public class SpecificItemPointCellLockerSelector : BaseCellLockerSelector
    {
        string path = "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/ItemGoalPointCellUnlocker.prefab";
        public BaseLevelGoal baseLevelGoal;

        public SpecificItemPointCellLockerSelector()
        {
            baseLevelGoal = new
                SpecificItemCollectGoal()
            { itemId = 1 };
        }

        public override void SelecteInCell(BaseCell baseCell)
        {
            ItemGoalPointCellUnlocker goalPointCellUnlocker = (ItemGoalPointCellUnlocker)AssetDatabase.LoadAssetAtPath<BaseCellUnlocker>(path);

            GameObject temp = GameObject.Instantiate(goalPointCellUnlocker.gameObject, baseCell.transform);
            ItemGoalPointCellUnlocker goalPointCellUnlocker1 = temp.GetComponent<ItemGoalPointCellUnlocker>();
            goalPointCellUnlocker1.LevelGoal = baseLevelGoal;
            goalPointCellUnlocker1.SetText();
            goalPointCellUnlocker1.SetLoakImage();
            baseCell.BaseCellUnlocker = goalPointCellUnlocker1;
            baseLevelGoal = new SpecificItemCollectGoal();
        }
    }

    [LabelText("Bread Toaster Locker")]
    [HideReferenceObjectPicker]
    public class BreadToasterLockerSelector : BaseCellLockerSelector
    {
        string path = "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/Mail Box.prefab";

        public override void SelecteInCell(BaseCell baseCell)
        {
            BreadToasterUnlocker breadToasterLockerSelector = (BreadToasterUnlocker)AssetDatabase.LoadAssetAtPath<BaseCellUnlocker>(path);

            BreadToasterUnlocker temp = (BreadToasterUnlocker)PrefabUtility.InstantiatePrefab(breadToasterLockerSelector, baseCell.transform);
            baseCell.BaseCellUnlocker = temp;
        }
    }

    [LabelText("Ice Cell Locker")]
    [HideReferenceObjectPicker]
    public class IceLockerSelector : BaseCellLockerSelector
    {
        string path = "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/IceCellUnlocker.prefab";

        public override void SelecteInCell(BaseCell baseCell)
        {
            if (baseCell.CellDefaultDataSO == null)
            {
                Debug.LogError("First Assign Default Data");
                return;
            }

            IceCellUnlocker iceLocker = (IceCellUnlocker)AssetDatabase.LoadAssetAtPath<BaseCellUnlocker>(path);

            IceCellUnlocker temp = (IceCellUnlocker)PrefabUtility.InstantiatePrefab(iceLocker, baseCell.transform);
            baseCell.BaseCellUnlocker = temp;
        }
    }

    [LabelText("Propeller Cell Locker")]
    [HideReferenceObjectPicker]
    public class PropellerLockerSelector : BaseCellLockerSelector
    {
        [PropertyRange(1, 3)]
        public int propellerCount = 1;

        public override void SelecteInCell(BaseCell baseCell)
        {
            string path = "";

            switch (propellerCount)
            {
                case 1:
                    path = "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/Propeller Cell Unlocker - 1.prefab";
                    break;
                case 2:
                    path = "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/Propeller Cell Unlocker - 2.prefab";
                    break;
                case 3:
                    path = "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/Propeller Cell Unlocker - 3.prefab";
                    break;
                default:
                    path = "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/Propeller Cell Unlocker - 1.prefab";
                    break;
            }

            PropellerCellUnlocker propellerCellUnlocker = (PropellerCellUnlocker)AssetDatabase.LoadAssetAtPath<BaseCellUnlocker>(path);

            PropellerCellUnlocker temp = (PropellerCellUnlocker)PrefabUtility.InstantiatePrefab(propellerCellUnlocker, baseCell.transform);
            baseCell.BaseCellUnlocker = temp;
        }
    }

    [LabelText("Wood Cell Locker")]
    [HideReferenceObjectPicker]
    public class WoodCellLockerSelector : BaseCellLockerSelector
    {
        [PropertyRange(1, 3)]
        public int woodCount = 1;

        public override void SelecteInCell(BaseCell baseCell)
        {
            string path = "";

            switch (woodCount)
            {
                case 1:
                    path = "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/Wood/Wood-1/WoodCellStackCellLocker - 1.prefab";
                    break;
                case 2:
                    path = "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/Wood/Wood-2/WoodCellStackCellLocker - 2.prefab";
                    break;
                case 3:
                    path = "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/Wood/Wood-3/WoodCellStackCellLocker - 3.prefab";
                    break;
                default:
                    path = "Assets/Hexa Stack/Prefabs/Gameplay/CellUnlocker/Wood/Wood-1/WoodCellStackCellLocker - 1.prefab";
                    break;
            }

            AdjacentCellStackCellUnlocker goalPointCellUnlocker = (AdjacentCellStackCellUnlocker)AssetDatabase.LoadAssetAtPath<BaseCellUnlocker>(path);

            AdjacentCellStackCellUnlocker temp = (AdjacentCellStackCellUnlocker)PrefabUtility.InstantiatePrefab(goalPointCellUnlocker, baseCell.transform);
            baseCell.BaseCellUnlocker = temp;
        }
    }
}