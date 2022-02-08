using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalDoor : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Chains, Lockpad, Confettis, Target;
    public static FinalDoor instance;
    private void Awake()
    {
        if (!instance)
            instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenTheGate()
    {
        Chains.SetActive(false);
        Lockpad.GetComponent<Animator>().SetTrigger("Unlock");
        Lockpad.GetComponent<Rigidbody>().isKinematic = false;
    }
    public void ConfettisOn()
    {
        Confettis.SetActive(true);
    }
}
