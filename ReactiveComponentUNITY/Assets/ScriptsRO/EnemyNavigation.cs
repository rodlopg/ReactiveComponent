using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
public class EnemyNavigation : MonoBehaviour
{
    public List<Transform> wayPoint;
    Transform playerTransform;
    NavMeshAgent navMeshAgent;
    bool roam = true;

    public int index = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectsWithTag("Player")[0].transform;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P) == true)
        {
            navMeshAgent.SetDestination(playerTransform.position);
            roam = false;
        }
        if (roam)
        {
            Walking();
        }
        
    }

    private void Walking()
    {



        if (wayPoint.Count == 0)
        {

            return;
        }


        float distanceToWaypoint = Vector3.Distance(wayPoint[index].position, transform.position);

        // Check if the agent is close enough to the current waypoint
        if (distanceToWaypoint <= 2)
        {

            index = (index + 1) % wayPoint.Count;
        }

        // Set the destination to the current waypoint
        navMeshAgent.SetDestination(wayPoint[index].position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        roam = true;
    }

}
