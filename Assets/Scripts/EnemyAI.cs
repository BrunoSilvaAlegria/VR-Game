using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State{ Patrol, Investigate, Search }

    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolStopDistance = 1.0f;
    [SerializeField] private float patrolWaitMin = 0.5f;
    [SerializeField] private float patrolWaitMax = 2.0f;

    [Header("Hearing")]
    [SerializeField] private float hearingRange = 12f;
    [SerializeField] private float baseUncertaintyRadius = 6f;
    [SerializeField] private float investigateStopDistance = 1.2f;

    [Header("Search")]
    [SerializeField] private int searchPointsCount = 5;
    [SerializeField] private float searchRadiusMin = 2f;
    [SerializeField] private float searchRadiusMax = 10f;
    [SerializeField] private float searchPointsStopDistance = 1.2f;
    [SerializeField] private float searchPointWait = 0.75f;
    
    [Header("Confidence")]
    [SerializeField] private float confidence = 0f;
    [SerializeField] private float confidenceGain = 0.6f;
    [SerializeField] private float confidenceDecayPerSec = 0.12f;

    private NavMeshAgent agent;
    private State state = State.Patrol;
    // Patrol state
    private int patrolIndex = -1;
    private float waitTimer = 0f;


    private Vector3 lastHeardPos;
    private Vector3 estimatedTarget;
    
    private int searchIndex = 0;
    private Vector3[] searchPoints;
    private float searchWaitTimer = 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        searchPoints = new Vector3[Mathf.Max(1, searchPointsCount)];
    }

    private void OnEnable() => SoundSystem.OnSound += OnSoundHeard;

    private void OnDisable() => SoundSystem.OnSound -= OnSoundHeard;

    private void Start()
    {
        GoToNextPatrolPoint();
    }

    private void Update()
    {
        // Decai confiança ao longo do tempo
        confidence = Mathf.Clamp01(confidence - confidenceDecayPerSec * Time.deltaTime);

        switch(state)
        {
            case State.Patrol:
                UpdatePatrol();
                break;

            case State.Investigate:
                UpdateInvestigate();
                break;

            case State.Search:
                UpdateSearch();
                break;
        }
    }

    private void UpdatePatrol()
    {
        if(patrolPoints == null || patrolPoints.Length == 0)
            return;
        
        if(agent.pathPending)
            return;
        if(agent.remainingDistance <= patrolStopDistance)
        {
            waitTimer -= Time.deltaTime;
            if(waitTimer <= 0f)
                GoToNextPatrolPoint();
            
        }
        
    }

    private void GoToNextPatrolPoint()
    {
        if(patrolPoints == null || patrolPoints.Length == 0)
            return;

        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[patrolIndex].position);

        waitTimer = Random.Range(patrolWaitMin, patrolWaitMax);
        
    }

    private void OnSoundHeard(Vector3 soundPos, float intensity)
    {
        if(Vector3.Distance(transform.position, soundPos) > hearingRange)
            return;
        
        lastHeardPos = soundPos;
        //Aumenta confiança com intensidade
        confidence = Mathf.Clamp01(confidence + confidenceGain * Mathf.Clamp01(intensity));

        //Menos confiança => mais incerteza (maior raio)
        float uncertainty = Mathf.Lerp(baseUncertaintyRadius, 1.0f, confidence);

        //Escolhe um alvo "noisy" perto do som
        Vector2 rnd = Random.insideUnitCircle * uncertainty;
        Vector3 noisyTarget = soundPos + new Vector3(rnd.x, 0f, rnd.y);

        //ajusta para um ponto valido no navmesh
        if(NavMesh.SamplePosition(noisyTarget, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
           estimatedTarget = hit.position;
        else
            estimatedTarget = soundPos;

        agent.SetDestination(estimatedTarget);
        state = State.Investigate; 

        searchIndex = 0;
        searchWaitTimer = 0f;
    }

    private void UpdateInvestigate()
    {
        if(agent.pathPending)
            return;
        
        if(agent.remainingDistance <= investigateStopDistance)
        {
            //Chegou ao ponto estimado: começa a procurar numa area
            BuildSearchPoints();
            state = State.Search;
        }
    }

    public void BuildSearchPoints()
    {
        // raio de busca depende da falta de confiança:
        // baixa confiança => procura mais espalhado

        float radius = Mathf.Lerp(searchRadiusMax, searchRadiusMin, 1f - confidence);

        for(int i = 0; i < searchPoints.Length; i++)
        {
            Vector2 rnd = Random.insideUnitCircle * radius;
            Vector3 candidate = lastHeardPos + new Vector3(rnd.x, 0f, rnd.y);

            if(NavMesh.SamplePosition(candidate, out NavMeshHit hit, 3.0f, NavMesh.AllAreas))
                searchPoints[i] = hit.position;
            else
                searchPoints[i] = lastHeardPos;
        }

        searchIndex = 0;
        agent.SetDestination(searchPoints[searchIndex]);
    }

    private void UpdateSearch()
    {
        if(agent.pathPending)
            return;
        
        if(agent.remainingDistance <= searchPointsStopDistance)
        {
            searchWaitTimer += Time.deltaTime;
            if(searchWaitTimer < searchPointWait) return;

            searchWaitTimer = 0f;
            searchIndex++;

            if(searchIndex >= searchPoints.Length)
            {
                state = State.Patrol;
                GoToNextPatrolPoint();
                return;
            }
            
            agent.SetDestination(searchPoints[searchIndex]);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(estimatedTarget, 0.2f);

        if(searchPoints != null)
        {
            Gizmos.color = Color.blue;
            for(int i = 0; i < searchPoints.Length; i++)
            {
                Gizmos.DrawWireSphere(searchPoints[i], 0.15f);
            }
        }
    }
}
