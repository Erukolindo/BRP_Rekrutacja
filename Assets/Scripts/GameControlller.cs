using UnityEngine;
using TMPro;

public enum GameLocalization
{
    SWAMPS,
    DUNGEON,
    CASTLE,
    CITY,
    TOWER
}

public class GameControlller : MonoBehaviour
{
    #region Singleton

    private static GameControlller _instance;

    public static GameControlller Instance
    {
        get
        {
            if (_instance == null) _instance = FindFirstObjectByType<GameControlller>();
            return _instance;
        }
        set => _instance = value;
    }

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    [SerializeField] private GameLocalization currentGameLocalization;
    [SerializeField] private TMP_Text scoreText;
    private int score = 0;

    public GameLocalization CurrentGameLocalization
    {
        get => currentGameLocalization;

        set => currentGameLocalization = value;
    }

    private bool _isPaused;

    public bool IsPaused
    {

        get => _isPaused;
        set
        {
            _isPaused = value;
            Time.timeScale = _isPaused ? 0f : 1f;
        }
    }

    public bool IsCurrentLocalization(GameLocalization localization)
    {
        return CurrentGameLocalization == localization;
    }

    public void UpdateScore(int delta)
    {
        score += delta;
        if (scoreText) scoreText.text = score.ToString();
    }
}