using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStartPanel : MonoBehaviour
{
    public GameStartEventChannel GameStartEvent;


    public Button easybutton;
    public Button normalbutton;
    public Button hardbutton;

    private CanvasGroup _canvasGroup;
    private float _fadeTime = 0.5f;

    void Awake()
    {
        //var data = DataPersistenceManager.instance.LoadGame();
        _canvasGroup = GetComponent<CanvasGroup>();
        easybutton?.onClick.AddListener(() => StartButtonClicked(GameModeType.Easy));
        normalbutton?.onClick.AddListener(() => StartButtonClicked(GameModeType.Normal));
        //if (data.HighestClearedMode >= GameModeType.Normal)
        //{
            hardbutton.interactable = true;
            hardbutton.GetComponentInChildren<TextMeshProUGUI>().SetText("Hard");
            hardbutton.onClick.AddListener(() => StartButtonClicked(GameModeType.Hard));
        //}
        // TODO: Add harder levels

        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _ = FadeIn();
    }

    private void OnDestroy()
    {
        easybutton?.onClick.RemoveAllListeners();
        normalbutton?.onClick.RemoveAllListeners();
        hardbutton?.onClick.RemoveAllListeners();
    }

    private void StartButtonClicked(GameModeType type)
    {
        GameStartEvent.RaiseEvent(type);
        _ = FadeOut();
    }

    private async Task FadeOut()
    {
        DOTween.Kill(_canvasGroup);
        _canvasGroup.blocksRaycasts = false;
        await _canvasGroup.DOFade(0f, _fadeTime).AsyncWaitForCompletion();
    }

    private async Task FadeIn()
    {
        DOTween.Kill(_canvasGroup);
        await _canvasGroup.DOFade(1f, _fadeTime).AsyncWaitForCompletion();
        _canvasGroup.blocksRaycasts = true;
    }
}
