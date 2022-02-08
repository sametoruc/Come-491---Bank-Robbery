using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class AgentBehaviour : MonoBehaviour
{
    public NavMeshSurface surface;
    public NavMeshAgent myAgent;
    public GameObject targetObject;
    public bool drawPathBool;
    public static AgentBehaviour instance;
    public float randomX, randomZ;
    public bool canReach;
    public Vector3 lastCorner;
    public float deger;
    public Animator anim;
    public Vector3[] corners;
    public GameObject footPrint;
    public GameObject[] footPrints;
    public GameObject FPParent;
    Coroutine myRoutine;
    
    void Awake()
	{
		if (!instance)
		{
            instance = this;
		}
	}
    // Start is called before the first frame update
    void Start()
    {
        targetObject = FinalDoor.instance.Target;
        myRoutine = null;
        myRoutine = StartCoroutine(StuckController());
        surface.BuildNavMesh();
        myAgent.enabled = true;
        footPrints = new GameObject[200]; //Creating Game Object Array For The Sprites
        myAgent.SetDestination(targetObject.transform.position);
        CreateSprite();
        anim.SetTrigger("Terrify");
    }

    // Update is called once per frame
    void Update()
    {
        /*if (canReach)
		{
            Vector3 toTarget = myAgent.steeringTarget - transform.position;
            float turnAngle = Vector3.Angle(transform.forward, toTarget);
            if(myAgent.speed>0)
            {
                //myAgent.acceleration = turnAngle * myAgent.speed;
            }
		}*/
        if(!canReach)
        {
            myAgent.velocity = Vector3.zero;
        }

        
    }
    public IEnumerator StuckController()
	{
		while (true)
		{
            Vector3 firstPos = transform.position;
            yield return new WaitForSeconds(5);
            Vector3 secondPos = transform.position;
            if (firstPos == secondPos)
            {
                anim.SetTrigger("Terrify");
            }
        }
        
	}
    public void IdleDance()
	{
        anim.SetBool("isRun", false);
        anim.SetBool("isWalk", false);
        anim.SetTrigger("isDance");
        anim.SetFloat("DanceBlend", Random.Range(0, 2));
    }
    public void CreateSprite()
	{
        if (canReach)
        {
            anim.SetBool("isWalk", false);
            anim.SetBool("isRun", true);
            anim.SetFloat("Blend", Random.Range(0, 4));
            DeleteSprites();
            int k = 0;
            for (int i = 0; i < corners.Length; i++)
            {
                if (i < corners.Length - 1)
                {
                    if ((corners[i] - corners[i + 1]).magnitude > 8)
                    {
                        for (int j = 0; j < ((int)(corners[i] - corners[i + 1]).magnitude / 8); j++)
                        {
                            GameObject gos = Instantiate(footPrint, (corners[i] + ((j + 1) * 8 * (corners[i + 1] - corners[i]).normalized) + new Vector3(0, 0.1f, 0)), Quaternion.identity);
                            gos.transform.SetParent(FPParent.transform);
                            gos.name = "footPrint" + k;
                            footPrints[k] = gos;
                            if (i < corners.Length - 1)
                            {
                                footPrints[k].transform.LookAt(corners[i+1]);                               
                            }
                            else
                            {
                                footPrints[k].transform.LookAt(targetObject.transform.position);
                            }
                            k++;
                        }

                    }
                }
            }
        }
        else
        {
            DeleteSprites();
            anim.SetBool("isWalk", false);
            anim.SetBool("isRun", false);
        }
        
	}
    public void DeleteSprites()
	{
        foreach (Transform childTransform in FPParent.transform) 
        {
            Destroy(childTransform.gameObject);
        }
    }
    /*public void Wander()
	{
        Debug.Log("Wander1");
        bool toRight = false, toLeft = false, toBack = false, toForward = false;
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.right, out hit, 2))
		{
            if (hit.collider.gameObject != null)
			{
                if(hit.collider.gameObject.tag == "Door" || hit.collider.gameObject.tag == "Wall")
				{
                    toRight = false;
                    
				}
				else
				{
                    toRight = true;
				}
			}
			else
			{
                toRight = true;
			}
		}
		else
		{
            toRight = true;
		}
        if (Physics.Raycast(transform.position, Vector3.left, out hit, 2))
        {
            if (hit.collider.gameObject != null)
            {
                if (hit.collider.gameObject.tag == "Door" || hit.collider.gameObject.tag == "Wall")
                {
                    toLeft = false;
                }
                else
                {
                    toLeft = true;
                }
            }
            else
            {
                toLeft = true;
            }
        }
		else
		{
            toLeft = true;
        }
        if (Physics.Raycast(transform.position, Vector3.forward, out hit, 2))
        {
            if (hit.collider.gameObject != null)
            {
                if (hit.collider.gameObject.tag == "Door" || hit.collider.gameObject.tag == "Wall")
                {
                    toForward = false;
                }
                else
                {
                    toForward = true;
                }
            }
            else
            {
                toForward = true;
            }
        }
		else
		{
            toForward = true;
		}
        if (Physics.Raycast(transform.position, Vector3.back, out hit, 2))
        {
            if (hit.collider.gameObject != null)
            {
                if (hit.collider.gameObject.tag == "Door" || hit.collider.gameObject.tag == "Wall")
                {
                    toBack = false;
                }
                else
                {
                    toBack = true;
                }
            }
            else
            {
                toBack = true;
            }
        }
		else
		{
            toBack = true;
		}
        int x1 = toRight ? 2 : 0;
        int x2 = toLeft ? 2 : 0;
        int z1 = toForward ? 2 : 0;
        int z2 = toBack ? 2 : 0;
        RandPos(x1, x2, z1, z2);
        

    }*/
    /*public void RandPos(int x1, int x2, int z1, int z2)
	{

        randomX = Random.Range(-x2, x1);
        randomZ = Random.Range(-z2, z1);
        wanderPos = new Vector3(((transform.position.x)+randomX), transform.position.y, ((transform.position.z)+randomZ));
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(wanderPos, out navHit, 2.0f, NavMesh.AllAreas))
        {
            //Debug.Log("WanderT");
            wanderBool = true;
            myAgent.SetDestination(wanderPos);
            anim.SetBool("isRun", false);
            anim.SetBool("isWalk", true);
            //Debug.Log(myAgent.destination);
        }
        else
        {
            //Debug.Log("WanderF");
            RandPos(x1, x2, z1, z2);
        }

    }*/
    public void CanReach()
	{
        corners = myAgent.path.corners;
        if (corners.Length > 1)
		{

            lastCorner = corners[corners.Length - 1];
        }
        if ((lastCorner - targetObject.transform.position).magnitude <= 2)
        {
            canReach = true;
        }
		else
		{
            canReach = false;
		}
        CreateSprite();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("FootPrint"))//Deletes Foot Print Which Behind The Path 
        {
            other.gameObject.SetActive(false);
        }
		if (other.gameObject.CompareTag("Key"))//Change Bool for Win Condition
        {
            GameController.instance.hasKey = true;
            other.gameObject.SetActive(false);
            FinalDoor.instance.OpenTheGate();
        }
		if (other.gameObject.CompareTag("Fire"))//Calls Lvl Fail When Player Enter The Fire
		{
            LvlFail();
		}
        if (other.gameObject.CompareTag("FinalDoor"))//Calls Lvl Fail When Player Enter The Fire
        {
            if(GameController.instance.hasKey)
            {
                LvlWon();
            }
            else
            {
                LvlFail();
            }
            
        }
    }
    public void LvlWon()
	{
        Debug.Log("LvlWon");
        StopCoroutine(myRoutine);
        myAgent.ResetPath();
        myAgent.enabled = false;
        IdleDance();
        GameController.instance.WinLevel();
    }
    public void LvlFail()
	{
        Debug.Log("LvlFail");
        StopCoroutine(myRoutine);
        //myAgent.ResetPath(); 
        myAgent.enabled = false;
        anim.SetBool("isWalk", false);
        anim.SetBool("isRun", false);
        GameController.instance.FailLevel();
    }
}
