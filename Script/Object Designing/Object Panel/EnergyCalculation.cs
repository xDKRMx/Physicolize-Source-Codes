using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EnergyCalculation : MonoBehaviour
{
    [Header("Overall Energy")]
    public float Overall_Energy;
    private float Initial_OverAll_Energy;
    public bool Is_Energies_Inputted = false;
    bool Overal_Force_Zero;
    [SerializeField]
    private float Instant_External_Force;
    public bool Net_Force_Impacted;
    [Header("Mechanic Energy")]
    [SerializeField]
    private float _MechanicEnergy;
    [SerializeField]
    private float Initial_MechanicEnergy;
    [Header("Thermal Energy")]
    [SerializeField]
    private float _ThermalEnergy;

    [Header("Potential Energy")]
    [SerializeField]
    private float _Height_potential;
    private float _Spring_potential;
    [SerializeField]
    private float Initial_PotentialEnergy;
    [Header("Kinetic Energy")]
    [SerializeField]
    private float _Postponing_Kinetic;
    private float _Turning_Kinetic;
    [SerializeField]
    private float Initial_KineticEnergy;
    [Header("Pie Chart Process")]
    public List<Image> pieChartPrefab = new List<Image>(); // Pie chart prefab with an Image component using a filled type
    public Dictionary<string, Color> sliceColors;
    private Image[] slices = new Image[3];
    void Start()
    {
        // Initialize the slice colors for each energy type
        sliceColors = new Dictionary<string, Color>
        {
            {"Kinetic", Color.yellow},
            {"Potential", Color.blue},
            {"Thermal", Color.red}

        };

        //Dictionary<string, float> energies = new Dictionary<string, float>
        //{
        //    {"Kinetic", 10f},
        //    {"Potential",10f},
        //    {"Thermal", 10f}

        //};
        //CreatePieChart(energies);
    }
    public void Set_Initial_Energy()
    {
        Is_Energies_Inputted = true;
        Initial_PotentialEnergy = _Spring_potential + _Height_potential;
        Initial_KineticEnergy = _Turning_Kinetic + _Postponing_Kinetic;
        Initial_MechanicEnergy = Initial_PotentialEnergy + Initial_KineticEnergy;
       
    }
    public void Instant_External_Clear()
    {
        Instant_External_Force = 0f;
    }
    public void ClearEnergy()
    {
        _Spring_potential = 0;
        _Height_potential = 0;
        _Turning_Kinetic = 0;
        _Postponing_Kinetic = 0;
        Initial_PotentialEnergy = _Spring_potential + _Height_potential;
        Initial_KineticEnergy = _Turning_Kinetic + _Postponing_Kinetic;
        Initial_MechanicEnergy = Initial_PotentialEnergy + Initial_KineticEnergy;
    }
    #region Overall Energy
    public void ReferanceEnergy(bool IsItBalanced, float Current_Netforce, float Distance)
    {
        if (IsItBalanced)
        {
            Overal_Force_Zero = true;
            Overall_Energy = _MechanicEnergy + _ThermalEnergy;
        }
        else
        {
            // If overallfoce is not zero Overall_force must be increased as long as the overall_force is zero
            Overal_Force_Zero = false;
            float ExternalEnergy = Current_Netforce * Distance;
            Instant_External_Force += ExternalEnergy;

            Overall_Energy = Initial_MechanicEnergy + Instant_External_Force;
        }
    }
    #endregion

    #region Mechanic
    public float MechanicEnergy(float Potential_Energy, float Kinetic_Energy)
    {
        
        _MechanicEnergy = Potential_Energy + Kinetic_Energy;
        return _MechanicEnergy;
    }
    #endregion

    #region Thermal
    public float ThermalEnergy()
    {
        //if (Overal_Force_Zero && Instant_External_Force == 0.0f) _ThermalEnergy =  0;
        //else if(!Overal_Force_Zero && Instant_External_Force != 0.0f) _ThermalEnergy = Instant_External_Force - Mathf.Abs(_MechanicEnergy - Initial_MechanicEnergy);
        //else _ThermalEnergy = Instant_External_Force - (_MechanicEnergy - Initial_MechanicEnergy);


        
        if ( Instant_External_Force != 0.0f)
        {
           
            _ThermalEnergy = Instant_External_Force - (_MechanicEnergy - Initial_MechanicEnergy);

        }
        else
        {
            
            _ThermalEnergy = 0;
        }
        
        return _ThermalEnergy;
    }
    public float GetThermal()
    {
        return _ThermalEnergy;
    }
    #endregion

    #region Potential
    //Potential Energy
    public float HeightPotentialEnergy(float Mass, float Gravity, float ReferanceHeight)
    {
        _Height_potential =Mathf.Abs(Mass * Gravity * ReferanceHeight);
        return _Height_potential;
    }
    public float FlexibilitySpringPotentialEnergy(float Coefficient, float Compressed_amount)
    {
        _Spring_potential = Coefficient * Mathf.Pow(Compressed_amount, 2) / 2;
        return _Spring_potential;
    }

    public float GetPotential()
    {
        return _Height_potential + _Spring_potential;
    }
    #endregion

    #region Kinetic
    //Kinetic Energy
    public float PostponingKineticEnergy(float Mass, float Velocity)
    {
        _Postponing_Kinetic = Mass * Velocity* Velocity / 2;
        return _Postponing_Kinetic;
    }
    public float TurnignKineticEnergy(float InertiaTensor, float AngularVelocity)
    {
        _Turning_Kinetic = InertiaTensor * Mathf.Pow(AngularVelocity, 2) / 2;
        return _Turning_Kinetic;
    }
    public float GetKinetic()
    {
        return _Turning_Kinetic + _Postponing_Kinetic;
    }
    #endregion
    public void CreatePieChart(Dictionary<string, float> energies)
    {
        float totalEnergy = energies.Values.Sum();
        float previousAngle = 0f;

        for (int i = 0; i < energies.Count; i++)
        {

            Image slice = pieChartPrefab[i];
            float fillAmount = totalEnergy != 0 ? energies.ElementAt(i).Value / totalEnergy : 1f;
            slice.fillAmount = fillAmount;
            slice.color = sliceColors[energies.ElementAt(i).Key];
            slice.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -previousAngle));
            previousAngle += fillAmount * 360f;
            slices[i] = slice;
        }
    }

    public void UpdatePieChart(Dictionary<string, float> energies)
    {

        float totalEnergy = energies.Values.Sum();
        float previousAngle = 0f;
        for (int i = 0; i < pieChartPrefab.Count; i++)
        {
            var energy = energies.ElementAt(i);
            pieChartPrefab[i].fillAmount = totalEnergy != 0 ? energy.Value / totalEnergy : 1f;
            pieChartPrefab[i].color = sliceColors[energy.Key];
            pieChartPrefab[i].transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -previousAngle));
            previousAngle += pieChartPrefab[i].fillAmount * 360f;
        }
    }
}
