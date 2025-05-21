using System;
using System.Collections.Generic;

namespace Tag.MetaGame
{
    [Serializable]
    public class DecoreItemWithOptionPlayerData : BaseDecoreItemPlayerData
    {
        public int selectedOption;
        public List<int> ownOptions;

        public DecoreItemWithOptionPlayerData(string taskId)
        {
            this.taskId = taskId;
            isUnlocked = false;
            selectedOption = 0;
            ownOptions = new List<int>();
        }

        public void OwnSelectedOption()
        {
            if (!ownOptions.Contains(selectedOption))
            {
                ownOptions.Add(selectedOption);
            }
        }
    }
}