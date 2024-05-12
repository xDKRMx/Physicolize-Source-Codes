using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ObjectType
{
    SingleObject,
    MultipleObject
}
public class ObjectAdjustment : MonoBehaviour
{
    [Header("Scripts")]
    public Building Build_Script;
    public ObjectCameraPanel Camera_Panel;
    public SimulationDuration Simulation_duration;
    [Header("Object Control")]
    public bool Object_Selected;
    [Header("Object Variables")]
    public RaycastHit hit;
    public GameObject Selected_Object;
    public GameObject Delete_Panel;
    private MeshRenderer[] Mesh_Count;
    private Rigidbody Main_Rigidbody;
    [Header("Object Velocity")]
    public float Velocity;
    public TextMeshProUGUI Velocity_Panel;
    [Header("Object Angular Velocity")]
    public float Angular_Velocity;
    public TextMeshProUGUI Angular_Velocity_Velocity_Panel;
    [Header("Object Angular Drag")]
    public bool Angular_Drag_Control;
    [SerializeField]
    float Angular_Drag;
    public TMP_InputField Angular_Drag_Field;
    public Toggle Angular_Drag_Toggle;
    [Header("Object Mass")]
    [SerializeField]
    float Mass;
    float[] mass_Ratios;
    public TMP_InputField Mass_Panel;
    [Header("Object Static")]
    [SerializeField]
    public bool Static = false;
    public Toggle Static_Panel;
    [Header("Object Drag")]
    public bool Drag_Control;
    [SerializeField]
    float Drag;
    public TMP_InputField Drag_Field;
    public Toggle Drag_Toggle;
    [Header("Physic Material")]
    [SerializeField]
    PhysicMaterial Object_Physic_Material;
    [Header("Object Bounciness")]
    [SerializeField]
    float Bounciness;
    public TMP_InputField Bounciness_Field;
    [Header("Object Inertia Tensor")]
    public Vector3 Inertia_Tensor;
    string Tensor_axis;
    public TMP_InputField[] Inertia_Tensor_Panel;
    [Header("Object Friction")]
    public bool FrictionControl;
    public Toggle Friction_Toggle;
    [Header("Dynamic Friction")]
    public float Dynamic_Friction;
    public TMP_InputField Dynamic_Friction_panel;
    [Header("Static Friction")]
    public float Static_Friction;
    public TMP_InputField Static_Friction_Panel;

    [Header("Object Connection")]
    public Toggle Connect_Object_Toggle;
    public bool Is_Object_Connected;
    public float The_Lowest_Distance_for_Connection ;
    public GameObject Connectable_GameObject;
    public TextMeshProUGUI Connectable_Object_Text;
    private FixedJoint Connection_Fixed = null;
    [Header("Object Overall Force")]
    public Vector3 Overall_Force_Vector = Vector3.zero;
    private Vector3 Previous_Overall_Force_Vector = Vector3.zero;
    public Vector3 Overall_Force_Direction;
    public TextMeshProUGUI Overall_Force_panel;
    public GameObject Force_Direction_indicator;
    Vector3 Current_Velocity;
    Vector3 Previous_Velocity;
    [Header("Object Overall Torque")]

    public Vector3 Overall_Torque_vector = Vector3.zero;
    public Vector3 Overall_Torque_Direction = Vector3.zero;
    public TextMeshProUGUI Overall_Torque_panel;
    public GameObject Torque_Direction_indicator;
    Vector3 Previous_Angular_Velocity = Vector3.zero;
    Vector3 Current_Angular_Velocity;

    [Header("Energy Variables")]
    EnergyCalculation E_C;
    [SerializeField]
    float ThermalEnergy;
    [SerializeField]
    float MechanicEnergy;
    bool Energy_Calculate;
    [SerializeField]
    float Potential_Energy;
    float Height_Potential;
    float Spring_Potential;
    [SerializeField]
    float Kinetic_Energy;
    public GameObject Referance_Plane;
    float turningKinetic;
    float PostponingKinetic;
    [Header("External Energy")]
    Vector3 Referance_point;
    Vector3 Current_point;
    [SerializeField]
    float Distance_between_point = 0f;
    //Force Zero Control plus Referance point adjustment
    bool Constant_referance = true;
    //Torque Zero Control 
    bool Zero_Torque_Control;
    [Header("Energy PieChart")]
    public Image[] PieCharts;

    void Update()
    {
        if (Selected_Object != null)
        {
            //Toggle and other Input element Activity whether the object is null or not
            Object_Selected = true;
            Mass_Panel.interactable = true;
            Static_Panel.interactable = true;
            Drag_Toggle.interactable = true;
            Bounciness_Field.interactable = true;
            Friction_Toggle.interactable = true;
            Angular_Drag_Toggle.interactable = true;
            
            Connect_Object_Toggle.interactable = true;
            foreach (var item in Inertia_Tensor_Panel)
            {
                item.interactable = true;
            }
           
        }
        else
        {
            //Toggle and other Input element DisActivity
            Object_Selected = false;
            Mass_Panel.interactable = false;
            Static_Panel.interactable = false;
            Drag_Toggle.interactable = false;
            Drag_Field.interactable = false;
            Bounciness_Field.interactable = false;
            Friction_Toggle.interactable = false;
            Dynamic_Friction_panel.interactable = false;
            Static_Friction_Panel.interactable = false;
            Angular_Drag_Toggle.interactable = false;
            Angular_Drag_Field.interactable = false;
            Connect_Object_Toggle.interactable = false;
            foreach (var item in Inertia_Tensor_Panel)
            {
                item.interactable = false;
            }
        }
        if (Input.GetMouseButton(0))
        {
            int layerMask = 1 << 8 | 1 << 9 | 1 << 11;
            Ray Ray_Camera = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(Ray_Camera, out hit, Mathf.Infinity, layerMask))
            {
                if (!hit.collider.gameObject.tag.Contains("Plane") && !hit.collider.gameObject.tag.Contains("Environment") && hit.collider.transform.gameObject != Selected_Object)
                {
                    if (hit.collider.gameObject.GetComponent<ParentIndicator>() != null)
                    {

                        Delete_Panel.SetActive(true);
                        if (Selected_Object != null) Selected_Object_Change();
                        Selected_Object = hit.collider.gameObject;
                        //Adjust game object's physical default values
                        DefaultValues();
                        //Showing Game object into the Camera panel
                        Camera_Process();
                        //Energy Default value
                         EnergyDefault();
                    }
                    else if (hit.collider.gameObject.GetComponentInParent<ParentIndicator>() != null)
                    {

                        Delete_Panel.SetActive(true);
                        if (Selected_Object != null) Selected_Object_Change();

                        Selected_Object = hit.collider.GetComponentInParent<ParentIndicator>().gameObject;
                        //Adjust game object's physical default values
                        DefaultValues();
                        //Showing Game object into the Camera panel
                        Camera_Process();
                        //Energy Default value
                         EnergyDefault();
                    }
                    else
                    {
                        Selected_Object = null;
                    }

                }

            }
        }
        //GAME OBJECT PANEL
        if (Object_Selected && Selected_Object.gameObject != null)
        {
            //Show the Velocity
            InstantVelocity();

            //Show the Angular Velocity
            InstantAngularVelocity();
            //Friction Control
            InstantFriction();
            //Connection Control
            InstantObjectConnection();
            //Show Overall Force
            if (Simulation_duration.Simulation_Play)
                Instant_Overall_Force();
            //Show Overall Torque
            if (Simulation_duration.Simulation_Play)
                Instant_Overall_Torque();
            //Energy Process
            if(Simulation_duration.Simulation_Play)
            Energy();
        }

    }
    #region Update Process
    //PHYSICIAL PROCESSES IN UPDATE 

    void InstantVelocity()
    {
        // Ýnstant Game Object Velocity
        Vector3 _velocity = Selected_Object.GetComponent<Rigidbody>().velocity;
        Velocity = _velocity.magnitude;
        Velocity_Panel.text = Velocity.ToString("F3") + " m/s";
    }
    void InstantAngularVelocity()
    {
        // Instant Game Object Angular Velocity
        Vector3 _Angular_velocity = Selected_Object.GetComponent<Rigidbody>().angularVelocity;
        Angular_Velocity = _Angular_velocity.magnitude;
        Angular_Velocity_Velocity_Panel.text = Angular_Velocity.ToString("F3") + " rad/s";
    }

    void InstantFriction()
    {
        if (Dynamic_Friction == 0 && Static_Friction == 0) FrictionControl = false;
        else FrictionControl = true;

        if (FrictionControl)
        {
            Friction_Toggle.isOn = true;
            Dynamic_Friction_panel.interactable = true;
            Static_Friction_Panel.interactable = true;
        }
        else
        {
            Friction_Toggle.isOn = false;
            Dynamic_Friction_panel.text = "0 N";
            Static_Friction_Panel.text = "0 N";
            Dynamic_Friction_panel.interactable = false;
            Static_Friction_Panel.interactable = false;
        }

    }
    public void InstantObjectConnection()
    {
        
        //Get the nearest object to the selected object for connecting each other unless the distance is below the max range
        if (Is_Object_Connected == false)
        {
            float min_distance_object = 0.0f;
            foreach (var item in Simulation_duration.Main_Simulation_Child_Objects)
            {
                if (item.gameObject != Selected_Object && item.gameObject.GetComponent<Collider>())
                {
                    if(!item.gameObject.transform.IsChildOf(Selected_Object.transform) && item.gameObject.transform.GetComponent<MeshRenderer>())
                    {

                        float Distance = Vector3.Distance(Selected_Object.transform.position, item.transform.position);
                        if (min_distance_object == 0.0f)
                        {
                            min_distance_object = Distance;
                            Connectable_GameObject = item.gameObject;
                        }
                        else if (min_distance_object >= Distance)
                        {
                            min_distance_object = Distance;
                            Connectable_GameObject = item.gameObject;
                        }
                    }
                   
                }

            }
            if (Connectable_GameObject != null && min_distance_object < The_Lowest_Distance_for_Connection)
            {
                Connectable_Object_Text.text = Connectable_GameObject.transform.name;
                Connect_Object_Toggle.interactable = true;
            }
            else
            {
                Connect_Object_Toggle.interactable = false;
                Connectable_Object_Text.text = "";
            }

        }
    }
    public void Energy()
    {
        if (Selected_Object != null && Energy_Calculate)
        {
            //Calculating Kinetic Energy
            PostponingKinetic = E_C.PostponingKineticEnergy(Mass, Velocity);
             if (Angular_Velocity != 0)
             {
                 turningKinetic = E_C.TurnignKineticEnergy(Inertia_Tensor.magnitude, Angular_Velocity);
             }
             Kinetic_Energy = turningKinetic + PostponingKinetic;
             //Calculating Potential Energy
             float Referance_height = Selected_Object.transform.position.y - Referance_Plane.transform.position.y;
             Height_Potential = E_C.HeightPotentialEnergy(Mass, Physics.gravity.magnitude, Referance_height);
             if (Selected_Object.GetComponent<SpringCode>())
             {
                 SpringCode sj = Selected_Object.GetComponent<SpringCode>();
                 Spring_Potential = E_C.FlexibilitySpringPotentialEnergy(sj.springForce, sj.currentDistance);
             }
             Potential_Energy = Height_Potential + Spring_Potential;
             //Calculating the ratio between Thermal and Mechanic Energy

             MechanicEnergy = E_C.MechanicEnergy(Potential_Energy, Kinetic_Energy);
            if(Simulation_duration.Simulation_Play)
            {
                // Energy calculation cooldown for not encountering so massive deviation on energy types. 
                if (Overall_Force_Vector == Vector3.zero) E_C.ReferanceEnergy(true, 0, 0f);
                else
                {
                    float Instant_Force_Alteration = 0.0f;
                    if (Overall_Force_Vector != Previous_Overall_Force_Vector) Instant_Force_Alteration = Mathf.Abs((Overall_Force_Vector - Previous_Overall_Force_Vector).magnitude);
                    else Instant_Force_Alteration = 0.0f;
                    Previous_Overall_Force_Vector = Overall_Force_Vector;
                    E_C.ReferanceEnergy(false, Instant_Force_Alteration, Distance_between_point);
                }
            }
            ThermalEnergy = E_C.ThermalEnergy();
            //Show these energy values onto the Pie Chart
            Dictionary<string, float> energies = new Dictionary<string, float>
            {
              {"Kinetic", Kinetic_Energy},
              {"Potential",Potential_Energy},
              {"Thermal", ThermalEnergy},

            };
            // Create the initial pie chart
            if(E_C.Is_Energies_Inputted)
            {
                E_C.UpdatePieChart(energies);
            }
            
        }
       
    }
   IEnumerator Energy_Cooldown()
   {
       yield return new WaitForSeconds(0.1f);
        Energy_Calculate = true;
   }
    void Instant_Overall_Force()
    {
        // It provides to calculate the overall force referenced by F = m*a; ==> F = OVERALL FORCE , m = Mass, a = accelaration
        Current_Velocity = Selected_Object.GetComponent<Rigidbody>().velocity;
        // Calculating the overall force direction and value by the alteration of the velocity or accelaration
        Vector3 acceleration = (Current_Velocity - Previous_Velocity) / Time.deltaTime;

        Previous_Velocity = Current_Velocity;
        Overall_Force_Vector = Mass * acceleration;
        
        float Overall_Force_Vector_magnitude = Overall_Force_Vector.magnitude;
        Overall_Force_panel.text = Overall_Force_Vector_magnitude.ToString("F3") + " N";
        if (Overall_Force_Vector != Vector3.zero)
        {
            Overall_Force_Direction = Overall_Force_Vector.normalized;
            Force_Direction_indicator.transform.localRotation = Quaternion.LookRotation(Overall_Force_Direction);
        }
        else
        {
                Constant_referance = true;

        }
        // Distance between referance and current point
        Current_point = Selected_Object.transform.position;
         Distance_between_point = Mathf.Abs(Vector3.Distance(Current_point, Referance_point));
        Referance_point = Current_point;
        if (Constant_referance)
        {
            StartCoroutine(Overall_Zero(true));
            Constant_referance = false;
            //Referance_point = Selected_Object.transform.position;
        }
    }
    void Instant_Overall_Torque()
    {
        //It provides to calculate the overall torque referenced by T = A *I  ===> T = NET TORQUE , a = Angular Accelaration, I = Inertia Tensor
        // Calculating the overall Torque direction and value by the alteration of the Angular velocity or  Angular accelaration
        Current_Angular_Velocity = Selected_Object.GetComponent<Rigidbody>().angularVelocity;
        Vector3 Angular_Acceleration = (Current_Angular_Velocity - Previous_Angular_Velocity) / Time.fixedDeltaTime;
        Previous_Angular_Velocity = Current_Angular_Velocity;
        Inertia_Tensor = Selected_Object.GetComponent<Rigidbody>().inertiaTensor;
        Overall_Torque_vector = Inertia_Tensor.magnitude * Angular_Acceleration;
        float Overall_Torque_Vector_magnitude = Overall_Torque_vector.magnitude;

        Overall_Torque_panel.text = Overall_Torque_Vector_magnitude.ToString("F3") + " N";
        if (Overall_Torque_vector != Vector3.zero)
        {
            Debug.Log(Overall_Torque_vector);
            Overall_Torque_Direction = Overall_Torque_vector.normalized;
            Torque_Direction_indicator.transform.rotation = Quaternion.LookRotation(Overall_Torque_Direction);
        }
        else Zero_Torque_Control = true;
        if (Zero_Torque_Control)
        {
            Zero_Torque_Control = false;
            StartCoroutine(Overall_Zero(false));
        }
    }
    private IEnumerator Overall_Zero(bool Is_It_Force)
    {
        if (Is_It_Force)
        {
            yield return new WaitForSeconds(0.3f);
            if (Overall_Force_Vector == Vector3.zero)
            {
               
                Force_Direction_indicator.transform.localRotation = Quaternion.identity;
            }
            
            yield return new WaitForSeconds(1.5f);
            //If the objects doesn't moves anymore and the overallforce is 0 so Thermal Energy will be shown as zero not to show nonsense value when the object moves again
            Debug.Log(Overall_Force_Vector);
            if (Overall_Force_Vector == Vector3.zero && Velocity == 0f && Simulation_duration.Simulation_Play)
            {
                E_C.Instant_External_Clear();
                E_C.ClearEnergy();
            }
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
            if (Overall_Torque_vector == Vector3.zero) Torque_Direction_indicator.transform.localRotation = Quaternion.identity;
        }
    }
    #endregion
    #region DefaultValues
    //Default Processes
    void DefaultValues()
    {
        Mesh_Count = Selected_Object.GetComponentsInChildren<MeshRenderer>();

        if (Selected_Object != null)
        {
          
            //Rigidbody Control
            if (!Selected_Object.GetComponent<Rigidbody>()) Selected_Object.AddComponent<Rigidbody>();
            Main_Rigidbody = Selected_Object.GetComponent<Rigidbody>();
            Rigidbody[] Rb_Children = Selected_Object.transform.GetComponentsInChildren<Rigidbody>();
            // Mass Control 
            if (Mesh_Count.Length > 1)
            {
                //If there is no rigidbody in Children Objecst so Mass is gonna be the Parent's rigidbody mass 
                if (Rb_Children.Length == 0 && Main_Rigidbody) Mass = Main_Rigidbody.mass;
                else
                {
                    
                    //If there is any rigidbodies in the parent object so sum all of them 
                    // Filter out the parent game object's rigidbody if it exists
                    var rigidbodies = Rb_Children.Where(rb => rb.gameObject != Selected_Object).ToArray();
                    // Calculate the total mass of all children with rigidbody
                    Mass = rigidbodies.Sum(rb => rb.mass);
                    // Store the mass ratios of each child with rigidbody
                    mass_Ratios = rigidbodies.Select(rb => (rb.mass / Mass) * 100).ToArray();
                    Mass += Main_Rigidbody.mass;
                    Mass_Panel.text = Mass.ToString();
                }

            }
            else
            {
                Mass = Main_Rigidbody.mass;
                Mass_Panel.text = Mass.ToString();
            }

            // Static Control 
            if (Mesh_Count.Length > 1)
            {
                //Multiple Object Static Values

                //Disactive parent object Gravity for the purpose of encountering with no bug
                Main_Rigidbody.useGravity = false;
                // Object Count

                bool[] statics = new bool[Rb_Children.Length];
                // Static Control for Children Objects
                for (int i = 0; i < Rb_Children.Length; i++)
                {

                    Rigidbody Rb_Selected_Child = Rb_Children[i];

                    if (Rb_Selected_Child)
                    {
                        //Valuate each object's rigibody constraints
                        statics[i] = Rb_Selected_Child.constraints == RigidbodyConstraints.FreezeAll;
                    }
                }

                //Decide the object static or not
                Static = statics.Contains(true);
                Static_Panel.isOn = Static;
            }
            else
            {
                //Decide the object static or not
                Static = Main_Rigidbody.constraints == RigidbodyConstraints.FreezeAll;
                Static_Panel.isOn = Static;
            }

            // Drag Control
            if (Main_Rigidbody.drag == 0f) Drag_Control = false;
            else Drag_Control = true;
            if (Drag_Control)
            {
                Drag = Main_Rigidbody.drag;
                Drag_Field.text = Drag.ToString();
                Drag_Field.interactable = true;
                Drag_Toggle.isOn = true;
            }
            else
            {
                Drag_Field.text = "0";
                Drag_Field.interactable = false;
                Drag_Toggle.isOn = false;
            }
            // Angular Drag Control
            if (Main_Rigidbody.angularDrag == 0f) Angular_Drag_Control = false;
            else Angular_Drag_Control = true;
            if (Drag_Control)
            {
                Angular_Drag = Main_Rigidbody.angularDrag;
                Angular_Drag_Field.text = Drag.ToString();
                Angular_Drag_Field.interactable = true;
                Angular_Drag_Toggle.isOn = true;
            }
            else
            {
                Angular_Drag_Field.text = "0";
                Angular_Drag_Field.interactable = false;
                Angular_Drag_Toggle.isOn = false;
            }

            //Physic Material Control
            Object_Physic_Material = null;
            if (Mesh_Count.Length > 1)
            {

                //Check the Physic material of the Child objects 

                Collider[] Children_Colliders = Selected_Object.GetComponentsInChildren<Collider>();
                bool[] Material_Control = new bool[Children_Colliders.Length];
                for (int Material_Index = 0; Material_Index < Children_Colliders.Length; Material_Index++)
                {
                    if (Children_Colliders[Material_Index].material == null) Material_Control[Material_Index] = false;
                    else Material_Control[Material_Index] = true;
                }
                if (Material_Control.Contains(false))
                {
                    //If there is no Physic Material in Child Object, Create a physic material for Child Objects
                    Adding_Physic_Material(true, Children_Colliders);
                }
                else
                {
                    Adding_Physic_Material(false, Children_Colliders);
                }
            }
            else
            {
                if (Selected_Object.GetComponent<Collider>())
                {
                    if (Selected_Object.GetComponent<Collider>().material == null)
                    {
                        Object_Physic_Material = new PhysicMaterial();
                        Bounciness = Object_Physic_Material.bounciness;
                        Dynamic_Friction = Object_Physic_Material.dynamicFriction;
                        Static_Friction = Object_Physic_Material.staticFriction;

                        Selected_Object.GetComponent<Collider>().material = Object_Physic_Material;
                    }
                    else
                    {
                        Object_Physic_Material = Selected_Object.GetComponent<Collider>().material;
                        Bounciness = Object_Physic_Material.bounciness;
                        Dynamic_Friction = Object_Physic_Material.dynamicFriction;
                        Static_Friction = Object_Physic_Material.staticFriction;


                    }

                }

            }
            Bounciness_Field.text = Bounciness.ToString();
            //Friction Control
            if (Dynamic_Friction == 0 && Static_Friction == 0) FrictionControl = false;
            else FrictionControl = true;
            if (FrictionControl)
            {
                Friction_Toggle.isOn = true;
                Dynamic_Friction_panel.interactable = true;
                Static_Friction_Panel.interactable = true;
                Dynamic_Friction_panel.text = Dynamic_Friction.ToString() + " N";
                Static_Friction_Panel.text = Static_Friction.ToString() + " N";
            }
            else
            {
                Friction_Toggle.isOn = false;
                Static_Friction_Panel.text = "0";
                Dynamic_Friction_panel.interactable = false;
                Static_Friction_Panel.interactable = false;
            }
            // Inertia Tensor Control
            Inertia_Tensor = Main_Rigidbody.inertiaTensor;
            for (int i = 0; i < Inertia_Tensor_Panel.Length; i++)
            {
                Inertia_Tensor_Panel[i].text = Inertia_Tensor[i].ToString("F2");
            }
            // Angular Drag Control
            if (Main_Rigidbody.angularDrag == 0f) Angular_Drag_Control = false;
            else Angular_Drag_Control = true;
            if (Angular_Drag_Control)
            {
                Angular_Drag = Main_Rigidbody.angularDrag;
                Angular_Drag_Field.text = Drag.ToString();
                Angular_Drag_Toggle.interactable = true;
                Angular_Drag_Toggle.isOn = true;
            }
            else
            {
                Angular_Drag_Field.text = "0";
                Angular_Drag_Field.interactable = false;
                Angular_Drag_Toggle.isOn = false;
            }
            //Connection Object
            if (Selected_Object.GetComponent<ParentIndicator>().Connected_Fixed_Joint)
            {
                Connection_Fixed = Selected_Object.GetComponent<ParentIndicator>().Connected_Fixed_Joint;
                Connectable_GameObject = Connection_Fixed.connectedBody.gameObject; 
                Connectable_Object_Text.text = Connectable_GameObject.transform.name;
                Connect_Object_Toggle.isOn = true;
            }
            else
            {
                Connectable_Object_Text.text = "";
                Connect_Object_Toggle.isOn = false;
            }
            //The max distance value for connecting the object to another object
            The_Lowest_Distance_for_Connection = 5f;
            //Referance and current position clarification
            E_C = Selected_Object.GetComponent<EnergyCalculation>();
            //E_C.Instant_External_Clear();
            //E_C.ClearEnergy();
            Referance_point = Vector3.zero;
            Current_point = Vector3.zero;
            //Energy Calculation Cooldown
            Energy_Calculate = false;
            StartCoroutine(Energy_Cooldown());
        }
    }
    #endregion
    #region Energy
    void EnergyDefault()
    {
        //Energy variable
        
        if(!E_C.Is_Energies_Inputted)
        {
            Energy_Calculate = true;
            Energy();
            E_C.Instant_External_Clear();
            Energy_Calculate = false;
        }
        //Get the Default Energy value by the EnergyCalculation script of the selected object
        Potential_Energy = E_C.GetPotential();
        Kinetic_Energy = E_C.GetKinetic();
        MechanicEnergy = Potential_Energy + MechanicEnergy;
        E_C.Overall_Energy = MechanicEnergy;
        ThermalEnergy = E_C.GetThermal();
        Dictionary<string, float> energies = new Dictionary<string, float>
        {
            {"Kinetic", Kinetic_Energy},
            {"Potential",Potential_Energy},
            {"Thermal", ThermalEnergy}

        };
        E_C.Set_Initial_Energy();
        E_C.pieChartPrefab.Clear();
        for (int i = 0; i < PieCharts.Length; i++)
        {
            E_C.pieChartPrefab.Add(PieCharts[i]);
        }
        
        E_C.UpdatePieChart(energies);
    }
    #endregion  

    #region Object Camera panel
    void Camera_Process()
    {
        //Showing the game object on the Object settings' screen 
        Camera_Panel.Is_Object_Active = true;

        Selected_Object.layer = 11;
        if (Mesh_Count.Length > 1)
        {
            foreach (MeshRenderer item in Mesh_Count)
            {
                item.gameObject.layer = 11;
            }
        }
        Camera_Panel.Selected_object = Selected_Object;
        foreach (Transform child in Selected_Object.GetComponentsInChildren<Transform>())
        {
            //Optimizate some position and rotation for camera and Indicator objects
            if (child.CompareTag("Camera Point"))
            {
                Camera_Panel.Camera_Point = child.gameObject;
                Camera_Panel.Camera_Parent_Object.transform.rotation = Camera_Panel.Camera_Point.transform.rotation;
                Camera_Panel.Distance_Offset = Selected_Object.transform.position - Camera_Panel.Camera_Point.transform.position;
                for (int i = 0; i < Camera_Panel.Indicator_Objects.Length; i++)
                {
                    Camera_Panel.Indicator_Objects[i].transform.rotation = Camera_Panel.Camera_Parent_Object.transform.rotation;
                }
            }
        }

    }
    void Selected_Object_Change()
    {
        Selected_Object.layer = 8;
        if (Mesh_Count.Length > 1)
        {
            foreach (MeshRenderer item in Mesh_Count)
            {
                item.gameObject.layer = 8;
            }
        }
    }
    #endregion

    #region Delete
    //Delete Processes
    public void DeleteObject()
    {
        if (Selected_Object != Build_Script.Current_Object)
        {
            Destroy(Selected_Object);
            Selected_Object = null;
            Delete_Panel.SetActive(false);
            //Camera panel process
            Camera_Panel.Is_Object_Active = false;
        }

    }
    #endregion

    #region Mass
    //Mass Adjustment for Gameobject
    public void MassValue(string Mass_Value)
    {
        float _Mass = SystemSettings.ParseValue(Mass_Value);
        if (_Mass > 0)
        {
            Mass = _Mass;
            if (Mesh_Count.Length > 1)
            {
                // Assign the mass value to the variable Mass

                // Get all the rigidbodies in the children of the selected object
                var rigidbodies = Selected_Object.transform.GetComponentsInChildren<Rigidbody>();
                // Filter out the parent game object's rigidbody if it exists
                rigidbodies = rigidbodies.Where(rb => rb.gameObject != Selected_Object).ToArray();
                // Adjust the mass of each rigidbody according to its mass ratio
                for (int i = 0; i < rigidbodies.Length; i++)
                {
                    // Calculate the new mass value for the rigidbody
                    float new_mass_value = (Mass * mass_Ratios[i]) / 100;
                    // Assign the new mass value to the rigidbody
                    rigidbodies[i].mass = new_mass_value;
                }
                Main_Rigidbody.mass = Mass;
                Mass_Panel.text = Mass.ToString();
            }
            else
            {
                // Adjust the exact mass to the selected object
                Main_Rigidbody.mass = Mass;
                Mass_Panel.text = Mass.ToString();
            }
        }
        else
        {
            Mass_Panel.text = Mass.ToString();
        }

    }
    #endregion

    #region Static
    //Static Object 
    public void ObjectStatic(bool IsStatic)
    {
        if (IsStatic) Static = true;
        else Static = false;
        if (Selected_Object != null) SpecifyProcess("Static", Static);
    }
    #endregion

    #region Drag
    //Drag
    public void IsDrag(bool _DragControl)
    {
        if (_DragControl) Drag_Control = true;
        else Drag_Control = false;
        if (Selected_Object != null) SpecifyProcess("Drag", Drag_Control);
    }
    //Valueating Drag
    public void DragValue(string Drag_value)
    {

        if (Drag_Control)
        {

            float _Drag = SystemSettings.ParseValue(Drag_value);
            if (_Drag != 0)
            {
                Drag = _Drag;
                Selected_Object.transform.GetComponent<Rigidbody>().drag = Drag;

                //Execute the same thing to the child objects
                if (Mesh_Count.Length > 1)
                {
                    for (int i = 0; i < Mesh_Count.Length; i++)
                    {
                        if (Selected_Object.transform.GetChild(i).GetComponent<Rigidbody>())
                        {
                            Selected_Object.transform.GetChild(i).GetComponent<Rigidbody>().drag = Drag;
                        }
                    }
                }
                Drag_Field.text = Drag.ToString();
            }
            else
            {
                Drag_Field.text = Drag.ToString();
            }
        }
    }
    #endregion
    #region Angular Drag
    //Angular Drag
    public void IsAngularDrag(bool _AngularDragControl)
    {
        if (_AngularDragControl) Angular_Drag_Control = true;
        else Angular_Drag_Control = false;
        if (Selected_Object != null) SpecifyProcess("AngularDrag", Angular_Drag_Control);
    }
    //Valueating Drag
    public void AngularDragValue(string _Angular_Drag_value)
    {

        if (Angular_Drag_Control)
        {

            float _Angular_Drag = SystemSettings.ParseValue(_Angular_Drag_value);
            if (_Angular_Drag != 0)
            {
                Angular_Drag = _Angular_Drag;
                Selected_Object.transform.GetComponent<Rigidbody>().angularDrag = Angular_Drag;

                //Execute the same thing to the child objects
                if (Mesh_Count.Length > 1)
                {
                    for (int i = 0; i < Mesh_Count.Length; i++)
                    {
                        if (Selected_Object.transform.GetChild(i).GetComponent<Rigidbody>())
                        {
                            Selected_Object.transform.GetChild(i).GetComponent<Rigidbody>().angularDrag = Angular_Drag;
                        }
                    }
                }
                Angular_Drag_Field.text = Angular_Drag.ToString();
            }
            else
            {
                Angular_Drag_Field.text = Angular_Drag.ToString();
            }
        }
    }
    #endregion

    #region Interia Tensor
    //Interia Tensor 
    public void Inertia_Tensor_Axis(string Tensor_Axis)
    {
        Tensor_axis = Tensor_Axis;
    }
    public void Inertia_Tensor_Value(string _Tensor_Value)
    {
        float Tensor_Value = SystemSettings.ParseValue(_Tensor_Value);
        if (Tensor_Value < 0) Tensor_Value = 0;
        Rigidbody rb = Selected_Object.transform.GetComponent<Rigidbody>();
        switch (Tensor_axis)
        {
            case "X":
                Inertia_Tensor = new Vector3(Tensor_Value, rb.inertiaTensor.y, rb.inertiaTensor.z);
                Selected_Object.transform.GetComponent<Rigidbody>().inertiaTensor = Inertia_Tensor;
                Inertia_Tensor_Panel[0].text = Inertia_Tensor.x.ToString("F2");
                break;
            case "Y":
                Inertia_Tensor = new Vector3(rb.inertiaTensor.x, Tensor_Value, rb.inertiaTensor.z);
                Selected_Object.transform.GetComponent<Rigidbody>().inertiaTensor = Inertia_Tensor;
                Inertia_Tensor_Panel[1].text = Inertia_Tensor.y.ToString("F2");
                break;
            case "Z":
                Inertia_Tensor = new Vector3(rb.inertiaTensor.x, rb.inertiaTensor.y, Tensor_Value);
                Selected_Object.transform.GetComponent<Rigidbody>().inertiaTensor = Inertia_Tensor;
                Inertia_Tensor_Panel[2].text = Inertia_Tensor.z.ToString("F2");
                break;
        }
    }
    #endregion
    //PHYSIC MATERIAL PROCESSES

    //Adding Physic Material
    public void Adding_Physic_Material(bool Physic_null, Collider[] Children_Colliders)
    {
        if (Physic_null)
        {
            Object_Physic_Material = new PhysicMaterial();
            Bounciness = Object_Physic_Material.bounciness;
            Dynamic_Friction = Object_Physic_Material.dynamicFriction;
            Static_Friction = Object_Physic_Material.staticFriction;
            for (int i = 0; i < Children_Colliders.Length; i++)
            {
                Children_Colliders[i].material = Object_Physic_Material;
            }
        }
        else
        {
            PhysicMaterial First_Material;
            for (int i = 0; i < Children_Colliders.Length; i++)
            {
                First_Material = Children_Colliders[i].material;
                if (Object_Physic_Material == null) Object_Physic_Material = First_Material;

                if (First_Material != Object_Physic_Material) Children_Colliders[i].material = Object_Physic_Material;
            }
            Bounciness = Object_Physic_Material.bounciness;
            Dynamic_Friction = Object_Physic_Material.dynamicFriction;
            Static_Friction = Object_Physic_Material.staticFriction;
        }
    }
    //Valueating Bouciness
    public void BouncinessValue(string Bounciness_Value)
    {
        float _Bounciness = SystemSettings.ParseValue(Bounciness_Value);
        if (_Bounciness > 1) _Bounciness = 1;

        Bounciness = _Bounciness;
        Bounciness_Field.text = Bounciness.ToString();
        Object_Physic_Material.bounciness = Bounciness;

    }

    //Object Friction(Itself)
    public void IsFriction(bool _FricitonControl)
    {
        //Friction Control
        if (_FricitonControl) FrictionControl = true;
        else FrictionControl = false;
        if (Selected_Object != null) SpecifyProcess("Friction", FrictionControl);
    }
    public void DynamicFrictionValue(string Dynamic_Value)
    {


        if (FrictionControl)
        {

            //Dynamic friction Adjustment
            float _DynamicFriction = SystemSettings.ParseValue(Dynamic_Value);
            Dynamic_Friction = _DynamicFriction;
            if (Dynamic_Friction > 1) Dynamic_Friction = 1;
            if (Mesh_Count.Length >= 1)
            {
                Object_Physic_Material.dynamicFriction = Dynamic_Friction;
                for (int i = 0; i < Mesh_Count.Length; i++)
                {
                    if (Selected_Object.transform.GetChild(i).GetComponent<Collider>()  && Selected_Object.transform.GetChild(i).GetComponent<Collider>().material == null) Selected_Object.transform.GetChild(i).GetComponent<Collider>().material = Object_Physic_Material;
                }
            }
            else
            {
                Object_Physic_Material.dynamicFriction = Dynamic_Friction;
                if (Selected_Object.GetComponent<Collider>().material == null) Selected_Object.GetComponent<Collider>().material = Object_Physic_Material;



            }
            Dynamic_Friction_panel.text = Dynamic_Friction.ToString() + " N";

        }

    }
    public void StaticFrictionValue(string Static_Value)
    {

        if (FrictionControl)
        {

            //Static friction Adjustment
            float _Static_Friction = SystemSettings.ParseValue(Static_Value);
            Static_Friction = _Static_Friction;
            if (Static_Friction > 1) Static_Friction = 1;
            if (Mesh_Count.Length > 1)
            {
                Object_Physic_Material.staticFriction = Static_Friction;
                for (int i = 0; i < Mesh_Count.Length; i++)
                {
                    if (Selected_Object.transform.GetChild(i).GetComponent<Collider>().material == null) Selected_Object.transform.GetChild(i).GetComponent<Collider>().material = Object_Physic_Material;
                }
            }
            else
            {
                Object_Physic_Material.staticFriction = Static_Friction;
                if (Selected_Object.GetComponent<Collider>().material == null) Selected_Object.GetComponent<Collider>().material = Object_Physic_Material;

            }
            Static_Friction_Panel.text = Static_Friction.ToString() + " N";
        }

    }
    public void Connect_Object(bool _Connection)
    {
        //It provides to connect an object to the other object by using fixed joint
        if(_Connection)
        {
            Is_Object_Connected = true;
           
            if (!Connectable_GameObject.GetComponent<Rigidbody>()) Connectable_GameObject.AddComponent<Rigidbody>();
            if (!Selected_Object.GetComponent<FixedJoint>()) Connection_Fixed = Selected_Object.AddComponent<FixedJoint>();
            Connection_Fixed = Selected_Object.GetComponent<FixedJoint>();
            Connection_Fixed.connectedBody = Connectable_GameObject.GetComponent<Rigidbody>();
            // It is indicator for it was connected.
            Selected_Object.GetComponent<ParentIndicator>().Is_Connected = true;
            Selected_Object.GetComponent<ParentIndicator>().Connected_Fixed_Joint = Connection_Fixed;
        }
        else
        {
            Is_Object_Connected = false;
            if(Selected_Object.GetComponent<ParentIndicator>().Connected_Fixed_Joint)
            {
                Connection_Fixed = Selected_Object.GetComponent<ParentIndicator>().Connected_Fixed_Joint;
                Destroy(Connection_Fixed);
                // It is indicator for it was connected.
            }
            Selected_Object.GetComponent<ParentIndicator>().Is_Connected = false;
        }
      
    }
    // SELECTED OBJECT SPECIFIED PROCOSSES
    public void SpecifyProcess(string Process_type, bool The_Process_State)
    {
        switch (Process_type)
        {
            case "Static":

                if (Mesh_Count.Length > 1)
                {

                    if (The_Process_State)
                    {
                        Selected_Object.transform.gameObject.isStatic = Static;
                        Main_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                        foreach (var item in Mesh_Count)
                        {
                            item.gameObject.isStatic = Static;
                            if (item.gameObject.GetComponent<Rigidbody>())
                            {
                                item.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                            }
                        }
                    }
                    else
                    {
                        Selected_Object.transform.gameObject.isStatic = Static;
                        Main_Rigidbody.constraints = RigidbodyConstraints.None;
                        foreach (var item in Mesh_Count)
                        {
                            item.gameObject.isStatic = Static;
                            if (item.gameObject.GetComponent<Rigidbody>())
                            {
                                item.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                            }
                        }
                    }
                }
                else
                {
                    if (The_Process_State)
                    {
                        Selected_Object.transform.gameObject.isStatic = Static;
                        Main_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                    }
                    else
                    {
                        Selected_Object.transform.gameObject.isStatic = Static;
                        Main_Rigidbody.constraints = RigidbodyConstraints.None;
                    }

                }
                break;
            case "Drag":

                if (Mesh_Count.Length > 1)
                {
                    if (The_Process_State)
                    {

                        Drag_Field.interactable = true;
                        Drag_Toggle.isOn = true;
                        if (Drag == 0) Drag = 1f;
                    }
                    else
                    {
                        Drag_Field.interactable = false;
                        Drag_Toggle.isOn = false;
                        Drag = 0f;
                    }
                    Selected_Object.transform.GetComponent<Rigidbody>().drag = Drag;
                    Drag_Field.text = Drag.ToString();
                    foreach (var item in Mesh_Count)
                    {
                        if (item.gameObject.GetComponent<Rigidbody>())
                        {
                            item.gameObject.GetComponent<Rigidbody>().drag = Drag;
                        }
                    }
                }
                else
                {
                    if (The_Process_State)
                    {
                        Drag_Field.interactable = true;
                        Drag_Toggle.isOn = true;
                        if (Drag == 0) Drag = 1f;
                        Selected_Object.transform.GetComponent<Rigidbody>().drag = Drag;
                        Drag_Field.text = Drag.ToString();
                    }
                    else
                    {
                        Drag_Field.interactable = false;
                        Drag_Toggle.isOn = false;
                        Drag = 0f;
                        Selected_Object.transform.GetComponent<Rigidbody>().drag = Drag;
                        Drag_Field.text = "0";
                    }

                }
                break;
            case "AngularDrag":
                if (Mesh_Count.Length > 1)
                {
                    if (The_Process_State)
                    {

                        Angular_Drag_Field.interactable = true;
                        Angular_Drag_Toggle.isOn = true;
                        if (Angular_Drag == 0) Angular_Drag = 1f;
                    }
                    else
                    {
                        Angular_Drag_Field.interactable = false;
                        Angular_Drag_Toggle.isOn = false;
                        Angular_Drag = 0f;
                    }
                    Selected_Object.transform.GetComponent<Rigidbody>().angularDrag = Angular_Drag;
                    Angular_Drag_Field.text = Angular_Drag.ToString();
                    foreach (var item in Mesh_Count)
                    {
                        if (item.gameObject.GetComponent<Rigidbody>())
                        {
                            item.gameObject.GetComponent<Rigidbody>().angularDrag = Angular_Drag;
                        }
                    }
                }
                else
                {
                    if (The_Process_State)
                    {
                        Angular_Drag_Field.interactable = true;
                        Angular_Drag_Toggle.isOn = true;
                        if (Angular_Drag == 0) Angular_Drag = 1f;
                        Selected_Object.transform.GetComponent<Rigidbody>().angularDrag = Angular_Drag;
                        Angular_Drag_Field.text = Angular_Drag.ToString();
                    }
                    else
                    {
                        Angular_Drag_Field.interactable = false;
                        Angular_Drag_Toggle.isOn = false;
                        Angular_Drag = 0f;
                        Selected_Object.transform.GetComponent<Rigidbody>().angularDrag = Angular_Drag;
                        Angular_Drag_Field.text = "0";
                    }

                }
                break;
            case "Friction":
                if (The_Process_State)
                {
                    if (Dynamic_Friction == 0 && Static_Friction == 0)
                    {
                        Dynamic_Friction = 12;
                        Static_Friction = 15;
                    }
                    Friction_Toggle.isOn = true;
                    Dynamic_Friction_panel.interactable = true;
                    Static_Friction_Panel.interactable = true;
                    Object_Physic_Material.staticFriction = Dynamic_Friction;
                    Object_Physic_Material.dynamicFriction = Static_Friction;
                    Dynamic_Friction_panel.text = Dynamic_Friction.ToString() + " N";
                    Static_Friction_Panel.text = Static_Friction.ToString() + " N";
                }
                else
                {
                    Friction_Toggle.isOn = false;
                    Object_Physic_Material.staticFriction = 0;
                    Object_Physic_Material.dynamicFriction = 0;
                    Dynamic_Friction = 0;
                    Static_Friction = 0;
                    Dynamic_Friction_panel.text = "0 N";
                    Static_Friction_Panel.text = "0 N";
                    Dynamic_Friction_panel.interactable = false;
                    Static_Friction_Panel.interactable = false;
                }
                break;

        }
    }

}
