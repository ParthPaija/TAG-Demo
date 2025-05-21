using System;

namespace Tag.MetaGame
{
    [Serializable]
    public class AreaPlayerData
    {
        public AreaState areaState;
        public string data;

        public AreaPlayerData(string data)
        {
            areaState = AreaState.Running;
            this.data = data;
        }
    }
}
