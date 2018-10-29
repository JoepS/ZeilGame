using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaMovement : MonoBehaviour {

    [SerializeField, Range(1, 10)] float _frequency = 1;
    [SerializeField, Range(5, 100)] float _multiplier = 1;
    [SerializeField, Range(0.01f, 1)] float _waitTime = 0.1f;
    [SerializeField, Range(1, 10)] float _speedMultiplier = 1;

    float _middleToOneBlockHeight;
    float _middleBlockHeight;

    List<GameObject> _seaBlocks;

	// Use this for initialization
	void Start () {
        _seaBlocks = new List<GameObject>();
        for(int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).gameObject.name.Contains("SeaBlock"))
                _seaBlocks.Add(this.transform.GetChild(i).gameObject);
        }
        StartCoroutine(moveSea());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public float GetMiddleBlockHeight()
    {
        return _middleBlockHeight;
    }

    public float GetMiddleToOneBlockHeight()
    {
        return _middleToOneBlockHeight;
    }

    public void SetSpeed(float movementSpeed)
    {
        if (movementSpeed < 0)
            movementSpeed = 0.1f;
        _speedMultiplier = movementSpeed;
    }

    public List<GameObject> GetSeaBlocks()
    {
        return _seaBlocks;
    }

    public IEnumerator moveSea()
    {
        float offset = 290;
        while (true)
        {
            float i;
            for (i = offset; i < offset+_seaBlocks.Count; i++)
            {

                float x = (i / _seaBlocks.Count) - 0.5f;
                /*Vector2 pos = _seaBlocks[(int)(i - offset)].GetComponent<RectTransform>().localPosition;
                pos.y = Mathf.PerlinNoise(x * _frequency, 0) * _multiplier * 100;
                _tempPos = x * _frequency;
                _seaBlocks[(int)(i-offset)].GetComponent<RectTransform>().localPosition = pos;*/

                Vector2 size = _seaBlocks[(int)(i - offset)].GetComponent<RectTransform>().sizeDelta;
                size.y = 900 + Mathf.PerlinNoise(x * _frequency, 0) * _multiplier * 100;
                _seaBlocks[(int)(i - offset)].GetComponent<RectTransform>().sizeDelta = size;

                if (i == offset + (_seaBlocks.Count / 2))
                    _middleBlockHeight = (size.y / 2);
                if (i == offset + (_seaBlocks.Count / 2) + 1)
                    _middleToOneBlockHeight = (size.y / 2);

            }
            offset += 1 * _speedMultiplier;
            yield return new WaitForSeconds(_waitTime);
        }
    }
}
