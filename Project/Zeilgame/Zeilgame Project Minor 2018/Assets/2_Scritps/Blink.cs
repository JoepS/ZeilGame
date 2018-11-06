using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blink : MonoBehaviour {

    float _blinkTime;
    float _timeBetweenFlashes;

    Color _color;

    public bool StartingToBlink = false;

    private void Start()
    {
        _color = new Color(1, 1, 1, 0);
        _blinkTime = 0.1f;
        _timeBetweenFlashes = 10;
    }

    public void StartFlashing()
    {
        StartCoroutine(Flash());
    }

    public void StopFlashing()
    {
        StopCoroutine(Flash());
        _color.a = 0;
        this.GetComponent<Image>().color = _color;
    }

    IEnumerator Flash()
    {
        while (true)
        {
            StartingToBlink = true;
            while(_color.a < 0.75)
            {
                _color.a += 0.25f;
                yield return new WaitForEndOfFrame();
                this.GetComponent<Image>().color = _color;
            }
            yield return new WaitForSeconds(_blinkTime);
            StartingToBlink = false;
            while (_color.a > 0)
            {
                _color.a -= 0.25f;
                this.GetComponent<Image>().color = _color;
                yield return new WaitForEndOfFrame();
            }
            _color.a = 0;
            yield return new WaitForSeconds(Random.Range(0, _timeBetweenFlashes));
        }
    }
}
