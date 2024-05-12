using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Building : MonoBehaviour
{
    [Header("SCRIPTS")]
    public ObjectAdjustment obj_ad;
    Building Obj_Building;
    [Header("MAIN SIMULATION")]
    public GameObject Main_Object;
    //OBJECT VARIABLE
    [Header("OBJECT VARIABLE")]
    public GameObject Selected_Object;
    public GameObject Transparent_Object;
    public GameObject Current_Object;
    public bool Multiple_Mesh;
    public bool compatible_rotation;
    float difference;
    float absolute_bottom;
    float AbsoluteCenter;
    //CREATION OKAY
    [Header("CREATION OKAY")]
    List<Material> default_material = new List<Material>();
    bool Create_Object_Okay;
    public Material Invalid_Place;
    public GameObject Build_Object_btn;
    // VALID PLACE CONTROL 
    Vector3 Postion_Control_Ray ;
    Vector3 Position_Direction = Vector3.down;
    RaycastHit Control_hit;
    public bool Position_Limit_Control;
    public Vector3 Limit_Position = Vector3.zero;
    //WORLD VARIABLES
    [Header("WORLD VARIABLES")]
    public Camera Active_Camera;
    RaycastHit hit;
    bool HasClicked = false;
    public bool Object_selected = false;
    public bool IsCreate = false;
    //AUTO CONSTRUCT MATERIAL
    [Header("AUTO CONSTRUCT MATERIAL")]
    public Material Valid_Auto_material;
    bool Auto_Contruct;
    GameObject Auto_Object;
    GameObject Auto_Construction_Object;
    public GameObject Holder;
    string Construction_Type = "";
    //SPRING VARIABLES
     public bool Is_It_Holder= false;
    public bool Holder_Limit_Control = false;
    //LOCAL ROTATION VARIABLES
    float AngleX;
    float AngleY;
    float AngleZ;
    Transform The_Lowest_Point;
    Transform The_Toppest_Point;
    [Header("TRANSFORM PANEL")]
    public GameObject Transform_Panel;
    public ObjectTransform Transform_Script;
    Quaternion rotation;
    Quaternion Previous_rotation;
    //INCLUDED ROTATION
    public Quaternion Alterred_Rotation;
    //INCLUDED POSITION
    public Vector3 Previous_Alterred_Position = new Vector3(999,999,999);
    public Vector3 Alterred_Position =  Vector3.zero;
    Vector3 Current_hit_rotation;
    bool Hit_Rotation_Changed = true;
    private void Start()
    {
        Previous_rotation = rotation;
        Obj_Building = this;
    }
    void Update()
    {
        rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        if (Previous_rotation != rotation)
        {
            Hit_Rotation_Changed = true;
            Previous_rotation = rotation;
        }
        if (Object_selected)
        {
            if (IsCreate)
            {
                Recreate();
                IsCreate = false;
            }
            // GENERAL TRANSFORM POSITION PROCESSES

            //There are 4  different consideration types to the gameObjects. These Types are considerred by Whether they have Children objects with Meshrenderer or not  and The pivot point of the Object is on the center point of the object  or on the edge. 
            Ray Ray_Camera = Active_Camera.ScreenPointToRay(Input.mousePosition);
            int layerMask = 1 << 8 | 1 << 9 | 1<< 11;
            if (Physics.Raycast(Ray_Camera, out hit, 500f, layerMask))
            {
                MeshRenderer[] meshRenderers = Current_Object.GetComponentsInChildren<MeshRenderer>();
                int ChildCount = Current_Object.transform.childCount;
                if (ChildCount > 1)
                {
                    

                    if (Current_Object.tag.Contains("CenterPivot"))
                    {

                        for (int i = 0; i < meshRenderers.Length; i++)
                        {

                            if (Current_Object.GetComponent<ParentCreateControl>().Plane_Intereaction == false)
                            {

                                if (Hit_Rotation_Changed && Auto_Contruct == false)
                                {
                                    Hit_Rotation_Changed = false;

                                    Current_Object.transform.rotation = rotation;
                                    Current_Object.transform.rotation *= Alterred_Rotation;
                                }

                            }
                            else
                            {
                                Current_Object.transform.rotation = Transparent_Object.transform.rotation;
                                Current_Object.transform.rotation *= Alterred_Rotation;
                            }

                        }
                        difference = AbsoluteCenter - absolute_bottom;
                        if (Auto_Contruct == false & Is_It_Holder == false)
                        {
                            Current_Object.transform.localPosition = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                            Vector3 Difference_Space = new Vector3(0f, difference + 0.15f, 0f);
                            Current_Object.transform.Translate(Difference_Space, Space.Self);

                            if(Alterred_Position != Vector3.zero)
                            {
                                Current_Object.transform.position += Alterred_Position;
                                Position_Limit_Control = Limit_Control();
                            }
                        }
                        #region Auto Construction System Overviewing
                        //AUTO CONSTRUCTION SYSTEM
                        //Whether the hit.point Interacted with the Auto Contruction Object or not
                        if (hit.collider.transform.gameObject.tag.Contains("Auto") && !hit.collider.gameObject.GetComponent<AutoConstruct>().Is_It_Full)
                        {

                            //Assign the variables of the specified Object
                            Auto_Contruct = true;
                            Auto_Object = hit.collider.GetComponentInParent<ParentIndicator>().gameObject;
                            Debug.Log(Auto_Object);
                            Auto_Construction_Object = hit.collider.transform.gameObject;
                            Construction_Type = Find_Construction_Type(hit.collider.transform.gameObject);
                           AutoConstruction("Center", Construction_Type);
                        }
                        else
                        {
                            Auto_Contruct = false;
                        }
                        #endregion
                    }
                    else if (Current_Object.tag.Contains("BottomPivot"))
                    {
                        for (int i = 0; i < meshRenderers.Length; i++)
                        {
                            if (Current_Object.GetComponent<ParentCreateControl>().Plane_Intereaction == false)
                            {
                                if (Hit_Rotation_Changed && Auto_Contruct == false)
                                {
                                    Hit_Rotation_Changed = false;
                                    Current_Object.transform.localRotation = rotation;
                                    Current_Object.transform.rotation *= Alterred_Rotation;
                                }

                            }
                            else
                            {
                                Current_Object.transform.rotation = Transparent_Object.transform.rotation;
                                Current_Object.transform.rotation *= Alterred_Rotation;
                            }

                        }
                        if (Auto_Contruct == false && Is_It_Holder == false)
                        {
                            Current_Object.transform.localPosition = new Vector3(hit.point.x, hit.point.y, hit.point.z);

                            if (Alterred_Position != Vector3.zero)
                            {
                                Current_Object.transform.position += Alterred_Position;
                                Position_Limit_Control = Limit_Control();
                            }
                        }
                        #region Auto Construction System Overviewing
                        //AUTO CONSTRUCTION SYSTEM
                        //Whether the hit.point Interacted with the Auto Contruction Object or not
                        if (hit.collider.transform.gameObject.tag.Contains("Auto") && !hit.collider.gameObject.GetComponent<AutoConstruct>().Is_It_Full)
                        {
                            //Assign the variables of the specified Object
                            Auto_Contruct = true;
                            Auto_Object = hit.collider.GetComponentInParent<ParentIndicator>().gameObject;
                            Auto_Construction_Object = hit.collider.transform.gameObject;
                            Construction_Type = Find_Construction_Type(hit.collider.transform.gameObject);
                            if (Auto_Contruct)
                            {
                                AutoConstruction("Bottom", Construction_Type);
                            }

                        }
                        else
                        {
                            Auto_Contruct = false;
                        }
                        #endregion
                    }
                    if (Current_Object.GetComponent<ParentCreateControl>().Object_Create_Okay == false)
                    {

                        for (int i = 0; i < meshRenderers.Length; i++)
                        {

                            meshRenderers[i].GetComponent<MeshRenderer>().material = Invalid_Place;
                            Create_Object_Okay = false;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < meshRenderers.Length; i++)
                        {
                            if (Auto_Contruct == false)
                            {
                                meshRenderers[i].GetComponent<MeshRenderer>().material = default_material[i];
                            }
                            Create_Object_Okay = true;
                        }
                    }

                }
                else
                {
                    if (Current_Object.GetComponent<CreateControl>().No_Intreaction == false)
                    {
                        Create_Object_Okay = false;
                        Current_Object.GetComponent<MeshRenderer>().material = Invalid_Place;
                    }
                    else
                    {

                        Create_Object_Okay = true;
                        if (Auto_Contruct == false)
                        {
                            Current_Object.GetComponent<MeshRenderer>().material = default_material[0];
                        }

                    }
                    if (Current_Object.tag.Contains("CenterPivot"))
                    {
                        if (Current_Object.GetComponent<CreateControl>().Intereaction_Plane)
                        {
                            if (Hit_Rotation_Changed)
                            {
                                Hit_Rotation_Changed = false;
                                Current_Object.transform.localRotation = Transparent_Object.transform.rotation;
                                Current_Object.transform.rotation *= Alterred_Rotation;
                            }
                            difference = AbsoluteCenter - absolute_bottom;
                            if (Auto_Contruct == false && Is_It_Holder == false)
                            {
                                Current_Object.transform.position = new Vector3(hit.point.x, hit.point.y + difference, hit.point.z);

                                if (Alterred_Position != Vector3.zero)
                                {
                                    Current_Object.transform.position += Alterred_Position;
                                    Position_Limit_Control = Limit_Control();
                                }
                            }
                        }
                        else
                        {
                            if (Hit_Rotation_Changed && !Auto_Contruct)
                            {
                                Hit_Rotation_Changed = false;
                                Current_Object.transform.localRotation = rotation;
                                Current_Object.transform.rotation *= Alterred_Rotation;
                                //Vector3 rotation_Value = new Vector3(rotation.x, rotation.y, rotation.z);
                                //Current_Object.transform.Rotate(rotation_Value, Space.Self);
                            }

                            difference = AbsoluteCenter - absolute_bottom;
                            //Adding the difference value into the local position of an current object
                            if (Auto_Contruct == false && Is_It_Holder == false)
                            {
                                Current_Object.transform.localPosition = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                                Vector3 Difference_Space = new Vector3(0f, difference + 0.1f, 0f);
                                Current_Object.transform.Translate(Difference_Space, Space.Self);

                                if (Alterred_Position != Vector3.zero)
                                {
                                    Current_Object.transform.position += Alterred_Position;
                                    Position_Limit_Control = Limit_Control();
                                }
                            }

                        }
                        #region Auto Construction System Overviewing
                        //AUTO CONSTRUCTION SYSTEM
                        //Whether the hit.point Interacted with the Auto Contruction Object or not
                        if (hit.collider.transform.gameObject.tag.Contains("Auto") && !hit.collider.gameObject.GetComponent<AutoConstruct>().Is_It_Full)
                        {
                            //Assign the variables of the specified Object
                            Auto_Contruct = true;
                            Auto_Object = hit.collider.GetComponentInParent<ParentIndicator>().gameObject;
                            Auto_Construction_Object = hit.collider.transform.gameObject;
                            
                            Construction_Type = Find_Construction_Type(hit.collider.transform.gameObject);
                            if (Auto_Contruct)
                            {
                                AutoConstruction("Center", Construction_Type);
                            }

                        }
                        else
                        {
                            Auto_Contruct = false;
                        }
                        #endregion
                    }
                    else if (Current_Object.tag.Contains("BottomPivot"))
                    {

                        if (Current_Object.GetComponent<CreateControl>().Intereaction_Plane)
                        {

                            if (Hit_Rotation_Changed)
                            {
                                Hit_Rotation_Changed = false;
                                Current_Object.transform.rotation = Quaternion.identity;
                                Current_Object.transform.rotation *= Alterred_Rotation;
                                //Vector3 rotation_Value = new Vector3(Transparent_Object.transform.rotation.x, Transparent_Object.transform.rotation.y, Transparent_Object.transform.rotation.z);
                                // Current_Object.transform.Rotate(rotation_Value, Space.Self);
                            }
                            if (Auto_Contruct == false && Is_It_Holder == false)
                            {
                                Current_Object.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);

                                if (Alterred_Position != Vector3.zero)
                                {
                                    Current_Object.transform.position += Alterred_Position;
                                    Position_Limit_Control = Limit_Control();
                                }
                            }
                        }
                        else
                        {
                            if (Hit_Rotation_Changed && !Auto_Contruct)
                            {
                                Hit_Rotation_Changed = false;
                                Current_Object.transform.localRotation = rotation;
                                Current_Object.transform.rotation *= Alterred_Rotation;
                            }

                            if (Auto_Contruct == false && Is_It_Holder == false)
                            {
                                //Adding the difference value into the local position of an current object
                                Current_Object.transform.localPosition = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                                Vector3 Difference_Space = new Vector3(0f, 0.01f, 0f);
                                Current_Object.transform.Translate(Difference_Space, Space.Self);

                                if (Alterred_Position != Vector3.zero)
                                {
                                    Current_Object.transform.position += Alterred_Position;
                                    if (Position_Limit_Control)
                                    {
                                        Current_Object.transform.position = Limit_Position;
                                    }
                                }
                            }

                        }
                        #region Auto Construction System Overviewing
                        //AUTO CONSTRUCTION SYSTEM
                        //Whether the hit.point Interacted with the Auto Contruction Object or not
                        if (hit.collider.transform.gameObject.tag.Contains("Auto") && !hit.collider.gameObject.GetComponent<AutoConstruct>().Is_It_Full)
                        {
                           
                            //Assign the variables of the specified Object
                            Auto_Contruct = true;
                            Auto_Object = hit.collider.GetComponentInParent<ParentIndicator>().gameObject;
                            Auto_Construction_Object = hit.collider.transform.gameObject;
                            Construction_Type = Find_Construction_Type(hit.collider.transform.gameObject);
                            if (Auto_Contruct)
                            {
                                AutoConstruction("Bottom", Construction_Type);
                            }

                        }
                        else
                        {
                            Auto_Contruct = false;
                        }
                        #endregion

                    }

                }
                #region Holder Object System
                if (Current_Object.transform.gameObject.CompareTag("HolderObject"))
                {
                    Is_It_Holder = true;
                }
                else
                {
                  
                    Is_It_Holder = false;
                }
                if (Is_It_Holder)
                {
                    //Rotation Adjustment 
                    if (Hit_Rotation_Changed)
                    {
                        Hit_Rotation_Changed = false;
                        Current_Object.transform.localRotation = rotation;
                        Current_Object.transform.rotation *= Alterred_Rotation;
                        if (Current_Object.transform.name.Contains("spring")) Current_Object.transform.rotation *= Quaternion.Euler(0, 0, 180);
                    }
                    //Position Adjustment 
                    Current_Object.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                    Vector3 Difference_Space;
                    difference = AbsoluteCenter - absolute_bottom + 0.1f;
                    if (Current_Object.transform.name.Contains("spring")) difference = -0.025f;
                    Difference_Space = new Vector3(0f, difference, 0f);
                    Current_Object.transform.Translate(Difference_Space, Space.Self);
                    if (Alterred_Position != Vector3.zero)
                    {
                        Current_Object.transform.position += Alterred_Position;
                        Position_Limit_Control = Limit_Control();
                    }
                    //Holder Auto Object
                    Auto_Object = hit.collider.transform.gameObject;
                    Construction_Type = "Holder-AutoObject";
                }
                #endregion

            }

            #region Game Object Replacement On The Platform 
            if (Input.GetMouseButton(1) && !HasClicked && Create_Object_Okay)
            {
                ObjectCreation();
            }
            #endregion
        }
        //WHEN CURRENT OBJECT IS NULL
        if (Current_Object == null)
        {
            Transform_Script.Object_activity = false;
            Transform_Script.TransformFinish();
            Transform_Panel.SetActive(false);
        }
        
    }
    public void ObjectCreation()
    {
        HasClicked = true;
        Object_selected = false;
        GameObject Constructed_object = Instantiate(Selected_Object, Current_Object.transform.localPosition, Current_Object.transform.localRotation);
        Constructed_object.transform.name = Selected_Object.transform.name;
        Constructed_object.transform.localScale = Current_Object.transform.localScale;
        Constructed_object.transform.SetParent(Main_Object.transform);
        GameObject Holder_Object = null;
        if (Auto_Contruct)
        {
            //If auto Contruction boolen is true Contruct the object by its Auto_Contrcution_System
            if (Construction_Type == "Object-AutoChain")
            {
                if (Auto_Object.transform.childCount > 1)
                {
                    //Calculate the lowest distance between the Current object and The Auto Objects
                    Transform[] Auto_Child_Objects = Auto_Object.GetComponentsInChildren<Transform>();
                    GameObject Connected_Object = null;
                    float The_lowest_Distance = 0.0f;
                    Rotation_Adjustment(Auto_Object);
                    float Low_Point_distance = Vector3.Distance(Constructed_object.transform.position, The_Lowest_Point.position);
                    float Top_Point_distance = Vector3.Distance(Constructed_object.transform.position, The_Toppest_Point.position);
                    bool Top_point =Math.Abs(Low_Point_distance) > Math.Abs(Top_Point_distance) ? true : false;
                    Vector3 Connected_Holder = Vector3.zero;
                    if (Top_point == false) Connected_Holder = The_Lowest_Point.position;
                    else Connected_Holder = The_Toppest_Point.position;
                    foreach (Transform item in Auto_Child_Objects)
                    {
                        if (item.name.Contains("Holder"))
                        {
                            float Distance = Vector3.Distance(item.transform.position, Constructed_object.transform.position);
                            if (The_lowest_Distance == 0.0f)
                            {
                                The_lowest_Distance = Distance;
                                Connected_Object = item.gameObject;
                            }
                            if (The_lowest_Distance >= Distance)
                            {
                                //Bounding up with the objects themselves by using Fixed Joint
                                Connected_Object = item.gameObject;

                                The_lowest_Distance = Distance;
                            }
                        }
                    }
                    FixedJoint Cons_f_j = Constructed_object.AddComponent<FixedJoint>();
                    Cons_f_j.connectedBody = Auto_Construction_Object.transform.parent.GetComponent<Rigidbody>();

                    Holder_Object = Instantiate(Holder, new Vector3(Connected_Holder.x, Connected_Holder.y, Connected_Holder.z), Holder.transform.rotation);
                    Holder_Object.transform.localRotation *= Quaternion.Euler(AngleX, AngleY, AngleZ);
                    Holder_Object.transform.SetParent(Constructed_object.transform);
                    FixedJoint Hold_f_j = Holder_Object.AddComponent<FixedJoint>();
                    Hold_f_j.connectedBody = Constructed_object.GetComponent<Rigidbody>();

                }
                else
                {
                    FixedJoint Cons_f_j = Constructed_object.AddComponent<FixedJoint>();
                    Cons_f_j.connectedBody = Auto_Construction_Object.transform.parent.GetComponent<Rigidbody>();
                }
            }
            else if (Construction_Type == "Chain-AutoHolder")
            {
                //Calculate the lowest distance between the Auto Object and the lowest and the toppest point of the Chain
                Transform[] Chain_Holder_Objects = Constructed_object.GetComponentsInChildren<Transform>();
                GameObject Connected_Object = null;
                GameObject Other_Auto_Object = null;
                float Auto_Object_Absolute_Distance = 0.0f;
                float Absolute_Distance = 0.0f;
                if(!Auto_Object.GetComponent<Rigidbody>())
                Auto_Object.AddComponent<Rigidbody>();
                Auto_Object.GetComponent<Rigidbody>().isKinematic = true;
                FixedJoint Auto_f_j = Auto_Object.AddComponent<FixedJoint>();
                foreach (Transform item in Chain_Holder_Objects)
                {
                    if (item.gameObject.name == "Holder Chain")
                    {

                        float Distance = Vector3.Distance(item.transform.position, Auto_Object.transform.position);
                        if (Absolute_Distance == 0.0f)
                        {
                            Absolute_Distance = Distance;
                            Connected_Object = item.gameObject;
                        }
                        else if (Absolute_Distance >= Distance)
                        {
                            Absolute_Distance = Distance;
                            Connected_Object = item.gameObject;
                        }
                    }
                    if (item.gameObject.name.Contains("Automatic Construct"))
                    {

                        float Distance = Vector3.Distance(item.transform.position, Auto_Object.transform.position);
                        if (Auto_Object_Absolute_Distance == 0.0f)
                        {
                            Auto_Object_Absolute_Distance = Distance;
                            Other_Auto_Object = item.gameObject;
                        }
                        else if (Auto_Object_Absolute_Distance >= Distance)
                        {
                            Auto_Object_Absolute_Distance = Distance;
                            Other_Auto_Object = item.gameObject;
                        }
                    }
                }
                //Bounding up with the objects themselves by using Fixed Joint
                if (Connected_Object != null)
                {

                    FixedJoint Con_f_j = Connected_Object.AddComponent<FixedJoint>();
                    Con_f_j.connectedBody = Auto_Object.GetComponent<Rigidbody>();
                    Auto_f_j.connectedBody = Connected_Object.GetComponent<Rigidbody>();
                }
            }
            else if (Construction_Type == "Object-AutoSpring")
            {
                Rotation_Adjustment(Auto_Object);
                //Rigidbody control for constructed Object
                //if (Constructed_object.GetComponent<Rigidbody>()) Constructed_object.AddComponent<Rigidbody>();
                //FixedJoint Cons_f_j = Constructed_object.AddComponent<FixedJoint>();
                //Cons_f_j.connectedBody = Auto_Object.GetComponent<Rigidbody>();
                //Cons_f_j.anchor = Constructed_object.transform.position;
                //Cons_f_j.breakForce = 10000f;
                //Cons_f_j.breakTorque = 10000f;
                //Connect the Objects with each other
                SpringJoint S_J;
                if (!Auto_Object.GetComponent<SpringJoint>()) S_J = Auto_Object.AddComponent<SpringJoint>();
                else S_J = Auto_Object.GetComponent<SpringJoint>();
                Auto_Object.GetComponent<SpringJoint>().connectedBody = Constructed_object.GetComponent<Rigidbody>();
                Auto_Object.GetComponent<SpringCode>().springJoint = S_J;
                Auto_Object.GetComponent<SpringCode>().Is_Jointed = false;
                Auto_Object.GetComponent<SpringCode>().Referance_Distance = true;
                float Anchor_Offset = Vector3.Distance(The_Toppest_Point.position,The_Lowest_Point.position);
                Vector3 Difference_Space = new Vector3(0f, -Anchor_Offset, 0f);
                Auto_Object.GetComponent<SpringCode>().springJoint.anchor = Difference_Space;
            }
            else if (Construction_Type == "Chain-AutoChain")
            {
                //Calculate the lowest distance between the Current object and The Auto Objects 
                Transform[] Auto_Child_Objects = Auto_Object.GetComponentsInChildren<Transform>();
                GameObject Connected_Auto_Chain_Holder = null;
                float Lowest_Auto_Chain_Distance = 0.0f;
                Rotation_Adjustment(Auto_Object);
                float Low_Point_distance = Vector3.Distance(Constructed_object.transform.position, The_Lowest_Point.position);
                float Top_Point_distance = Vector3.Distance(Constructed_object.transform.position, The_Toppest_Point.position);

                bool Top_point =Math.Abs(Low_Point_distance) >Math.Abs(Top_Point_distance) ? true : false;
                Vector3 Connected_Holder = Vector3.zero;
                if (Top_point == false)
                {
                    Connected_Holder = The_Lowest_Point.position;
                }
                else
                {
                    Connected_Holder = The_Toppest_Point.position;
                }
                foreach (Transform Auto_Chain_Holder in Auto_Child_Objects)
                {
                    // Assign the Auto-Chain holder which has the lowest point to the Connected_Holder
                    if (Auto_Chain_Holder.name.Contains("Holder"))
                    {
                        float Distance =Math.Abs(Vector3.Distance(Auto_Chain_Holder.transform.position, Connected_Holder));
                        if (Lowest_Auto_Chain_Distance == 0.0f)
                        {
                            Lowest_Auto_Chain_Distance = Distance;
                            Connected_Auto_Chain_Holder = Auto_Chain_Holder.gameObject;
                        }
                        else if (Lowest_Auto_Chain_Distance >= Distance)
                        {
                            Lowest_Auto_Chain_Distance = Distance;
                            Connected_Auto_Chain_Holder = Auto_Chain_Holder.gameObject;
                        }
                    }
                }
                //In this Section we select the top point for connecting  the chain to the other chain
                Transform[] Chain_Objects = Constructed_object.GetComponentsInChildren<Transform>();
                Rotation_Adjustment(Constructed_object);
                float The_lowest_Distance = 0.0f;
                GameObject Connected_Chain_Holder = null;
                foreach (Transform Chain_Holder in Chain_Objects)
                {
                    //Assign the Current Chain holder which has the lowest point to the The_Connection_Point
                    if (Chain_Holder.name.Contains("Holder"))
                    {
                        float Distance =Math.Abs(Vector3.Distance(Chain_Holder.transform.position, Connected_Auto_Chain_Holder.transform.position));
                        if (The_lowest_Distance == 0.0f)
                        {
                            The_lowest_Distance = Distance;
                            Connected_Chain_Holder = Chain_Holder.gameObject;
                        }
                        else if (The_lowest_Distance >= Distance)
                        {
                            Connected_Chain_Holder = Chain_Holder.gameObject;

                            The_lowest_Distance = Distance;
                        }
                    }
                }
                Connected_Chain_Holder.GetComponent<FixedJoint>().connectedBody = Connected_Auto_Chain_Holder.GetComponent<Rigidbody>();
            }
            else if (Construction_Type == "Chain-AutoSpool")
            {
                GameObject Auto_Holder_Parent = Auto_Construction_Object.transform.parent.parent.gameObject;
                if (!Auto_Holder_Parent.GetComponent<Rigidbody>()) Auto_Holder_Parent.AddComponent<Rigidbody>();
                Transform[] Chain_Objects = Constructed_object.GetComponentsInChildren<Transform>();
                GameObject Connected_Chain_Holder = null;
                float The_lowest_Distance = 0.0f;
                foreach (Transform Chain_Holder in Chain_Objects)
                {
                    //Assign the Current Chain holder which has the lowest point to the The_Connection_Point
                    if (Chain_Holder.name.Contains("Holder"))
                    {
                        float Distance = Vector3.Distance(Chain_Holder.transform.position, Auto_Holder_Parent.transform.position);
                        if (The_lowest_Distance == 0.0f)
                        {
                            Connected_Chain_Holder = Chain_Holder.gameObject;
                            The_lowest_Distance = Distance;
                        }
                        else if (The_lowest_Distance >= Distance)
                        {
                            Connected_Chain_Holder = Chain_Holder.gameObject;

                            The_lowest_Distance = Distance;
                        }
                    }
                }
                FixedJoint Auto_F_S = Auto_Holder_Parent.AddComponent<FixedJoint>();
                Auto_F_S.connectedBody = Connected_Chain_Holder.GetComponent<Rigidbody>();
            }
            else if (Construction_Type == "Spool-AutoChain")
            {
                //Calculate the lowest distance between the Current object and The Auto Objects
                Rotation_Adjustment(Auto_Object);
                FixedJoint Cons_f_j = Constructed_object.AddComponent<FixedJoint>();
                Cons_f_j.connectedBody = Auto_Construction_Object.transform.parent.GetComponent<Rigidbody>();
            }
            //It is the indicator of the rejection of bound up with another object with this object 
            Auto_Contruct = false;
            //Auto_Construction_Object.GetComponent<AutoConstruct>().Is_It_Full = true;
        }
        // Connecting Holder object to the object is not under the roof of the Auto-construction System. So It is considered as a different system
        if (Is_It_Holder)
        {
            Is_It_Holder = false;
            Rotation_Adjustment(Constructed_object);

            //Constructed_object.transform.rotation = rotation;
            //if(Constructed_object.transform.name.Contains("spring")) Constructed_object.transform.rotation *= Quaternion.Euler(0, 0, 180);

            if (Constructed_object.transform.name.Contains("Spool"))
            {
                Constructed_object.GetComponent<SpoolCode>().Build = Obj_Building;
                Constructed_object.GetComponent<SpoolCode>().Connected_Object = Auto_Object;
                Constructed_object.GetComponent<SpoolCode>().hit = hit;
                Constructed_object.GetComponent<SpoolCode>().Spool_Anchorage();
            }
            else
            {
                Holder_Limit_Control = Holder_Connection_Control(Constructed_object);
                Debug.Log(Holder_Limit_Control);
                if(Holder_Limit_Control)
                {
                    FixedJoint Cons_f_j = Constructed_object.AddComponent<FixedJoint>();
                    Cons_f_j.connectedBody = Auto_Object.GetComponent<Rigidbody>();
                }
                else
                {
                    if (Constructed_object.GetComponent<FixedJoint>()) Destroy(Constructed_object.GetComponent<FixedJoint>());
                    if (Constructed_object.GetComponent<SpringJoint>()) Destroy(Constructed_object.GetComponent<SpringJoint>());
                }
            }

            

        }
        //Control for Whether the Constructed object is a prepared Simulation or not
        if (Constructed_object.GetComponent<ParentIndicator>().PreparedObject)
        {
            SimulationsReplacement  S_R = Constructed_object.GetComponent<SimulationsReplacement>();
            S_R.System_Setting = Main_Object.GetComponent<SystemSettings>();
            S_R.Simulation_Adjustments(S_R.Simulation_Index);
        } 

        Destroy(Current_Object);
        StartCoroutine(ObjectCooldown());
        //Build Object Button Process
        Build_Object_btn.SetActive(false);
    }
    void Rotation_Adjustment(GameObject The_Object)
    {
        // Get the toppest and the lowest point of the Holder
        Transform[] The_Transform_Objects = The_Object.GetComponentsInChildren<Transform>();
        for (int i = 0; i < The_Transform_Objects.Length; i++)
        {
            if (The_Transform_Objects[i].CompareTag("Low point"))
            {

                The_Lowest_Point = The_Transform_Objects[i];
                
            }
            if (The_Transform_Objects[i].CompareTag("Top point"))
            {
                The_Toppest_Point = The_Transform_Objects[i];
              
            }
        }
        // Rotate The Current object by the angle between the toppest and the lowest point
        AngleX = Vector3.Angle(The_Toppest_Point.position - The_Lowest_Point.position, Vector3.right);
        AngleY = Vector3.Angle(The_Toppest_Point.position - The_Lowest_Point.position, Vector3.up);
        AngleZ = Vector3.Angle(The_Toppest_Point.position - The_Lowest_Point.position, Vector3.forward);
    }
    
    string Find_Construction_Type(GameObject Auto_Collider)
    {
        // It provides to find thw AutoCondtruction type 
        //Note: Connecting Holder to an object is not a auto-construction system so it is considerred another system
        if (Current_Object.name.Contains("Object") && Auto_Collider.tag == "AutoChain")
        {
            return "Object-AutoChain";
        }
        else if (Current_Object.name.Contains("Chain") && Auto_Collider.tag == "AutoHolder")
        {
            return "Chain-AutoHolder";
        }
        else if (Current_Object.name.Contains("Object") && Auto_Collider.tag == "AutoSpring")
        {
            return "Object-AutoSpring";
        }
        else if (Current_Object.name.Contains("Chain") && Auto_Collider.tag == "AutoChain")
        {
            return "Chain-AutoChain";
        }
        else if (Current_Object.name.Contains("Chain") && Auto_Collider.tag == "AutoSpool")
        {
            return "Chain-AutoSpool";
        }
        else if (Current_Object.name.Contains("Spool") && Auto_Collider.tag == "AutoChain")
        {
            return "Spool-AutoChain";
        }

        else
        {
            return "NO";
        }
    }
    // It provides to Connect the Objects Each other When The hit Point on top of the Auto-Construction Object
    public  void AutoConstruction(string Pivot_point, string Construction_Type)
    {
        if (Construction_Type == "Object-AutoChain")
        {
            Vector3 Mini_axis_Auto_Object;
            bool Top_point = false;
            //To querry whether the lowest or the toppest point is closer to the Current_Object and by the result. Specify the position for Current_Object
            Rotation_Adjustment(Auto_Object);

            float Low_Point_distance = Vector3.Distance(Current_Object.transform.position, The_Lowest_Point.position);
            float Top_Point_distance = Vector3.Distance(Current_Object.transform.position, The_Toppest_Point.position);
            Top_point =Math.Abs(Low_Point_distance) >Math.Abs(Top_Point_distance) ? true : false;
            if (Top_point == false) Mini_axis_Auto_Object = The_Lowest_Point.position;
            else Mini_axis_Auto_Object = The_Toppest_Point.position;

            // Adjusting the rotation
            if (Hit_Rotation_Changed)
            {
                Hit_Rotation_Changed = false;
                Current_Object.transform.rotation *= Quaternion.Euler(AngleX, AngleY, AngleZ);
            }


            //Position Alteration limit
            Current_Object.transform.localPosition = Mini_axis_Auto_Object;
            float Limit_Distance = Vector3.Distance(Current_Object.transform.localPosition + Alterred_Position, Mini_axis_Auto_Object);
            //Position Point
            if (Limit_Distance <= .05f + difference) Current_Object.transform.localPosition = Current_Object.transform.localPosition + Alterred_Position;
            
            //Transform Point center
            if (Pivot_point == "Center")
            {

                difference = AbsoluteCenter - absolute_bottom;
                Vector3 Difference_Space = new Vector3(0f, difference + 0.1f, 0f);
                Current_Object.transform.Translate(Difference_Space, Space.Self);
            }
            //Material Part
            if (Current_Object.transform.childCount > 1)
            {
                for (int i = 0; i < Current_Object.transform.childCount; i++)
                {
                    if (Current_Object.transform.GetChild(i).GetComponent<MeshRenderer>())
                        Current_Object.transform.GetChild(i).GetComponent<MeshRenderer>().material = Valid_Auto_material;
                }
            }
            else Current_Object.GetComponent<MeshRenderer>().material = Valid_Auto_material;
        }
        else if (Construction_Type == "Chain-AutoHolder")
        {
            Rotation_Adjustment(Auto_Object);
            
            // Adjusting the rotation
            if (Hit_Rotation_Changed)
            {
                Hit_Rotation_Changed = false;
                Current_Object.transform.rotation *= Quaternion.Euler(AngleX, AngleY, AngleZ);
            }
            //Position Point
            Current_Object.transform.localPosition = The_Lowest_Point.position;
            difference = AbsoluteCenter - absolute_bottom;
            Vector3 Difference_Space = new Vector3(0f, difference + 0.02f, 0f);
            Current_Object.transform.Translate(Difference_Space, Space.Self);
            //Position Alteration limit
            float Limit_Distance = Vector3.Distance(Current_Object.transform.localPosition + Alterred_Position, The_Toppest_Point.position);
            if (Limit_Distance <= .05f + difference) Current_Object.transform.localPosition = Current_Object.transform.localPosition + Alterred_Position;
            //Material Part
            if (Current_Object.transform.childCount > 1)
            {
                for (int i = 0; i < Current_Object.transform.childCount; i++)
                {
                    if (Current_Object.transform.GetChild(i).GetComponent<MeshRenderer>())
                        Current_Object.transform.GetChild(i).GetComponent<MeshRenderer>().material = Valid_Auto_material;
                }
            }
            else Current_Object.GetComponent<MeshRenderer>().material = Valid_Auto_material;
        }
        else if (Construction_Type == "Object-AutoSpring")
        {
            Rotation_Adjustment(Auto_Object);
            //Position Point
            Current_Object.transform.localPosition = The_Toppest_Point.position;
            // Adjusting the rotation
            if (Hit_Rotation_Changed)
            {
                Hit_Rotation_Changed = false;
                Current_Object.transform.rotation *= Quaternion.Euler(AngleX, AngleY, AngleZ);
            }
            else if (Pivot_point == "Center")
            {
                difference = AbsoluteCenter - absolute_bottom;
                Vector3 Difference_Space = new Vector3(0f, difference, 0f);
                Current_Object.transform.Translate(Difference_Space, Space.Self);

            }
            //Position Alteration limit
            float Limit_Distance = Vector3.Distance(Current_Object.transform.localPosition + Alterred_Position, The_Lowest_Point.position);
            if (Limit_Distance <= .05f + difference) Current_Object.transform.localPosition = Current_Object.transform.localPosition + Alterred_Position;
            //Material Part
            if (Current_Object.transform.childCount > 1)
            {
                for (int i = 0; i < Current_Object.transform.childCount; i++)
                {
                    if (Current_Object.transform.GetChild(i).GetComponent<MeshRenderer>())
                        Current_Object.transform.GetChild(i).GetComponent<MeshRenderer>().material = Valid_Auto_material;
                }
            }
            else Current_Object.GetComponent<MeshRenderer>().material = Valid_Auto_material;
        }
        else if (Construction_Type == "Chain-AutoChain")
        {
            Vector3 Mini_axis_Auto_Object;
            bool Top_point = false;
            Rotation_Adjustment(Auto_Object);
            float Low_Point_distance = Vector3.Distance(Current_Object.transform.position, The_Lowest_Point.position);
            float Top_Point_distance = Vector3.Distance(Current_Object.transform.position, The_Toppest_Point.position);
            Top_point =Math.Abs(Low_Point_distance) >Math.Abs(Top_Point_distance) ? true : false;

            if (Top_point == false) Mini_axis_Auto_Object = The_Lowest_Point.position;
            else Mini_axis_Auto_Object = The_Toppest_Point.position;
            // Adjusting the rotation
            if (Hit_Rotation_Changed)
            {
                Hit_Rotation_Changed = false;
                Current_Object.transform.rotation *= Quaternion.Euler(AngleX, AngleY, AngleZ);
            }
            //Position Point
            Current_Object.transform.position = Mini_axis_Auto_Object;
            difference = AbsoluteCenter - absolute_bottom;
            Vector3 Difference_Space = new Vector3(0f, difference - 0.1f, 0f);
            Current_Object.transform.Translate(Difference_Space, Space.Self);
            //Position Alteration limit
            float Limit_Distance = Vector3.Distance(Current_Object.transform.localPosition + Alterred_Position, Mini_axis_Auto_Object);
            if (Limit_Distance <= .05f + difference) Current_Object.transform.localPosition = Current_Object.transform.localPosition + Alterred_Position;
            //Material Part
            if (Current_Object.transform.childCount > 1)
            {
                for (int i = 0; i < Current_Object.transform.childCount; i++)
                {
                    if (Current_Object.transform.GetChild(i).GetComponent<MeshRenderer>())
                        Current_Object.transform.GetChild(i).GetComponent<MeshRenderer>().material = Valid_Auto_material;
                }
            }
            else Current_Object.GetComponent<MeshRenderer>().material = Valid_Auto_material;

        }
        else if (Construction_Type == "Chain-AutoSpool")
        {
            Rotation_Adjustment(Auto_Object);
            // Adjusting the rotation
            if (Hit_Rotation_Changed)
            {
                Hit_Rotation_Changed = false;
                Current_Object.transform.localRotation = Quaternion.Euler(AngleX, AngleY, AngleZ);
            }
            //Position Point
            Current_Object.transform.position = Auto_Construction_Object.transform.parent.position;
            difference = AbsoluteCenter - absolute_bottom;
            Vector3 Difference_Space = new Vector3(0f, difference, 0f);
            Current_Object.transform.Translate(Difference_Space, Space.Self); 
            //Position Alteration limit
            float Limit_Distance = Vector3.Distance(Current_Object.transform.localPosition + Alterred_Position, Auto_Construction_Object.transform.parent.position);
            if (Limit_Distance <= .05f + difference) Current_Object.transform.localPosition = Current_Object.transform.localPosition + Alterred_Position;
            //Material Part
            if (Current_Object.transform.childCount > 1)
            {
                for (int i = 0; i < Current_Object.transform.childCount; i++)
                {
                    if (Current_Object.transform.GetChild(i).GetComponent<MeshRenderer>())
                        Current_Object.transform.GetChild(i).GetComponent<MeshRenderer>().material = Valid_Auto_material;
                }
            }
            else Current_Object.GetComponent<MeshRenderer>().material = Valid_Auto_material;
        }
        else if (Construction_Type == "Spool-AutoChain")
        {
            Vector3 Mini_axis_Auto_Object;
            bool Top_point = false;
            Rotation_Adjustment(Auto_Object);
            float Low_Point_distance = Vector3.Distance(Current_Object.transform.position, The_Lowest_Point.position);
            float Top_Point_distance = Vector3.Distance(Current_Object.transform.position, The_Toppest_Point.position);
            Top_point =Math.Abs(Low_Point_distance) >Math.Abs(Top_Point_distance) ? true : false;

            if (Top_point == false) Mini_axis_Auto_Object = The_Lowest_Point.position;
            else Mini_axis_Auto_Object = The_Toppest_Point.position;

            // Adjusting the rotation
            if (Hit_Rotation_Changed)
            {
                Hit_Rotation_Changed = false;
                Current_Object.transform.rotation *= Quaternion.Euler(AngleX, AngleY, AngleZ);
            }
            //Position Point
            Current_Object.transform.position = Mini_axis_Auto_Object;
            difference = AbsoluteCenter - absolute_bottom;
            Vector3 Difference_Space = new Vector3(0f, -difference, 0f);
            Current_Object.transform.Translate(Difference_Space, Space.Self);
            //Position Alteration limit
            float Limit_Distance = Vector3.Distance(Current_Object.transform.localPosition + Alterred_Position, Auto_Construction_Object.transform.parent.position);
            if (Limit_Distance <= .05f + difference) Current_Object.transform.localPosition = Current_Object.transform.localPosition + Alterred_Position;
            //Material Part
            if (Current_Object.transform.childCount > 1)
            {
                for (int i = 0; i < Current_Object.transform.childCount; i++)
                {
                    if (Current_Object.transform.GetChild(i).GetComponent<MeshRenderer>())
                        Current_Object.transform.GetChild(i).GetComponent<MeshRenderer>().material = Valid_Auto_material;
                }
            }
            else Current_Object.GetComponent<MeshRenderer>().material = Valid_Auto_material;
        }
    }

    public IEnumerator ObjectCooldown()
    {
        yield return null;
        HasClicked = false;
    }
    // CREATE THE SELECTED OBJECT 
    public void Recreate()
    {

        Current_Object = Instantiate(Transparent_Object, Input.mousePosition, Transparent_Object.transform.rotation);
       
       
        default_material.Clear();

        //Calculate the bottomest and center point of the Current Object
        Calculate_Abs_Points();

        Ray Ray_Camera = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(Ray_Camera, out hit,100f, (1 << 8)))
        {
            Current_Object.transform.position = hit.point;
        }
        //Transform Process
        Alterred_Rotation =  Quaternion.Euler(0, 0, 0);
        Alterred_Position = Vector3.zero;
        Transform_Panel.SetActive(true);
        Transform_Script.SpecifiedObject = Current_Object;
        Transform_Script.ShowTransform();
        //Build Object Button Process
        Build_Object_btn.SetActive(true);
    }
    // CONTROL FOR WHETHER THE CURRENT OBJECT'S POSITION EXCEEDS LIMITED POSITIOINS OR NOT
    public bool Limit_Control()
    {
        bool Limit_Obj = false;
        Postion_Control_Ray = Current_Object.transform.position;
        Position_Direction = Vector3.down;
        if (Physics.Raycast(Postion_Control_Ray, Position_Direction, out Control_hit, 200f))
        {
            if (!Control_hit.collider.transform.tag.Contains("Plane") && !Control_hit.collider.transform.tag.Contains("BottomPivot") && !Control_hit.collider.transform.tag.Contains("CenterPivot"))
            {

                Limit_Obj = true;
                if(Limit_Position ==  Vector3.zero)
                {
                    Limit_Position = Current_Object.transform.position;
                }
                Current_Object.transform.position = Limit_Position;

            }
            else
            {
                Limit_Obj = false;
                Limit_Position = Vector3.zero;
            }
        }
        return Limit_Obj;
    }
    public void Calculate_Abs_Points()
    {
        int childCount = Current_Object.transform.childCount;
        if (childCount >= 1)
        {
            absolute_bottom = 0.0f;
            float SumCenter = 0.0f;
            int meshCount = 0;

            MeshRenderer[] meshRenderers = Current_Object.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                //Back up the Default Material of the Selected Object
                MeshRenderer Child_Object = meshRenderers[i];
                Material prefabMaterial = Child_Object.material;
                default_material.Add(prefabMaterial);
                //GET THE ABSOLUTE BOTTOMEST AND ABSOLUTE  CENTER OF THE CURRENT OBJECT

                float bottom_object = meshRenderers[i].bounds.min.y;
                float SpecificCenter = meshRenderers[i].bounds.center.y;
                if (absolute_bottom == 0.0f)
                {
                    absolute_bottom = bottom_object;
                }
                if (absolute_bottom >= bottom_object)
                {
                    absolute_bottom = bottom_object;
                }
                SumCenter += SpecificCenter;
                meshCount++;

            }
            //AbsoluteCenter = SumCenter / meshCount;
            AbsoluteCenter = Current_Object.transform.position.y;
        }
        else
        {

            default_material.Add(Current_Object.GetComponent<MeshRenderer>().material);

            //GET THE BOTTOMEST AND CENTER OF THE CURRENT OBJECT
            absolute_bottom = Current_Object.GetComponent<MeshRenderer>().bounds.min.y;
            AbsoluteCenter = Current_Object.GetComponent<MeshRenderer>().bounds.center.y;

        }
    }
    public bool Holder_Connection_Control(GameObject Holder_Object)
    {
        Rotation_Adjustment(Holder_Object);
        Vector3 rayDirection = The_Lowest_Point.transform.InverseTransformDirection(Vector3.up);
        RaycastHit _hit;
        Debug.DrawRay(The_Lowest_Point.transform.position, rayDirection, Color.red, 30f);
        
        if (Physics.Raycast(The_Lowest_Point.transform.position, rayDirection, out _hit, float.MaxValue))
        {
            float Distance = Mathf.Abs(Vector3.Distance(The_Lowest_Point.transform.position, _hit.point));
            float Limit_Distance = Mathf.Abs(Vector3.Distance(The_Lowest_Point.transform.position, The_Toppest_Point.transform.position));
            //Instantiate(Mini_Object, The_Lowest_Point.transform.position, Quaternion.identity);
            //Instantiate(Mini_Object, The_Toppest_Point.transform.position, Quaternion.identity);
            //Instantiate(Mini_Object, _hit.point, Quaternion.identity);
            if (Distance > Limit_Distance) return false;
            else return true;
        }
        else return false;
    }

}
