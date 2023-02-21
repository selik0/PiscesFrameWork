/****************
 *@class name:		StateImage
 *@description:		显示不同状态的图片
 *@author:			selik0
 *@date:			2023-02-13 21:51:08
 *@version: 		V1.0.0
*************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
namespace UnityEngine.UI
{
    public class StateImage : Image
    {
        [Serializable]
        public class StateData
        {
            public int state;
            public string statusDesc;
            public Sprite sprite;

            public StateData(int _status, string _statusDesc)
            {
                state = _status;
                statusDesc = _statusDesc;
            }
        }

        [FormerlySerializedAs("stateDataList")]
        [SerializeField]
        private List<StateData> m_StateDataList = new List<StateData>();

        [FormerlySerializedAs("state")]
        [SerializeField]
        private int m_CurrentState;

        public int currentState
        {
            get => m_CurrentState;
            set => SetState(value);
        }

        private Dictionary<int, StateData> m_StateDataDict = new Dictionary<int, StateData>();
        private Dictionary<int, StateData> _stateDataDict
        {
            get
            {
#if UNITY_EDITOR
                if (m_StateDataDict.Count <= 0)
#endif
                {
                    foreach (var stateData in m_StateDataList)
                    {
                        if (!m_StateDataDict.ContainsKey(stateData.state))
                            m_StateDataDict.Add(stateData.state, stateData);
                    }
                }
                return m_StateDataDict;
            }
        }

        public void SetState(int state)
        {
            if (!_stateDataDict.ContainsKey(state))
                return;
            m_CurrentState = state;
            PlayEffect();
        }

        void PlayEffect()
        {
            StateData currentStateData = null;
            if (!_stateDataDict.TryGetValue(m_CurrentState, out currentStateData))
                return;

            sprite = currentStateData.sprite;
        }
    }
}