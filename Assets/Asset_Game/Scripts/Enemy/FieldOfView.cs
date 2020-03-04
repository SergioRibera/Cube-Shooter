using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TypeFieldOfView { Vision, Attack }
public class FieldOfView : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    public float meshResolution = 10;
    public int edgeResolveIterations = 4;
    public float edgeDstThreshold = 0.5f;

    public float maskCutawayDst = .1f;

    EnemyController IaControl;
    Mesh viewMesh;

    void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";

        InvokeRepeating("FindTargets", 0, 0.5f);
        IaControl = (GetComponent<EnemyController>() == null) ? GetComponentInParent<EnemyController>() : GetComponent<EnemyController>();
    }

    void FindTargets()
    {
        visibleTargets.Clear();
        //IaControl.player = null;
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                    IaControl.player = target;
                    IaControl.move = true;
                }else{
                    //Si hay un obstáculo en el camino
                    IaControl.player = null;
                    IaControl.move = false;
                }
            }
        }

        if(targetsInViewRadius.Length == 0) {
            //Si no hay targets en el área
            IaControl.player = null;
            IaControl.move = false;
        }
    }
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}