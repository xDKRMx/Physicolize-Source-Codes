using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraProcess : MonoBehaviour
{
    [Header("Camera Variables")]
    public GameObject[] The_Cameras;
    public GameObject Active_Camera;
    public int Current_Index = 0;
    public TextMeshProUGUI Camera_Text;
    float Adding_Y_angle ;
    [Header("Camera Zoom")]
    public Slider zoomSlider;
    [Header("Camera Position")]
    public GameObject[] Max_Points;
    public GameObject[] Min_Points;
    public GameObject Selected_Max_Point;
    public GameObject Selected_Min_Point;
    public Slider PositionSlider;
    [Header("Camera Rotation")]
    public float Max_Rotation_Angle = -30f;
    public float Min_Rotation_Angle =  30f;
    public Slider RotationSlider;
    [Header("Axis Indicators")]
    public GameObject[] Axis_Indicators;
    [Header("Scripts")]
    public Building Build;
    private void Start()
    {
        if (The_Cameras.Length > 0)
        {
            Active_Camera = The_Cameras[0];
            ActivateSelectedCamera();
        }
    }
    //Activate the Camera
    private void ActivateSelectedCamera()
    {
        Active_Camera.SetActive(true);
        Build.Active_Camera = Active_Camera.GetComponent<Camera>();
        Camera_Text.text = "Camera " + (Current_Index+1);
        //Calling default zoom value
        SetZoomSliderValue();

        SetPositionSliderValue();

        SetRotationSliderValue();
    }
    //Changing the Camera 
    public void CameraChange(bool isNext)
    {
        if (isNext)
        {
            Current_Index = (Current_Index + 1) % The_Cameras.Length;
        }
        else
        {
            Current_Index = (Current_Index - 1 + The_Cameras.Length) % The_Cameras.Length;
        }
        Active_Camera.SetActive(false);
        Active_Camera = The_Cameras[Current_Index];
        ActivateSelectedCamera();
    }

  
    //Zooming the Camera
    public void ZoomingCamera(float zoomingValue)
    {
        // Interpolate the zoom value between field of view values
        if (!Active_Camera.GetComponent<Camera>().orthographic)
        {
            float newFOV = Mathf.Lerp(30f, 60f, zoomingValue);
            Active_Camera.GetComponent<Camera>().fieldOfView = newFOV;
        }
    }
    //Zoom Slider default value
    void SetZoomSliderValue()
    {
        float normalizedValue = Mathf.InverseLerp(30f, 60f, Active_Camera.GetComponent<Camera>().fieldOfView);
        zoomSlider.value = normalizedValue;
    }

    //Locating The Camera
    public void PositionCamera(float zoomingValue)
    {
        //Locate the Camera by the Slider value
        Selected_Max_Point = Max_Points[Current_Index];
        Selected_Min_Point = Min_Points[Current_Index];
        float normalizedPositionValue = Mathf.Lerp(Selected_Min_Point.transform.position.y, Selected_Max_Point.transform.position.y, zoomingValue);
        Vector3 newPosition = new Vector3(Active_Camera.transform.position.x,
                                          normalizedPositionValue,
                                          Active_Camera.transform.position.z);

        Active_Camera.transform.position = newPosition;

    }

    // Position Slider default value
    void SetPositionSliderValue()
    {
        //Set the default value of the slider by the camera position
        Selected_Max_Point = Max_Points[Current_Index];
        Selected_Min_Point = Min_Points[Current_Index];
        float normalizedPositionValue = Mathf.InverseLerp(Selected_Min_Point.transform.position.y, Selected_Max_Point.transform.position.y, Active_Camera.transform.position.y);
        Vector3 newPosition = new Vector3(Active_Camera.transform.position.x,
                                       Mathf.Lerp(Selected_Min_Point.transform.position.y, Selected_Max_Point.transform.position.y, normalizedPositionValue),
                                       Active_Camera.transform.position.z);
        Active_Camera.transform.position = newPosition;
        PositionSlider.value = normalizedPositionValue;
    }


    //Rotating The Camera
    public void RotationCamera(float zoomingValue)
    {
        float normalizedPositionValue = Mathf.Lerp(30, -30, zoomingValue);
        Adding_Y_angle = -90 * Current_Index;
        Active_Camera.transform.localRotation = Quaternion.Euler(normalizedPositionValue, Adding_Y_angle, 0f);
        if(Current_Index % 2 == 0) Axis_Indicators[Current_Index].transform.localRotation = (Current_Index == 0) ?  Quaternion.Euler(-normalizedPositionValue, Adding_Y_angle, 0f) : Quaternion.Euler(normalizedPositionValue, Adding_Y_angle, 0f);
        else Axis_Indicators[Current_Index].transform.localRotation = (Current_Index == 1) ?  Quaternion.Euler(0f, -Adding_Y_angle, -normalizedPositionValue) : Quaternion.Euler(0f, -Adding_Y_angle, normalizedPositionValue);
    }

    // Rotating Slider default value
    void SetRotationSliderValue()
    {
        float Current_Rotation = Active_Camera.transform.localRotation.x;
        float normalizedPositionValue = Mathf.InverseLerp(30, -30, Current_Rotation);
        RotationSlider.value = normalizedPositionValue;
      
    }
}
