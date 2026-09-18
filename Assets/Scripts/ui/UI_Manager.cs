using UnityEngine;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    public TextMeshProUGUI _HealthText;
    public TextMeshProUGUI _WaveText;
    public TextMeshProUGUI _GameOverWaveText;

    public HealthManager _PlayerHealth;
    public EnemySpawnerScript _EnemySpawnerScript;
    public GameObject _BossBar;
    bool enableBossBar;

    public bool _GameOver;
    PlayerShoot player;

    private void Start()
    {
        Time.timeScale = 1;
        player = FindAnyObjectByType<PlayerShoot>();
    }

    void Update()
    {
        _HealthText.text = _PlayerHealth._CurrentHealth.ToString();
        _WaveText.text = _EnemySpawnerScript.wavecount.ToString();
        _GameOverWaveText.text = "Wave: " + _EnemySpawnerScript.wavecount.ToString();

        if (enableBossBar)
        {
            _BossBar.SetActive(true);
        }
        else
        {
            _BossBar.SetActive(false);
        }
    }
    public  void ToggleBossBar()
    {
        enableBossBar = !enableBossBar;
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        player.enabled = false;
        _GameOver = true;
}
}
