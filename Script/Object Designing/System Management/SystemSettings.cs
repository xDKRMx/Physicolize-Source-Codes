using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;

public class SystemSettings : MonoBehaviour
{
    public GameObject System_Panel;
    public PhysicMaterial Environment_Material;
    [Header("Environmental Object")]
    public GameObject[] Environmental_Objects;
    [Header("Gravity Variables")]
    public Toggle Gravity_toggle;
    private string Axis;
    public Vector3 Gravity_Value;
    private Vector3 default_value;
    public bool IsGravity;
    public TMP_InputField X_Axis;
    public TMP_InputField Y_Axis;
    public TMP_InputField Z_Axis;
    [Header("Environment Friction")]
    public PhysicMaterial Enviroment_Material;
    bool FrictionControl;
    public Toggle Friction_Toggle;
    [Header("Dynamic Friction")]
    [SerializeField]
    float Dynamic_Friction;
    public TMP_InputField Dynamic_Friction_panel;
    [Header("Static Friction")]
    [SerializeField]
    float Static_Friction;
    public TMP_InputField Static_Friction_Panel;
    [Header("Object Drag")]
    public bool Drag_Control;
    [SerializeField]
    float Drag;
    public TMP_InputField Drag_Field;
    public Toggle Drag_Toggle;
    void Start()
    {
        //Gravity Adjustment
        Gravity_Value = Physics.gravity;
        default_value = Gravity_Value;
        IsGravity = true;
        StartCoroutine(Gravity_activity_IE(true));
        //Friction Adjustment
        Dynamic_Friction = Enviroment_Material.dynamicFriction;
        Static_Friction = Enviroment_Material.staticFriction;
        Friction_Toggle.isOn = true;
        Dynamic_Friction_panel.interactable = true;
        Static_Friction_Panel.interactable = true;
        Dynamic_Friction_panel.text = Dynamic_Friction.ToString() + " N";
        Static_Friction_Panel.text = Static_Friction.ToString() + " N";
        //Drag Adjustment
        Drag_Control = true;
        Drag = Environmental_Objects[0].GetComponent<Rigidbody>().drag;
        Drag_Field.text = Drag.ToString();
        Drag_Field.interactable = true;
        Drag_Toggle.isOn = true;
    }
    #region GET BACK
    //Get back to lobby
    public void Get_Back_Lobby()
    {
        SceneManager.LoadScene(0);
    }
    #endregion
    #region
    //CONVERT STRING TYPE FROM THE PANEL TO FLOAT TYPE
    public static float ParseValue(string value)
    {

        //If value consists of minuses or commas
        if (value.Count(c => c == '-') >= 1)
        {
            value = "-" + value.Replace("-", "");
        }
        
        if (value.Count(c => c == '.') > 1)
        {
            int firstCommaIndex = value.IndexOf('.');
            value = value.Substring(0, firstCommaIndex + 1) + value.Substring(firstCommaIndex + 1).Replace(".", "");
          
        }
        if (value.All(c => c == '-' || c == '.'))
        {
            value = "0";
        }
        //If value is a NEWTON force
        if (value.Contains("N")) value = value.Replace("N", "");
        else if (value.Contains("F")) value = value.Replace("F", "");
        return string.IsNullOrEmpty(value) ? 0 : float.Parse(value);
    }
    #endregion

    //Gravity Processes
    public void Axis_Gravity(string _Axis)
    {
        if (_Axis != "")
        {
            Axis = _Axis;
        }
    }
    public void Gravity_value(string Gravity_value)
    {
        float Gravity = ParseValue(Gravity_value);
        if (Gravity > 100) Gravity = 100f;
        switch (Axis)
        {
            case "X":
                Gravity_Value = new Vector3(Gravity, Gravity_Value.y, Gravity_Value.z);
                X_Axis.text = Gravity_Value.x.ToString();
                break;
            case "Y":
                Gravity_Value = new Vector3(Gravity_Value.x, Gravity, Gravity_Value.z);
                Y_Axis.text = Gravity_Value.y.ToString();
                break;
            case "Z":
                Gravity_Value = new Vector3(Gravity_Value.x, Gravity_Value.y, Gravity);
                Z_Axis.text = Gravity_Value.z.ToString();
                break;
        }
        
        Physics.gravity = Gravity_Value;
        if(Gravity_Value == Vector3.zero)
        {
            IsGravity = false;
            StartCoroutine(Gravity_activity_IE(false));

        }
    }
    public void Gravity_Activity(bool Gravity_Active)
    {
        
        if (Gravity_Active)
        {
            IsGravity = true;
            Gravity_Value = default_value;
            Physics.gravity = Gravity_Value;
            StartCoroutine(Gravity_activity_IE(true));
        }
        else
        {
            IsGravity = false;
            Gravity_Value = Vector3.zero;
            Physics.gravity = Gravity_Value;
            StartCoroutine(Gravity_activity_IE(false));
        }
    }
    IEnumerator Gravity_activity_IE(bool active)
    {
        yield return new WaitForSeconds(0.3f);
        Gravity_toggle.isOn = active;
        X_Axis.interactable = active;
        Y_Axis.interactable = active;
        Z_Axis.interactable = active;
        if (active == false)
        {
            X_Axis.text = "0";
            Y_Axis.text = "0";
            Z_Axis.text = "0";
        }
       else
       {
           
            X_Axis.text = Gravity_Value.x.ToString();
            Y_Axis.text = Gravity_Value.y.ToString();
            Z_Axis.text = Gravity_Value.z.ToString();
       }
       
    }
    //Friction Processes
    public void IsFriction(bool _FricitonControl)
    {
        FrictionControl = _FricitonControl;
        Enviroment_Friction_Control(_FricitonControl);
    }

    void Enviroment_Friction_Control(bool _FrictionControl)
    {
        FrictionControl = _FrictionControl;
        Friction_Toggle.isOn = FrictionControl;
        Dynamic_Friction_panel.interactable = FrictionControl;
        Static_Friction_Panel.interactable = FrictionControl;

        if (FrictionControl && Dynamic_Friction == 0 && Static_Friction == 0)
        {
            Dynamic_Friction = 12;
            Static_Friction = 15;
        }

        Enviroment_Material.staticFriction = FrictionControl ? Static_Friction : 0;
        Enviroment_Material.dynamicFriction = FrictionControl ? Dynamic_Friction : 0;

        Dynamic_Friction_panel.text = Dynamic_Friction.ToString() + " N";
        Static_Friction_Panel.text = Static_Friction.ToString() + " N";
    }

    public void DynamicFrictionValue(string Dynamic_Value)
    {
        if (FrictionControl)
        {
            Dynamic_Friction = ParseValue(Dynamic_Value);
            Enviroment_Material.dynamicFriction = Dynamic_Friction;
            Dynamic_Friction_panel.text = Dynamic_Friction.ToString() + " N";
            CheckZeroFriction();
        }
    }

    public void StaticFrictionValue(string Static_Value)
    {
        if (FrictionControl)
        {
            Static_Friction = ParseValue(Static_Value);
            Enviroment_Material.staticFriction = Static_Friction;
            Static_Friction_Panel.text = Static_Friction.ToString() + " N";
            CheckZeroFriction();
        }
    }
    void CheckZeroFriction()
    {
        if (Dynamic_Friction == 0 && Static_Friction == 0)
        {
            Enviroment_Friction_Control(false);
        }
    }
    //Drag Processes
    //Drag
    public void IsDrag(bool _DragControl)
    {
        Drag_Control = _DragControl;
        if (Drag_Control) _Environment_Drag(Drag_Control);
        else _Environment_Drag(Drag_Control);

    }
    //Valueating Drag
    public void DragValue(string Drag_value)
    {

        if (Drag_Control)
        {

            float _Drag = ParseValue(Drag_value);
            Drag = _Drag;
            Drag_Field.text = Drag.ToString();
            foreach (var item in Environmental_Objects)
            {
                Rigidbody rb = item.GetComponent<Rigidbody>();
                rb.drag = Drag;
            }
            
            if (Drag == 0)
            {
                Drag_Control = false;
                _Environment_Drag(Drag_Control);
            }
        }
    }
    void _Environment_Drag(bool _DragControl)
    {
        if(_DragControl)
        {
          
            Drag = 1;
            Drag_Field.interactable = true;
            Drag_Toggle.isOn = true;
            Drag_Field.text = Drag.ToString();
        }
        else
        {
            Drag = 0;
            foreach (var item in Environmental_Objects)
            {
                Rigidbody rb = item.GetComponent<Rigidbody>();
                rb.drag = Drag;
            }
            Drag_Field.text = "0";
            Drag_Field.interactable = false;
            Drag_Toggle.isOn = false;
          
        }
    }

}

