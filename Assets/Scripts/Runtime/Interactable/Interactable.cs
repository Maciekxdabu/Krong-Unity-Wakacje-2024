using Assets.Scripts.Runtime.Character;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    // SETUP
    [SerializeField] private float _task_finish_time_seconds = 3.0f;
    [SerializeField] private float _task_minions_needed = 10;
    public UnityEvent<Interactable> TaskDoneCallback;

    // Interface visuals
    [SerializeField] private GameObject _doneCube;
    [SerializeField] private TextMeshPro textLabel;

    // Current state
    [SerializeField] private float _current_progress_seconds = 0.0f;
    [SerializeField] private HashSet<Minion> _minions = new HashSet<Minion>();


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
        _doneCube.transform.localScale = new Vector3(pct, 0.3f, 0.3f);
        textLabel.SetText($"{_minions.Count}/{_task_minions_needed}");
    }


    public void StartInteractionWithMinion(Minion minion)
    {
        _minions.Add(minion);
    }

    public void EndInteractionWithMinion(Minion minion)
    {
        _minions.Remove(minion);
    }

    public bool DoesNeedMoreMinions()
    {
        if( _minions.Count >= _task_minions_needed ) return false;
        return true;
    }

}
