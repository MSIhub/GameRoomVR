using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserNameUpdater : MonoBehaviour
{
    [SerializeField] private Player _player;
    private String _userName="Default";
    // Start is called before the first frame update
    void Start()
    {
        var textField = GetComponentInChildren<Text>();
        textField.text = _player.UserName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
