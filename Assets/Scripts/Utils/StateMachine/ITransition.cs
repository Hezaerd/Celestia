namespace celestia.utils.statemachine
{
	public interface ITransition
	{
		IState TargetState { get; }
		IPredicate Condition { get; }
	}
}