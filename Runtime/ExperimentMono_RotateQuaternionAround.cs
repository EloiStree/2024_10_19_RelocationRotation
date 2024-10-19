using Eloi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ExperimentMono_RotateQuaternionAround : MonoBehaviour
{
    public Transform m_fromDirection;
    public Transform m_toDirection;
    public Vector3 euler = new Vector3(50, 0, 0);
    public Transform m_whatToRotate;
    public Transform m_aroundWhat;
    public Transform m_newPositionDebugDraw;
    public Transform m_newPositionDebug;


    public Quaternion rotationToApply;
    public Vector3 pivot;
    public Quaternion currentRotationPoint;
    public Vector3 currentPoint;
    public Vector3 rightPoint;
    public Vector3 forwardPoint;
    public Vector3 upPoint;
    public Vector3 currentPointRelocate;
    public Vector3 rightPointRelocated;

    [ContextMenu("Rotate Once")]
    public void RotateOnce() { 

        Vector3 fromDirection = m_aroundWhat.position - m_fromDirection.position;
        Vector3 toDirection = m_aroundWhat.position - m_toDirection.position;
        rotationToApply = Quaternion.FromToRotation(fromDirection, toDirection);
        //rotationToApply = Quaternion.Euler(euler);


        pivot = m_aroundWhat.position;
        currentRotationPoint = m_whatToRotate.rotation;
        currentPoint = m_whatToRotate.position;
        rightPoint =currentPoint+ currentRotationPoint * Vector3.right;
        forwardPoint = currentPoint + currentRotationPoint * Vector3.forward;
        upPoint = currentPoint + currentRotationPoint * Vector3.up;

        Debug.DrawLine(currentPoint,  rightPoint, Color.red, 5);
        Debug.DrawLine(currentPoint,  forwardPoint, Color.blue, 5);
        Debug.DrawLine(currentPoint, upPoint, Color.green, 5);

        currentPointRelocate= Eloi.RelocationUtility.RotatePointAroundPivot(currentPoint, pivot, rotationToApply);
        rightPointRelocated= Eloi.RelocationUtility.RotatePointAroundPivot(rightPoint, pivot, rotationToApply);

        Debug.DrawLine(currentPointRelocate, rightPointRelocated, Color.red, 5);
        Vector3 centerToPointDir = currentPointRelocate - pivot;
        Vector3 newRigthDirection = rightPointRelocated - currentPointRelocate;
        Vector3 newForwardDirection = Vector3.Cross(newRigthDirection,centerToPointDir).normalized;
        Debug.DrawLine(currentPointRelocate, currentPointRelocate+newForwardDirection, Color.blue, 5);

        //Quaternion newRotation = Quaternion.LookRotation(newForwardDirection, Vector3.Cross(newForwardDirection, newRigthDirection));
        Quaternion newRotation = Quaternion.LookRotation(newForwardDirection, centerToPointDir);


        m_newPositionDebugDraw.position = currentPointRelocate;
        m_newPositionDebugDraw.rotation = newRotation;


        
        RelocationUtility.RotateAroundPivot(currentPoint, currentRotationPoint, pivot, rotationToApply
            , out Vector3 newPositionDebug, out Quaternion newRotationDebug);

        m_newPositionDebug.position = newPositionDebug;
        m_newPositionDebug.rotation = newRotationDebug;


    }

    public bool m_applyRotation;
    // Update is called once per frame
    void Update()
    {
        RotateOnce();
    }
}
