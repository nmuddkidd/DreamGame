using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalGameManager : MonoBehaviour
{
    public static SurvivalGameManager Instance { get; private set; }

    [Header("Survival Timer")]
    [SerializeField] private float surviveDurationSeconds = 300f;
    [SerializeField] private Text clockText;

    [Header("Lose Conditions")]
    [SerializeField] private float customerFailSeconds = 30f;
    [SerializeField] private float customerWarnSeconds = 15f;
    [SerializeField] private float trashFailSeconds = 60f;
    [SerializeField] private float trashWarnSeconds = 30f;

    [Header("Warning UI")]
    [SerializeField] private Text warningText;

    [Header("Result UI")]
    [SerializeField] private Text resultText;

    [Header("Monster Trigger")]
    [SerializeField] private GameObject activeMonsterOnLose;
    [SerializeField] private GameObject monsterPrefabOnLose;
    [SerializeField] private Transform monsterSpawnPoint;

    private readonly Dictionary<NPC_Movement, float> waitingCustomers = new Dictionary<NPC_Movement, float>();
    private readonly Dictionary<int, float> activeTrashTasks = new Dictionary<int, float>();

    private int nextTrashTaskId = 1;
    private float elapsedTime;
    private bool runEnded;
    private Transform playerTransform;
    private MonsterAnimator monsterAnimator;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        CachePlayerAndMonster();

        UpdateClockText();
        SetWarning(string.Empty);
        SetResult(string.Empty);
    }

    void Update()
    {
        if (runEnded)
        {
            return;
        }

        elapsedTime += Time.deltaTime;
        UpdateClockText();

        CleanupMissingCustomers();

        float now = Time.time;
        float oldestCustomerWait = GetOldestDuration(waitingCustomers, now);
        float oldestTrashWait = GetOldestDuration(activeTrashTasks, now);

        if (oldestCustomerWait >= customerFailSeconds)
        {
            TriggerLoss("You let a customer wait too long.");
            return;
        }

        if (oldestTrashWait >= trashFailSeconds)
        {
            TriggerLoss("Trash was left out too long.");
            return;
        }

        if (elapsedTime >= surviveDurationSeconds)
        {
            TriggerWin();
            return;
        }

        UpdateWarnings(oldestCustomerWait, oldestTrashWait);
    }

    public void OnCustomerWaitStarted(NPC_Movement npc)
    {
        if (npc == null || runEnded)
        {
            return;
        }

        waitingCustomers[npc] = Time.time;
    }

    public void OnCustomerWaitEnded(NPC_Movement npc)
    {
        if (npc == null)
        {
            return;
        }

        waitingCustomers.Remove(npc);
    }

    public int RegisterTrashDrop()
    {
        if (runEnded)
        {
            return -1;
        }

        int taskId = nextTrashTaskId++;
        activeTrashTasks[taskId] = Time.time;
        return taskId;
    }

    public void CompleteTrashTask(int taskId)
    {
        if (taskId < 0)
        {
            return;
        }

        activeTrashTasks.Remove(taskId);
    }

    private void TriggerLoss(string reason)
    {
        runEnded = true;
        SetWarning(string.Empty);
        SetResult("Try to run until 6:00 AM.. You lose.. " + reason);

        ActivateMonsterChase();
    }

    private void ActivateMonsterChase()
    {
        CachePlayerAndMonster();

        if (monsterAnimator != null && playerTransform != null)
        {
            monsterAnimator.BeginChase(playerTransform);
            return;
        }

        if (monsterPrefabOnLose != null)
        {
            Vector3 spawnPosition = monsterSpawnPoint != null ? monsterSpawnPoint.position : Vector3.zero;
            Quaternion spawnRotation = monsterSpawnPoint != null ? monsterSpawnPoint.rotation : Quaternion.identity;
            GameObject monsterInstance = Instantiate(monsterPrefabOnLose, spawnPosition, spawnRotation);
            MonsterAnimator spawnedMonster = monsterInstance.GetComponent<MonsterAnimator>();
            if (spawnedMonster != null && playerTransform != null)
            {
                spawnedMonster.BeginChase(playerTransform);
            }
        }
    }

    private void TriggerWin()
    {
        runEnded = true;
        SetWarning(string.Empty);
        SetResult("6:00 AM - You survived.");
    }

    private void UpdateWarnings(float oldestCustomerWait, float oldestTrashWait)
    {
        string warning = string.Empty;

        if (oldestCustomerWait >= customerWarnSeconds)
        {
            warning = "customer has been waiting for a while....";
        }

        if (oldestTrashWait >= trashWarnSeconds)
        {
            if (!string.IsNullOrEmpty(warning))
            {
                warning += "\n";
            }
            warning += "trash hasnt been taken out in a while....";
        }

        SetWarning(warning);
    }

    private void UpdateClockText()
    {
        if (clockText == null)
        {
            return;
        }

        float progress = Mathf.Clamp01(elapsedTime / surviveDurationSeconds);
        int hourOffset = Mathf.FloorToInt(progress * 6f);
        int militaryHour = 12 + hourOffset;
        int displayHour = militaryHour % 12;
        if (displayHour == 0)
        {
            displayHour = 12;
        }

        clockText.text = displayHour + ":00 AM";
    }

    private void SetWarning(string warning)
    {
        if (warningText == null)
        {
            return;
        }

        warningText.text = warning;
        warningText.enabled = !string.IsNullOrEmpty(warning);
    }

    private void SetResult(string result)
    {
        if (resultText == null)
        {
            return;
        }

        resultText.text = result;
        resultText.enabled = !string.IsNullOrEmpty(result);
    }

    private void CleanupMissingCustomers()
    {
        if (waitingCustomers.Count == 0)
        {
            return;
        }

        List<NPC_Movement> toRemove = null;
        foreach (KeyValuePair<NPC_Movement, float> entry in waitingCustomers)
        {
            if (entry.Key == null)
            {
                if (toRemove == null)
                {
                    toRemove = new List<NPC_Movement>();
                }
                toRemove.Add(entry.Key);
            }
        }

        if (toRemove != null)
        {
            foreach (NPC_Movement npc in toRemove)
            {
                waitingCustomers.Remove(npc);
            }
        }
    }

    private static float GetOldestDuration<T>(Dictionary<T, float> entries, float now)
    {
        float oldest = 0f;
        foreach (KeyValuePair<T, float> entry in entries)
        {
            float duration = now - entry.Value;
            if (duration > oldest)
            {
                oldest = duration;
            }
        }

        return oldest;
    }

    private void CachePlayerAndMonster()
    {
        if (playerTransform == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
            else
            {
                FPSController controller = FindFirstObjectByType<FPSController>();
                if (controller != null)
                {
                    playerTransform = controller.transform;
                }
            }
        }

        if (monsterAnimator == null && activeMonsterOnLose != null)
        {
            monsterAnimator = activeMonsterOnLose.GetComponent<MonsterAnimator>();
        }
    }
}
