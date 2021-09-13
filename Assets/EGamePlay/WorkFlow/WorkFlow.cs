using EGamePlay;

public class WorkFlow : Entity
{
    public WorkFlowSource FlowSource { get; set; }
    public WorkFlow PreWorkFlow { get; set; }
    public WorkFlow PostWorkFlow { get; set; }
    // TODO: Branch 流程条件分支，根据上一流程的状态选择不同的分支流程
    // TODO: Valve 阀门，可以事先给流程添加阀门，通过阀门可以随时阻塞流程
    // TODO: ToRepeat 重复流程机制


    public virtual void Startup()
    {
        
    }

    public void Finish()
    {
        FlowSource.OnFlowFinish();
    }

    public WorkFlow ToEnter<T>() where T : WorkFlow
    {
        //Log.Debug($"{GetType().Name}->ToEnter {typeof(T).Name}");
        var workflow = AddChild<T>();
        workflow.FlowSource = FlowSource;
        PostWorkFlow = workflow;
        workflow.PreWorkFlow = this;
        workflow.PostWorkFlow = null;
        return workflow;
    }

    public void ToRestart()
    {
        ToEnter<WorkRestartFlow>().PostWorkFlow = FlowSource.PostWorkFlow;
    }

    public void ToEnd()
    {
        ToEnter<WorkEndFlow>();
    }
}
