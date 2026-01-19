using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance;

    [Header("Progression Data")] // --- NEW ---
    public int playerLevel = 1;
    public float currentXP = 0;
    public float xpToNextLevel = 500; // Difficulty curve
    public float difficultyMultiplier = 1.2f; // 20% harder each level

    /* ───────── MATCH RESULT ───────── */
    public bool lastMatchWon = false;

    /* ───────── MATCH STATS ───────── */
    public float totalDamageDealt;
    public float totalEnergySpent;
    public float matchDuration;

    private void Awake()
    {
        // Singleton Pattern: Ensure only one brain exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // --- NEW FUNCTION: Call this when match ends ---
    public void GrantXP(float amount)
    {
        currentXP += amount;
        Debug.Log($"Gained {amount} XP! Total: {currentXP}/{xpToNextLevel}");

        // Level Up Logic
        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentXP -= xpToNextLevel;
        playerLevel++;
        xpToNextLevel *= difficultyMultiplier; // Increase difficulty
        
        Debug.Log("LEVEL UP! New Level: " + playerLevel);
        
        // Optional: Play Level Up Sound here if AudioManager exists
        if(AudioManager.Instance) AudioManager.Instance.PlayVictory(); 
    }
    // ----------------------------------------------

    /* ───────── RESET BEFORE MATCH ───────── */
    public void ResetMatchStats()
    {
        totalDamageDealt = 0f;
        totalEnergySpent = 0f;
        matchDuration = 0f;
    }

    /* ───────── TRACKERS ───────── */
    public void AddDamage(float amount)
    {
        totalDamageDealt += amount;
    }

    public void AddEnergySpent(float amount)
    {
        totalEnergySpent += amount;
    }

    public void UpdateMatchTime(float delta)
    {
        matchDuration += delta;
    }

    /* ───────── SCENE LOADING ───────── */
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        Time.timeScale = 1f;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
            yield return null;
    }
}