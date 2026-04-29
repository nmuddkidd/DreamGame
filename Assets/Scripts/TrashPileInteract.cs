using UnityEngine;

public class TrashPileInteract : MonoBehaviour
{
    [SerializeField] private GameObject trashBagPrefab;
    [SerializeField] private Vector3 spawnOffset = new Vector3(0f, 0.5f, 0f);
    [SerializeField] private bool destroyPileAfterSpawn = true;

    private bool hasSpawned = false;
    private int trashTaskId = -1;

    public void SetTaskId(int taskId)
    {
        trashTaskId = taskId;
    }

    public bool SpawnTrashBag()
    {
        if (hasSpawned || trashBagPrefab == null)
        {
            return false;
        }

        hasSpawned = true;
        GameObject bagInstance = Instantiate(trashBagPrefab, transform.position + spawnOffset, Quaternion.identity);
        TrashBagDrag bagDrag = bagInstance.GetComponent<TrashBagDrag>();
        if (bagDrag == null)
        {
            bagDrag = bagInstance.GetComponentInChildren<TrashBagDrag>();
        }
        if (bagDrag != null)
        {
            bagDrag.SetTaskId(trashTaskId);
        }

        if (destroyPileAfterSpawn)
        {
            Transform pileRoot = transform.parent;
            if (pileRoot != null)
            {
                Destroy(pileRoot.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        return true;
    }
}
