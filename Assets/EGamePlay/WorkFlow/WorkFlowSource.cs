using EGamePlay;
using System;
using System.Collections.Generic;
using System.Collections;

public class WorkFlowSource : Entity
{
    //public List<Type> FlowTypeList { get; private set; } = new List<Type>();
    //public Queue<WorkFlow> WorkFlowSequence { get; private set; } = new Queue<WorkFlow>();
    public WorkFlow CurrentWorkFlow { get; private set; }
    public WorkFlow PostWorkFlow { get; private set; }


    public WorkFlow ToEnter<T>() where T : WorkFlow
    {
        //FlowTypeList.Add(typeof(T));

        var workflow = PostWorkFlow = Parent.CreateChild<T>();
        workflow.FlowSource = this;

        //if (WorkFlowSequence.Peek() != null)
        //{
        //    workflow.PreWorkFlow = WorkFlowSequence.Peek();
        //}
        //WorkFlowSequence.Enqueue(workflow);
        return workflow;
    }

    public void Startup()
    {
        CurrentWorkFlow = PostWorkFlow;
        CurrentWorkFlow.Startup();
    }

    public void OnFlowFinish()
    {
        CurrentWorkFlow = CurrentWorkFlow.PostWorkFlow;
        CurrentWorkFlow.Startup();
    }
}
