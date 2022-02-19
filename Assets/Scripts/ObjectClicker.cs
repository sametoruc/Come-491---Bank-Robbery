using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class ObjectClicker : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !GameController.instance.isLevelCompleted)
        {

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000))
            {
                if (hit.transform != null)
                {
                    if (hit.transform.tag == "Door")
                    {                      
                        if(hit.transform.gameObject.GetComponent<Animator>().GetBool("isOpen"))
                        {
                            hit.transform.gameObject.GetComponent<Animator>().SetBool("isOpen", false);
                            hit.transform.GetComponent<NavMeshObstacle>().enabled = true;
                        }
                        else
                        {
                            hit.transform.gameObject.GetComponent<Animator>().SetBool("isOpen", true);
                            hit.transform.GetComponent<NavMeshObstacle>().enabled = false;
                        }
                        StartCoroutine(DelayedTarget());
                        
                    }
                }
            }
        }
    }
    public IEnumerator DelayedTarget()
	{
        AgentBehaviour.instance.myAgent.ResetPath();
        AgentBehaviour.instance.myAgent.SetDestination(AgentBehaviour.instance.targetObject.transform.position);        
        yield return new WaitForSeconds(0.1f);
        AgentBehaviour.instance.CanReach();

    }
}
