using UnityEngine;

namespace EGamePlay
{
    public class ComponentView: MonoBehaviour
    {
        public string Type;
        public object Component { get; set; }
    }
}