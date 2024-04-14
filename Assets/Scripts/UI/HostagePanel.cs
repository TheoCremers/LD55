using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class HostagePanel : MonoBehaviour
{
    public HostageUsedEventChannel hostageUsedEventChannel;
    public VoidEventChannel hostageGainedEvent;

    public Button sacrificeButton;
    public Button recruitButton;
    public Button ransomButtom;
    public GameObject hostagePrefab;
    public RectTransform hostageParent;

    private CanvasGroup _canvasGroup;
    private List<Hostage> _hostageList;
    private float _fadeTime = 0.5f;

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _hostageList = GetComponentsInChildren<Hostage>().ToList();
        sacrificeButton?.onClick.AddListener(() => OnHostageUsed(HostageUseType.Sacrifice));
        recruitButton?.onClick.AddListener(() => OnHostageUsed(HostageUseType.Recruit));
        ransomButtom?.onClick.AddListener(() => OnHostageUsed(HostageUseType.Ransom));
        hostageGainedEvent.OnEventRaised += AddHostage;

        if (_hostageList.Count == 0)
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
        }
    }

    private void OnDestroy()
    {
        sacrificeButton?.onClick.RemoveAllListeners();
        recruitButton?.onClick.RemoveAllListeners();
        ransomButtom?.onClick.RemoveAllListeners();
    }

    public void AddHostage()
    {
        var newHostageObject = Instantiate(hostagePrefab);
        newHostageObject.transform.SetParent(hostageParent, false);
        var newHostage = newHostageObject.GetComponent<Hostage>();
        if (_hostageList.Count == 0) { _ = FadeIn(); }
        _hostageList.Add(newHostage);
    }

    private void OnHostageUsed(HostageUseType type)
    {
        var hostage = _hostageList.FirstOrDefault();
        if (hostage != null)
        {
            hostageUsedEventChannel.RaiseEvent(type, hostage.knowledge, hostage.lifeForce, hostage.ransomValue);
            _hostageList.Remove(hostage);
            Destroy(hostage.gameObject);
            if (_hostageList.Count == 0)
            {
                _ = FadeOut();
            }
        }
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
