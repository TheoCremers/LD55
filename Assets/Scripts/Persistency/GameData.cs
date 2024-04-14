
[System.Serializable]
public class GameData
{
    public GameModeType HighestClearedMode;

    public GameData()
    {
        HighestClearedMode = GameModeType.Easy;
    }
}
