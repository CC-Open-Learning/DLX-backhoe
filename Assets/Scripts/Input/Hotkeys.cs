using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemoteEducation
{
    public static class Hotkeys
    {
        private static HashSet<object> locks = new HashSet<object>();

        public static bool AddLock(object o)
        {
            return locks.Add(o);
        }

        public static bool RemoveLock(object o)
        {
            return locks.Remove(o);
        }

        public static bool GetKeyDown(KeyCode key)
        {
            return locks.Count <= 0 && Input.GetKeyDown(key);
        }

        public static bool GetKey(KeyCode key)
        {
            return locks.Count <= 0 && Input.GetKey(key);
        }

        public static bool GetKeyUp(KeyCode key)
        {
            return locks.Count <= 0 && Input.GetKeyUp(key);
        }
    }
}
