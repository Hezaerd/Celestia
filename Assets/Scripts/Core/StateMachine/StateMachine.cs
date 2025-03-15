using System;
using System.Collections.Generic;

namespace Core.StateMachine
{
	public abstract class StateMachine
	{
		private StateNode _current;
		private readonly Dictionary<Type, StateNode> _nodes = new Dictionary<Type, StateNode>();
		private readonly HashSet<ITransition> _anyTransitions = new HashSet<ITransition>();

		private void Update()
		{
			ITransition transition = GetTransition();
			if (transition != null)
				ChangeState(transition.TargetState);
			
			_current.State?.Update();
		}
		
		private void FixedUpdate()
		{
			_current.State?.FixedUpdate();
		}
		
		private void SetState(IState state)
		{
			_current = _nodes[state.GetType()];
			_current.State?.OnEnter();
		}

		public void ChangeState(IState state)
		{
			if (state == _current.State)
				return;
			
			IState previousState = _current.State;
			IState nextState = _nodes[state.GetType()].State;
			
			previousState?.OnExit();
			nextState?.OnEnter();
			
			_current = _nodes[state.GetType()];
		}

		private ITransition GetTransition()
		{
			foreach (ITransition transition in _current.Transitions)
				if (transition.Condition.Evaluate())
					return transition;
			
			foreach (ITransition transition in _anyTransitions)
				if (transition.Condition.Evaluate())
					return transition;
			
			return null;
		}
		
		public void AddTransition(IState from, IState to, IPredicate condition)
		{
			GetOrCreateNode(from).AddTransition(GetOrCreateNode(to).State, condition);
		}
		
		public void AddAnyTransition(IState to, IPredicate condition)
		{
			_anyTransitions.Add(new Transition(GetOrCreateNode(to).State, condition));
		}
		
		private StateNode GetOrCreateNode(IState state)
		{
			StateNode node = _nodes.GetValueOrDefault(state.GetType());

			if (node != null)
				return node;
			
			node = new StateNode(state);
			_nodes.Add(state.GetType(), node);

			return node;
		}
		
		private class StateNode
		{
			public IState State { get; }
			public HashSet<ITransition> Transitions { get; }

			public StateNode(IState state)
			{
				State = state;
				Transitions = new HashSet<ITransition>();
			}
			
			public void AddTransition(IState targetState, IPredicate condition)
			{
				Transitions.Add(new Transition(targetState, condition));
			}
		}
	}
}