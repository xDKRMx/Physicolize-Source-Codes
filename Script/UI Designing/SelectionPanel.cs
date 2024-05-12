using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class SelectionPanel : MonoBehaviour
{
    //Open the Platform variations panel
    public GameObject selectionpanel;
    public GameObject Panel_Indicator;
    //Open the Simulation Settings panel
    public GameObject Management_btn;
    //Simulation Settings Panel Animation
    public Animator Management_Animator;
    public void OpenSelectionPanel()
    {
        selectionpanel.SetActive(!selectionpanel.activeSelf);
        bool isOpen = selectionpanel.activeSelf;
        float targetRotation = isOpen ? 90f : 0f;
        StartCoroutine(RotateUIElement(targetRotation, Panel_Indicator));
    }

    IEnumerator RotateUIElement(float targetRotation, GameObject Target_Object)
    {
        float currentRotation = Target_Object.transform.rotation.eulerAngles.z;
        float t = 0f;
        float rotationSpeed = 100f; 
        while (t < 1f)
        {
            t += Time.deltaTime * rotationSpeed;
            float newRotation = Mathf.Lerp(currentRotation, targetRotation, t);
            Target_Object.transform.rotation = Quaternion.Euler(0f, 0f, newRotation);
            yield return null;
        }
    }
    public void OpenManagementPanel()
    {

        float currentAngel = Management_btn.transform.rotation.eulerAngles.z;
        float targetRotation = currentAngel == 0f ? 180f : 0f;
        bool Management_animation_control = targetRotation == 180f ? true : false;
        StartCoroutine(RotateUIElement(targetRotation, Management_btn));
        
        
        if(Management_animation_control)
        {
            Management_Animator.SetBool("IsOn", true);
            Management_Animator.SetBool("IsOff", false);
           
        }
        else
        {
            Management_Animator.SetBool("IsOn", false);
            Management_Animator.SetBool("IsOff", true);
        }
    }
}
