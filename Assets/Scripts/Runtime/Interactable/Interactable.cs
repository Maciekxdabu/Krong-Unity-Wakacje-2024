using Assets.Scripts.Runtime.Character;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    // SETUP
    [SerializeField] private float _task_finish_time_seconds = 3.0f;
    [SerializeField] private float _task_minions_needed = 10;
    [SerializeField] private List<Transform> _minion_positions;
    public UnityEvent<Interactable> TaskDoneCallback;
    private List<bool> _positions_taken = new List<bool>();

    // Interface visuals
    [SerializeField] private Image _progressCircle;
    [SerializeField] private TextMeshProUGUI _textLabel;

    // Current state
    [SerializeField] private float _current_progress_seconds = 0.0f;
    [SerializeField] private HashSet<Minion> _minions = new HashSet<Minion>();

    private void Awake()
    {
        for(int i=0; i < _minion_positions.Count; i++)
        {
            _positions_taken.Add(false);
        }
    }

    void FixedUpdate()
    {
        updateTaskPercentage();
        updateVisuals();
    }

    private void updateTaskPercentage()
    {
        if (_minions.Count < _task_minions_needed)
        {
            return;
        }
        _current_progress_seconds += Time.deltaTime;
        if (_current_progress_seconds > _task_finish_time_seconds)
        {
            TaskDoneCallback.Invoke(this);
            _current_progress_seconds = 0;
            gameObject.SetActive(false);
        }
    }

    private void updateVisuals()
    {
        var pct = _current_progress_seconds / _task_finish_time_seconds;
        _progressCircle.fillAmount = pct;
        _textLabel.SetText($"{_minions.Count}/{_task_minions_needed}");
    }


    public void StartInteractionWithMinion(Minion minion)
    {
        _minions.Add(minion);
    }

    public void EndInteractionWithMinion(Minion minion)
    {
        _minions.Remove(minion);
        DeassignAllPosition();
    }

    public bool DoesNeedMoreMinions()
    {
        if( _minions.Count >= _task_minions_needed ) return false;
        return true;
    }

    public Vector3 AssignPosition()
    {
        if(_minion_positions.Count == 0) return transform.position;
        for(int i = 0;  i < _minion_positions.Count; i++)
        {
            if (!_positions_taken[i])
            {
                _positions_taken[i] = true;
                return _minion_positions[i].position;
            }
        }
        return transform.position;
    }
    public void DeassignAllPosition()
    {
        for(int i = 0; i < _positions_taken.Count ; i++)
        {
            _positions_taken[i] = false;
        }
    }

    //minion.transform.SetPositionAndRotation(_minion_positions[i].position,
    //    _minion_positions[i].rotation);
    //_positions_taken[i] = true;
    //break;
}
