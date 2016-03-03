using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pair<T, S>
{
    public T First;
    public S Second;
}

/*
A (slightly over-engineered) finite state machine with generic type as the states (usually I use an enum). 
Has global or state specific callbacks for state changes as well as an Update callback that can be run each Update, 
FixedUpdate frame, or even every few seconds and will call a specified update function of the state that it is in.
*/

public class FSM<T> where T : struct, System.IComparable
{
	Dictionary<T, StateData> _stateData;
	bool _canDynamicallyAddStates = true;
	
	public bool alwaysDoStateChangeImmediately = false;
	public bool fireStateChangeEventsOnNonStateChange = false;
	
	public StateUpdateFunction globalUpdateFunction = null;
	public StateChangeFunction globalStateChangeFunction = null;

	bool stateUnset = true;

	public T CurrentState
	{
		get
		{
			return _currentState;
		}
	}

	T _currentState;
	
	T _targetState;
	
	public delegate void StateUpdateFunction(T currentState);
	public delegate void StateChangeFunction(T  oldState, T currentState);

	
	public class StateData
	{
		public StateUpdateFunction updateFunction;


		
		public StateChangeFunction changeToStateFunction = null;
		public StateChangeFunction changeFromStateFunction = null;
		
		HashSet<T> allowablePaths { get; set; } // if null it means let all states through
		
		public IEnumerable<T> AllowablePaths
		{
			get
			{
				foreach (T t in allowablePaths)
				{
					yield return t;
				}
			}
		}
		
		public bool IsChangeToStateAllowed(T toState)
		{
			if (allowablePaths == null) return true;
			
			else return allowablePaths.Contains(toState);
		}
		
		public void AddAllowableStateChange(T toState)
		{
			AddNullStateChange();
			
			allowablePaths.Add(toState);
		}
		public void AddNullStateChange()
		{
			if (allowablePaths == null)
				allowablePaths = new HashSet<T>();
		}
	}
	
	public StateData this [T t]
	{
		get
		{
			if (!_stateData.ContainsKey(t))
			{
				if (_canDynamicallyAddStates)
				{
					_stateData[t] = new StateData();
				}
				else
				{
					Debug.LogError("Accessing state that is not allowed in FSM: "+t+"\n" +
						"Allowable States are:\n"+
						string.Join("\n", new List<T>(_stateData.Keys).ConvertAll((state) => state.ToString()).ToArray()));
					
					return null;
				}
			}
			
			
			return _stateData[t];
			
		}
	}
	
	public FSM()
	{
		_stateData = new Dictionary<T, StateData>();
	}

	public FSM(IEnumerable<T> allowedStates) : this ()
	{
		foreach (T allowedState in allowedStates)
		{
			AddStateExplicitly(allowedState);
		}
	}
	
	public FSM(IEnumerable<Pair<T,T>> allowedTransistions) : this ()
	{
		foreach (Pair<T,T> allowedTransistion in allowedTransistions)
		{
			T fromState = allowedTransistion.First;
			T toState = allowedTransistion.Second;
			
			AddStateWithNoAllowedTransistions(fromState);
			_stateData[fromState].AddNullStateChange();
			
			AddStateTransistionExplicitly(fromState, toState);
			
		}
	}
	
	public void AddStateExplicitly(T stateToAdd)
	{
		if (!_stateData.ContainsKey(stateToAdd))
		{
			_stateData[stateToAdd] = new StateData();
		}
		_canDynamicallyAddStates = false;
	}
	public void AddStateWithNoAllowedTransistions(T fromState)
	{
		AddStateExplicitly(fromState);
		_stateData[fromState].AddNullStateChange();
		
	}
	public void AddStateTransistionExplicitly(T fromState, T toState)
	{
		AddStateExplicitly(fromState);
		AddStateExplicitly(toState);
		_stateData[fromState].AddAllowableStateChange(toState);
	}
	
	public void ChangeState(T newState, bool performChangeImmediately = false)
	{
		if (!_stateData.ContainsKey(newState))
		{
			if (_canDynamicallyAddStates)
			{
				_stateData[newState] = new StateData();
			}
			else
			{
				
				Debug.LogError("Changing to state that is not allowed in FSM: "+newState+"\n" +
					"Allowable States are:\n"+
					string.Join("\n", new List<T>(_stateData.Keys).ConvertAll((t) => t.ToString()).ToArray()));
				return;
			}
		}



		if (_stateData[newState].IsChangeToStateAllowed(newState))
		{
			_targetState = newState;
			
			if (alwaysDoStateChangeImmediately || performChangeImmediately)
			{
				DoStateChangeCheckAndNotify();
			}
		}
		else
		{
			Debug.LogError("Illegal State Change: "+_currentState +" -> "+newState+"\n" +
				"Allowable States are:\n"+
				string.Join("\n", new List<T>(_stateData[newState].AllowablePaths).ConvertAll((t) => t.ToString()).ToArray()));
		}

	}
	
	void DoStateChangeCheckAndNotify()
	{
		if (fireStateChangeEventsOnNonStateChange || _currentState.CompareTo(_targetState) != 0 || stateUnset)
		{
			stateUnset = false;
			T newState = _targetState;
			T oldState = _currentState;

			_currentState = _targetState;

			if (globalStateChangeFunction != null)
			{
				globalStateChangeFunction(oldState, newState);
			}
			if (this[oldState].changeFromStateFunction != null)
			{
				this[oldState].changeToStateFunction(oldState, newState);
			}
			if (this[newState].changeToStateFunction != null)
		    {
				this[newState].changeToStateFunction(oldState, newState);
			}

			
		}
	}
	
	public void UpdateAll()
	{
		DoStateChangeCheckAndNotify();
		
		if (this[_currentState].updateFunction != null)
		{
			this[_currentState].updateFunction(_currentState);
		}
		if (globalUpdateFunction != null)
		{
			globalUpdateFunction(_currentState);
		}
	}
}
