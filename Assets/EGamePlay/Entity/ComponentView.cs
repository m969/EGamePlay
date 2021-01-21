using UnityEngine;

namespace ET
{
    public class ComponentView: MonoBehaviour
    {
        public string Type;
        public object Component { get; set; }
    }
}