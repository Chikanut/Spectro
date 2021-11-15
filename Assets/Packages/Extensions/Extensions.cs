using System;
using System.Collections.Generic;
using UnityEngine;

namespace Packages.Utils.Extensions
{
    public static class Extensions {
    
        private const float Tolerance = 0.01f;
    
        public static bool BoxOverlapObject(this BoxCollider2D collider2D, LayerMask layerMask) {
            var collider = Physics2D.OverlapBox(collider2D.bounds.center, collider2D.bounds.size,
                collider2D.transform.eulerAngles.z, layerMask);
            return collider && collider2D.Distance(collider).distance < Tolerance;
        }

        public static bool CircleOverlapObject(this CircleCollider2D collider2D, LayerMask layerMask) {
            return Physics2D.OverlapCircle(collider2D.bounds.center,
                collider2D.radius, layerMask);
        }

        public static Bounds GetMaxBounds(this GameObject g) {
            var b = new Bounds(g.transform.position, Vector3.zero);

            foreach (Renderer r in g.GetComponentsInChildren<Renderer>())
            {
                if(r.GetComponent<ParticleSystem>())
                    continue;
            
                b.Encapsulate(r.bounds);
            }

            return b;
        }

    
    
        static readonly System.Random Randomizer = new System.Random();
    
        public static IEnumerable<T> GetRandom<T>(this T[] list, int numItems) {
            var items = new HashSet<T>(); // don't want to add the same item twice; otherwise use a list
            while (numItems > 0 )
                // if we successfully added it, move on
                if( items.Add(list[Randomizer.Next(list.Length)]) ) numItems--;

            return items;
        }
    
        public static void SetRectPositionFromWorldPosition(this RectTransform rectTransform, Vector3 worldPosition, Camera camera = null) {
            if (camera == null) {
                camera = Camera.main;
            }
            rectTransform.anchorMin = camera.WorldToViewportPoint(worldPosition);
            rectTransform.anchorMax = rectTransform.anchorMin;
        }

        public static DateTime DoubleToDate(double time)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).AddMilliseconds(time);
        }

        public static void SetSortingLayerRecursively(GameObject obj, int layer, int startOrder = 0)
        {
            if (null == obj)
            {
                return;
            }

            var renderer = obj.GetComponentsInChildren<Renderer>();

            foreach (var r in renderer)
            {
                r.sortingLayerID = layer;
                r.sortingOrder += startOrder;
            }
        }

        public static void SetLayerRecursively(GameObject obj, int newLayer)
        {
            if (null == obj)
            {
                return;
            }

            obj.layer = newLayer;

            foreach (Transform child in obj.transform)
            {
                if (null == child)
                {
                    continue;
                }
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)System.Enum.Parse(typeof(T), value, true);
        }
    }
}
