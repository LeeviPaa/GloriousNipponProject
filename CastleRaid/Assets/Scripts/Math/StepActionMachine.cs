using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class StepActionMachine
{
    public enum StepActionTime { Start, Update, End }

    private int state = -1;

    [SerializeField]
    private List<StepData> stepData;

    [System.Serializable]
    public class StepData
    {
        public UnityEvent OnStateStart;
        public UnityEvent OnStateUpdate;
        public UnityEvent OnStateEnd;
    }

    void StartStep()
    {
        if (state < stepData.Count && state >= 0)
        {
            stepData[state].OnStateStart.Invoke();
        }
    }

    void EndStep()
    {
        if (state < stepData.Count && state >= 0)
        {
            stepData[state].OnStateEnd.Invoke();
        }
    }

    public void Next()
    {
        EndStep();
        state++;
        StartStep();
    }

    public void Update()
    {
        stepData[state].OnStateUpdate.Invoke();
    }

    public void SetState(int value)
    {
        EndStep();
        state = value;
        StartStep();
    }

    public int GetState()
    {
        return state;
    }

    public void AddStepAction(int step, StepActionTime time, UnityAction action)
    {
        if (step >= stepData.Count)
        {
            for (int i = 0; i < step - stepData.Count + 1; i++)
            {
                stepData.Add(new StepData());
            }
        }

        switch (time)
        {
            case StepActionTime.Start:
                stepData[step].OnStateStart.AddListener(action);
                break;

            case StepActionTime.Update:
                stepData[step].OnStateUpdate.AddListener(action);
                break;

            case StepActionTime.End:
                stepData[step].OnStateEnd.AddListener(action);
                break;

            default:
                break;
        }
    }
}
