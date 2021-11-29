using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : Bullet
{
    public Transform target;
    //추적을 위한 navmesh
    NavMeshAgent nav;

    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        Destroy(gameObject, 10.0f); //플레이어가 계속 피하면 10초뒤 사라짐.
    }
    void Update()
    {
        nav.SetDestination(target.position);
    }
}
