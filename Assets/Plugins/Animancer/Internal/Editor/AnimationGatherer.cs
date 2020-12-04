// Animancer // https://kybernetik.com.au/animancer // Copyright 2020 Kybernetik //

#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Animancer.Editor
{
    /// <summary>[Editor-Only]
    /// A system that procedurally gathers animations throughout the hierarchy without needing explicit references.
    /// </summary>
    /// <remarks>
    /// This class is [Editor-Only] because it uses reflection and is not particularly efficient, but it does not
    /// actually use any Editor Only functionality so it could be made usable at runtime by simply removing the
    /// <c>#if UNITY_EDITOR</c> at the top of the file and <c>#endif</c> at the bottom.
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/AnimationGatherer
    /// 
    public static class AnimationGatherer
    {
        /************************************************************************************************************************/
        #region Recursion Guard
        /************************************************************************************************************************/

        private const int MaxFieldDepth = 7;

        /************************************************************************************************************************/

        private static readonly HashSet<object>
            RecursionGuard = new HashSet<object>();

        private static int _CallCount;

        private static bool BeginRecursionGuard(object obj)
        {
            if (RecursionGuard.Contains(obj))
                return false;

            RecursionGuard.Add(obj);
            return true;
        }

        private static void EndCall()
        {
            if (_CallCount == 0)
                RecursionGuard.Clear();
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Cache
        /************************************************************************************************************************/

        private static readonly Dictionary<GameObject, HashSet<AnimationClip>>
            ObjectToClips = new Dictionary<GameObject, HashSet<AnimationClip>>();

        /************************************************************************************************************************/

        static AnimationGatherer()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.hierarchyChanged += ClearCache;
            UnityEditor.Selection.selectionChanged += ClearCache;
#endif
        }

        /************************************************************************************************************************/

        /// <summary>Clears all cached animations.</summary>
        public static void ClearCache()
        {
            if (ObjectToClips.Count == 0)
                return;

            foreach (var clips in ObjectToClips.Values)
            {
                ObjectPool.Release(clips);
            }

            ObjectToClips.Clear();
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Exceptions
        /************************************************************************************************************************/

        /// <summary>An action that can be performed in response to an exception.</summary>
        public enum ExceptionHandler
        {
            /// <summary>
            /// Store the exception in a list so it can be displayed in the <see cref="TransitionPreviewWindow"/> if
            /// the user wants to debug it. This is the default because some systems are not prepared to have their
            /// members accessed via reflection and we don't want to bother the user with non-critical errors.
            /// </summary>
            /// <remarks>Only</remarks>
            Store,

            /// <summary>Log the exception normally.</summary>
            Log,
        }

        /************************************************************************************************************************/

        /// <summary>
        /// The exceptions that have been stored according to the <see cref="ExceptionCapacity"/>.
        /// This property is null if no exceptions have been thrown yet.
        /// </summary>
        internal static List<Exception> Exceptions { get; private set; }

        private static int _ExceptionCapacity = 10;

        /// <summary>
        /// A positive value causes exceptions thrown while gathering animations to be stores in a list so they can be
        /// displayed in the <see cref="TransitionPreviewWindow"/> if the user wants to debug them rather than logging
        /// them immediately.
        /// <para></para>
        /// 0 causes exceptions to be ignored entirely and any negative value causes them to be logged immediately.
        /// </summary>
        public static int ExceptionCapacity
        {
            get => _ExceptionCapacity;
            set
            {
                _ExceptionCapacity = value;
                if (Exceptions != null)
                {
                    if (value > 0)
                    {
                        if (value < Exceptions.Count)
                            Exceptions.RemoveRange(value, Exceptions.Count - value);

                        Exceptions.Capacity = value;
                    }
                    else
                    {
                        Exceptions = null;
                    }
                }
            }
        }

        /************************************************************************************************************************/

        private static void HandleException(Exception exception)
        {
            if (_ExceptionCapacity > 0)
            {
                if (Exceptions == null)
                    Exceptions = new List<Exception>(_ExceptionCapacity);

                if (Exceptions.Count < Exceptions.Capacity)
                    Exceptions.Add(exception);
            }
            else if (_ExceptionCapacity < 0)
            {
                Debug.LogException(exception);
            }
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/

        /// <summary>
        /// Fills the `clips` with any <see cref="AnimationClip"/>s referenced by components in the same hierarchy as
        /// the `gameObject`. See <see cref="ICharacterRoot"/> for details.
        /// </summary>
        public static void GatherFromGameObject(GameObject gameObject, ICollection<AnimationClip> clips)
        {
            if (!BeginRecursionGuard(gameObject))
                return;

            try
            {
                _CallCount++;
                if (!ObjectToClips.TryGetValue(gameObject, out var clipSet))
                {
                    clipSet = ObjectPool.AcquireSet<AnimationClip>();
                    GatherFromComponents(gameObject, clipSet);
                    ObjectToClips.Add(gameObject, clipSet);
                }

                clips.Gather(clipSet);
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
            finally
            {
                _CallCount--;
                EndCall();
            }
        }

        /// <summary>
        /// Fills the `clips` with any <see cref="AnimationClip"/>s referenced by components in the same hierarchy as
        /// the `gameObject`. See <see cref="ICharacterRoot"/> for details.
        /// </summary>
        public static void GatherFromGameObject(GameObject gameObject, ref AnimationClip[] clips, bool sort)
        {
            if (!BeginRecursionGuard(gameObject))
                return;

            try
            {
                _CallCount++;

                using (ObjectPool.Disposable.AcquireSet<AnimationClip>(out var clipSet))
                {
                    GatherFromComponents(gameObject, clipSet);

                    if (clips == null || clips.Length != clipSet.Count)
                        clips = new AnimationClip[clipSet.Count];

                    clipSet.CopyTo(clips);
                }

                if (sort)
                    Array.Sort(clips, (a, b) => a.name.CompareTo(b.name));
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
            finally
            {
                _CallCount--;
                EndCall();
            }
        }

        /************************************************************************************************************************/

        private static void GatherFromComponents(GameObject gameObject, HashSet<AnimationClip> clips)
        {
            var root = AnimancerEditorUtilities.FindRoot(gameObject);

            using (ObjectPool.Disposable.AcquireList<MonoBehaviour>(out var components))
            {
                root.GetComponentsInChildren(true, components);
                GatherFromComponents(components, clips);
            }
        }

        /************************************************************************************************************************/

        private static void GatherFromComponents(List<MonoBehaviour> components, HashSet<AnimationClip> clips)
        {
            var i = components.Count;
            GatherClips:
            try
            {
                while (--i >= 0)
                {
                    GatherFromObject(components[i], clips, 0);
                }
            }
            catch (Exception exception)
            {
                HandleException(exception);
                goto GatherClips;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Gathers all animations from the `source`s fields.</summary>
        private static void GatherFromObject(object source, ICollection<AnimationClip> clips, int depth)
        {
            if (source is AnimationClip clip)
            {
                clips.Add(clip);
                return;
            }

            if (!MightContainAnimations(source.GetType()))
                return;

            if (!BeginRecursionGuard(source))
                return;

            try
            {
                if (clips.GatherFromSource(source))
                    return;
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
            finally
            {
                RecursionGuard.Remove(source);
            }

            GatherFromFields(source, clips, depth);
        }

        /************************************************************************************************************************/

        /// <summary>Types mapped to a delegate that can quickly gather their clips.</summary>
        private static readonly Dictionary<Type, Action<object, ICollection<AnimationClip>>>
            TypeToGatherer = new Dictionary<Type, Action<object, ICollection<AnimationClip>>>();

        /// <summary>
        /// Uses reflection to gather <see cref="AnimationClip"/>s from fields on the `source` object.
        /// </summary>
        private static void GatherFromFields(object source, ICollection<AnimationClip> clips, int depth)
        {
            if (depth >= MaxFieldDepth ||
                source == null ||
                !BeginRecursionGuard(source))
                return;

            var type = source.GetType();

            if (!TypeToGatherer.TryGetValue(type, out var gatherClips))
            {
                gatherClips = BuildClipGatherer(type, depth);
                TypeToGatherer.Add(type, gatherClips);
            }

            gatherClips?.Invoke(source, clips);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Creates a delegate to gather <see cref="AnimationClip"/>s from all relevant fields in a given `type`.
        /// </summary>
        private static Action<object, ICollection<AnimationClip>> BuildClipGatherer(Type type, int depth)
        {
            if (!MightContainAnimations(type))
                return null;

            Action<object, ICollection<AnimationClip>> gatherer = null;

            while (type != null)
            {
                var fields = type.GetFields(AnimancerEditorUtilities.InstanceBindings);
                for (int i = 0; i < fields.Length; i++)
                {
                    var field = fields[i];
                    var fieldType = field.FieldType;
                    if (!MightContainAnimations(fieldType))
                        continue;

                    if (fieldType == typeof(AnimationClip))
                    {
                        gatherer += (obj, clips) =>
                        {
                            var clip = (AnimationClip)field.GetValue(obj);
                            clips.Gather(clip);
                        };
                    }
                    else if (typeof(IAnimationClipSource).IsAssignableFrom(fieldType) ||
                        typeof(IAnimationClipCollection).IsAssignableFrom(fieldType))
                    {
                        gatherer += (obj, clips) =>
                        {
                            var source = field.GetValue(obj);
                            clips.GatherFromSource(source);
                        };
                    }
                    else if (typeof(ICollection).IsAssignableFrom(fieldType))
                    {
                        gatherer += (obj, clips) =>
                        {
                            var collection = (ICollection)field.GetValue(obj);
                            if (collection != null)
                            {
                                foreach (var item in collection)
                                {
                                    GatherFromObject(item, clips, depth + 1);
                                }
                            }
                        };
                    }
                    else
                    {
                        gatherer += (obj, clips) =>
                        {
                            var source = field.GetValue(obj);
                            if (source == null ||
                                (source is Object sourceObject && sourceObject == null))
                                return;

                            GatherFromObject(source, clips, depth + 1);
                        };
                    }
                }

                type = type.BaseType;
            }

            return gatherer;
        }

        /************************************************************************************************************************/

        private static bool MightContainAnimations(Type type)
        {
            return
                !type.IsPrimitive &&
                !type.IsEnum &&
                !type.IsAutoClass &&
                !type.IsPointer;
        }

        /************************************************************************************************************************/
    }
}

#endif

