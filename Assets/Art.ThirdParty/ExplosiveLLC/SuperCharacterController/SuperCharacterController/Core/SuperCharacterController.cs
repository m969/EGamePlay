using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Custom character controller, to be used by attaching the component to an object
/// and writing scripts attached to the same object that recieve the "SuperUpdate" message
/// </summary>
public class SuperCharacterController : MonoBehaviour
{
    [SerializeField]
    Vector3 debugMove = Vector3.zero;

    [SerializeField]
    QueryTriggerInteraction triggerInteraction;

    [SerializeField]
    bool fixedTimeStep;

    [SerializeField]
    int fixedUpdatesPerSecond;

    [SerializeField]
    bool clampToMovingGround;

    [SerializeField]
    bool debugSpheres;

    [SerializeField]
    bool debugGrounding;

    [SerializeField]
    bool debugPushbackMesssages;

    /// <summary>
    /// Describes the Transform of the object we are standing on as well as it's CollisionType, as well
    /// as how far the ground is below us and what angle it is in relation to the controller.
    /// </summary>
    [SerializeField]
    public struct Ground
    {
        public RaycastHit hit { get; set; }
        public RaycastHit nearHit { get; set; }
        public RaycastHit farHit { get; set; }
        public RaycastHit secondaryHit { get; set; }
        public SuperCollisionType collisionType { get; set; }
        public Transform transform { get; set; }

        public Ground(RaycastHit hit, RaycastHit nearHit, RaycastHit farHit, RaycastHit secondaryHit, SuperCollisionType superCollisionType, Transform hitTransform)
        {
            this.hit = hit;
            this.nearHit = nearHit;
            this.farHit = farHit;
            this.secondaryHit = secondaryHit;
            this.collisionType = superCollisionType;
            this.transform = hitTransform;
        }
    }

    [SerializeField]
    CollisionSphere[] spheres =
        new CollisionSphere[3] {
            new CollisionSphere(0.5f, true, false),
            new CollisionSphere(1.0f, false, false),
            new CollisionSphere(1.5f, false, true),
        };

    public LayerMask Walkable;

    [SerializeField]
    Collider ownCollider;

    [SerializeField]
    public float radius = 0.5f;

    public float deltaTime { get; private set; }
    public SuperGround currentGround { get; private set; }
    public CollisionSphere feet { get; private set; }
    public CollisionSphere head { get; private set; }

    /// <summary>
    /// Total height of the controller from the bottom of the feet to the top of the head
    /// </summary>
    public float height { get { return Vector3.Distance(SpherePosition(head), SpherePosition(feet)) + radius * 2; } }

    public Vector3 up { get { return transform.up; } }
    public Vector3 down { get { return -transform.up; } }
    public List<SuperCollision> collisionData { get; private set; }
    public Transform currentlyClampedTo { get; set; }
    public float heightScale { get; set; }
    public float radiusScale { get; set; }
    public bool manualUpdateOnly { get; set; }

    public delegate void UpdateDelegate();
    public event UpdateDelegate AfterSingleUpdate;

    private Vector3 initialPosition;
    private Vector3 groundOffset;
    private Vector3 lastGroundPosition;
    private bool clamping = true;
    private bool slopeLimiting = true;

    private List<Collider> ignoredColliders;
    private List<IgnoredCollider> ignoredColliderStack;

    private const float Tolerance = 0.05f;
    private const float TinyTolerance = 0.01f;
    private const string TemporaryLayer = "TempCast";
    private const int MaxPushbackIterations = 2;
    private int TemporaryLayerIndex;
    private float fixedDeltaTime;

    private static SuperCollisionType defaultCollisionType;

    void Awake()
    {
        collisionData = new List<SuperCollision>();

        TemporaryLayerIndex = LayerMask.NameToLayer(TemporaryLayer);

        ignoredColliders = new List<Collider>();
        ignoredColliderStack = new List<IgnoredCollider>();

        currentlyClampedTo = null;

        fixedDeltaTime = 1.0f / fixedUpdatesPerSecond;

        heightScale = 1.0f;

		if(ownCollider)
		{
			IgnoreCollider(ownCollider);
		}

        foreach (var sphere in spheres)
        {
            if (sphere.isFeet)
                feet = sphere;

            if (sphere.isHead)
                head = sphere;
        }

		if(feet == null)
		{
			Debug.LogError("[SuperCharacterController] Feet not found on controller");
		}

		if(head == null)
		{
			Debug.LogError("[SuperCharacterController] Head not found on controller");
		}

		if(defaultCollisionType == null)
		{
			defaultCollisionType = new GameObject("DefaultSuperCollisionType", typeof(SuperCollisionType)).GetComponent<SuperCollisionType>();
		}

        currentGround = new SuperGround(Walkable, this, triggerInteraction);

        manualUpdateOnly = false;

        gameObject.SendMessage("SuperStart", SendMessageOptions.DontRequireReceiver);
    }

    void Update()
    {
        // If we are using a fixed timestep, ensure we run the main update loop
        // a sufficient number of times based on the Time.deltaTime
        if (manualUpdateOnly)
            return;

        if (!fixedTimeStep)
        {
            deltaTime = Time.deltaTime;

            SingleUpdate();
            return;
        }
        else
        {
            float delta = Time.deltaTime;

            while (delta > fixedDeltaTime)
            {
                deltaTime = fixedDeltaTime;

                SingleUpdate();

                delta -= fixedDeltaTime;
            }

            if (delta > 0f)
            {
                deltaTime = delta;

                SingleUpdate();
            }
        }
    }

    public void ManualUpdate(float deltaTime)
    {
        this.deltaTime = deltaTime;

        SingleUpdate();
    }

    void SingleUpdate()
    {
        // Check if we are clamped to an object implicity or explicity
        bool isClamping = clamping || currentlyClampedTo != null;
        Transform clampedTo = currentlyClampedTo != null ? currentlyClampedTo : currentGround.transform;

        if (clampToMovingGround && isClamping && clampedTo != null && clampedTo.position - lastGroundPosition != Vector3.zero)
            transform.position += clampedTo.position - lastGroundPosition;

        initialPosition = transform.position;

        ProbeGround(1);

        transform.position += debugMove * deltaTime;

        gameObject.SendMessage("SuperUpdate", SendMessageOptions.DontRequireReceiver);

        collisionData.Clear();

        RecursivePushback(0, MaxPushbackIterations);

        ProbeGround(2);

		if(slopeLimiting)
		{
			SlopeLimit();
		}

        ProbeGround(3);

		if(clamping)
		{
			ClampToGround();
		}

        isClamping = clamping || currentlyClampedTo != null;
        clampedTo = currentlyClampedTo != null ? currentlyClampedTo : currentGround.transform;

		if(isClamping)
		{
			lastGroundPosition = clampedTo.position;
		}

		if(debugGrounding)
		{
			currentGround.DebugGround(true, true, true, true, true);
		}

		if(AfterSingleUpdate != null)
		{
			AfterSingleUpdate();
		}
    }

    void ProbeGround(int iter)
    {
        PushIgnoredColliders();
        currentGround.ProbeGround(SpherePosition(feet), iter);
        PopIgnoredColliders();
    }

    /// <summary>
    /// Prevents the player from walking up slopes of a larger angle than the object's SlopeLimit.
    /// </summary>
    /// <returns>True if the controller attemped to ascend a too steep slope and had their movement limited</returns>
    bool SlopeLimit()
    {
        Vector3 n = currentGround.PrimaryNormal();
        float a = Vector3.Angle(n, up);

        if(a > currentGround.superCollisionType.SlopeLimit)
        {
            Vector3 absoluteMoveDirection = Math3d.ProjectVectorOnPlane(n, transform.position - initialPosition);

            // Retrieve a vector pointing down the slope
            Vector3 r = Vector3.Cross(n, down);
            Vector3 v = Vector3.Cross(r, n);

            float angle = Vector3.Angle(absoluteMoveDirection, v);

			if(angle <= 90.0f)
			{
				return false;
			}

            // Calculate where to place the controller on the slope, or at the bottom, based on the desired movement distance
            Vector3 resolvedPosition = Math3d.ProjectPointOnLine(initialPosition, r, transform.position);
            Vector3 direction = Math3d.ProjectVectorOnPlane(n, resolvedPosition - transform.position);

            RaycastHit hit;

            // Check if our path to our resolved position is blocked by any colliders
            if (Physics.CapsuleCast(SpherePosition(feet), SpherePosition(head), radius, direction.normalized, out hit, direction.magnitude, Walkable, triggerInteraction))
            {
                transform.position += v.normalized * hit.distance;
            }
            else
            {
                transform.position += direction;
            }

            return true;
        }

        return false;
    }

    void ClampToGround()
    {
        float d = currentGround.Distance();
        transform.position -= up * d;
    }

    public void EnableClamping()
    {
        clamping = true;
    }

    public void DisableClamping()
    {
        clamping = false;
    }

    public void EnableSlopeLimit()
    {
        slopeLimiting = true;
    }

    public void DisableSlopeLimit()
    {
        slopeLimiting = false;
    }

    public bool IsClamping()
    {
        return clamping;
    }

    /// <summary>
    /// Check if any of the CollisionSpheres are colliding with any walkable objects in the world.
    /// If they are, apply a proper pushback and retrieve the collision data
    /// </summary>
    void RecursivePushback(int depth, int maxDepth)
    {
        PushIgnoredColliders();

        bool contact = false;

        foreach (var sphere in spheres)
        {
            foreach (Collider col in Physics.OverlapSphere((SpherePosition(sphere)), radius, Walkable, triggerInteraction))
            {
                Vector3 position = SpherePosition(sphere);
                Vector3 contactPoint;
                bool contactPointSuccess = SuperCollider.ClosestPointOnSurface(col, position, radius, out contactPoint);
                
                if (!contactPointSuccess)
                {
                    return;
                }

				if(debugPushbackMesssages)
				{
					DebugDraw.DrawMarker(contactPoint, 2.0f, Color.cyan, 0.0f, false);
				}
                    
                Vector3 v = contactPoint - position;
                if(v != Vector3.zero)
                {
                    // Cache the collider's layer so that we can cast against it
                    int layer = col.gameObject.layer;

                    col.gameObject.layer = TemporaryLayerIndex;

                    // Check which side of the normal we are on
                    bool facingNormal = Physics.SphereCast(new Ray(position, v.normalized), TinyTolerance, v.magnitude + TinyTolerance, 1 << TemporaryLayerIndex);

                    col.gameObject.layer = layer;

                    // Orient and scale our vector based on which side of the normal we are situated
                    if(facingNormal)
                    {
                        if(Vector3.Distance(position, contactPoint) < radius)
                        {
                            v = v.normalized * (radius - v.magnitude) * -1;
                        }
                        else
                        {
                            // A previously resolved collision has had a side effect that moved us outside this collider
                            continue;
                        }
                    }
                    else
                    {
                        v = v.normalized * (radius + v.magnitude);
                    }

                    contact = true;

                    transform.position += v;

                    col.gameObject.layer = TemporaryLayerIndex;

                    // Retrieve the surface normal of the collided point
                    RaycastHit normalHit;

                    Physics.SphereCast(new Ray(position + v, contactPoint - (position + v)), TinyTolerance, out normalHit, 1 << TemporaryLayerIndex);

                    col.gameObject.layer = layer;

                    SuperCollisionType superColType = col.gameObject.GetComponent<SuperCollisionType>();

					if(superColType == null)
					{
						superColType = defaultCollisionType;
					}

                    // Our collision affected the collider; add it to the collision data
                    var collision = new SuperCollision()
                    {
                        collisionSphere = sphere,
                        superCollisionType = superColType,
                        gameObject = col.gameObject,
                        point = contactPoint,
                        normal = normalHit.normal
                    };

                    collisionData.Add(collision);
                }
            }            
        }

        PopIgnoredColliders();

        if(depth < maxDepth && contact)
        {
            RecursivePushback(depth + 1, maxDepth);
        }
    }

    protected struct IgnoredCollider
    {
        public Collider collider;
        public int layer;

        public IgnoredCollider(Collider collider, int layer)
        {
            this.collider = collider;
            this.layer = layer;
        }
    }

    private void PushIgnoredColliders()
    {
        ignoredColliderStack.Clear();

        for(int i = 0; i < ignoredColliders.Count; i++)
        {
            Collider col = ignoredColliders[i];
            ignoredColliderStack.Add(new IgnoredCollider(col, col.gameObject.layer));
            col.gameObject.layer = TemporaryLayerIndex;
        }
    }

    private void PopIgnoredColliders()
    {
        for(int i = 0; i < ignoredColliderStack.Count; i++)
        {
            IgnoredCollider ic = ignoredColliderStack[i];
            ic.collider.gameObject.layer = ic.layer;
        }

        ignoredColliderStack.Clear();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if(debugSpheres)
        {
            if(spheres != null)
            {
                if(heightScale == 0) heightScale = 1;

                foreach(var sphere in spheres)
                {
                    Gizmos.color = sphere.isFeet ? Color.green : (sphere.isHead ? Color.yellow : Color.cyan);
                    Gizmos.DrawWireSphere(SpherePosition(sphere), radius);
                }
            }
        }
    }

    public Vector3 SpherePosition(CollisionSphere sphere)
    {
		if(sphere.isFeet)
		{
			return transform.position + sphere.offset * up;
		}
		else
		{
			return transform.position + sphere.offset * up * heightScale;
		}
    }

    public bool PointBelowHead(Vector3 point)
    {
        return Vector3.Angle(point - SpherePosition(head), up) > 89.0f;
    }

    public bool PointAboveFeet(Vector3 point)
    {
        return Vector3.Angle(point - SpherePosition(feet), down) > 89.0f;
    }

    public void IgnoreCollider(Collider col)
    {
        ignoredColliders.Add(col);
    }

    public void RemoveIgnoredCollider(Collider col)
    {
        ignoredColliders.Remove(col);
    }

    public void ClearIgnoredColliders()
    {
        ignoredColliders.Clear();
    }

    public class SuperGround
    {
        public SuperGround(LayerMask walkable, SuperCharacterController controller, QueryTriggerInteraction triggerInteraction)
        {
            this.walkable = walkable;
            this.controller = controller;
            this.triggerInteraction = triggerInteraction;
        }

        private class GroundHit
        {
            public Vector3 point { get; private set; }
            public Vector3 normal { get; private set; }
            public float distance { get; private set; }

            public GroundHit(Vector3 point, Vector3 normal, float distance)
            {
                this.point = point;
                this.normal = normal;
                this.distance = distance;
            }
        }

        private LayerMask walkable;
        private SuperCharacterController controller;
        private QueryTriggerInteraction triggerInteraction;

        private GroundHit primaryGround;
        private GroundHit nearGround;
        private GroundHit farGround;
        private GroundHit stepGround;
        private GroundHit flushGround;

        public SuperCollisionType superCollisionType { get; private set; }
        public Transform transform { get; private set; }

        private const float groundingUpperBoundAngle = 60.0f;
        private const float groundingMaxPercentFromCenter = 0.85f;
        private const float groundingMinPercentFromcenter = 0.50f;

        /// <summary>
        /// Scan the surface below us for ground. Follow up the initial scan with subsequent scans
        /// designed to test what kind of surface we are standing above and handle different edge cases
        /// </summary>
        /// <param name="origin">Center of the sphere for the initial SphereCast</param>
        /// <param name="iter">Debug tool to print out which ProbeGround iteration is being run (3 are run each frame for the controller)</param>
        public void ProbeGround(Vector3 origin, int iter)
        {
            ResetGrounds();

            Vector3 up = controller.up;
            Vector3 down = -up;

            Vector3 o = origin + (up * Tolerance);

            // Reduce our radius by Tolerance squared to avoid failing the SphereCast due to clipping with walls
            float smallerRadius = controller.radius - (Tolerance * Tolerance);

            RaycastHit hit;

            if(Physics.SphereCast(o, smallerRadius, down, out hit, Mathf.Infinity, walkable, triggerInteraction))
            {
                var superColType = hit.collider.gameObject.GetComponent<SuperCollisionType>();

                if (superColType == null)
                {
                    superColType = defaultCollisionType;
                }

                superCollisionType = superColType;
                transform = hit.transform;

                // By reducing the initial SphereCast's radius by Tolerance, our casted sphere no longer fits with
                // our controller's shape. Reconstruct the sphere cast with the proper radius
                SimulateSphereCast(hit.normal, out hit);

                primaryGround = new GroundHit(hit.point, hit.normal, hit.distance);

                // If we are standing on a perfectly flat surface, we cannot be either on an edge,
                // On a slope or stepping off a ledge
                if(Vector3.Distance(Math3d.ProjectPointOnPlane(controller.up, controller.transform.position, hit.point), controller.transform.position) < TinyTolerance)
                {
                    return;
                }

                // As we are standing on an edge, we need to retrieve the normals of the two
                // faces on either side of the edge and store them in nearHit and farHit

                Vector3 toCenter = Math3d.ProjectVectorOnPlane(up, (controller.transform.position - hit.point).normalized * TinyTolerance);

                Vector3 awayFromCenter = Quaternion.AngleAxis(-80.0f, Vector3.Cross(toCenter, up)) * -toCenter;

                Vector3 nearPoint = hit.point + toCenter + (up * TinyTolerance);
                Vector3 farPoint = hit.point + (awayFromCenter * 3);

                RaycastHit nearHit;
                RaycastHit farHit;

                Physics.Raycast(nearPoint, down, out nearHit, Mathf.Infinity, walkable, triggerInteraction);
                Physics.Raycast(farPoint, down, out farHit, Mathf.Infinity, walkable, triggerInteraction);

                nearGround = new GroundHit(nearHit.point, nearHit.normal, nearHit.distance);
                farGround = new GroundHit(farHit.point, farHit.normal, farHit.distance);

                // If we are currently standing on ground that should be counted as a wall,
                // we are likely flush against it on the ground. Retrieve what we are standing on
                if(Vector3.Angle(hit.normal, up) > superColType.StandAngle)
                {
                    // Retrieve a vector pointing down the slope
                    Vector3 r = Vector3.Cross(hit.normal, down);
                    Vector3 v = Vector3.Cross(r, hit.normal);

                    Vector3 flushOrigin = hit.point + hit.normal * TinyTolerance;

                    RaycastHit flushHit;

                    if(Physics.Raycast(flushOrigin, v, out flushHit, Mathf.Infinity, walkable, triggerInteraction))
                    {
                        RaycastHit sphereCastHit;

                        if(SimulateSphereCast(flushHit.normal, out sphereCastHit))
                        {
                            flushGround = new GroundHit(sphereCastHit.point, sphereCastHit.normal, sphereCastHit.distance);
                        }
                        else
                        {
                            // Uh oh
                        }
                    }
                }

                // If we are currently standing on a ledge then the face nearest the center of the
                // controller should be steep enough to be counted as a wall. Retrieve the ground
                // it is connected to at it's base, if there exists any
                if(Vector3.Angle(nearHit.normal, up) > superColType.StandAngle || nearHit.distance > Tolerance)
                {
                    SuperCollisionType col = null;
                
                    if(nearHit.collider != null)
                    {
                        col = nearHit.collider.gameObject.GetComponent<SuperCollisionType>();
                    }
                    
                    if(col == null)
                    {
                        col = defaultCollisionType;
                    }

                    // We contacted the wall of the ledge, rather than the landing. Raycast down
                    // the wall to retrieve the proper landing
                    if(Vector3.Angle(nearHit.normal, up) > col.StandAngle)
                    {
                        // Retrieve a vector pointing down the slope
                        Vector3 r = Vector3.Cross(nearHit.normal, down);
                        Vector3 v = Vector3.Cross(r, nearHit.normal);

                        RaycastHit stepHit;

                        if(Physics.Raycast(nearPoint, v, out stepHit, Mathf.Infinity, walkable, triggerInteraction))
                        {
                            stepGround = new GroundHit(stepHit.point, stepHit.normal, stepHit.distance);
                        }
                    }
                    else
                    {
                        stepGround = new GroundHit(nearHit.point, nearHit.normal, nearHit.distance);
                    }
                }
            }
            // If the initial SphereCast fails, likely due to the controller clipping a wall,
            // fallback to a raycast simulated to SphereCast data
            else if(Physics.Raycast(o, down, out hit, Mathf.Infinity, walkable, triggerInteraction))
            {
                var superColType = hit.collider.gameObject.GetComponent<SuperCollisionType>();

                if(superColType == null)
                {
                    superColType = defaultCollisionType;
                }

                superCollisionType = superColType;
                transform = hit.transform;

                RaycastHit sphereCastHit;

                if(SimulateSphereCast(hit.normal, out sphereCastHit))
                {
                    primaryGround = new GroundHit(sphereCastHit.point, sphereCastHit.normal, sphereCastHit.distance);
                }
                else
                {
                    primaryGround = new GroundHit(hit.point, hit.normal, hit.distance);
                }
            }
            else
            {
                Debug.LogWarning("[SuperCharacterComponent]: WALKABLE LAYER NOT PROPERLY SET.  SEE README FILE.");
            }
        }

        private void ResetGrounds()
        {
            primaryGround = null;
            nearGround = null;
            farGround = null;
            flushGround = null;
            stepGround = null;
        }

        public bool IsGrounded(bool currentlyGrounded, float distance)
        {
            Vector3 n;
            return IsGrounded(currentlyGrounded, distance, out n);
        }

        public bool IsGrounded(bool currentlyGrounded, float distance, out Vector3 groundNormal)
        {
            groundNormal = Vector3.zero;

            if(primaryGround == null || primaryGround.distance > distance)
            {
                return false;
            }

            // Check if we are flush against a wall
            if(farGround != null && Vector3.Angle(farGround.normal, controller.up) > superCollisionType.StandAngle)
            {
                if(flushGround != null && Vector3.Angle(flushGround.normal, controller.up) < superCollisionType.StandAngle && flushGround.distance < distance)
                {
                    groundNormal = flushGround.normal;
                    return true;
                }

                return false;
            }

            // Check if we are at the edge of a ledge, or on a high angle slope
            if(farGround != null && !OnSteadyGround(farGround.normal, primaryGround.point))
            {
                // Check if we are walking onto steadier ground
                if(nearGround != null && nearGround.distance < distance && Vector3.Angle(nearGround.normal, controller.up) < superCollisionType.StandAngle && !OnSteadyGround(nearGround.normal, nearGround.point))
                {
                    groundNormal = nearGround.normal;
                    return true;
                }

                // Check if we are on a step or stair
                if(stepGround != null && stepGround.distance < distance && Vector3.Angle(stepGround.normal, controller.up) < superCollisionType.StandAngle)
                {
                    groundNormal = stepGround.normal;
                    return true;
                }

                return false;
            }


            if(farGround != null)
            {
                groundNormal = farGround.normal;
            }
            else
            {
                groundNormal = primaryGround.normal;
            }

            return true;
        }

        /// <summary>
        /// To help the controller smoothly "fall" off surfaces and not hang on the edge of ledges,
        /// check that the ground below us is "steady", or that the controller is not standing
        /// on too extreme of a ledge
        /// </summary>
        /// <param name="normal">Normal of the surface to test against</param>
        /// <param name="point">Point of contact with the surface</param>
        /// <returns>True if the ground is steady</returns>
        private bool OnSteadyGround(Vector3 normal, Vector3 point)
        {
            float angle = Vector3.Angle(normal, controller.up);

            float angleRatio = angle / groundingUpperBoundAngle;

            float distanceRatio = Mathf.Lerp(groundingMinPercentFromcenter, groundingMaxPercentFromCenter, angleRatio);

            Vector3 p = Math3d.ProjectPointOnPlane(controller.up, controller.transform.position, point);

            float distanceFromCenter = Vector3.Distance(p, controller.transform.position);

            return distanceFromCenter <= distanceRatio * controller.radius;
        }

        public Vector3 PrimaryNormal()
        {
            return primaryGround.normal;
        }

        public float Distance()
        {
            return primaryGround.distance;
        }

        public void DebugGround(bool primary, bool near, bool far, bool flush, bool step)
        {
            if(primary && primaryGround != null)
            {
                DebugDraw.DrawVector(primaryGround.point, primaryGround.normal, 2.0f, 1.0f, Color.yellow, 0, false);
            }

            if(near && nearGround != null)
            {
                DebugDraw.DrawVector(nearGround.point, nearGround.normal, 2.0f, 1.0f, Color.blue, 0, false);
            }

            if(far && farGround != null)
            {
                DebugDraw.DrawVector(farGround.point, farGround.normal, 2.0f, 1.0f, Color.red, 0, false);
            }

            if(flush && flushGround != null)
            {
                DebugDraw.DrawVector(flushGround.point, flushGround.normal, 2.0f, 1.0f, Color.cyan, 0, false);
            }

            if(step && stepGround != null)
            {
                DebugDraw.DrawVector(stepGround.point, stepGround.normal, 2.0f, 1.0f, Color.green, 0, false);
            }
        }

        /// <summary>
        /// Provides raycast data based on where a SphereCast would contact the specified normal
        /// Raycasting downwards from a point along the controller's bottom sphere, based on the provided
        /// normal
        /// </summary>
        /// <param name="groundNormal">Normal of a triangle assumed to be directly below the controller</param>
        /// <param name="hit">Simulated SphereCast data</param>
        /// <returns>True if the raycast is successful</returns>
        private bool SimulateSphereCast(Vector3 groundNormal, out RaycastHit hit)
        {
            float groundAngle = Vector3.Angle(groundNormal, controller.up) * Mathf.Deg2Rad;

            Vector3 secondaryOrigin = controller.transform.position + controller.up * Tolerance;

            if(!Mathf.Approximately(groundAngle, 0))
            {
                float horizontal = Mathf.Sin(groundAngle) * controller.radius;
                float vertical = (1.0f - Mathf.Cos(groundAngle)) * controller.radius;

                // Retrieve a vector pointing up the slope
                Vector3 r2 = Vector3.Cross(groundNormal, controller.down);
                Vector3 v2 = -Vector3.Cross(r2, groundNormal);

                secondaryOrigin += Math3d.ProjectVectorOnPlane(controller.up, v2).normalized * horizontal + controller.up * vertical;
            }
            
            if(Physics.Raycast(secondaryOrigin, controller.down, out hit, Mathf.Infinity, walkable, triggerInteraction))
            {
                // Remove the tolerance from the distance travelled
                hit.distance -= Tolerance + TinyTolerance;

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

[Serializable]
public class CollisionSphere
{
    public float offset;
    public bool isFeet;
    public bool isHead;

    public CollisionSphere(float offset, bool isFeet, bool isHead)
    {
        this.offset = offset;
        this.isFeet = isFeet;
        this.isHead = isHead;
    }
}

public struct SuperCollision
{
    public CollisionSphere collisionSphere;
    public SuperCollisionType superCollisionType;
    public GameObject gameObject;
    public Vector3 point;
    public Vector3 normal;
}