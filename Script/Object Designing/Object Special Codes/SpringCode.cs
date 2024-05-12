using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpringCode : MonoBehaviour
{
    [Header("Spring Variables")]
    public SpringJoint springJoint;
    GameObject Spring_Object;
    public bool Is_Jointed = false;
    public bool Referance_Distance = false;
    public float Default_Distance;
    private Vector3 originalScale;
    public float currentDistance;
    public float springForce;
    [Header("Durability to the max force")]
    private float timeInMaxMinScale = 0f;
    private float maxMinScaleDuration = 0.5f;
    private void Start()
    {
        Spring_Object = this.gameObject;
        springJoint = Spring_Object.GetComponent<SpringJoint>();
        //Spring Adjustment
        springJoint.maxDistance = 0.01f;
        originalScale = Spring_Object.transform.localScale;
        springForce = 0f;
        springJoint.autoConfigureConnectedAnchor = true;
        while (springForce <= 1000)
        {
            springForce++;
            springJoint.spring = springForce;
        }
    }
    void Update()
    {
        //Spring Pull and Push Algorithms
        if (springJoint != null && springJoint.connectedBody)
        {
          
            
            if (Referance_Distance)
            {
                Referance_Distance = false;
                Vector3 _Default_Distance = springJoint.connectedBody.position- Spring_Object.transform.position;
                Default_Distance = _Default_Distance.magnitude *1.25f ;
                originalScale = Spring_Object.transform.localScale;
            }
            Vector3 distanceVector = springJoint.connectedBody.position - Spring_Object.transform.position;
            currentDistance = distanceVector.magnitude;
            float Ratio = currentDistance / Default_Distance;
            float Alteration_ratio = 0f;
            float Referance_Coefficent = 1 / 0.5f;
            Alteration_ratio = Ratio * Referance_Coefficent;
            Spring_Object.transform.localScale = new Vector3(originalScale.x, originalScale.y * Alteration_ratio, originalScale.z);
            if (currentDistance >= Default_Distance *2)
            {
                timeInMaxMinScale += Time.deltaTime;

                if (timeInMaxMinScale >= maxMinScaleDuration)
                {
                    Is_Jointed = false;
                    Destroy(springJoint.connectedBody.GetComponent<FixedJoint>());
                    Destroy(springJoint);
                    Spring_Object.GetComponentInChildren<AutoConstruct>().Is_It_Full = true;
                    Spring_Object.transform.localScale = originalScale;

                }
            }
            else
            {
                timeInMaxMinScale = 0f;
            }
        }
        else Spring_Object.transform.localScale = originalScale;
        
        //  springJoint.connectedAnchor = springJoint.connectedBody.transform.transform.position;  if (Is_Jointed && springJoint != null)
        //    {

        //        if(Referance_Distance)
        //        {
        //            Referance_Distance = false;
        //            Default_Distance = Vector3.Distance(springJoint.connectedBody.position, Spring_Object.transform.position);
        //        }
        //        springJoint.anchor = springJoint.connectedBody.transform.transform.position;
        //        springJoint.connectedAnchor = springJoint.connectedBody.transform.transform.position;
        //        Vector3 distanceVector = springJoint.connectedBody.position - Spring_Object.transform.position;
        //        currentDistance = distanceVector.magnitude;
        //        minDistance = springJoint.minDistance;
        //        maxDistance = springJoint.maxDistance;

        //        if (maxDistance > minDistance)
        //        {
        //            float normalizedDistance = Mathf.Lerp(minDistance, maxDistance, currentDistance);

        //            springForce = Mathf.Lerp(0f, maxSpringForce, normalizedDistance) * springConstant;

        //            springJoint.spring = springForce ;

        //            float sensitivity = 1f; // Duyarlýlýk ayarý
        //            float scaleFactor = Mathf.Lerp(minScale, maxScale, Mathf.Pow(normalizedDistance, sensitivity)) * springConstant* 9999f;

        //            if (!float.IsNaN(scaleFactor))
        //            {
        //                scaleFactor = Mathf.Clamp(scaleFactor, minScale, maxScale);
        //                transform.localScale = new Vector3(originalScale.x, scaleFactor, originalScale.z);

        //                if (currentDistance >= maxDistance || currentDistance <= minDistance || currentDistance >= Default_Distance)
        //                {
        //                    timeInMaxMinScale += Time.deltaTime;

        //                    if (timeInMaxMinScale >= maxMinScaleDuration)
        //                    {
        //                        Is_Jointed = false;
        //                        Destroy(springJoint.connectedBody.GetComponent<FixedJoint>());
        //                        Destroy(springJoint);

        //                    }
        //                }
        //                else
        //                {
        //                    timeInMaxMinScale = 0f;
        //                }
        //            }
        //            else
        //            {
        //                Debug.LogError("Scale factor is NaN!");
        //            }
        //        }
        //        else
        //        {
        //            Debug.LogWarning("maxDistance should be greater than minDistance! Setting maxDistance to defaultMaxDistance.");
        //            springJoint.maxDistance = defaultMaxDistance;
        //        }
        //    }


        //}
    }
}