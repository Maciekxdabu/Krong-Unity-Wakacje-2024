using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private GameObject _doneCube;
    [SerializeField] private float _task_time_done_seconds = 0.0f;
    [SerializeField] private float _task_time_seconds = 3.0f;
    [SerializeField] private float _minions_needed = 10;
    [SerializeField] private UnityEvent _actionDoneCallback;

    void FixedUpdate()
    {
        _task_time_done_seconds += Time.deltaTime;
        if (_task_time_done_seconds > _task_time_seconds) { 
            _actionDoneCallback.Invoke();
            _task_time_done_seconds = 0;
            gameObject.SetActive(false);
        }
        var pct = _task_time_done_seconds / _task_time_seconds;
        _doneCube.transform.localScale = new Vector3(pct, 0.3f, 0.3f);
    }
}
