using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Pedestrains : MonoBehaviour
{

    public NavMeshAgent agent;
    public Animator animator;
    public GameObject PATH;
    private Transform[] PathPoints;
    public int index=0;
    public float minDistance=10;
   
    void Start()
    {
       agent = GetComponent<NavMeshAgent>();  
       animator = GetComponent<Animator>();    

        PathPoints = new Transform[PATH.transform.childCount];

        for(int i = 0; i < PathPoints.Length; i++)
        {
            PathPoints[i] = PATH.transform.GetChild(i);
        }
    }

  
    void Update()
    {
        roam();
    }

    void roam()
    {

        if (Vector3.Distance(transform.position, PathPoints[index].position) < minDistance)
        {
            index++;

            if(index >= PathPoints.Length)
            {
                index = 0;
            }
            
        }

        agent.SetDestination(PathPoints[index].position);
        animator.SetFloat("vertical", !agent.isStopped ? 1 : 0);
    }

    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Car"))
        {
            agent.isStopped = true;
            animator.SetBool("isFalling",true);
        }
        else
        {
            animator.SetBool("isFalling", false);
        }
    }
}
