using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingDisplay : MonoBehaviour {
    [SerializeField]
    Text txt;
	// Use this for initialization
	void Start () {
        var ranking = ScoreManager.GetRanking();
        foreach (var r in ranking)
            txt.text += r.ToString() + "\n";
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
