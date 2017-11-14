using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class StepActionMachine
{
    public enum StepActionTime { Start, Update }

    private int state = -1;

    [SerializeField]
    private List<StepData> stepData;

    [System.Serializable]
    public class StepData
    {
        [SerializeField]
        private UnityEvent StateStartInspector;
        [SerializeField]
        private UnityEvent StateUpdateInspector;
        public delegate void StepActionDelegate();
        public StepActionDelegate StateStart;
        public StepActionDelegate StateUpdate;

        public void OnStateStart()
        {
            StateStartInspector.Invoke();
            if (StateStart != null)
            {
                StateStart.Invoke();
            }
        }

        public void OnStateUpdate()
        {
            StateUpdateInspector.Invoke();
            if (StateUpdate != null)
            {
                StateUpdate.Invoke();
            }
        }
    }

    void StartStep()
    {
        if (state < stepData.Count && state >= 0)
        {
            stepData[state].OnStateStart();
        }
    }

    public void Next()
    {
        state++;
        StartStep();
    }

    public void Update()
    {
        if (state < stepData.Count && state >= 0)
        {
            stepData[state].OnStateUpdate();
        }
    }

    public void SetState(int value)
    {
        state = value;
        StartStep();
    }

    public int GetState()
    {
        return state;
    }

    public void AddStepAction(int step, StepActionTime time, StepData.StepActionDelegate action)
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
                stepData[step].StateStart += action;
                break;

            case StepActionTime.Update:
                stepData[step].StateUpdate += action;
                break;

            default:
                break;
        }
    }
}
