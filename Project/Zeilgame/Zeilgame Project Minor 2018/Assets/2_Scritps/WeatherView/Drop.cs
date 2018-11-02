using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour {

    float DropSpeed = 500;
    Vector3 startPos;

    bool _moving = true;

	// Use this for initialization
	void Start () {
        startPos = this.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
        if (_moving)
        {
            this.transform.localPosition -= new Vector3(0, 1 * DropSpeed * Time.deltaTime, 0);
            if (Vector3.Distance(startPos, this.transform.localPosition) > Screen.height)
                this.gameObject.SetActive(false);
        }
	}
}
