using UnityEngine;

public class TrashBagDrag : MonoBehaviour
{
    [SerializeField] private Vector3 holdOffset = new Vector3(0.35f, -0.25f, 1.4f);
    [SerializeField] private float followLerpSpeed = 18f;

    private Transform followTarget;
    private Rigidbody rb;
    private Collider[] colliders;
    private int trashTaskId = -1;

    public bool IsBeingDragged { get; private set; }

    public void SetTaskId(int taskId)
    {
        trashTaskId = taskId;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
    }

    void Update()
    {
        if (!IsBeingDragged || followTarget == null)
        {
            return;
        }

        Vector3 desiredPosition = followTarget.TransformPoint(holdOffset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followLerpSpeed * Time.deltaTime);
    }

    public void BeginDrag(Transform target)
    {
        if (target == null)
        {
            return;
        }

        followTarget = target;
        IsBeingDragged = true;

        SetCollidersEnabled(false);

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }

    public void EndDrag()
    {
        IsBeingDragged = false;

        SetCollidersEnabled(true);

        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }

    public void Consume()
    {
        if (SurvivalGameManager.Instance != null)
        {
            SurvivalGameManager.Instance.CompleteTrashTask(trashTaskId);
        }
        EndDrag();
        Destroy(gameObject);
    }

    private void SetCollidersEnabled(bool enabled)
    {
        if (colliders == null)
        {
            return;
        }

        foreach (Collider collider in colliders)
        {
            if (collider != null)
            {
                collider.enabled = enabled;
            }
        }
    }
}
