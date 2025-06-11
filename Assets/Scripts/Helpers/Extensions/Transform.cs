using System.Collections.Generic;
using UnityEngine;

namespace RemoteEducation.Extensions
{
    public static class TransformExtensions
    {

        public static Transform FindRecursive(this Transform trm, string name)
        {
            Transform child = null;

            foreach (Transform t in trm)
            {
                if (t.name == name)
                {
                    child = t;
                    return child;
                }
                else if (t.childCount > 0)
                {
                    child = t.FindRecursive(name);
                    if (child)
                    {
                        return child;
                    }
                }
            }

            return child;
        }

        public static void GetAllChildren(this Transform parent, List<Transform> transforms)
        {
            foreach (Transform t in parent)
            {
                transforms.Add(t);

                GetAllChildren(t, transforms);
            }
        }

        public static void CopyTransform(this Transform to, Transform from)
        {
            to.position = from.position;
            to.rotation = from.rotation;
            to.localScale = from.localScale;
        }
    }
}
