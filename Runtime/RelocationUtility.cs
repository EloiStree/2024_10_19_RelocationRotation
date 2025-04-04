using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eloi { 

    public class RelocationUtility 
    {
        public static void GetWorldToLocal_Point(in Vector3 worldPosition, in Transform rootReference, out Vector3 localPosition)
        {
            Vector3 p = rootReference.position;
            Quaternion r = rootReference.rotation;
            GetWorldToLocal_Point(in worldPosition, in p, in r, out localPosition);
        }
        public static void GetLocalToWorld_Point(in Vector3 localPosition, in Transform rootReference, out Vector3 worldPosition)
        {
            Vector3 p = rootReference.position;
            Quaternion r = rootReference.rotation;
            GetLocalToWorld_Point(in localPosition, in p,in r, out worldPosition);
        }
        public static void GetWorldToLocal_Point(in Vector3 worldPosition, in Vector3 positionReference, in Quaternion rotationReference, out Vector3 localPosition) =>
            localPosition = Quaternion.Inverse(rotationReference) * (worldPosition - positionReference);
        
        public static void GetLocalToWorld_Point(in Vector3 localPosition, in Vector3 positionReference, in Quaternion rotationReference, out Vector3 worldPosition)=>
            worldPosition = (rotationReference * localPosition) + (positionReference);

        public static void GetWorldToLocal_DirectionalPoint(in Vector3 worldPosition, in Quaternion worldRotation, in Transform rootReference, out Vector3 localPosition, out Quaternion localRotation)
        {
            Vector3 p = rootReference.position;
            Quaternion r = rootReference.rotation;
            GetWorldToLocal_DirectionalPoint(in worldPosition, in worldRotation, in p, in r, out localPosition, out localRotation);
        }
        public static void GetLocalToWorld_DirectionalPoint(in Vector3 localPosition, in Quaternion localRotation, in Transform rootReference, out Vector3 worldPosition, out Quaternion worldRotation)
        {
            Vector3 p = rootReference.position;
            Quaternion r = rootReference.rotation;
            GetLocalToWorld_DirectionalPoint(in localPosition, in localRotation, in p, in r, out worldPosition, out worldRotation);
        }
        public static void GetWorldToLocal_DirectionalPoint(in Vector3 worldPosition, in Quaternion worldRotation, in Vector3 positionReference, in Quaternion rotationReference, out Vector3 localPosition, out Quaternion localRotation)
        {
            localRotation = Quaternion.Inverse(rotationReference)* worldRotation;
            localPosition = Quaternion.Inverse(rotationReference) * (worldPosition - positionReference);
        }
        public static void GetLocalToWorld_DirectionalPoint(in Vector3 localPosition, in Quaternion localRotation, in Vector3 positionReference, in Quaternion rotationReference, out Vector3 worldPosition, out Quaternion worldRotation)
        {
            worldRotation = rotationReference* localRotation ;
            worldPosition = (rotationReference * localPosition) + (positionReference);
        }

        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            return RotatePointAroundPivot(point, pivot, Quaternion.Euler(angles));
        }

        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            return rotation * (point - pivot) + pivot;
        }

        public static void RotateAroundPivot(Transform whatToRotate, Transform centerRotation, Quaternion rotationToApply)
        {
            RotateAroundPivot(whatToRotate.position, whatToRotate.rotation, centerRotation.position, rotationToApply, out Vector3 newPosition, out Quaternion newRotation);
            whatToRotate.position = newPosition;
            whatToRotate.rotation = newRotation;
        }
        public static void RotateAroundPivot(Transform whatToRotate, Vector3 centerRotation, Quaternion rotationToApply)
        {
            RotateAroundPivot(whatToRotate.position, whatToRotate.rotation, centerRotation, rotationToApply, out Vector3 newPosition, out Quaternion newRotation);
            whatToRotate.position = newPosition;
            whatToRotate.rotation = newRotation;
        }

        public static void RotateAroundPivot(
            Vector3 whatToRotatePosition,
            Quaternion whatToRotateRotation,
            Vector3 centerRotation,
            Quaternion rotationToApply,
            out Vector3 newPosition, 
            out Quaternion newRotation)
            
        {
            //Rotate the right point to in aim to reconstruct the forward direction
            Vector3 rightPoint = whatToRotatePosition + whatToRotateRotation * Vector3.right;
            Vector3 currentPointRelocate = Eloi.RelocationUtility.RotatePointAroundPivot(whatToRotatePosition, centerRotation, rotationToApply);
            Vector3 rightPointRelocated = Eloi.RelocationUtility.RotatePointAroundPivot(rightPoint, centerRotation, rotationToApply);
            Vector3 centerToPointDirection = currentPointRelocate - centerRotation;
            Vector3 pointToRightDirection = rightPointRelocated - currentPointRelocate;
            Vector3 newForwardDirection = Vector3.Cross(pointToRightDirection, centerToPointDirection).normalized;
            newPosition =currentPointRelocate;
            newRotation= Quaternion.LookRotation(newForwardDirection, centerToPointDirection);

        }


        public static void RotateTargetAroundPointMath(
            Vector3 postion, Quaternion rotation ,
            Vector3 center, Quaternion rotationFrom, Quaternion rotationTo,
            out Vector3 newPosition, out Quaternion newRotation)
        {
            // Calculate the rotation difference (quaternion multiplication)
            Quaternion rotationDifference = rotationTo * Quaternion.Inverse(rotationFrom);

            // Calculate the direction from the center to the object to move
            Vector3 direction = postion - center;

            // Rotate the direction by the rotation difference
            Vector3 rotatedDirection = rotationDifference * direction;

            // Update the position of the object
            newPosition = center + rotatedDirection;

            // Apply the rotation difference to the object
            newRotation = rotationDifference * rotation;

        }

        public static  void RotateTargetAroundPointMath(Transform whatToMove, Vector3 center, Quaternion rotationFrom, Quaternion rotationTo)
        {
            RotateTargetAroundPointMath(
                whatToMove.position, whatToMove.rotation, 
                center, rotationFrom, rotationTo, 
                out Vector3 newPosition, out Quaternion newRotation);
            whatToMove.position = newPosition;
            whatToMove.rotation = newRotation;
            
        }


        public static void RotateTargetAroundPointByCreatingEmpyPoint(Transform whatToMove, Vector3 centroide, Quaternion rotationFrom, Quaternion rotationTo)
        {
            Quaternion toRotate = rotationTo * Quaternion.Inverse(rotationFrom);

            Transform p = whatToMove.parent;
            GameObject g = new GameObject("t");
            Transform t = g.transform;
            t.position = centroide;

            whatToMove.parent = t;
            t.rotation *= toRotate;
            whatToMove.parent = p;
            if (Application.isPlaying)
               GameObject.DestroyImmediate(g);
            else
                GameObject.Destroy(g);
        }
    }
}
