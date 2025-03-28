namespace celestia.utils.statemachine
{
	public interface IState
	{
		void OnEnter();
		void Update();
		void FixedUpdate();
		void OnExit();	
	}
}