using UnityEngine;
using System.Collections;

// entity will first navigate to point 1
// once at patrol location, there are two possible states, looking and moving
// looking:
//      the entity changes their orientation around the y axis to face in different directions.  Direction changes every 2 to 5 seconds.
// moving: 
//      the entity moves from current point to next point.  If the next point is a terminal point, there is a small chance that the entity will only move part of the way
//      and then return to the current point.
public class EnemyPatrol : MonoBehaviour
{
    public Transform[] points;
    public float chanceToBreak = 0.05f;
    Transform player;
    PlayerHealth playerHealth;
    NavMeshAgent nav;
    State currentState = State.LOOKING;
    State lastMovingState = State.MOVINGUP;
    float lookingTime = 0.0f;
    int currentPoint = 0;
    enum State
    {
        LOOKING,
        MOVINGUP,
        MOVINGDOWN,
    };

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        nav = GetComponent<NavMeshAgent>();
    }


    void Update()
    {
        if (/*enemyHealth.currentHealth > 0 && */playerHealth.currentHealth > 0)
        {
            if (currentState == State.MOVINGUP)
            {
                // check if done navigating
                if (nav.remainingDistance < nav.stoppingDistance)
                {
                    // check if at terminal point
                    if (isAtTerminalPoint())
                    {
                        currentState = State.LOOKING;
                        lastMovingState = State.MOVINGUP;
                    }
                    else
                    {
                        nav.SetDestination(points[currentPoint + 1].position);
                        currentPoint += 1;
                    }

                }
            }
            else if (currentState == State.MOVINGDOWN)
            {
                // check if done navigating
                if (nav.remainingDistance < nav.stoppingDistance)
                {
                    // check if at terminal point
                    if (isAtTerminalPoint())
                    {
                        currentState = State.LOOKING;
                        lastMovingState = State.MOVINGDOWN;
                    }
                    else
                    {
                        nav.SetDestination(points[currentPoint - 1].position);
                        currentPoint -= 1;
                    }
                }
            }
            else if (currentState == State.LOOKING)
            {
                // timer management
                if (lookingTime <= 0.0f)
                {
                    lookingTime = 0.0f;
                }
                else
                {
                    lookingTime -= Time.deltaTime;
                }

                if (lookingTime == 0.0f)
                {
                    // if timer has finished, change look direction and restart timer or start moving
                    Turn();
                    int move = Random.Range(1, 4);
                    if (move == 1)// start moving
                    {
                        if (lastMovingState == State.MOVINGDOWN)
                        {
                            currentState = State.MOVINGUP;
                            nav.SetDestination(points[1].position);
                            currentPoint = 1;
                        }
                        else
                        {
                            currentState = State.MOVINGDOWN;
                            nav.SetDestination(points[points.GetLength(0) - 2].position);
                            currentPoint = points.GetLength(0) - 2;
                        }
                    }
                    else
                    {
                        // keep looking with new timer
                        lookingTime = Random.Range(2.0f, 4.0f);
                    }
                }
            }
        }
        else
        {
            nav.enabled = false;
        }       
    }
    void Turn()
    {
        Vector3 rotation = nav.gameObject.transform.rotation.eulerAngles;
        // get random rotation
        nav.gameObject.transform.rotation = Quaternion.Euler(rotation.x, Random.rotation.eulerAngles.y, rotation.z);
    }
    bool isAtTerminalPoint()
    {
        return Vector3.Distance(nav.destination, points[0].position) < nav.stoppingDistance || 
            Vector3.Distance(nav.destination, points[points.GetLength(0) - 1].position) < nav.stoppingDistance;        
    }
}
