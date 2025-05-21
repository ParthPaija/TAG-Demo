using System;

namespace Tag.MetaGame
{
    public static class AreaUtility
    {
        public static string AreaNoToAreaId(int areaNo)
        {
            return $"A{areaNo}";
        }
        public static int AreaIdToAreaNo(string areaId)
        {
            return Int32.Parse(areaId.Remove(0, 1));
        }
        public static string GetNextAreaId(string areaId)
        {
            return AreaNoToAreaId(AreaIdToAreaNo(areaId) + 1);
        }
    }
}
