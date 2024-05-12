using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddingForceTorque : MonoBehaviour
{
    [Header("Other Script")]
    public Building Building;
    public SimulationDuration Simulation_Duration;
    public ObjectAdjustment Obj_adj;
    [Header("Force and Torque UI Panel")]
    Camera _Camera;
    RaycastHit hit;
    public TextMeshProUGUI[] _Resultant_text;
    public Button Add_Force_btn;
    public Button Add_Torque_btn;
    public GameObject[] Apply_Force_Buttons;
    [Header("Force Camera Optimization")]
    public Camera[] Torque_Force_Cameras;
   public bool _Overview_Changed = false;
    [Header("Force and Torque Default Variables")]
    public Vector3 Force_Value;
    public Vector3 Torque_Value;
    private string _Axis;
    public GameObject Force_Overview_Object;
    public GameObject Torque_Overview_Object;
    [Header("Force and Torque Environment Variables")]
    public Transform Referance_Point;
    public GameObject Force_Object_prefab;
    public GameObject Torque_Object_prefab;
    public GameObject Force_Object;
    public GameObject Torque_Object;
    GameObject Active_Force;
    public Material Valid_Material;
    public Material Default_Material;
    public bool Is_it_On_SelectedObject;
    public GameObject Selected_Object;
    public bool Is_It_Enabled;
    public GameObject Force_Indicator_Point;
    [Header("Adding Force Variables")]
    public GameObject Force_direction_indicator;
    public GameObject Force_direction_indicator_Parent;
    public float Impact_Time;
    bool Is_It_Infinity;
    public TMP_InputField Time_text;
    private void Start()
    {
        foreach (var item in Torque_Force_Cameras)
        {
            item.enabled = _Overview_Changed;
        }
    }
    private void Update()
    {
        _Camera = Building.Active_Camera;
        if (Force_Object != null) Active_Force = Force_Object;
        else if (Torque_Object != null) Active_Force = Torque_Object;
        else Active_Force = null;

        if (Force_Value != Vector3.zero) Add_Force_btn.interactable = true;
        else Add_Force_btn.interactable = false;
        if (Torque_Value != Vector3.zero) Add_Torque_btn.interactable = true;
        else Add_Torque_btn.interactable = false;
        if (Building.Current_Object == null && Is_It_Enabled)
        {
            Ray Ray_Camera = _Camera.ScreenPointToRay(Input.mousePosition);
            int layerMask = 1 << 8 | 1 << 9 | 1 << 11;
            if (Physics.Raycast(Ray_Camera, out hit, 500f, layerMask))
            {
                Vector3 offset = new Vector3(0, 0.6f, 0);
                Active_Force.transform.position = hit.point + offset;
                if (hit.transform.gameObject.layer == 8 || hit.transform.gameObject.layer == 11)
                {
                    Is_it_On_SelectedObject = true;
                    if (hit.collider.gameObject.GetComponent<ParentIndicator>() != null) Selected_Object = hit.transform.gameObject;
                    else if (hit.collider.gameObject.GetComponentInParent<ParentIndicator>() != null) Selected_Object = hit.collider.GetComponentInParent<ParentIndicator>().gameObject;
                    else Selected_Object = null;
                    MeshRenderer[] materials = Active_Force.GetComponentsInChildren<MeshRenderer>();
                    foreach (var item in materials)
                    {
                        item.material = Valid_Material;
                    }
                }
                else
                {
                    Is_it_On_SelectedObject = false;
                    MeshRenderer[] materials = Active_Force.GetComponentsInChildren<MeshRenderer>();
                    foreach (var item in materials)
                    {
                        item.material = Default_Material;
                    }
                }
            }
        }
        else if (Building.Current_Object != null && Is_It_Enabled)
        {
            Object_Destroy();
        }
        if (Is_It_Enabled && Is_it_On_SelectedObject && Input.GetMouseButton(1) && Selected_Object != null)
        {

            //If add Force or Add Torque object enabled Force or torque it to the selected Object
            Applying_Force_Torque();
            
        }
        
        if (Is_It_Enabled && Is_it_On_SelectedObject && Force_Object != null) Apply_Force_Buttons[0].SetActive(true);
        else if (Is_It_Enabled && Is_it_On_SelectedObject && Torque_Value != null) Apply_Force_Buttons[1].SetActive(true);
        else
        {
            foreach (var item in Apply_Force_Buttons) item.SetActive(false);

        }
    }
    public void Applying_Force_Torque()
    {
        if (!Selected_Object.GetComponent<ALLforces>()) Selected_Object.AddComponent<ALLforces>();
        ALLforces Adding_Force = Selected_Object.GetComponent<ALLforces>();
        Adding_Force.Object_itself = Selected_Object;
        if (Is_It_Infinity) Impact_Time = Mathf.Infinity;
        else Impact_Time = SystemSettings.ParseValue(Time_text.text);
        if (Impact_Time <= 0) Impact_Time = 0.5f;

        Adding_Force.Impacting_time = Impact_Time;
        Adding_Force.Impacting_Force_Direction_Indicator = Force_direction_indicator;
        Adding_Force.Indicator_Parent = Force_direction_indicator_Parent;
        Adding_Force.Simulation_Duration = Simulation_Duration;
        Adding_Force.Obj_adj = Obj_adj;
        if (Force_Object != null)
        {
            Adding_Force.Force_Direction.Add(Force_Value);
            Adding_Force.Impacting_Object(true);

        }
        else if (Torque_Value != null)
        {
            Adding_Force.Torque_Direction.Add(Torque_Value);
            Adding_Force.Impacting_Object(false);
        }
        Object_Destroy();
    }
    public void Finding_Axis(string Axis)
    {
        _Axis = Axis;

    }
    public void Typing_Force(string _Force_Value)
    {
        //Adjust the force or torque value by inputting th axisses of the force or torque panel
        float _Value_flt = SystemSettings.ParseValue(_Force_Value);
        switch (_Axis)
        {
            case "X_F":
                Force_Value.x = _Value_flt;
                break;
            case "Y_F":
                Force_Value.y = _Value_flt;
                break;
            case "Z_F":
                Force_Value.z = _Value_flt;
                break;
            case "X_T":
                Torque_Value.x = _Value_flt;

                break;
            case "Y_T":
                Torque_Value.y = _Value_flt;
                break;
            case "Z_T":
                Torque_Value.z = _Value_flt;
                break;
        }
        //Showing the direction of force or torque on the overview panel
        if (_Axis.Contains("F"))
        {
            Vector3 Vector_Rotation = Force_Value.normalized;
            float Resultant_force = Vector3.Distance(Vector3.zero, Force_Value);
            Force_Overview_Object.transform.rotation = Quaternion.LookRotation(Vector_Rotation);
            _Resultant_text[0].text = Resultant_force.ToString("F2");
            if (Force_Object != null) Force_Object.transform.rotation = Quaternion.LookRotation(Vector_Rotation);
           
        }
        else
        {
            Vector3 Vector_Rotation = Torque_Value.normalized;
            float Resultant_force = Vector3.Distance(Vector3.zero, Torque_Value);
            Torque_Overview_Object.transform.rotation = Quaternion.LookRotation(Vector_Rotation);
            _Resultant_text[1].text = Resultant_force.ToString("F2");
            if (Torque_Object != null) Torque_Object.transform.rotation = Quaternion.LookRotation(Vector_Rotation);
        }
        //If there is no change on the Torque and force Indicators Disable The Camera for Optimization
        //We enable the camera for a while to show the changw on torque and force
        _Overview_Changed = true;
        StartCoroutine(Camera_Enabling_Cooldown());
        
    }
    IEnumerator Camera_Enabling_Cooldown()
    {
        foreach (var item in Torque_Force_Cameras)
        {
            item.enabled = _Overview_Changed;
        }
        yield return new WaitForSeconds(1f);
        _Overview_Changed = !_Overview_Changed;
        foreach (var item in Torque_Force_Cameras)
        {
            item.enabled = _Overview_Changed;
        }
       
    }
    public void AddingItInEnvironment(string AddingType)
    {
        //Creating the tengible force or torque object in order to select the object you will force
        if (AddingType == "Force")
        {
            Vector3 Vector_Rotation = Force_Value.normalized;
            if (Force_Object == null) if (Force_Value != Vector3.zero) Force_Object = Instantiate(Force_Object_prefab, Referance_Point.transform.position, Quaternion.LookRotation(Vector_Rotation));
                else Force_Object.transform.rotation = Quaternion.LookRotation(Vector_Rotation);

            if (Torque_Object != null) Destroy(Torque_Object);
        }
        else if (AddingType == "Torque")
        {
            Vector3 Vector_Rotation = Torque_Value.normalized;
            if (Torque_Object == null) if (Torque_Value != Vector3.zero) Torque_Object = Instantiate(Torque_Object_prefab, Referance_Point.transform.position, Quaternion.LookRotation(Vector_Rotation));

                else Torque_Object.transform.rotation = Quaternion.LookRotation(Vector_Rotation);
            if (Force_Object != null) Destroy(Force_Object);
        }
        Is_It_Enabled = true;
    }
    void Object_Destroy()
    {

        if (Force_Object != null) Destroy(Force_Object);
        else if (Torque_Value != null) Destroy(Torque_Object);
        Is_It_Enabled = false;
    }
    public void Infinity(bool Infinityness)
    {
        if (Infinityness) Is_It_Infinity = true;
        else Is_It_Infinity = false;
    }
}
