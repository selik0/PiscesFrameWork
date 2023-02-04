/****************
 *@class name:		MyNavigation
 *@description:		
 *@author:			selik0
 *@date:			2023-02-01 16:22:45
 *@version: 		V1.0.0
*************************************************************************/
using System;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    [Serializable]
    public struct MyNavigation : IEquatable<MyNavigation>
    {
        /*
         * This looks like it's not flags, but it is flags,
         * the reason is that Automatic is considered horizontal
         * and verical mode combined
         */
        [Flags]
        public enum Mode
        {
            None = 0, 
            Horizontal = 1, 
            Vertical = 2, 
            Automatic = 3, 
            Explicit = 4,
        }

        // Which method of navigation will be used.
        [SerializeField]
        private Mode m_Mode;

        [Tooltip("Enables navigation to wrap around from last to first or first to last element. Does not work for automatic grid navigation")]
        [SerializeField]
        private bool m_WrapAround;

        // Game object selected when the joystick moves up. Used when navigation is set to "Explicit".
        [SerializeField]
        private MySelectable m_SelectOnUp;

        // Game object selected when the joystick moves down. Used when navigation is set to "Explicit".
        [SerializeField]
        private MySelectable m_SelectOnDown;

        // Game object selected when the joystick moves left. Used when navigation is set to "Explicit".
        [SerializeField]
        private MySelectable m_SelectOnLeft;

        // Game object selected when the joystick moves right. Used when navigation is set to "Explicit".
        [SerializeField]
        private MySelectable m_SelectOnRight; public Mode mode { get { return m_Mode; } set { m_Mode = value; } }
        public bool wrapAround { get { return m_WrapAround; } set { m_WrapAround = value; } }
        public MySelectable selectOnUp { get { return m_SelectOnUp; } set { m_SelectOnUp = value; } }
        public MySelectable selectOnDown { get { return m_SelectOnDown; } set { m_SelectOnDown = value; } }
        public MySelectable selectOnLeft { get { return m_SelectOnLeft; } set { m_SelectOnLeft = value; } }
        public MySelectable selectOnRight { get { return m_SelectOnRight; } set { m_SelectOnRight = value; } }
        static public MyNavigation defaultNavigation
        {
            get
            {
                var defaultNav = new MyNavigation();
                defaultNav.m_Mode = Mode.Automatic;
                defaultNav.m_WrapAround = false;
                return defaultNav;
            }
        }

        public bool Equals(MyNavigation other)
        {
            return mode == other.mode &&
                selectOnUp == other.selectOnUp &&
                selectOnDown == other.selectOnDown &&
                selectOnLeft == other.selectOnLeft &&
                selectOnRight == other.selectOnRight;
        }
    }
}
