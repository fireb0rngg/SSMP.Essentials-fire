using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SSMPUtils.Utils
{
    internal static class GameObjectUtils
    {
        public static GameObject? FindGameObjectInChildren(this GameObject parent, string path)
        {
            string[] names = path.Split('/');

            if (parent == null) return null;

            for (int i = 0; i < names.Length; i++)
            {
                var name = names[i];

                var nextObject = FindGameObjectLayer(name, parent);
                if (nextObject == null) return null;

                parent = nextObject;
            }

            return parent;
        }

        static GameObject? FindGameObjectLayer(string name, GameObject parent)
        {
            var childCount = parent.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = parent.transform.GetChild(i);
                if (child.name == name) return child.gameObject;
            }

            Log.LogWarning($"{name} not found");
            return null;
        }
    }
}
