using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentIndicator : MonoBehaviour
{
    //There are some control boelean for the purpose of if this object has a special property separating from other objects

    //Connection Control
    //It stands for controlling whether this object connected with any object before or nor. If it is true it will be showed on the object settings panel else the Connect panel wil be showed empty
    public bool Is_Connected;
    public FixedJoint Connected_Fixed_Joint;
    //This object is prepared object or not
    public bool PreparedObject;

}
