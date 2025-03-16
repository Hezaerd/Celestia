namespace MHL.Core.StateMachine
{
	public interface ITransition
	{
		IState TargetState { get; }
		IPredicate Condition { get; }
	}
}