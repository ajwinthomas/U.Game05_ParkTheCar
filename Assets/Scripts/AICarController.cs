using NUnit.Framework;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class AICarController : MonoBehaviour
{
    public Transform[] Targets;
    private int currentTargetIndex = 0;
    private Transform Target => Targets !=null && Targets.Length > 0 ? Targets[currentTargetIndex] : null;
    private NavMeshAgent agent;
    public GameObject[] Wheels;
    public float rotationSpeed = 360f;
    public float maxSteerAngle = 30f;
    public float steerSpeed = 5f;
    public Transform frontLeftWheel;
    public Transform frontRightWheel;

    public float detectionRange = 5f;
    public LayerMask obstacleMask;
    public float sideOffset = 3f;
    public float rerouteDistance = 5f;
    private bool isAvoiding = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        MoveAgent();
    }

    void MoveAgent()
    {
        if (agent == null || Target == null) return;

        //Obstacle Avoidance Check
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 forward = transform.forward;

        bool obstacleAhead = Physics.Raycast(origin, forward, out RaycastHit hit, detectionRange, obstacleMask);
        if (obstacleAhead)
        {
            Debug.Log("there is an obstacle in the front " + hit.collider.gameObject.name);
            
        }

        if (obstacleAhead &&  !isAvoiding && hit.collider.transform != Target)
        {
            //check obstacle check for right and left

            Vector3 leftOrigin = origin - transform.right * sideOffset;
            Vector3 rightOrigin = origin + transform.right * sideOffset;

            bool leftClear = !Physics.Raycast(leftOrigin, forward, detectionRange, obstacleMask);
            bool rightClear = !Physics.Raycast(rightOrigin, forward, detectionRange, obstacleMask);

            if (leftClear)
            {
                Vector3 leftReroute =  transform.position - transform.right * rerouteDistance;
                agent.SetDestination(leftReroute);
                isAvoiding = true;
                agent.isStopped = false;
            }
            else if (rightClear)
            {
                Vector3 rightReroute = transform.position + transform.right * rerouteDistance;
                agent.SetDestination(rightReroute);
                isAvoiding = true;
                agent.isStopped = false;
            }
            else
            {
                //no way to go , so stop
                agent.isStopped = true;
            }
        }
        else
        {
            //no obstacle in the route, free to go
            if (agent.isStopped) agent.isStopped = false;

            if (isAvoiding && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
            {
                //Reached reroute spot.resume to main target
                agent.SetDestination(Target.position);
                isAvoiding = false;
            }
            else if (!isAvoiding)
            {
                if(agent.remainingDistance <= agent.stoppingDistance + 0.1f)
                {
                    //Reached the current target move to the other
                    currentTargetIndex = (currentTargetIndex + 1) % Targets.Length;
                    agent.SetDestination(Target.position);
                }
                else
                {
                    agent.SetDestination(Target.position);
                }
            }
        }        
        AnimateWheel();
        SteerFrontWheel();
    }




    void AnimateWheel()
    {
        float speed = agent.velocity.magnitude;
        foreach(GameObject wheel in Wheels)
        {
            wheel.transform.Rotate(Vector3.right * speed * rotationSpeed * Time.deltaTime); 
        }
    }

    void SteerFrontWheel()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
        float steeringInput = localVelocity.x;
        float targetSteerAngle = Mathf.Clamp(steeringInput * maxSteerAngle, -maxSteerAngle, maxSteerAngle);

        if(Mathf.Abs(steeringInput)  < 1.0f)
        {
            targetSteerAngle = 0f;
        }

        Quaternion targetRotation = Quaternion.Euler(0, targetSteerAngle, 0);

        frontLeftWheel.localRotation = Quaternion.Lerp(frontLeftWheel.localRotation, targetRotation, steerSpeed * Time.deltaTime);
        frontRightWheel.localRotation = Quaternion.Lerp(frontRightWheel.localRotation,targetRotation,steerSpeed * Time.deltaTime);
    }



    //Debug helper for visualize rays

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Vector3 origin = transform.position + Vector3.up * 0.5f;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(origin, transform.forward * detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(origin - transform.right * sideOffset, transform.forward * detectionRange);
        Gizmos.DrawRay(origin + transform.right * sideOffset,transform.forward * detectionRange);   
    }
}
