/****************
 *@class name:		MyToggleGroup
 *@description:		
 *@author:			selik0
 *@date:			2023-02-01 15:26:28
 *@version: 		V1.0.0
*************************************************************************/
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/MyToggle Group", 32)]
    [DisallowMultipleComponent]
    public class MyToggleGroup : UIBehaviour
    {
        [SerializeField] private bool m_AllowSwitchOff = false;
        public bool allowSwitchOff { get { return m_AllowSwitchOff; } set { m_AllowSwitchOff = value; } }

        // 当MyToggleGroup的allowSwitchOff == false && m_AllowNoChange == true时点击isOn==true的MyToggle时不发出事件
        [SerializeField] private bool m_NoChangeDontSend = true;
        public bool noChangeDontSend { get { return m_NoChangeDontSend; } set { m_NoChangeDontSend = value; } }

        protected List<MyToggle> m_Toggles = new List<MyToggle>();

        protected MyToggleGroup() { }
        protected override void Start()
        {
            EnsureValidState();
            base.Start();
        }

        protected override void OnEnable()
        {
            EnsureValidState();
            base.OnEnable();
        }

        private void ValidateToggleIsInGroup(MyToggle toggle)
        {
            if (toggle == null || !m_Toggles.Contains(toggle))
                throw new ArgumentException(string.Format("MyToggle {0} is not part of ToggleGroup {1}", new object[] { toggle, this }));
        }
        public void NotifyToggleOn(MyToggle toggle, bool sendCallback = true)
        {
            ValidateToggleIsInGroup(toggle);
            // disable all toggles in the group
            for (var i = 0; i < m_Toggles.Count; i++)
            {
                if (m_Toggles[i] == toggle)
                    continue;

                if (sendCallback)
                    m_Toggles[i].isOn = false;
                else
                    m_Toggles[i].SetIsOnWithoutNotify(false);
            }
        }
        public void UnregisterToggle(MyToggle toggle)
        {
            if (m_Toggles.Contains(toggle))
                m_Toggles.Remove(toggle);
        }
        public void RegisterToggle(MyToggle toggle)
        {
            if (!m_Toggles.Contains(toggle))
                m_Toggles.Add(toggle);
        }
        public void EnsureValidState()
        {
            if (!allowSwitchOff && !AnyTogglesOn() && m_Toggles.Count != 0)
            {
                m_Toggles[0].isOn = true;
                NotifyToggleOn(m_Toggles[0]);
            }

            IEnumerable<MyToggle> activeToggles = ActiveToggles();

            if (activeToggles.Count() > 1)
            {
                MyToggle firstActive = GetFirstActiveToggle();

                foreach (MyToggle toggle in activeToggles)
                {
                    if (toggle == firstActive)
                    {
                        continue;
                    }
                    toggle.isOn = false;
                }
            }
        }
        public bool AnyTogglesOn()
        {
            return m_Toggles.Find(x => x.isOn) != null;
        }
        public IEnumerable<MyToggle> ActiveToggles()
        {
            return m_Toggles.Where(x => x.isOn);
        }
        public MyToggle GetFirstActiveToggle()
        {
            IEnumerable<MyToggle> activeToggles = ActiveToggles();
            return activeToggles.Count() > 0 ? activeToggles.First() : null;
        }
        public void SetAllTogglesOff(bool sendCallback = true)
        {
            bool oldAllowSwitchOff = m_AllowSwitchOff;
            m_AllowSwitchOff = true;

            if (sendCallback)
            {
                for (var i = 0; i < m_Toggles.Count; i++)
                    m_Toggles[i].isOn = false;
            }
            else
            {
                for (var i = 0; i < m_Toggles.Count; i++)
                    m_Toggles[i].SetIsOnWithoutNotify(false);
            }

            m_AllowSwitchOff = oldAllowSwitchOff;
        }
    }
}
