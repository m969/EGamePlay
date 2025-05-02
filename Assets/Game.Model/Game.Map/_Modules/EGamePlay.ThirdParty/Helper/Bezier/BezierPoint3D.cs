using System;
using UnityEngine;

#if EGAMEPLAY_ET
using Unity.Mathematics;
using Vector3 = Unity.Mathematics.float3;
using Quaternion = Unity.Mathematics.quaternion;
using JsonIgnore = MongoDB.Bson.Serialization.Attributes.BsonIgnoreAttribute;
#endif


namespace NaughtyBezierCurves
{
    [System.Serializable]
    public class BezierPoint3D
    {
        public enum HandleType
        {
            Connected,
            Broken
        }

        // Serializable Fields
        //[SerializeField]
        [HideInInspector, JsonIgnore]
        [Tooltip("The curve that the point belongs to")]
        private BezierCurve3D curve = null;

        [SerializeField, JsonIgnore]
        private HandleType handleType = HandleType.Connected;

        [SerializeField, JsonIgnore]
        private Vector3 leftHandleLocalPosition = new Vector3(-0.5f, 0f, 0f);

        [SerializeField, JsonIgnore]
        private Vector3 rightHandleLocalPosition = new Vector3(0.5f, 0f, 0f);

        // Properties

        /// <summary>
        /// Gets or sets the curve that the point belongs to.
        /// </summary>
        [JsonIgnore]
        public BezierCurve3D Curve
        {
            get
            {
                return this.curve;
            }
            set
            {
                this.curve = value;
            }
        }
        [JsonIgnore]
        public Vector3 CurvePosition
        {
            get
            {
                if (curve == null) return Vector3.zero;
                return this.curve.OriginPosition;
            }
        }

        /// <summary>
        /// Gets or sets the type/style of the handle.
        /// </summary>
        public HandleType HandleStyle
        {
            get
            {
                return this.handleType;
            }
            set
            {
                this.handleType = value;
            }
        }

        /// <summary>
        /// Gets or sets the position of the transform.
        /// </summary>
        [JsonIgnore]
        public Vector3 Position
        {
            get
            {
                return CurvePosition + LocalPosition;
            }
            set
            {
                LocalPosition = value - CurvePosition;
            }
        }

        /// <summary>
        /// Gets or sets the position of the transform.
        /// </summary>
        public Vector3 LocalPosition;
        //{
        //    get
        //    {
        //        return this.transform.localPosition;
        //    }
        //    set
        //    {
        //        this.transform.localPosition = value;
        //    }
        //}

        /// <summary>
        /// Gets or sets the local position of the left handle.
        /// If the HandleStyle is Connected, the local position of the right handle is automaticaly set.
        /// </summary>
        public Vector3 LeftHandleLocalPosition
        {
            get
            {
                return this.leftHandleLocalPosition;
            }
            set
            {
                this.leftHandleLocalPosition = value;
                if (this.handleType == HandleType.Connected)
                {
                    this.rightHandleLocalPosition = -value;
                }
            }
        }
        [JsonIgnore]
        public Vector3 InTangent { get => LeftHandleLocalPosition; set => LeftHandleLocalPosition = value; }

        /// <summary>
        /// Gets or sets the local position of the right handle.
        /// If the HandleType is Connected, the local position of the left handle is automaticaly set.
        /// </summary>
        public Vector3 RightHandleLocalPosition
        {
            get
            {
                return this.rightHandleLocalPosition;
            }
            set
            {
                this.rightHandleLocalPosition = value;
                if (this.handleType == HandleType.Connected)
                {
                    this.leftHandleLocalPosition = -value;
                }
            }
        }
        [JsonIgnore]
        public Vector3 OutTangent { get => RightHandleLocalPosition; set => RightHandleLocalPosition = value; }

        /// <summary>
        /// Gets or sets the position of the left handle.
        /// If the HandleStyle is Connected, the position of the right handle is automaticaly set.
        /// </summary>
        [JsonIgnore]
        public Vector3 LeftHandlePosition
        {
            get
            {
                if (this.handleType == HandleType.Broken) return Position;
                return Position + (this.LeftHandleLocalPosition);
            }
            //set
            //{
            //    this.LeftHandleLocalPosition = (value) - Position;
            //}
        }

        /// <summary>
        /// Gets or sets the position of the right handle.
        /// If the HandleType is Connected, the position of the left handle is automaticaly set.
        /// </summary>
        [JsonIgnore]
        public Vector3 RightHandlePosition
        {
            get
            {
                if (this.handleType == HandleType.Broken) return Position;
                return Position + (this.RightHandleLocalPosition);
            }
            //set
            //{
            //    this.RightHandleLocalPosition = (value) - Position;
            //}
        }
    }
}
