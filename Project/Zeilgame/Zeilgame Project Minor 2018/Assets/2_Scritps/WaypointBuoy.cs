using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointBuoy : MonoBehaviour {

    public GameObject upwindTarget { get { return _upwindTarget; } }
    [SerializeField] GameObject _upwindTarget;
    public GameObject downwindTarget { get { return _downwindTarget; } }
    [SerializeField] GameObject _downwindTarget;
    
}
