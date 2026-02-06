using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State { CalmPatrol, SuspiciousRoam, Investigate, Search }

    [Header("Movement")]
    [SerializeField] private float stopDistance = 1.2f;

    [Header("Calm Patrol (optional waypoints)")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolWaitMin = 0.6f;
    [SerializeField] private float patrolWaitMax = 2.0f;

    [Header("Roam (NavMesh random)")]
    [SerializeField] private float calmRoamRadius = 8f;        // quando está calmo
    [SerializeField] private float suspiciousRoamRadius = 10f; // quando suspeito (mas sem pista fresca)
    [SerializeField] private float roamRepathTime = 2.5f;

    [Header("Hearing")]
    [SerializeField] private float hearingRange = 14f;

    [Header("Suspicion Model")]
    [Range(0f, 1f)]
    [SerializeField] private float suspicion = 0f;
    [SerializeField] private float suspicionGain = 0.55f;
    [SerializeField] private float suspicionDecayPerSec = 0.08f;
    [SerializeField] private float suspicionFollowLerp = 0.65f;

    [Header("Uncertainty (Ghost Target)")]
    [SerializeField] private float uncertaintyRadiusMax = 8f; // baixa suspeita => grande erro
    [SerializeField] private float uncertaintyRadiusMin = 1.5f;// alta suspeita => pequeno erro

    [Header("Investigate/Search")]
    [SerializeField] private int searchPointsCount = 5;
    [SerializeField] private float searchPointWait = 0.75f;

    private NavMeshAgent agent;
    private State state = State.CalmPatrol;

    // Patrol
    private int patrolIndex = -1;
    private float waitTimer = 0f;

    // Suspicion memory
    private Vector3 suspicionCenter;
    private bool hasSuspicionCenter = false;
    private Vector3 lastHeardPos;
    private float lastSoundTime = -999f;

    // Roam
    private float roamTimer = 0f;

    // Search
    private Vector3[] searchPoints;
    private int searchIndex = 0;
    private float searchWaitTimer = 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        searchPoints = new Vector3[Mathf.Max(1, searchPointsCount)];
    }

    private void OnEnable()  => SoundSystem.OnSound += OnSoundHeard;
    private void OnDisable() => SoundSystem.OnSound -= OnSoundHeard;

    private void Start()
    {
        if (!hasSuspicionCenter)
        {
            suspicionCenter = transform.position;
            hasSuspicionCenter = true;
        }

        // Começa a mover-se
        if (patrolPoints != null && patrolPoints.Length > 0)
            GoToNextPatrolPoint();
        else
            SetRandomDestinationAround(transform.position, calmRoamRadius);
    }

    private void Update()
    {
        // Suspicion decai sempre
        suspicion = Mathf.Clamp01(suspicion - suspicionDecayPerSec * Time.deltaTime);

        // Estados baseados em suspicion
        if (state == State.CalmPatrol && suspicion > 0.25f)
            state = State.SuspiciousRoam;

        if (state == State.SuspiciousRoam && suspicion < 0.15f)
            state = State.CalmPatrol;

        switch (state)
        {
            case State.CalmPatrol:
                UpdateCalmPatrol();
                break;

            case State.SuspiciousRoam:
                UpdateSuspiciousRoam();
                break;

            case State.Investigate:
                UpdateInvestigate();
                break;

            case State.Search:
                UpdateSearch();
                break;
        }
    }

    // ---------------- HEARING ----------------
    private void OnSoundHeard(Vector3 soundPos, float intensity)
    {
        if (Vector3.Distance(transform.position, soundPos) > hearingRange)
            return;

        lastSoundTime = Time.time;
        lastHeardPos = soundPos;

        // Atualiza centro de suspeita (não salta instantaneamente)
        if (!hasSuspicionCenter)
        {
            suspicionCenter = soundPos;
            hasSuspicionCenter = true;
        }
        else
        {
            suspicionCenter = Vector3.Lerp(suspicionCenter, soundPos, suspicionFollowLerp);
        }

        // Aumenta suspeita
        float gain = suspicionGain * Mathf.Clamp01(intensity);
        suspicion = Mathf.Clamp01(suspicion + gain);

        // Gera um alvo impreciso perto do som
        Vector3 ghost = GetGhostTargetNear(soundPos, suspicion);

        agent.SetDestination(ghost);
        state = State.Investigate;

        // Reset search
        searchIndex = 0;
        searchWaitTimer = 0f;
    }

    // Alvo estimado com erro controlado por suspicion
    private Vector3 GetGhostTargetNear(Vector3 center, float s)
    {
        float uncertainty = Mathf.Lerp(uncertaintyRadiusMax, uncertaintyRadiusMin, s);
        Vector2 rnd = Random.insideUnitCircle * uncertainty;
        Vector3 candidate = center + new Vector3(rnd.x, 0f, rnd.y);

        if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            return hit.position;

        // fallback: tenta o centro
        if (NavMesh.SamplePosition(center, out hit, 3f, NavMesh.AllAreas))
            return hit.position;

        return center;
    }

    // ---------------- CALM PATROL ----------------
    private void UpdateCalmPatrol()
    {
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            if (agent.pathPending) return;

            if (agent.remainingDistance <= stopDistance)
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                    GoToNextPatrolPoint();
            }
        }
        else
        {
            // roam calmo aleatório
            roamTimer -= Time.deltaTime;
            if (roamTimer <= 0f)
            {
                SetRandomDestinationAround(transform.position, calmRoamRadius);
                roamTimer = roamRepathTime;
            }
        }
    }

    private void GoToNextPatrolPoint()
    {
        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[patrolIndex].position);
        waitTimer = Random.Range(patrolWaitMin, patrolWaitMax);
    }

    // ---------------- SUSPICIOUS ROAM ----------------
    private void UpdateSuspiciousRoam()
    {
        // Mesmo sem som recente, ele continua “a rondar” a zona suspeita
        roamTimer -= Time.deltaTime;

        // Se houve som muito recente, deixa Investigate/Search tratar disso
        // (já mudamos de state no OnSoundHeard)

        if (roamTimer <= 0f)
        {
            // Roam com centro na suspicionCenter
            SetRandomDestinationAround(suspicionCenter, suspiciousRoamRadius);
            roamTimer = roamRepathTime;
        }
    }

    private void SetRandomDestinationAround(Vector3 center, float radius)
    {
        Vector2 rnd = Random.insideUnitCircle * radius;
        Vector3 candidate = center + new Vector3(rnd.x, 0f, rnd.y);

        if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, radius, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
        else
            agent.SetDestination(center);
    }

    // ---------------- INVESTIGATE ----------------
    private void UpdateInvestigate()
    {
        if (agent.pathPending) return;

        if (agent.remainingDistance <= stopDistance)
        {
            BuildSearchPointsAround(lastHeardPos);
            state = State.Search;
        }
    }

    // ---------------- SEARCH ----------------
    private void BuildSearchPointsAround(Vector3 center)
    {
        // Se suspeita alta, procura mais apertado (mais perigoso)
        float radius = Mathf.Lerp(10f, 2f, suspicion);

        for (int i = 0; i < searchPoints.Length; i++)
        {
            Vector2 rnd = Random.insideUnitCircle * radius;
            Vector3 candidate = center + new Vector3(rnd.x, 0f, rnd.y);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 3f, NavMesh.AllAreas))
                searchPoints[i] = hit.position;
            else
                searchPoints[i] = center;
        }

        searchIndex = 0;
        agent.SetDestination(searchPoints[searchIndex]);
    }

    private void UpdateSearch()
    {
        if (agent.pathPending) return;

        if (agent.remainingDistance <= stopDistance)
        {
            searchWaitTimer += Time.deltaTime;
            if (searchWaitTimer < searchPointWait)
                return;

            searchWaitTimer = 0f;
            searchIndex++;

            if (searchIndex >= searchPoints.Length)
            {
                // Depois de procurar, volta a rondar a zona (se ainda suspeito)
                state = (suspicion > 0.2f) ? State.SuspiciousRoam : State.CalmPatrol;
                roamTimer = 0f; // força escolher destino já
                return;
            }

            agent.SetDestination(searchPoints[searchIndex]);
        }
    }

    // ---------------- DEBUG ----------------
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRange);

        Gizmos.color = new Color(1f, 0.4f, 0f, 1f);
        Gizmos.DrawSphere(suspicionCenter, 0.25f);
    }
}
