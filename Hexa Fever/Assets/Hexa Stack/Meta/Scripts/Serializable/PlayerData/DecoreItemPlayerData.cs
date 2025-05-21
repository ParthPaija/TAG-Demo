using System;

namespace Tag.MetaGame
{
    [Serializable]
    public class DecoreItemPlayerData : BaseDecoreItemPlayerData
    {
        public DecoreItemPlayerData(string taskId)
        {
            this.taskId = taskId;
            isUnlocked = false;
        }
    }
}