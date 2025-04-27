using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine;
using Sirenix.OdinInspector;

//namespace EGamePlay.Combat
//{
//    [NotKeyable]
//    public class SkillPlayableAsset : PlayableAsset, IPropertyPreview, ITimelineClipAsset
//    {
//        [SerializeField]
//        [LabelText("应用效果")]
//        public uint EffectIndex;
        
//        /// <summary>
//        /// GameObject in the scene to control, or the parent of the instantiated prefab.
//        /// </summary>
//        [SerializeField] public ExposedReference<GameObject> sourceGameObject;
//        PlayableAsset m_ControlDirectorAsset;
//        double m_Duration = PlayableBinding.DefaultDuration;
//        bool m_SupportLoop;

//        public override double duration { get; }
//        public ClipCaps clipCaps { get; }


//        public SkillPlayableAsset()
//        {

//        }

//        public void OnEnable()
//        {

//        }

//        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
//        {
//            Playable root = Playable.Null;
//            if (!root.IsValid())
//                root = Playable.Create(graph);
//            return root;
//        }

//        public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
//        {
            
//        }
//    }
//}