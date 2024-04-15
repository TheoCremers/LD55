
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    public static DataPersistenceManager instance { get; private set; }
    private static GameData _gameData;
    private FileDataHandler _dataHandler;

    private void Awake()
    {
        instance = this;
        _dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
    }


    public GameData NewGame()
    {
        _gameData = new GameData();
        return _gameData;
    }

    public GameData LoadGame()
    {
        //_gameData = _dataHandler.Load();
        if (_gameData == null)
        {
            return NewGame();
        }

        return _gameData;
    }

    public void SaveGame(GameData data)
    {
        if (_gameData.HighestClearedMode < data.HighestClearedMode)
        {
            _gameData = data;
            //_dataHandler.Save(data);
        }
    }
}
