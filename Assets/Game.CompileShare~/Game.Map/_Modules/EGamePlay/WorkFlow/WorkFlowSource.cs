using ECS;
using EGamePlay;

public class WorkFlowSource : EcsEntity
{
    public WorkFlow CurrentWorkFlow { get; private set; }
    public WorkFlow PostWorkFlow { get; private set; }


    public WorkFlow ToEnter<T>() where T : WorkFlow, new()
    {
        var workflow = AddChild<T>();
        PostWorkFlow = workflow;
        workflow.FlowSource = this;
        return workflow;
    }

    public void Startup()
    {
        CurrentWorkFlow = PostWorkFlow;
        CurrentWorkFlow.Startup();
    }

    public void OnFlowFinish()
    {
        //Log.Debug(($"{GetType().Name}->OnFlowFinish {CurrentWorkFlow.GetType().Name} {CurrentWorkFlow.PostWorkFlow}"));
        CurrentWorkFlow = CurrentWorkFlow.PostWorkFlow;
        CurrentWorkFlow.Startup();
    }
}
