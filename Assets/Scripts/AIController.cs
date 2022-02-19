using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AIController : MonoBehaviour
{
    public enum AIType
    {
        Static,
        Dynamic
    }

    public AIType aiType;
    
    public enum AIState
    {
        StandLook,
        Wander,
        Suspect,
        Chase
    }

    public AIState aiState;

    [SerializeField] private GameObject ExclamationObj;


    [Header("Agent Settings")]
    //
    [SerializeField] private NavMeshAgent myAgent;
    [SerializeField] private float agentDefaultSpeed;
    [SerializeField] private float agentRunSpeed;
    [SerializeField] private float lookTime;
    private float lookDirection = 1;
    private float staticLookCounter;
    [SerializeField] private float lookSpeed;

    [Header("WayPoint")]
    //
    [SerializeField] private List<Transform> WayPointList;
    private int wayPointIndex;
    private int wayPointDirection = 1;

    [Header("Field of Views")] 
    //
    [SerializeField] private FieldOfView realFOV;
    [SerializeField] private FieldOfView secondFOV;

    private float lookCounter;
    private Vector3 lookPos;
    private Vector3 firstPos;

    private Transform player;
    private GameController gameController;
    
    #region Animation Parameters

    [Header("Animation Parameters")]
    //
    [SerializeField] private Animator anim;
    
    [SerializeField] private string animParameterWalk;
    [SerializeField] private string animParameterIdle;

    [SerializeField] private string animParameterRun;

    [SerializeField] private string animParameterSuspected;
    
    
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        StartMethods();   
    }
    
    #region StartMethods

    void StartMethods()
    {
        SetAgentSpeed();
        SetFirstLookTime();
        SetFirstPos();
        SetPlayerTransform();
        GetGameController();
        WayPointListDebug();
    }
    void SetAgentSpeed()
    {
        myAgent.speed = agentDefaultSpeed;
    }
    void SetFirstLookTime()
    {
        lookCounter = lookTime;
    }
    void SetFirstPos()
    {
        firstPos = transform.position;
    }
    void SetPlayerTransform()
    {
        player = AgentBehaviour.instance.transform;
    }

    void GetGameController()
    {
        gameController = GameController.instance;
    }

    void WayPointListDebug()
    {
        if (aiType == AIType.Dynamic && WayPointList.Count < 2)
        {
            Debug.LogWarning("Way Point List Error!");
        }
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (!gameController.isLevelCompleted)
        {
            CheckVisableTarget();
            StaticAIBehaviour();
            DynamicAIBehaviour();
            SetAcceleration();
        }
        else
        {
            myAgent.SetDestination(transform.position);
        }
    }
    
    #region AIMovements

    void StaticAIBehaviour()
    {
        if (aiType == AIType.Static)
        {
            if (aiState == AIState.StandLook)
            {
                if (staticLookCounter <= 0)
                {
                    staticLookCounter = Random.Range(1, 5);
                    lookDirection = -lookDirection;
                }

                staticLookCounter -= Time.deltaTime;

                transform.Rotate(Vector3.up * lookDirection * lookSpeed * Time.deltaTime);

                myAgent.SetDestination(transform.position);
                AnimationIdle();

            }
            else if (aiState == AIState.Wander)
            {
                myAgent.SetDestination(firstPos);
                if (myAgent.remainingDistance < 0.25f)
                {
                    myAgent.velocity = Vector3.zero;
                    myAgent.angularSpeed = 0;
                    aiState = AIState.StandLook;
                }
                ChangeAgentSpeed(agentDefaultSpeed);
                AnimationWalkStart();
            }
            else if (aiState == AIState.Suspect)
            {
                
                myAgent.SetDestination(transform.position);
                if (!ExclamationObj.activeInHierarchy)
                {
                    ExclamationObj.SetActive(true);
                }

                lookCounter -= Time.deltaTime;
                if (lookCounter < 0)
                {
                    lookCounter = lookTime;
                    ExclamationObj.SetActive(false);
                    aiState = AIState.Chase;
                }
                transform.LookAt(lookPos);
                AnimationSuspected();
            }
            else if (aiState == AIState.Chase)
            {
                myAgent.SetDestination(player.position);
                transform.LookAt(lookPos);
                ChangeAgentSpeed(agentRunSpeed);
                AnimationRunStart();
            }
        }
    }
    void DynamicAIBehaviour()
    {
        if (aiType == AIType.Dynamic)
        {

            aiState = aiState == AIState.StandLook ? AIState.Wander : aiState;
            
            if (aiState == AIState.Wander)
            {
                myAgent.SetDestination(WayPointList[wayPointIndex].position);
                if (myAgent.remainingDistance < 0.5f)
                {
                    if (wayPointIndex + wayPointDirection < WayPointList.Count && wayPointIndex + wayPointDirection >= 0)
                    {
                        wayPointIndex += wayPointDirection;
                    }
                    else
                    {
                        wayPointDirection = -wayPointDirection;
                        wayPointIndex += wayPointDirection;
                    }
                }
                ChangeAgentSpeed(agentDefaultSpeed);
                AnimationWalkStart();
            }
            else if (aiState == AIState.Suspect)
            {
                if (!ExclamationObj.activeInHierarchy)
                {
                    ExclamationObj.SetActive(true);
                }
                
                myAgent.SetDestination(transform.position);

                lookCounter -= Time.deltaTime;
                if (lookCounter < 0)
                {
                    lookCounter = lookTime;
                    ExclamationObj.SetActive(false);
                    aiState = AIState.Chase;
                }
                
                transform.LookAt(lookPos);
                AnimationSuspected();
            }
            else if (aiState == AIState.Chase)
            {
                myAgent.SetDestination(player.position);
                transform.LookAt(lookPos);
                ChangeAgentSpeed(agentRunSpeed);
                AnimationRunStart();
            }
        }
    }

    void CheckVisableTarget()
    {
        if (realFOV.visibleTargets.Count > 0)
        {
            lookPos = realFOV.visibleTargets[0].position;
            lookPos.y = transform.position.y;

            if (aiState != AIState.Chase)
            {
                aiState = AIState.Suspect;
            }
            Debug.Log("Player Spotted!");
        }
        else if (aiState == AIState.Chase)
        {
            if (aiType == AIType.Dynamic)
            {
                FindClosestWayPoint();
            }

            aiState = AIState.Wander;
        }
        else
        {
            lookCounter = lookTime;
        }
    }
    
    void FindClosestWayPoint()
    {
        float minDistance = Mathf.Infinity;
        float tempDist;
        for (int i = 0; i < WayPointList.Count; i++)
        {
            tempDist = Vector3.Distance(transform.position, WayPointList[i].position);
            if (tempDist < minDistance)
            {
                wayPointIndex = i;
                minDistance = tempDist;
            }
        }
    }

    void SetAcceleration()
    {
        if (myAgent.hasPath)
        {
            Vector3 toTarget = myAgent.steeringTarget - transform.position;
            float turnAngle = Vector3.Angle(transform.forward, toTarget);
            myAgent.acceleration = turnAngle * agentDefaultSpeed;
        }
    }

    void ChangeAgentSpeed(float speed)
    {
        myAgent.speed = speed;
    }
    
    #endregion
    
    #region Animations

    void AnimationIdle()
    {
        anim.SetBool(animParameterRun, false);
        anim.SetBool(animParameterSuspected,false);
        anim.SetBool(animParameterWalk,false);
        anim.SetBool(animParameterIdle, true);
    }

    void AnimationWalkStart()
    {
        anim.SetBool(animParameterIdle, false);
        anim.SetBool(animParameterSuspected, false);
        anim.SetBool(animParameterRun,false);
        anim.SetBool(animParameterWalk,true);
    }

    void AnimationRunStart()
    {
        anim.SetBool(animParameterIdle, false);
        anim.SetBool(animParameterSuspected,false);
        anim.SetBool(animParameterWalk,false);
        anim.SetBool(animParameterRun,true);
    }

    void AnimationSuspected()
    {
        anim.SetBool(animParameterIdle, false);
        anim.SetBool(animParameterSuspected,true);
        anim.SetBool(animParameterRun,false);
        anim.SetBool(animParameterWalk, false);
    }

    #endregion
}
