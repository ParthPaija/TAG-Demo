namespace Tag.HexaStack
{
    public enum SoundType
    {
        None = 0,

        // Music
        CoreBackgroundMusic = 2,
        MetaBackgroundMusic = 3,

        // SFX - UI
        ButtonClick = 21,
        RemoveItem = 22,
        Hammer = 23,
        ButtonClose = 24,
        CoinCollect,
        TileMove,
        TilePickup,
        TileSpawn,
        TileUnlock,
        WoodBreak,

        LevelFail = 40,
        LevelComplete = 41,

        AreaCompleteBackground = 101,
        AreaCompleteText = 102,
        StarCollectForTask = 103,

        Drill_Start,
        Ice_Block_Break,
        Mail_Collect,
        Mail_Box_Close,
        Propeller_Fly_Loop,
        Propeller_Fly_Start,
        Propeller_Hit,
    }
}