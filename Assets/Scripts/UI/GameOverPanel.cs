using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    public GameOverEventChannel gameOverEventchannel;
    public TextMeshProUGUI gameOverTextMesh;
    public Button restartButton;

    private string[] tips = new string[] 
    {
        "You lose knowledge when your acolytes are sacrificed",
        "Try to group summons with your soldiers",
        "Enemy soldiers get stronger over time; Try to push aggressively"
    };

    private void Awake()
    {
        gameOverEventchannel.OnEventRaised += OnGameOver;
        restartButton.onClick.AddListener(RestartGame);
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        gameOverEventchannel.OnEventRaised -= OnGameOver;
        restartButton.onClick.RemoveAllListeners();
    }

    private void OnGameOver(bool isVictorious, GameModeType gameModeType, float clearTime)
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
        StringBuilder stringBuilder = new(); 
        if (isVictorious)
        {
            TimeSpan clearTimeSpan = TimeSpan.FromSeconds(clearTime);

            stringBuilder.AppendLine("Congratulations!");
            stringBuilder.AppendLine($"You beat Bloodrites on {gameModeType} difficulty");
            stringBuilder.AppendLine($"Your cleartime was {clearTimeSpan.ToString("mm\\:ss\\.ff")}");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Bloodrites was made for Ludum Dare 55th edition");
            stringBuilder.AppendLine("The theme was 'summoning'");
            stringBuilder.AppendLine("This game was created by:");
            stringBuilder.AppendLine("Ruud Cremers - Code");
            stringBuilder.AppendLine("Theo Cremers - Code");
            stringBuilder.AppendLine("Laurens de Vos - Art");
            stringBuilder.AppendLine();
            stringBuilder.Append("Thank you for playing, we hope you enjoyed it!");
        }
        else
        {
            stringBuilder.AppendLine("Game over");
            stringBuilder.AppendLine();
            stringBuilder.Append($"Tip: {GetRandomTip()}");
        }
        gameOverTextMesh.text = stringBuilder.ToString();
    }

    private string GetRandomTip()
    {
        return tips[UnityEngine.Random.Range(0, tips.Length)];
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
