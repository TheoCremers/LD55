using UnityEngine;
using System.Collections.Generic;

public class GameAssets : MonoBehaviour
{
    // [SerializeField] public List<SFXMetadata> SFXMetadata = new List<SFXMetadata>();
    //
    // [SerializeField] public List<BGMMetadata> BGMMetadata = new List<BGMMetadata>();

    public List<GameModeSettings> GameModeSettingsList = new List<GameModeSettings>();

    public Transform DamagePopup; 

    private static GameAssets _instance;

    public static GameAssets Instance 
    {
        get 
        {
            if (_instance == null) 
            {
                _instance = Instantiate(Resources.Load<GameAssets>("GameAssets"));
            }
            return _instance;
        }
    }    

    void Awake ()
    {
        _instance = this;
    }  

    // Wrapper that allows object persistance from static classes
    public void Persist(GameObject gameObject)
    {
        DontDestroyOnLoad(gameObject);
    }
}