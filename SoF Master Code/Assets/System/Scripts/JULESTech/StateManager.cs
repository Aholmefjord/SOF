using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSys
{
    public interface IState
    {
        void OnEnter();
        void Update(float _dt);
        void OnExit();

        bool IsDone { get; set; }
        bool IsInit { get; set; }
        string Name { get; }
    }

    public sealed class StateManager
    {
        private List<IState> stateList = new List<IState>();

        private StateManager()
        {
        }

        public static StateManager Instance
        {
            get { return Nested.instance; }
        }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly StateManager instance = new StateManager();
        }

        public void Update(float _dt)
        {
            if (stateList.Count == 0)
                return;

            IState currState = stateList[0];

            if (!currState.IsInit)
            {
                currState.OnEnter();
                currState.IsInit = true;
            }

            currState.Update(_dt);

            if (currState.IsDone)
            {
                currState.OnExit();
                RemoveState(0);
            }
        }

        public int NumberOfStates()
        {
            return stateList.Count;
        }

        public void AddFront(IState _newState)
        {
            stateList.Insert(0, _newState);
        }

        public void AddBack(IState _newState)
        {
            stateList.Add(_newState);
        }

        public void RemoveState(int _index)
        {
            stateList.RemoveAt(_index);
        }

        public IState GetState(int _index)
        {
            return stateList[_index];
        }

        public IState GetFirstState()
        {
            if (stateList.Count == 0)
                return null;

            return stateList[0];
        }

        public IState GetLastState()
        {
            if (stateList.Count == 0)
                return null;

            return stateList[stateList.Count - 1];
        }

        public void ClearAllStates()
        {
            if (stateList.Count < 2)
                return;

            int index = stateList.Count - 1;

            while (index != 0)
            {
                stateList[index].OnExit();
                RemoveState(index);
                --index;
            }
        }
    }
}