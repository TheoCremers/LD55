using DG.Tweening;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameStartPanel : MonoBehaviour
{
    public GameStartEventChannel GameStartEvent;

    public Button easybutton;
    public Button normalbutton;
    public Button hardbutton;
    public Button veryhardbutton;
    public Button insanebutton;
    public Button demonicbutton;
    public Button impossiblebutton;
    public Button challengebutton;
   

    private CanvasGroup _canvasGroup;
    private float _fadeTime = 0.5f;

    void Awake()
    {
        veryhardbutton.gameObject.SetActive(false);
        insanebutton.gameObject.SetActive(false);
        demonicbutton.gameObject.SetActive(false);
        impossiblebutton.gameObject.SetActive(false);
        challengebutton.gameObject.SetActive(false);

        var gameData = DataPersistenceManager.instance.LoadGame();
        _canvasGroup = GetComponent<CanvasGroup>();
        easybutton?.onClick.AddListener(() => StartButtonClicked(GameModeType.Easy));
        normalbutton?.onClick.AddListener(() => StartButtonClicked(GameModeType.Normal));
        
        // beetje code duplicatie, ach ja
        if (gameData.HighestClearedMode >= GameModeType.Normal)
        {
            hardbutton.interactable = true;
            hardbutton.GetComponentInChildren<TextMeshProUGUI>().SetText("Hard");
            hardbutton.onClick.AddListener(() => StartButtonClicked(GameModeType.Hard));
            veryhardbutton.gameObject.SetActive(true);
        }
        if (gameData.HighestClearedMode >= GameModeType.Hard)
        {
            veryhardbutton.interactable = true;
            veryhardbutton.GetComponentInChildren<TextMeshProUGUI>().SetText("Very Hard");
            veryhardbutton.onClick.AddListener(() => StartButtonClicked(GameModeType.VeryHard));
            insanebutton.gameObject.SetActive(true);
        }
        if (gameData.HighestClearedMode >= GameModeType.VeryHard)
        {
            insanebutton.interactable = true;
            insanebutton.GetComponentInChildren<TextMeshProUGUI>().SetText("Insane");
            insanebutton.onClick.AddListener(() => StartButtonClicked(GameModeType.Insane));
            demonicbutton.gameObject.SetActive(true);
        }
        if (gameData.HighestClearedMode >= GameModeType.Insane)
        {
            demonicbutton.interactable = true;
            demonicbutton.GetComponentInChildren<TextMeshProUGUI>().SetText("Demonic");
            demonicbutton.onClick.AddListener(() => StartButtonClicked(GameModeType.Demonic));
            impossiblebutton.gameObject.SetActive(true);
        }
        if (gameData.HighestClearedMode >= GameModeType.Demonic)
        {
            impossiblebutton.interactable = true;
            impossiblebutton.GetComponentInChildren<TextMeshProUGUI>().SetText("Impossible");
            impossiblebutton.onClick.AddListener(() => StartButtonClicked(GameModeType.Impossible));
            challengebutton.gameObject.SetActive(true);
        }
        if (gameData.HighestClearedMode >= GameModeType.Impossible)
        {
            challengebutton.interactable = true;
            challengebutton.GetComponentInChildren<TextMeshProUGUI>().SetText("Challenge");
            challengebutton.onClick.AddListener(() => StartButtonClicked(GameModeType.Challenge));
        }

        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _ = FadeIn();
        Time.timeScale = 0f;
    }

    private void OnDestroy()
    {
        easybutton?.onClick.RemoveAllListeners();
        normalbutton?.onClick.RemoveAllListeners();
        hardbutton?.onClick.RemoveAllListeners();
    }

    private void StartButtonClicked(GameModeType type)
    {
        Time.timeScale = 1f;
        GameStartEvent.RaiseEvent(type);
        _ = FadeOut();
    }

    private async Task FadeOut()
    {
        DOTween.Kill(_canvasGroup);
        _canvasGroup.blocksRaycasts = false;
        await _canvasGroup.DOFade(0f, _fadeTime).SetUpdate(true).AsyncWaitForCompletion();
    }

    private async Task FadeIn()
    {
        DOTween.Kill(_canvasGroup);
        await _canvasGroup.DOFade(1f, _fadeTime).SetUpdate(true).AsyncWaitForCompletion();
        _canvasGroup.blocksRaycasts = true;
    }
}
