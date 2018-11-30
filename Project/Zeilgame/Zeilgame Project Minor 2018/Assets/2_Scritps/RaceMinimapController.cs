using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceMinimapController : MonoBehaviour {

    [SerializeField] GameObject _trackParent;
    [SerializeField] GameObject _buoyImage;
    [SerializeField] GameObject _playerImage;
    [SerializeField] GameObject _opponentParent;
    [SerializeField] GameObject _opponentImage;
    List<GameObject> _opponentImages = new List<GameObject>();

    [SerializeField] GameObject _waypointPanel;
    [SerializeField] GameObject _boat;

    List<GameObject> _opponents = new List<GameObject>();

    Vector3 _boatImageOffset = new Vector3(0, 0, 90);

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        UpdatePlayerImage();
        if(_opponents.Count > 0)
            UpdateOpponent();
	}

    public void AddOpponent(GameObject opponent)
    {
        _opponents.Add(opponent);
        GameObject oi = GameObject.Instantiate(_opponentImage);
        oi.SetActive(true);
        oi.transform.SetParent(_opponentParent.transform);
        oi.transform.localScale = Vector3.one;
        _opponentImages.Add(oi);

    }

    public void UpdatePlayerImage()
    {
        Vector2 pos = -_waypointPanel.transform.localPosition;
        pos.x = MainGameController.Map(pos.x, -8192, 8192, -250, 250);
        pos.y = MainGameController.Map(pos.y, -8192, 8192, -250, 250);
        _playerImage.transform.localPosition = pos;
        _playerImage.transform.localEulerAngles = _boat.transform.localEulerAngles + _boatImageOffset;
    }

    public void UpdateOpponent()
    {
        for(int i = 0; i < _opponents.Count; i++) {
            GameObject opponent = _opponents[i];
            Vector2 pos = opponent.transform.GetChild(0).localPosition;
            pos.x = MainGameController.Map(pos.x, -8192, 8192, -250, 250);
            pos.y = MainGameController.Map(pos.y, -8192, 8192, -250, 250);
            _opponentImages[i].transform.localPosition = pos;
            _opponentImages[i].transform.localEulerAngles = opponent.transform.GetChild(0).localEulerAngles + _boatImageOffset;
        }
    }

    public void DrawTrack(Track t)
    {
        foreach(Vector2 loc in t.GetWaypoints())
        {
            GameObject buoy = GameObject.Instantiate(_buoyImage);
            buoy.transform.SetParent(_trackParent.transform);
            buoy.transform.localScale = Vector3.one;
            Vector2 pos = loc;
            pos.x = MainGameController.Map(pos.x, -8192, 8192, -250, 250);
            pos.y = MainGameController.Map(pos.y, -8192, 8192, -250, 250);

            buoy.transform.localPosition = pos;
        }
    }
}
