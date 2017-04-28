using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ClipperLib;
using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;

namespace Water2DTool
{
    #region Enumerators
    /// <summary>
    /// Describes how the water handles should be animated.
    /// </summary>
    public enum Water2D_AnimationMethod
    {
        /// <summary>
        /// The water will not be animated.
        /// </summary>
        None,
        /// <summary>
        /// The animated handle follows the movement of a referenced object.
        /// </summary>
        Follow,
        /// <summary>
        /// The animated handle's global position is set to that of a referenced object.
        /// </summary>
        Snap
    }
    /// <summary>
    /// Describes the direction of the water flow.
    /// </summary>
    public enum Water2D_FlowDirection
    {
        /// <summary>
        /// The water flow will push the objects up.
        /// </summary>
        Up,
        /// <summary>
        /// The water flow will push the objects down.
        /// </summary>
        Down,
        /// <summary>
        /// The water flow will push the objects to the left.
        /// </summary>
        Left,
        /// <summary>
        /// The water flow will push the objects to the right.
        /// </summary>
        Right
    }
    /// <summary>
    /// Determines which clipping method will be used to calculate the shape of the polygon that is below the water.
    /// </summary>
    public enum Water2D_ClippingMethod
    {
        /// <summary>
        /// Uses the Sutherland Hodgman polygon clipping algorithm. This is the cheapest option in terms of 
        /// performance because the clipping polygon is always a horizontal line.
        /// </summary>
        Simple,
        /// <summary>
        /// Uses the clipper library. This option is best to use when you want the objects to better react to water waves.
        /// </summary>
        Complex,
    }
    /// <summary>
    /// Describes how the Buoyant Force applied to a floating object, should be simulated.
    /// </summary>
    public enum Water2D_BuoyantForceMode
    {
        /// <summary>
        /// A physics based Buoyant Force will be applied to a floating object. This method takes into
        /// account the mass of the object and it's shape. Use this for a more realistic simulation
        /// of Buoyant Forces. It's a little more expensive then the Linear.
        /// </summary>
        PhysicsBased,
        /// <summary>
        /// A linear Buoyant Force will be applied to a floating object. This method does not take into
        /// account the objects shape.
        /// </summary>
        Linear
    }
    /// <summary>
    /// Describes how surface waves like the one created by the wind should be generated.
    /// </summary>
    public enum Water2D_SurfaceWaves
    {
        /// <summary>
        /// No surface waves will be generated.
        /// </summary>
        None,
        /// <summary>
        /// Generates small random water splashes.
        /// </summary>
        RandomSplashes,
        /// <summary>
        /// Sine waves with random values are used to simulate waves.
        /// </summary>
        SineWaves,
        /// <summary>
        /// When this option is selected the value of the variable "controlPointVelocity" will be 
        /// used to change the velocity of a control point. The value of this variable can be changed
        /// using the animator or by a script.
        /// </summary>
        ControlPoint
    }

    /// <summary>
    /// Determines the number of sine waves.
    /// </summary>
    public enum Water2D_SineWaves
    {
        /// <summary>
        /// A single sine wave will be used to change the velocity of the survace vertices.
        /// </summary>
        SingleSineWave,
        /// <summary>
        /// Multiple sine waves will be used to change the velocity of the survace vertices.
        /// </summary>
        MultipleSineWaves
    }

    /// <summary>
    /// Describes the type of the water.
    /// </summary>
    public enum Water2D_Type
    {
        /// <summary>
        /// The water will react to objects and will influence their position.
        /// </summary>
        Dynamic,
        /// <summary>
        /// The water won't react to objects and won't influence their position, but can be animated.
        /// </summary>
        Decorative
    }

    /// <summary>
    /// Sets the position of the control point.
    /// </summary>
    public enum Water2D_ControlPointPosition
    {
        /// <summary>
        /// The control point will be located on the left edge of the water.
        /// </summary>
        LeftEdge,
        /// <summary>
        /// The control point will be located on the right edge of the water.
        /// </summary>
        RightEdge
    }

    /// <summary>
    /// Determines the number of particle system that are instantiated when an object hits the water surface.
    /// </summary>
    public enum Water2D_ParticleSystem
    {
        /// <summary>
        /// A single particle system will be instantiated for an object.
        /// </summary>
        PerObject,
        /// <summary>
        /// A particle system will be instantiated for every surface vertex that is 
        /// overlapping the iteration region of the object.
        /// </summary>
        PerVertex
    }
    #endregion

    [RequireComponent(typeof(Water2D_Tool)), RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
    public class Water2D_Simulation : MonoBehaviour
    {
        #region Private fields
        /// <summary>
        /// List of segments where an object was detected in the previous frame.
        /// </summary>
        private List<bool> foundObjOnPrevFrame;
        /// <summary>
        /// List of segments where an object was detected in the current frame.
        /// </summary>
        private List<int> foundObjectIn;
        /// <summary>
        /// Surface vertices velocities.
        /// </summary>
        private List<float> velocities;
        /// <summary>
        /// Surface vertices cccelerations.
        /// </summary>
        private List<float> accelerations;
        private List<float> leftDeltas;
        private List<float> rightDeltas;
        /// <summary>
        ///  List of 2D coliders.
        /// </summary>
        private List<Collider2D> floatingObjects2D;
        /// <summary>
        ///  List of 3D colliders.
        /// </summary>
        private List<Collider> floatingObjects3D;
        /// <summary>
        ///  Temporary list of 2D colliders.
        /// </summary>
        private List<Collider2D> tempObj2D;
        /// <summary>
        ///  Temporary list of 3D colliders.
        /// </summary>
        private List<Collider> tempObj3D;
        /// <summary>
        /// List of Y values created from the overlapping of a number of sine waves.
        /// This values are used to change the velocity of the surface vertices and simulate idle waves.
        /// </summary>
        private List<float> sineY;
        /// <summary>
        /// Water mesh vertices.
        /// </summary>
        private Vector3[] vertices;
        /// <summary>
        /// Water mesh UVs.
        /// </summary>
        private Vector2[] UVs;
        /// <summary>
        /// A phase to apply to each sine wave.
        /// </summary>
        private float phase = 0;
        /// <summary>
        /// The number of vertices the water surface (top edge of the water) has.
        /// </summary>
        private int surfaceVertsCount;
        /// <summary>
        /// The local position of the water line on the Y axis.
        /// </summary>
        private float waterLineCurrentLocalPos;
        /// <summary>
        /// The local position of the water line on the Y axis at the start of the current frame.
        /// </summary>
        private float waterLinePreviousLocalPos;
        /// <summary>
        /// The scale of the force that should be applied to an object.
        /// </summary>
        private float forceFactor;
        /// <summary>
        /// The position where the upwards force should be applied to an object submerged in the water.
        /// </summary>
        private Vector3 forcePosition;
        /// <summary>
        /// The upward force that should be applied to an object submerged in the water.
        /// </summary>
        private Vector3 upLift;
        /// <summary>
        /// The default water height. This value is set in the Start method.
        /// </summary>
        private float defaultWaterHeight;
        /// <summary>
        /// The default water area. This value is set in the Start method.
        /// </summary>
        private float defaultWaterArea;
        /// <summary>
        /// The position on the Y axis the TopEdge object had on the previous frame.
        /// </summary>
        private float prevTopEdgeYPos;
        /// <summary>
        /// The position on the Y axis the BottomEdge object had on the previous frame.
        /// </summary>
        private float prevBottomEdgeYPos;
        /// <summary>
        /// The position on the X axis the LeftEdge object had on the previous frame.
        /// </summary>
        private float prevLeftEdgeXPos;
        /// <summary>
        /// The position on the X axis the RightEdge object had on the previous frame.
        /// </summary>
        private float prevRightEdgeXPos;
        /// <summary>
        /// The difference between the surface vertices and the current waterline position.
        /// </summary>
        private float[] vertYOffsets;
        /// <summary>
        /// Used to determine if the water mesh should be recreated from scratch.
        /// </summary>
        private bool recreateWaterMesh = false;
        /// <summary>
        /// Used to determine if the water mesh height should be updated.
        /// </summary>
        private bool updateWaterHeight = false;
        /// <summary>
        /// The amount by which the water height should be increased after all the calculations are done.
        /// </summary>
        private float waterLineYPosOffset = 0;
        /// <summary>
        /// The value that defaultWaterAreaOffset had at the end of the previous frame
        /// (has at the start of the current frame).
        /// </summary>
        private float prevDefaultWaterAreaOffset;
        /// <summary>
        /// Water Mesh.
        /// </summary>
        private Mesh meshFilter;
        /// <summary>
        /// Water2D component.
        /// </summary>
        private Water2D_Tool water2D;
        /// <summary>
        /// Used to generate a random splash.
        /// </summary>
        private bool makeSplash = false;
        /// <summary>
        /// The variable is set to true when a collider with the tag "Player" 
        /// is found by the OnTriggerEnter2D method.
        /// </summary>
        private bool onTriggerPlayerDetected = false;
        /// <summary>
        /// The area of the polygon segment that is below the waterline.
        /// </summary>
        private float area = 0;
        /// <summary>
        /// The mass of the water that has the area equal to the area of the polygon segment that is below the waterline.
        /// </summary>
        private float displacedMass = 0;
        /// <summary>
        /// When animating an edge of the water, Water2D compares the difference between the current position
        /// of a referenced object and the one it had on the previous frame to the precisionFactor. 
        /// If the difference is bigger than the precisionFactor, than the object moved on the current frame
        /// from the position it had on the previous frame. Comparing the position the object has on the current
        /// frame to the one that it had on the previous frame directly is not a good idea as Unity may round the 
        /// numbers differently even if the reference object didn't change its position. Do not change the value of 
        /// this field if you are not sure what it does.
        /// </summary>
        private float precisionFactor = 0.001f;
        /// <summary>
        /// The global position of the left handle.
        /// </summary>
        private Vector3 leftHandleGlobalPos;
        /// <summary>
        /// A list of points that form the water line.
        /// </summary>
        private List<Vector2> waterLinePoints;
        /// <summary>
        /// It is used to convert float numbers to int and back.
        /// </summary>
        private float scaleFactor = 100000f;
        #endregion

        #region Public fields
        public Water2D_BuoyantForceMode buoyantForceMode = Water2D_BuoyantForceMode.PhysicsBased;
        public Water2D_SurfaceWaves surfaceWaves = Water2D_SurfaceWaves.None;
        public Water2D_Type waterType = Water2D_Type.Dynamic;
        public Water2D_AnimationMethod animationMethod = Water2D_AnimationMethod.None;
        public Water2D_ClippingMethod clippingMethod = Water2D_ClippingMethod.Simple;
        public Water2D_FlowDirection flowDirection = Water2D_FlowDirection.Right;
        public Water2D_ControlPointPosition controlPointPos = Water2D_ControlPointPosition.LeftEdge;
        public Water2D_SineWaves sineWavesType = Water2D_SineWaves.SingleSineWave;
        public Water2D_ParticleSystem particleSystemInstantiation = Water2D_ParticleSystem.PerObject;
        /// <summary>
        /// List of sine wave amplitudes.
        /// </summary>
        public List<float> sineAmplitudes = new List<float>();
        /// <summary>
        /// The amount by which a particular sine wave is stretched.
        /// </summary>
        public List<float> sineStretches = new List<float>();
        /// <summary>
        /// Sine wave phase offset.
        /// </summary>
        public List<float> phaseOffset = new List<float>();
        /// <summary>
        /// This is the variable that should be changed if animateWaterArea is set to true.
        /// </summary>
        public float defaultWaterAreaOffset;
        /// <summary>
        /// The number of Sine waves.
        /// </summary>
        public int sineWaves = 4;
        /// <summary>
        /// The spring constant.
        /// </summary>
        public float springConstant = 0.02f;
        /// <summary>
        /// The damping applied to the surface vertices velocities.
        /// </summary>
        public float damping = 0.04f;
        /// <summary>
        /// It controls how fast the waves spread.
        /// </summary>
        public float spread = 0.03f;
        /// <summary>
        /// Determines how much force should be applied to an object submerged in the water. A value of 3 means 
        /// that 3m under the water, the force applied to an object will be 2 times greater than the force applied 
        /// at the surface of the water.
        /// </summary>
        public float floatHeight = 3;
        /// <summary>
        /// A bounce damping for the object bounce.
        /// </summary>
        public float bounceDamping = 0.15f;
        /// <summary>
        /// Offsets the position where the upwards force should be applied to an object submerged in the water.
        /// </summary>
        public Vector3 forcePositionOffset;
        /// <summary>
        /// Limits the velocity on the Y axis that is applied to a waterline vertex.
        /// </summary>
        public float collisionVelocity = 0.0125f;
        /// <summary>
        /// Used to animate the position of the water line on the Y axis. Assign an animated
        /// object to this field if you want to increase or decrease the water level.
        /// </summary>
        public Transform topEdge;
        /// <summary>
        /// Used to animate the position of the water bottom on the Y axis. Will also affect the waterline position.
        /// </summary>
        public Transform bottomEdge;
        /// <summary>
        /// Used to animate the position of the left edge of the water.
        /// </summary>
        public Transform leftEdge;
        /// <summary>
        /// Used to animate the position of the right edge of the water.
        /// </summary>
        public Transform rightEdge;
        /// <summary>
        /// A particle system prefab for water splash effect.
        /// </summary>
        public GameObject particleS;
        /// <summary>
        /// When set to true will force the default water area to be constant. So if the water width
        /// decreases (increases) the water height will increase (decrease).
        /// </summary>
        public bool constantWaterArea = false;
        /// <summary>
        /// When set to true the objects that are submerged in the water will make the water rise.
        /// </summary>
        public bool waterDisplacement = false;
        /// <summary>
        /// Set this to true if you want to animate the default water area.
        /// </summary>
        public bool animateWaterArea = false;
        /// <summary>
        /// Controls the water wave propagation speed.
        /// </summary>
        public float waveSpeed = 8;
        /// <summary>
        /// Objects with downwards velocity greater than the value of velocityFilter won't create waves.
        /// </summary>
        public float velocityFilter = -2f;
        /// <summary>
        /// The density of the water.
        /// </summary>
        public float waterDensity = 1f;
        /// <summary>
        /// The number of vertices a regular polygon should have. 
        /// </summary>
        public int polygonCorners = 8;
        /// <summary>
        /// The maximum drag that should be applied to a colliders edge.
        /// </summary>
        public float maxDrag = 500f;
        /// <summary>
        /// The maximum lift that should be applied to a colliders edge.
        /// </summary>
        public float maxLift = 200f;
        /// <summary>
        /// Drag Coefficient.
        /// </summary>
        public float dragCoefficient = 0.4f;
        /// <summary>
        /// Lift Coefficient.
        /// </summary>
        public float liftCoefficient = 0.25f;
        /// <summary>
        /// How often should a random splash be generated.
        /// </summary>
        public float timeStep = 0.5f;
        /// <summary>
        /// The max value of a random amplitude.
        /// </summary>
        public float maxAmplitude = 0.1f;
        /// <summary>
        /// The min value of a random amplitude.
        /// </summary>
        public float minAmplitude = 0.01f;
        /// <summary>
        /// The max value of a random stretch.
        /// </summary>
        public float maxStretch = 2f;
        /// <summary>
        /// The min value of a random stretch.
        /// </summary>
        public float minStretch = 0.8f;
        /// <summary>
        /// The max value of a random phase offset.
        /// </summary>
        public float maxPhaseOffset = 0.1f;
        /// <summary>
        /// The min value of a random phase offset.
        /// </summary>
        public float minPhaseOffset = 0.02f;
        /// <summary>
        /// The max value of a random velocity.
        /// </summary>
        public float maxVelocity = 0.1f;
        /// <summary>
        /// The min value of a random velocity.
        /// </summary>
        public float minVelocity = -0.1f;
        /// <summary>
        /// The force scale used when Buoyant Force Mode is set to Linear. A value of 1f will make the object with 
        /// the mass of 1kg float at the surface of the water.
        /// </summary>
        public float forceScale = 4f;
        /// <summary>
        /// The drag coefficient used when Buoyant Force Mode is set to Linear.
        /// </summary>
        public float liniarBFDragCoefficient = 0.1f;
        /// <summary>
        /// The angular drag coefficient used when Buoyant Force Mode is set to Linear.
        /// </summary>
        public float liniarBFAbgularDragCoefficient = 0.01f;
        /// <summary>
        /// The bottom region of a colliders bounding box that can affect the velocity of a vertex.
        /// We use this value to limit the ability of the objects with big bounding boxes to affect 
        /// the velocity of the surface vertices. If we don't do this, a long object that falls down 
        /// slowly will push the water down for a longer period of time and produce a not very realistic simulation.
        /// </summary>
        public float interactionRegion = 1f;
        /// <summary>
        /// The global position of the water line on the Y axis.
        /// </summary>
        public Vector3 waterLineCurrentGlobalPos;
        /// <summary>
        /// Splash sound effect.
        /// </summary>
        public AudioClip splashSound;
        /// <summary>
        /// The size for the players bounding box.
        /// </summary>
        public Vector2 playerBoundingBoxSize = new Vector2(1f, 1f);
        /// <summary>
        /// By default the center of the bounding box will be the transform.position of the 
        /// object. Use this variable to offset the players bounding box center.
        /// </summary>
        public Vector2 playerBoundingBoxCenter = Vector2.zero;
        /// <summary>
        /// Use this variable to set the scale for the buoyant force that is applied to
        /// the object with the "Player" tag.
        /// </summary>
        public float playerBuoyantForceScale = 0.5f;
        /// <summary>
        /// When enabled will show in the Scene View the shape of the polygon that is below the waterline.
        /// </summary>
        public bool showPolygon = false;
        /// <summary>
        /// When enabled will show in the Scene View the velocity direction, drag direction, 
        /// lift direction and the normal of a leading edge.
        /// </summary>
        public bool showForces = false;
        /// <summary>
        /// Will scale down (up) the velocity that is applied to the neighbor vertices when RandomWave method is called.
        /// </summary>
        public float neighborVertVelocityScale = 0.5f;
        /// <summary>
        /// Will scale down (up) the velocity that is applied to a vertex from a sine wave.
        /// </summary>
        public float sineWaveVelocityScale = 0.05f;
        /// <summary>
        /// The radius of a sphere that will be used to check if there is a collider near a surface vertex.
        /// </summary>
        public float overlapSphereRadius = 0.05f;
        /// <summary>
        /// Should the spring simulation be enabled?.
        /// </summary>
        public bool springSimulation = true;
        /// <summary>
        /// The offset on the Y axis from the position of a referenced object.
        /// </summary>
        public float topEdgeYOffset = 0f;
        /// <summary>
        /// The offset on the Y axis from the position of a referenced object.
        /// </summary>
        public float bottomEdgeYOffset = 0f;
        /// <summary>
        /// The offset on the X axis from the position of a referenced object.
        /// </summary>
        public float leftEdgeXOffset = 0f;
        /// <summary>
        /// The offset on the X axis from the position of a referenced object.
        /// </summary>
        public float rightEdgeXOffset = 0f;
        /// <summary>
        /// Offsets the position where the particle systems are created on the Z axis.
        /// </summary>
        public Vector3 particleSystemPosOffset = Vector3.zero;
        /// <summary>
        /// The sorting layer name for the particle system.
        /// </summary>
        public string particleSystemSortingLayerName = "Default";
        /// <summary>
        /// The order in layer for the particle system.
        /// </summary>
        public int particleSystemOrderInLayer = 0;
        /// <summary>
        /// The number of vertical mesh segments that should fit in a water line segment.
        /// </summary>
        public int meshSegmentsPerWaterLineSegment = 4;
        public bool showClippingPlolygon = false;
        /// <summary>
        /// Should the water flow be enabled?.
        /// </summary>
        public bool waterFlow = false;
        /// <summary>
        /// The angle of the water flow. This value controls the direction of the water flow. 
        /// </summary>
        public float flowAngle = 0f;
        /// <summary>
        /// The force of the water flow. 
        /// </summary>
        public float waterFlowForce = 5f;
        /// <summary>
        /// Should the direction be controlled using an angle value specified by the developer?.
        /// </summary>
        public bool useAngles = false;
        /// <summary>
        /// The amplitude of a sine wave. This variable controls the height of the sine wave
        /// </summary>
        public float waveAmplitude = 0.1f;
        /// <summary>
        /// The sine wave stretch. The bigger the value of the stretch the more compact the waves are.
        /// </summary>
        public float waveStretch = 1f;
        /// <summary>
        /// Sine wave phase offset. The bigger the value of the phase offset, the faster the waves move to the left (right). 
        /// </summary>
        public float wavePhaseOffset = 0.2f;
        /// <summary>
        /// The velocity that will be added to the velocity of the control point vertex. 
        /// </summary>
        public float controlPointVelocity = 0;
        /// <summary>
        /// When enabled the sine waves will affect only the velocity of the control point vertex. 
        /// </summary>
        public bool useControlPoints = false;
        /// <summary>
        /// When enabled the sine waves amplitude, stretch and phase offset will be generated randomaly at the start of the game. 
        /// </summary>
        public bool randomValues = true;
        /// <summary>
        /// Should the particle system sorting layer and order in layer be set when it is instantiated?
        /// </summary>
        public bool particleSystemSorting = false;
        #endregion

        #region Class methods
        void Start()
        {
            meshFilter = GetComponent<MeshFilter>().sharedMesh;
            water2D = GetComponent<Water2D_Tool>();

            vertices = meshFilter.vertices;
            surfaceVertsCount = water2D.surfaceVertsCount / 2;
            UVs = meshFilter.uv;

            waterLineCurrentGlobalPos = transform.TransformPoint(vertices[surfaceVertsCount + 1]);
            waterLineCurrentLocalPos = vertices[surfaceVertsCount + 1].y;
            waterLinePreviousLocalPos = vertices[surfaceVertsCount + 1].y;
            defaultWaterHeight = Mathf.Abs(water2D.handlesPosition[0].y - water2D.handlesPosition[1].y);
            defaultWaterArea = Mathf.Abs(water2D.handlesPosition[0].y - water2D.handlesPosition[1].y) * Mathf.Abs(water2D.handlesPosition[2].x - water2D.handlesPosition[3].x);

            sineY = new List<float>();
            floatingObjects3D = new List<Collider>();
            floatingObjects2D = new List<Collider2D>();
            waterLinePoints = new List<Vector2>();

            foundObjOnPrevFrame = new List<bool>();
            foundObjectIn = new List<int>();
            tempObj2D = new List<Collider2D>();
            tempObj3D = new List<Collider>();

            velocities = new List<float>();
            accelerations = new List<float>();
            leftDeltas = new List<float>();
            rightDeltas = new List<float>();

            for (int i = 0; i < surfaceVertsCount; i++)
            {
                velocities.Add(0.0f);
                accelerations.Add(0.0f);
                leftDeltas.Add(0.0f);
                rightDeltas.Add(0.0f);
                sineY.Add(0.0f);

                if (particleSystemInstantiation == Water2D_ParticleSystem.PerVertex && i < surfaceVertsCount - 1)
                    foundObjOnPrevFrame.Add(false);
            }

            if (topEdge != null)
                prevTopEdgeYPos = topEdge.transform.position.y;

            if (bottomEdge != null)
                prevBottomEdgeYPos = bottomEdge.transform.position.y;

            if (leftEdge != null)
                prevLeftEdgeXPos = leftEdge.transform.position.x;

            if (rightEdge != null)
                prevRightEdgeXPos = rightEdge.transform.position.x;

            if (sineWavesType == Water2D_SineWaves.MultipleSineWaves && randomValues)
                GenerateSineVariables();

            if (surfaceVertsCount > meshSegmentsPerWaterLineSegment)
            {
                int n = 0;
                while (n < surfaceVertsCount)
                {
                    waterLinePoints.Add(transform.TransformPoint(vertices[surfaceVertsCount + n]));
                    n += meshSegmentsPerWaterLineSegment;
                }

                if (meshSegmentsPerWaterLineSegment != 1)
                {
                    Vector2 lastVert = transform.TransformPoint(vertices[surfaceVertsCount + surfaceVertsCount - 1]);
                    waterLinePoints.Add(new Vector2(lastVert.x, lastVert.y));
                }
            }
            else
            {
                waterLinePoints.Add(transform.TransformPoint(vertices[surfaceVertsCount]));
                Vector2 lastVert = transform.TransformPoint(vertices[surfaceVertsCount + surfaceVertsCount - 1]);
                waterLinePoints.Add(new Vector2(lastVert.x, lastVert.y));
            }
        }

        /// <summary>
        /// Generate a list of random values for the sine amplitudes, stretches and phase offsets.
        /// </summary>
        void GenerateSineVariables()
        {
            sineAmplitudes.Clear();
            sineStretches.Clear();
            phaseOffset.Clear();

            for (int i = 0; i < sineWaves; i++)
            {
                // Controls the height of the sine wave.
                sineAmplitudes.Add(Random.Range(minAmplitude, maxAmplitude));

                // The bigger the value the more compact the waves are.
                sineStretches.Add(Random.Range(minStretch, maxStretch));

                // The bigger the value the faster the waves move to the left (right).
                phaseOffset.Add(Random.Range(minPhaseOffset, maxPhaseOffset));
            }
        }

        void FixedUpdate()
        {
            if (waterDisplacement)
            {
                WaterDisplacement();
            }

            if (surfaceWaves != Water2D_SurfaceWaves.None)
            {
                if (surfaceWaves == Water2D_SurfaceWaves.SineWaves)
                {
                    SineWaves();
                }

                if (surfaceWaves == Water2D_SurfaceWaves.RandomSplashes && !makeSplash)
                {
                    StartCoroutine(RandomWave());
                }

                if (surfaceWaves == Water2D_SurfaceWaves.ControlPoint)
                {
                    if (controlPointPos == Water2D_ControlPointPosition.LeftEdge)
                        velocities[0] += controlPointVelocity;
                    else
                        velocities[surfaceVertsCount - 1] += controlPointVelocity;
                }
            }

            if (animationMethod != Water2D_AnimationMethod.None)
                WaterAnimation();

            if (springSimulation)
                WaterWaves();

            WaterMesh();

            if (!springSimulation && surfaceWaves != Water2D_SurfaceWaves.None)
                UpdateWaterLinePoints();

            Buoyancy();

            meshFilter.vertices = vertices;
            meshFilter.uv = UVs;

            meshFilter.RecalculateBounds();

            waterLineYPosOffset = 0;
        }

        /// <summary>
        /// Applies buoyancy to the objects in the water based on the buoyancy settings.
        /// </summary>
        private void Buoyancy()
        {
            if (buoyantForceMode == Water2D_BuoyantForceMode.PhysicsBased && waterType == Water2D_Type.Dynamic)
            {
                PhysicsBasedBuoyantForce();
            }
            if (buoyantForceMode == Water2D_BuoyantForceMode.Linear && waterType == Water2D_Type.Dynamic)
            {
                LinearBuoyantForce();
            }
        }

        /// <summary>
        /// Calculates the height of the displaced water when an object is submerged in the water.
        /// </summary>
        private void WaterDisplacement()
        {
            // Resets the global and local positions of the waterline to their default values.
            waterLineCurrentLocalPos = water2D.handlesPosition[1].y + defaultWaterHeight;
            waterLineCurrentGlobalPos = transform.TransformPoint(new Vector3(0f, waterLineCurrentLocalPos, 0f));

            if (!water2D.use3DCollider)
            {
                int len = floatingObjects2D.Count;
                for (int i = 0; i < len; i++)
                {
                    ApplyWaterDisplacement(i, true);
                }
            }
            else
            {
                int len = floatingObjects3D.Count;
                for (int i = 0; i < len; i++)
                {
                    ApplyWaterDisplacement(i, false);
                }
            }
        }

        /// <summary>
        /// Calculates the height of the displaced water when an object is submerged in the water.
        /// </summary>
        /// <param name="cIndex">The index of a collider in the floatingObjects2D or floatingObjects3D list.</param>
        /// <param name="a2DCollider">Is this a 2D collider?.</param>
        /// <remarks>
        /// It's not 100% accurate. 
        /// </remarks>
        private void ApplyWaterDisplacement(int cIndex, bool a2DCollider)
        {
            // Calculates the height of the displaced water by objects.
            if ((a2DCollider ? floatingObjects2D[cIndex].GetComponent<Collider2D>().bounds.min.y : floatingObjects3D[cIndex].GetComponent<Collider>().bounds.min.y) < waterLineCurrentGlobalPos.y)
            {
                BoxCollider2D boxColl2D = null;
                PolygonCollider2D polyColl2D = null;
                CircleCollider2D circleColl2D = null;

                BoxCollider boxColl = null;
                SphereCollider sphereColl = null;
                CapsuleCollider capsuleColl = null;

                bool isIntersecting = true;

                if (a2DCollider)
                {
                    // Gets the BoxCollider2D component of the current object if it has one.
                    boxColl2D = floatingObjects2D[cIndex].GetComponent<BoxCollider2D>();
                    // Gets the PolygonCollider2D component of the current object if it has one.
                    polyColl2D = floatingObjects2D[cIndex].GetComponent<PolygonCollider2D>();
                    // Gets the CircleCollider2D component of the current object if it has one.
                    circleColl2D = floatingObjects2D[cIndex].GetComponent<CircleCollider2D>();
                }
                else
                {
                    // Gets the BoxCollider component of the current object if it has one.
                    boxColl = floatingObjects3D[cIndex].GetComponent<BoxCollider>();
                    // Gets the SphereCollider component of the current object if it has one.
                    sphereColl = floatingObjects3D[cIndex].GetComponent<SphereCollider>();
                    // Gets the CapsuleCollider component of the current object if it has one.
                    capsuleColl = floatingObjects3D[cIndex].GetComponent<CapsuleCollider>();
                }


                // An array that contains 2 points that form a line.
                Vector2[] lineP = new Vector2[2];
                // The segment of the polygon that is below the waterline.
                Vector2[] submergedPolygon;
                // The area of the polygon segment that is below the waterline.
                float submergedArea = 0;

                // A line from the top left corner of the water to the top right corner.
                lineP[0] = new Vector2(water2D.handlesPosition[2].x, waterLineCurrentGlobalPos.y);
                lineP[1] = new Vector2(water2D.handlesPosition[3].x, waterLineCurrentGlobalPos.y);

                if (a2DCollider)
                {
                    // If the object has a BoxCollider2D, calculates the area of the polygon segment that is submerged in the water.
                    if (boxColl2D != null)
                    {
                        Vector2[] boXCorners = new Vector2[4];
                        boXCorners = GetBoxVerticesGlobalPosition(cIndex, true);
                        submergedPolygon = Water2D_PolygonClipping.GetIntersectedPolygon(boXCorners, lineP, out isIntersecting);
                        submergedArea = GetPolygonArea(submergedPolygon);
                    }

                    // If the object has a circle collider, calculates the area of the polygon segment that is submerged in the water.
                    if (circleColl2D != null)
                    {
                        Vector2[] polyCorners = new Vector2[polygonCorners];
                        polyCorners = GetPolygonVerticesGlobalPosition(cIndex, polygonCorners, true);
                        submergedPolygon = Water2D_PolygonClipping.GetIntersectedPolygon(polyCorners, lineP, out isIntersecting);
                        submergedArea = GetPolygonArea(submergedPolygon);
                    }

                    // If the object has a polygon collider, calculates the area of the polygon segment that is submerged in the water.
                    if (polyColl2D != null)
                    {
                        Vector2[] polyPoints = polyColl2D.points;

                        // Gets the global position of the polygon vertices.
                        for (int n = 0; n < polyPoints.Length; n++)
                        {
                            polyPoints[n] = floatingObjects2D[cIndex].transform.TransformPoint(polyPoints[n]);
                        }

                        // Reverses the polygon vertices order if they are arranged clockwise.
                        if (Water2D_PolygonClipping.IsClockwise(polyPoints))
                        {
                            System.Array.Reverse(polyPoints);
                        }

                        submergedPolygon = Water2D_PolygonClipping.GetIntersectedPolygon(polyPoints, lineP, out isIntersecting);
                        submergedArea = GetPolygonArea(submergedPolygon);
                    }
                }
                else
                {
                    if (boxColl != null)
                    {
                        Vector2[] boXCorners = new Vector2[4];
                        boXCorners = GetBoxVerticesGlobalPosition(cIndex, false);
                        submergedPolygon = Water2D_PolygonClipping.GetIntersectedPolygon(boXCorners, lineP, out isIntersecting);
                        submergedArea = GetPolygonArea(submergedPolygon);
                    }

                    if (sphereColl != null)
                    {
                        Vector2[] polyCorners = new Vector2[polygonCorners];
                        polyCorners = GetPolygonVerticesGlobalPosition(cIndex, polygonCorners, false);
                        submergedPolygon = Water2D_PolygonClipping.GetIntersectedPolygon(polyCorners, lineP, out isIntersecting);
                        submergedArea = GetPolygonArea(submergedPolygon);
                    }

                    if (capsuleColl != null)
                    {
                        Vector2[] polyCorners = new Vector2[polygonCorners];
                        polyCorners = GetCapsuleVerticesGlobalPosition(capsuleColl, polygonCorners);
                        submergedPolygon = Water2D_PolygonClipping.GetIntersectedPolygon(polyCorners, lineP, out isIntersecting);
                        submergedArea = GetPolygonArea(submergedPolygon);
                    }
                }

                // The height of the displaced water for the current object.
                float waterHeightOffset = submergedArea / (Mathf.Abs(water2D.handlesPosition[2].x - water2D.handlesPosition[3].x));

                // These values are updated so the next iteration can know where the current waterline position is.
                waterLineYPosOffset += waterHeightOffset;
                waterLineCurrentLocalPos += waterHeightOffset;
                waterLineCurrentGlobalPos.y += waterHeightOffset;
            }
        }

        /// <summary>
        /// Creates small waves in random places.
        /// </summary>
        IEnumerator RandomWave()
        {
            makeSplash = true;

            // The index of the vertex whose velocity will be changed.
            int randomVert = Random.Range(0, surfaceVertsCount - 1);
            // A random velocity value.
            float randomVelocity = Random.Range(minVelocity, maxVelocity);

            velocities[randomVert] += randomVelocity;

            // Changing the velocity of a single vertex won't produce a very realistic splash, 
            // so we change the velocity of its neighbours too.
            if (randomVert > 0)
            {
                velocities[randomVert - 1] += randomVelocity * neighborVertVelocityScale;
            }
            if (randomVert < surfaceVertsCount - 1)
            {
                velocities[randomVert + 1] += randomVelocity * neighborVertVelocityScale;
            }

            yield return new WaitForSeconds(timeStep);
            makeSplash = false;
        }

        /// <summary>
        /// Updates the Y values of the sine waves for each surface vertex and changes the velocities.
        /// </summary>
        private void SineWaves()
        {
            phase += 1f;
            int len0 = 0;
            int len = 0; ;

            if (springSimulation)
            {
                len = surfaceVertsCount;

                if (useControlPoints)
                {
                    if (controlPointPos == Water2D_ControlPointPosition.LeftEdge)
                    {
                        len0 = 0;
                        len = 1;
                    }
                    else
                    {
                        len0 = surfaceVertsCount - 1;
                        len = surfaceVertsCount;
                    }
                }

                if (sineWavesType == Water2D_SineWaves.SingleSineWave)
                {
                    for (int i = len0; i < len; i++)
                    {
                        velocities[i] += waveAmplitude * Mathf.Sin(vertices[i].x * waveStretch + phase * wavePhaseOffset) * sineWaveVelocityScale;
                    }
                }
                else
                {
                    for (int i = len0; i < len; i++)
                    {
                        sineY[i] = OverlapSineWaves(vertices[i].x);
                        velocities[i] += sineY[i] * sineWaveVelocityScale;
                    }
                }
            }
            else
            {
                len = surfaceVertsCount;

                if (sineWavesType == Water2D_SineWaves.SingleSineWave)
                {
                    for (int i = 0; i < len; i++)
                    {
                        float y = waveAmplitude * Mathf.Sin(vertices[i].x * waveStretch + phase * wavePhaseOffset);
                        vertices[i + len].y = waterLineCurrentLocalPos + y;
                    }
                }
                else
                {
                    for (int i = 0; i < len; i++)
                    {
                        sineY[i] = OverlapSineWaves(vertices[i].x);
                        vertices[i + len].y = waterLineCurrentLocalPos + sineY[i];
                    }
                }
            }
        }

        /// <summary>
        /// Overlaps multiple sine waves to achieve a more realistic water wave simulation.
        /// </summary>
        /// <param name="x">The position of a vertex on the X axis.</param>
        private float OverlapSineWaves(float x)
        {
            float y = 0;

            for (int i = 0; i < sineWaves; i++)
            {
                y = y + sineAmplitudes[i] * Mathf.Sin(x * sineStretches[i] + phase * phaseOffset[i]);
            }

            return y;
        }

        /// <summary>
        /// Animates the top, bottom, left and right edges of the water.
        /// </summary>
        private void WaterAnimation()
        {
            recreateWaterMesh = false;
            updateWaterHeight = false;

            // Updates the top, left and right handles position if waterDisplacement is set to true
            // and waterLineYPosOffset is different from zero (it means there is an object in the water). 
            if (waterDisplacement && waterLineYPosOffset != 0)
            {
                // When the water is displaced by an object we don't need to recreate the mesh from scratch,
                // but only update the surface vertices position and the bottom vertices UVs.
                updateWaterHeight = true;
                UpdateTopLeftRightHandles();
            }

            // The default water area is updated based on the difference between the defaultWaterAreaOffset and
            // prevDefaultWaterAreaOffset.
            if (animateWaterArea && Mathf.Abs(defaultWaterAreaOffset - prevDefaultWaterAreaOffset) > precisionFactor)
            {
                defaultWaterArea += defaultWaterAreaOffset - prevDefaultWaterAreaOffset;
                prevDefaultWaterAreaOffset = defaultWaterAreaOffset;

                // Current water width.
                float waterWidth = Mathf.Abs(water2D.handlesPosition[2].x - water2D.handlesPosition[3].x);
                // The default water height is updated based on the new water area.
                defaultWaterHeight = defaultWaterArea / waterWidth;

                UpdateTopLeftRightHandles();
                recreateWaterMesh = true;
            }

            // Updates the position of the top and bottom handles as well as right and left if the referenced objects are not null and
            // their current position changed from the previous frame.

            // The position of the handles is updated by adding to them the difference between the current position
            // of the referenced objects on the Y axis (top, bottom handles), X axis (left, right handles) and the one 
            // the referenced objects had on the previous frame. The new handles position is calculated in
            // this way because the position of the referenced objects is global, but the handles values must be local.
            // By doing it this way we don't need to convert from global position to local.
            if (topEdge != null && Mathf.Abs(topEdge.position.y - prevTopEdgeYPos) > precisionFactor)
            {
                if (animationMethod == Water2D_AnimationMethod.Follow)
                {
                    // The default water height is updated based on the difference between the position the 
                    // reference object had on the previous frame and the position on the current frame.
                    defaultWaterHeight += topEdge.position.y - prevTopEdgeYPos;
                }
                else
                {
                    defaultWaterHeight = Mathf.Abs(transform.InverseTransformPoint(topEdge.position).y - water2D.handlesPosition[1].y + topEdgeYOffset);
                }
                // Because the default water height has changed, the default water area must be updated too.
                defaultWaterArea = defaultWaterHeight * Mathf.Abs(water2D.handlesPosition[2].x - water2D.handlesPosition[3].x);
                prevTopEdgeYPos = topEdge.transform.position.y;

                // The position on the Y axis of the top, left, right, handles is updated based on the new 
                // defaultWaterHeight and waterLineYPosOffset values.
                UpdateTopLeftRightHandles();
                updateWaterHeight = true;
            }

            if (bottomEdge != null && Mathf.Abs(bottomEdge.position.y - prevBottomEdgeYPos) > precisionFactor)
            {
                if (animationMethod == Water2D_AnimationMethod.Follow)
                    water2D.handlesPosition[1] = new Vector2(water2D.handlesPosition[1].x, water2D.handlesPosition[1].y + bottomEdge.position.y - prevBottomEdgeYPos);
                else
                    water2D.handlesPosition[1] = new Vector2(water2D.handlesPosition[1].x, transform.InverseTransformPoint(bottomEdge.position).y + bottomEdgeYOffset);
                UpdateTopLeftRightHandles();
                prevBottomEdgeYPos = bottomEdge.position.y;
                updateWaterHeight = true;
            }

            // Updates the position of the left handler on the X axis. 
            if (leftEdge != null && Mathf.Abs(leftEdge.position.x - prevLeftEdgeXPos) > precisionFactor)
            {
                if (animationMethod == Water2D_AnimationMethod.Follow)
                    water2D.handlesPosition[2] = new Vector2(water2D.handlesPosition[2].x + leftEdge.position.x - prevLeftEdgeXPos, water2D.handlesPosition[2].y);
                else
                    water2D.handlesPosition[2] = new Vector2(transform.InverseTransformPoint(leftEdge.position).x + leftEdgeXOffset, water2D.handlesPosition[2].y);

                prevLeftEdgeXPos = leftEdge.transform.position.x;

                float waterWidth = Mathf.Abs(water2D.handlesPosition[2].x - water2D.handlesPosition[3].x);

                // If constantWaterArea is set to true, the default water height must be updated too.
                if (constantWaterArea)
                {
                    defaultWaterHeight = defaultWaterArea / waterWidth;
                    water2D.handlesPosition[0] = new Vector2(water2D.handlesPosition[0].x, water2D.handlesPosition[1].y + defaultWaterHeight + waterLineYPosOffset);
                }

                // The positions on the X axis of the top and bottom handles are updated.
                water2D.handlesPosition[0] = new Vector2(water2D.handlesPosition[2].x + waterWidth / 2, water2D.handlesPosition[0].y);
                water2D.handlesPosition[1] = new Vector2(water2D.handlesPosition[2].x + waterWidth / 2, water2D.handlesPosition[1].y);

                // The mesh must be recreated from scratch.
                recreateWaterMesh = true;
            }

            // Updates the position of the right handler on the X axis.
            if (rightEdge != null && Mathf.Abs(rightEdge.position.x - prevRightEdgeXPos) > precisionFactor)
            {
                if (animationMethod == Water2D_AnimationMethod.Follow)
                    water2D.handlesPosition[3] = new Vector2(water2D.handlesPosition[3].x + rightEdge.position.x - prevRightEdgeXPos, water2D.handlesPosition[3].y);
                else
                    water2D.handlesPosition[3] = new Vector2(transform.InverseTransformPoint(rightEdge.position).x + rightEdgeXOffset, water2D.handlesPosition[3].y);

                prevRightEdgeXPos = rightEdge.position.x;

                float waterWidth = Mathf.Abs(water2D.handlesPosition[2].x - water2D.handlesPosition[3].x);

                // If constantWaterArea is set to true, the default water height must be updated too.
                if (constantWaterArea)
                {
                    defaultWaterHeight = defaultWaterArea / waterWidth;
                    water2D.handlesPosition[0] = new Vector2(water2D.handlesPosition[0].x, water2D.handlesPosition[1].y + defaultWaterHeight + waterLineYPosOffset);
                }

                // The positions on the X axis of the top and bottom handles are updated.
                water2D.handlesPosition[0] = new Vector2(water2D.handlesPosition[2].x + waterWidth / 2, water2D.handlesPosition[0].y);
                water2D.handlesPosition[1] = new Vector2(water2D.handlesPosition[2].x + waterWidth / 2, water2D.handlesPosition[1].y);

                // The mesh must be recreated from scratch.
                recreateWaterMesh = true;
            }
        }

        /// <summary>
        /// Checks if the water Mesh height must be updated or the mesh must be recreated from scratch.
        /// </summary>
        private void WaterMesh()
        {
            // Recreates the water mesh from scratch based on the new handles positions.
            if (recreateWaterMesh)
                RecreateWaterMesh();

            // Updates the water height based on the new top and bottom handles positions.
            if (updateWaterHeight && !recreateWaterMesh)
                UpdateWaterHeight();
        }

        /// <summary>
        /// Updates the height of the water Mesh and recalculates the UV's for the bottom vertices.
        /// </summary>
        private void UpdateWaterHeight()
        {
            vertYOffsets = new float[surfaceVertsCount];
            // The curent water height.
            float height = Mathf.Abs(water2D.handlesPosition[0].y - water2D.handlesPosition[1].y);

            for (int i = 0; i < surfaceVertsCount; i++)
            {
                vertYOffsets[i] = vertices[surfaceVertsCount + i].y - waterLinePreviousLocalPos;

                float V = height / water2D.unitsPerUV.y;
                // The UVs for the bottom vertices are recalculated to eliminate the texture stretching.
                UVs[i] = new Vector2(UVs[i].x, water2D.verticalTiling && V > 1 ? 0 : 1 - V);
                // The positions on the Y axis of the bottom vertices are reset in case the bottom edge of the water is animated.
                vertices[i].y = water2D.handlesPosition[1].y;
                // This updates the surface vertices so that the waves created by objects are not lost.
                vertices[surfaceVertsCount + i].y = water2D.handlesPosition[0].y + vertYOffsets[i];

                if (water2D.cubeWater)
                    for (int j = 1; j <= water2D.zSegments + 1; j++)
                        vertices[j * surfaceVertsCount + surfaceVertsCount + i].y = vertices[surfaceVertsCount + i].y;
            }

            if (!water2D.use3DCollider)
            {
                // Here the size of the BoxCollider2D component is updated.
                BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
                boxCollider2D.size = new Vector2(water2D.width, height + water2D.colliderOffset);
                Vector2 center = water2D.handlesPosition[1];
                center.y += height / 2f + water2D.colliderOffset / 2f;

#if (UNITY_4_6 || UNITY_4_5)
                boxCollider2D.center = center;
#else
                boxCollider2D.offset = center;
#endif
            }
            else
            {
                // Here the size of the BoxCollider2D component is updated.
                BoxCollider boxCollider = GetComponent<BoxCollider>();
                boxCollider.size = new Vector3(water2D.width, height + water2D.colliderOffset, water2D.boxColliderZSize);
                Vector3 center = water2D.handlesPosition[1];
                center.y += height / 2f + water2D.colliderOffset / 2f;
                center.z += water2D.boxColliderZOffset;
                boxCollider.center = center;
            }

            waterLinePreviousLocalPos = water2D.handlesPosition[1].y + defaultWaterHeight + waterLineYPosOffset;
            waterLineCurrentGlobalPos = transform.TransformPoint(vertices[surfaceVertsCount + 1]);
            waterLineCurrentLocalPos = water2D.handlesPosition[1].y + defaultWaterHeight + waterLineYPosOffset;
        }

        /// <summary>
        /// Recreates the water Mesh from scratch. This is done if the water width changed.
        /// <summary>
        private void RecreateWaterMesh()
        {
            vertYOffsets = new float[surfaceVertsCount];

            // We store the difference between the current water line position and the surface
            // vertices so that after the mesh is recreated we can update their position based 
            // on the new waterline position. If we don't do this after the mesh is recreated 
            // the waves created by falling objects will disappear.
            for (int i = 0; i < surfaceVertsCount; i++)
            {
                vertYOffsets[i] = vertices[surfaceVertsCount + i].y - waterLinePreviousLocalPos;
            }

            // Recreates the water Mesh from scratch.
            water2D.RecreateWaterMesh();

            // Updates the size of different list variables, if the number of surface vertices changed from the previous frame.
            UpdateVariables();

            // If the surface vertices number is bigger we update their position based on the 
            // size of the vertYOffsets list. The new vertices ar left unchanged.
            if (vertYOffsets.Length < surfaceVertsCount || vertYOffsets.Length == surfaceVertsCount)
            {
                for (int i = 0; i < vertYOffsets.Length; i++)
                {
                    vertices[surfaceVertsCount + i].y = waterLineCurrentLocalPos + vertYOffsets[i];
                    if (water2D.cubeWater)
                        vertices[surfaceVertsCount + surfaceVertsCount + i].y = vertices[surfaceVertsCount + i].y;
                }
            }

            // If the surface vertices number is smaller we update their position based on the 
            // size of the surfaceVertsCount list.
            if (vertYOffsets.Length > surfaceVertsCount)
            {
                for (int i = 0; i < surfaceVertsCount; i++)
                {
                    vertices[surfaceVertsCount + i].y = waterLineCurrentLocalPos + vertYOffsets[i];
                    if (water2D.cubeWater)
                        vertices[surfaceVertsCount + surfaceVertsCount + i].y = vertices[surfaceVertsCount + i].y;
                }
            }

            CreateWaterLinePoints();
        }

        /// <summary>
        /// Creates a list of points that wil be used to define a clipping polygon.
        /// <summary>
        private void CreateWaterLinePoints()
        {
            waterLinePoints.Clear();

            if (surfaceVertsCount > meshSegmentsPerWaterLineSegment)
            {
                int n = 0;
                while (n < surfaceVertsCount)
                {
                    waterLinePoints.Add(transform.TransformPoint(vertices[surfaceVertsCount + n]));
                    n += meshSegmentsPerWaterLineSegment;
                }

                if (meshSegmentsPerWaterLineSegment != 1)
                {
                    Vector2 lastVert = transform.TransformPoint(vertices[surfaceVertsCount + surfaceVertsCount - 1]);
                    waterLinePoints.Add(new Vector2(lastVert.x, lastVert.y));
                }
            }
            else
            {
                waterLinePoints.Add(transform.TransformPoint(vertices[surfaceVertsCount]));
                Vector2 lastVert = transform.TransformPoint(vertices[surfaceVertsCount + surfaceVertsCount - 1]);
                waterLinePoints.Add(new Vector2(lastVert.x, lastVert.y));
            }
        }

        /// <summary>
        /// Updates the position of the Top, Right and Left handles.
        /// </summary>
        private void UpdateTopLeftRightHandles()
        {
            water2D.handlesPosition[0] = new Vector2(water2D.handlesPosition[0].x, water2D.handlesPosition[1].y + defaultWaterHeight + waterLineYPosOffset);
            water2D.handlesPosition[2] = new Vector2(water2D.handlesPosition[2].x, water2D.handlesPosition[1].y + defaultWaterHeight / 2 + waterLineYPosOffset / 2);
            water2D.handlesPosition[3] = new Vector2(water2D.handlesPosition[3].x, water2D.handlesPosition[1].y + defaultWaterHeight / 2 + waterLineYPosOffset / 2);
        }

        /// <summary>
        /// Simulates water waves generated by falling objects.
        /// </summary>
        private void WaterWaves()
        {
            leftHandleGlobalPos = transform.TransformPoint(water2D.handlesPosition[2]);

            // If the water is not dynamic and there are no objects in the water we do not need to update the velocity of the surface vertices.
            int objCount;

            if (!water2D.use3DCollider)
                objCount = floatingObjects2D.Count;
            else
                objCount = floatingObjects3D.Count;

            if (waterType == Water2D_Type.Dynamic && objCount > 0)
            {
                foundObjectIn.Clear();

                if (!water2D.use3DCollider)
                {
                    int len = floatingObjects2D.Count;
                    for (int i = 0; i < len; i++)
                    {
                        GenerateWaterWaves(i, true);
                    }
                }
                else
                {
                    int len = floatingObjects3D.Count;
                    for (int i = 0; i < len; i++)
                    {
                        GenerateWaterWaves(i, false);
                    }
                }
            }

            UpdateVertsPosition();

            if (clippingMethod == Water2D_ClippingMethod.Complex)
                UpdateWaterLinePoints();
        }

        /// <summary>
        /// Updates the position of the water line points. This are the points 
        /// that will be used to define a clipping polygon.
        /// <summary>
        private void UpdateWaterLinePoints()
        {
            int n = 0;
            Vector2 vertGlobalPos = Vector2.zero;
            int len = waterLinePoints.Count;

            for (int i = 0; i < len - 1; i++)
            {
                vertGlobalPos = transform.TransformPoint(vertices[surfaceVertsCount + n]);
                waterLinePoints[i] = vertGlobalPos;
                n += meshSegmentsPerWaterLineSegment;
            }

            vertGlobalPos = transform.TransformPoint(vertices[surfaceVertsCount + surfaceVertsCount - 1]);
            waterLinePoints[len - 1] = vertGlobalPos;

            if (showClippingPlolygon)
            {
                for (int j = 0; j < waterLinePoints.Count - 1; j++)
                {
                    Debug.DrawLine(waterLinePoints[j], waterLinePoints[j + 1], Color.cyan);
                }
            }
        }


        /// <summary>
        /// Simulates water waves generated by falling objects.
        /// </summary>
        /// <param name="cIndex">The index of a collider in the floatingObjects2D or floatingObjects3D list.</param>
        /// <param name="a2DCollider">Is this a 2D collider?.</param>
        private void GenerateWaterWaves(int cIndex, bool a2DCollider)
        {
            // The distance from the left handle to the center of the collider.
            float distance;

			if (cIndex >= floatingObjects2D.Count)
				return;
            else if (floatingObjects2D[cIndex] == null)
            {
                floatingObjects2D.RemoveAt(cIndex);
                return;
            }

            if (a2DCollider)
                distance = Mathf.Abs(leftHandleGlobalPos.x - floatingObjects2D[cIndex].bounds.center.x);
            else
                distance = Mathf.Abs(leftHandleGlobalPos.x - floatingObjects3D[cIndex].bounds.center.x);

            // The current water width.
            float waterWidth = Mathf.Abs(water2D.handlesPosition[3].x - water2D.handlesPosition[2].x);

            // The distance from the left handle to the left side of the bounding box of the collider.
            float minX;
            if (a2DCollider)
                minX = distance - floatingObjects2D[cIndex].bounds.extents.x;
            else
                minX = distance - floatingObjects3D[cIndex].bounds.extents.x;
            // The distance from the left handle to the right side of the bounding box of the collider.
            float maxX = distance + (a2DCollider ? floatingObjects2D[cIndex].bounds.extents.x : floatingObjects3D[cIndex].bounds.extents.x);

            if (minX < 0)
                minX = 0;
            if (maxX > waterWidth)
                maxX = waterWidth;

            // The index of the surface vertex that is closest to the left edge of the colliders bounding box.
            int minIndex = (int)Mathf.Floor(minX / (1f / water2D.segmentsPerUnit));
            // The index of the surface vertex that is closest to the right edge of the colliders bounding box.
            int maxIndex = (int)Mathf.Floor(maxX / (1f / water2D.segmentsPerUnit));

            // We make sure that we don't get out of bounds indexes.
            if (maxIndex > surfaceVertsCount - 1)
                maxIndex = surfaceVertsCount - 1;
            if (minIndex < 0)
                minIndex = 0;


            for (int i = minIndex; i < maxIndex; i++)
            {
                // Global position of 2 surface vertices.
                Vector2 vertex1 = vertices[i + surfaceVertsCount];
                vertex1 = transform.TransformPoint(vertex1);

                Vector2 vertex2 = vertices[i + surfaceVertsCount + 1];
                vertex2 = transform.TransformPoint(vertex2);

                // We check for objects between these 2 vertices.
                // We do this because we want only the objects that are overlapping the current vertex to affect its velocity.
                RaycastHit2D[] hit2D = null;
                Collider[] hit3D = null;

                if (a2DCollider)
                    hit2D = Physics2D.LinecastAll(vertex1, vertex2);
                else
                    hit3D = Physics.OverlapSphere(new Vector3(vertex1.x, vertex1.y, transform.position.z + water2D.boxColliderZOffset), overlapSphereRadius);

                int len;
                if (a2DCollider)
                    len = hit2D.Length;
                else
                    len = hit3D.Length;

                for (int j = 0; j < len; j++)
                {
                    // If the object does not have a rigidbody component or its center is below the waterline it will be ignored.
                    if (a2DCollider)
                    {
                        if (hit2D[j].rigidbody == null || hit2D[j].collider.bounds.center.y < waterLineCurrentGlobalPos.y)
                            continue;
                    }
                    else
                    {
                        if (hit3D[j].GetComponent<Rigidbody>() == null || hit3D[j].GetComponent<Collider>().bounds.center.y < waterLineCurrentGlobalPos.y)
                            continue;
                    }

                    Vector2 collBoundsSize;
                    if (a2DCollider)
                        collBoundsSize = hit2D[j].collider.bounds.size;
                    else
                        collBoundsSize = hit3D[j].GetComponent<Collider>().bounds.size;

                    // The distance from the water line to the bottom of the object.
                    float distanceFromWaterLine = collBoundsSize.y / 2f - Mathf.Abs(waterLineCurrentGlobalPos.y - (a2DCollider ? hit2D[j].collider.bounds.center.y : hit3D[j].GetComponent<Collider>().bounds.center.y));

                    // Limits the ability of an object to interact with the water to a small region.
                    // If the distance from the bottom of the object to the water line is greater than the interaction region, the object will be ignored.
                    if (distanceFromWaterLine > (interactionRegion > collBoundsSize.y ? collBoundsSize.y : interactionRegion))
                    {
                        continue;
                    }

                    Vector3 objectVelocity = new Vector3(0, 0, 0);

                    if (a2DCollider)
                        objectVelocity = hit2D[j].rigidbody.velocity;
                    else
                        objectVelocity = hit3D[j].GetComponent<Rigidbody>().velocity;

                    if (objectVelocity.y < velocityFilter)
                    {
                        velocities[i] = objectVelocity.y * collisionVelocity;

                        // If on the previous frame in this segment an object was not detected a new particle system will be instantiated.
                        if (particleS != null && objectVelocity.y < velocityFilter - 2f)
                        {
                            if (a2DCollider)
                            {
                                if (particleSystemInstantiation == Water2D_ParticleSystem.PerObject && !tempObj2D.Contains(hit2D[j].collider))
                                    continue;
                            }
                            else
                            {
                                if (particleSystemInstantiation == Water2D_ParticleSystem.PerObject && !tempObj3D.Contains(hit3D[j].GetComponent<Collider>()))
                                    continue;
                            }

                            if (particleSystemInstantiation == Water2D_ParticleSystem.PerVertex && foundObjOnPrevFrame[i])
                                continue;

                            // If the current collider has the tag "Player" and is not in the floatingObjects list it is ignored.
                            // This is done so that if the Player has multiple colliders only one generates water particles.
                            if (a2DCollider)
                            {
                                if (hit2D[j].collider.tag == "Player" && !floatingObjects2D.Contains(hit2D[j].collider))
                                    continue;
                            }
                            else
                            {
                                if (hit3D[j].GetComponent<Collider>().tag == "Player" && !floatingObjects3D.Contains(hit3D[j].GetComponent<Collider>()))
                                    continue;
                            }

                            if (particleSystemInstantiation == Water2D_ParticleSystem.PerVertex)
                            {
                                // Adds the index of the segment where a particle system was instantiated to the foundObjectIn list.
                                // Makes sure on the next frame a new particle system is not instantiated here.
                                foundObjectIn.Add(i);
                            }

                            // Instantiates a new particle system from a prefab object.
                            GameObject splash;

                            if (particleSystemInstantiation == Water2D_ParticleSystem.PerObject)
                            {
                                Vector3 pos;

                                if (a2DCollider)
                                    pos = hit2D[j].transform.position;
                                else
                                    pos = hit3D[j].transform.position;

                                splash = Instantiate(particleS, new Vector3(pos.x + particleSystemPosOffset.x, vertex2.y + particleSystemPosOffset.y, transform.position.z + water2D.boxColliderZOffset + particleSystemPosOffset.z), Quaternion.Euler(new Vector3(270f, 0, 0))) as GameObject;
                            }
                            else
                                splash = Instantiate(particleS, new Vector3(vertex2.x + particleSystemPosOffset.x, vertex2.y + particleSystemPosOffset.y, transform.position.z + water2D.boxColliderZOffset + particleSystemPosOffset.z), Quaternion.Euler(new Vector3(270f, 0, 0))) as GameObject;

                            if (particleSystemSorting)
                            {
                                // The sorting layer of the particle system is set to the same as the waters.
                                splash.GetComponent<Renderer>().sortingLayerName = particleSystemSortingLayerName;
                                splash.GetComponent<Renderer>().sortingOrder = particleSystemOrderInLayer;
                            }

                            // Makes the water parent of the particle system.
                            splash.transform.parent = transform;

                            // The particle system will be destroyed a second after it was instantiated.
                            Destroy(splash, 1f);
                        }

                        // Generates a splash sound.
                        if (a2DCollider && splashSound != null && tempObj2D.Contains(hit2D[j].collider))
                        {
                            AudioSource.PlayClipAtPoint(splashSound, vertex2);
                        }

                        if (!a2DCollider && splashSound != null && tempObj3D.Contains(hit3D[j].GetComponent<Collider>()))
                        {
                            AudioSource.PlayClipAtPoint(splashSound, vertex2);
                        }

                        if (a2DCollider && tempObj2D.Contains(hit2D[j].collider))
                        {
                            // The collider is removed from tempObj list so that the next segment doesn't generate a sound effect too.
                            tempObj2D.Remove(hit2D[j].collider);
                        }

                        if (!a2DCollider && tempObj3D.Contains(hit3D[j].GetComponent<Collider>()))
                        {
                            // The collider is removed from tempObj list so that the next segment doesn't generate a sound effect too.
                            tempObj3D.Remove(hit3D[j].GetComponent<Collider>());
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Updates the position of the surface vertices.
        /// </summary>
        /// <remarks>
        /// Based on:
        /// http://gamedevelopment.tutsplus.com/tutorials/make-a-splash-with-2d-water-effects--gamedev-236
        /// </remarks>
        private void UpdateVertsPosition()
        {
            if (particleSystemInstantiation == Water2D_ParticleSystem.PerVertex)
            {
                // Rsets the foundObjOnPrevFrame list.
                for (int i = 0; i < surfaceVertsCount - 1; i++)
                {
                    foundObjOnPrevFrame[i] = false;
                }

                // We set the segment where an object was found in the current frame to true 
                // so that on the next frame a particle system won't be Instantiated in this place.
                int len = foundObjectIn.Count;

                //foreach (int id in foundObjectIn)
                for (int i = 0; i < len; i++)
                {
                    foundObjOnPrevFrame[foundObjectIn[i]] = true;
                }
            }

            // Applies damping, velocity, and acceleration.
            for (int i = 0; i < surfaceVertsCount; i++)
            {
                float yDisplacement = vertices[surfaceVertsCount + i].y - waterLineCurrentLocalPos;
                accelerations[i] = -springConstant * yDisplacement - velocities[i] * damping;
                velocities[i] += accelerations[i];
                yDisplacement += velocities[i];
                vertices[surfaceVertsCount + i].y = yDisplacement + waterLineCurrentLocalPos;
            }

            // Applies wave motion.
            for (int i = 0; i < surfaceVertsCount; i++)
            {
                float y = vertices[surfaceVertsCount + i].y;
                if (i > 0)
                {
                    float leftVertY = vertices[surfaceVertsCount + i - 1].y;
                    leftDeltas[i] = spread * (y - leftVertY);
                    velocities[i - 1] += leftDeltas[i] * waveSpeed;
                }

                if (i < surfaceVertsCount - 1)
                {
                    float rightVertY = vertices[surfaceVertsCount + i + 1].y;
                    rightDeltas[i] = spread * (y - rightVertY);
                    velocities[i + 1] += rightDeltas[i] * waveSpeed;
                }
            }

            // Updates the neighbour vertices.
            for (int i = 0; i < surfaceVertsCount; i++)
            {
                if (i > 0)
                {
                    vertices[surfaceVertsCount + i - 1].y += leftDeltas[i];
                }

                if (i < surfaceVertsCount - 1)
                {
                    vertices[surfaceVertsCount + i + 1].y += rightDeltas[i];
                }
            }

            if (water2D.cubeWater)
            {
                for (int k = 2; k < water2D.zSegments + 3; k++)
                {
                    for (int i = 0; i < surfaceVertsCount; i++)
                    {
                        vertices[k * surfaceVertsCount + i].y = vertices[surfaceVertsCount + i].y;
                    }
                }
            }
        }

        /// <summary>
        /// Applies a buoyant force, drag and lift to an object submerged in the water.
        /// </summary>
        private void PhysicsBasedBuoyantForce()
        {
            // The global position of the left handle.
            leftHandleGlobalPos = transform.TransformPoint(water2D.handlesPosition[2]);

            if (!water2D.use3DCollider)
            {
                int len = floatingObjects2D.Count;
                for (int i = 0; i < len; i++)
                {
                    ApplyPhysicsBasedBuoyantForce(i, true);
                }
            }
            else
            {
                int len = floatingObjects3D.Count;
                for (int i = 0; i < len; i++)
                {
                    ApplyPhysicsBasedBuoyantForce(i, false);
                }
            }
        }

        /// <summary>
        /// Applies a buoyant force, drag and lift to an object submerged in the water.
        /// </summary>
        /// <param name="cIndex">The index of a collider in the floatingObjects2D or floatingObjects3D list.</param>
        /// <param name="a2DCollider">Is this a 2D collider?.</param>
        private void ApplyPhysicsBasedBuoyantForce(int cIndex, bool a2DCollider)
        {
            // The global position of the closest vertex to the center of the collider.
            Vector3 globalPosOfVert;
            float distance;
            bool isIntersecting = true;

            if (a2DCollider)
                distance = Mathf.Abs(leftHandleGlobalPos.x - floatingObjects2D[cIndex].bounds.center.x);
            else
                distance = Mathf.Abs(leftHandleGlobalPos.x - floatingObjects3D[cIndex].bounds.center.x);

            // The index of the surface vertex that is closest to the colliders bounding box center.
            int index = (int)Mathf.Floor(distance * water2D.segmentsPerUnit);

            // We make sure that we don't get out of bounds indexes.
            if (index > surfaceVertsCount - 1)
                index = surfaceVertsCount - 1;

            globalPosOfVert = transform.TransformPoint(vertices[index + surfaceVertsCount]);

            area = 0;
            displacedMass = 0;
            // The segment of the polygon that is below the waterline.
            Vector2[] intersectionPolygon;

            if ((a2DCollider ? floatingObjects2D[cIndex].GetComponent<Collider2D>().bounds.min.y : floatingObjects3D[cIndex].GetComponent<Collider>().bounds.min.y) < globalPosOfVert.y)
            {
                BoxCollider2D boxColl2D = null;
                PolygonCollider2D polyColl2D = null;
                CircleCollider2D circleColl2D = null;

                BoxCollider boxColl = null;
                SphereCollider sphereColl = null;
                CapsuleCollider capsuleColl = null;

                if (a2DCollider)
                {
                    // Gets the BoxCollider2D component of the current object if it has one.
                    boxColl2D = floatingObjects2D[cIndex].GetComponent<BoxCollider2D>();
                    // Gets the PolygonCollider2D component of the current object if it has one.
                    polyColl2D = floatingObjects2D[cIndex].GetComponent<PolygonCollider2D>();
                    // Gets the CircleCollider2D component of the current object if it has one.
                    circleColl2D = floatingObjects2D[cIndex].GetComponent<CircleCollider2D>();
                }
                else
                {
                    // Gets the BoxCollider component of the current object if it has one.
                    boxColl = floatingObjects3D[cIndex].GetComponent<BoxCollider>();
                    // Gets the SphereCollider component of the current object if it has one.
                    sphereColl = floatingObjects3D[cIndex].GetComponent<SphereCollider>();
                    // Gets the CapsuleCollider component of the current object if it has one.
                    capsuleColl = floatingObjects3D[cIndex].GetComponent<CapsuleCollider>();
                }

                // An array that contains 44 points that form a box.
                Vector2[] boXVertices = new Vector2[4];
                // An array that contains 2 points that form a line.
                Vector2[] linePoints = new Vector2[2];

                if (clippingMethod == Water2D_ClippingMethod.Simple)
                {
                    linePoints[0] = new Vector2(water2D.handlesPosition[2].x, globalPosOfVert.y);
                    linePoints[1] = new Vector2(water2D.handlesPosition[3].x, globalPosOfVert.y);
                }

                if ((a2DCollider ? floatingObjects2D[cIndex].GetComponent<Collider2D>().tag : floatingObjects3D[cIndex].GetComponent<Collider>().tag) == "Player")
                {
                    // The global position of an imaginary bounding box.
                    boXVertices = GetPlayerBoudingBoxVerticesGlobalPos(cIndex, a2DCollider);

                    // If the bottom of the bounding is above the vertex, a buoyant force should not be applied to the object.
                    if (boXVertices[1].y > globalPosOfVert.y)
                        return;

                    intersectionPolygon = Water2D_PolygonClipping.GetIntersectedPolygon(boXVertices, linePoints, out isIntersecting);
                    ApplyPhysicsForces(cIndex, intersectionPolygon, a2DCollider);

                    // This will make sure the buoyant force is applied only once.
                    // The other colliders for this game object will be ignored.
                    return;
                }

                int minIndex = 0;
                int maxIndex = 0;

                if (clippingMethod == Water2D_ClippingMethod.Complex)
                {
                    float waterWidth = Mathf.Abs(water2D.handlesPosition[3].x - water2D.handlesPosition[2].x);

                    // The distance from the left handle to the left side of the bounding box of the collider.
                    float minX;

                    if (a2DCollider)
                        minX = distance - floatingObjects2D[cIndex].bounds.extents.x;
                    else
                        minX = distance - floatingObjects3D[cIndex].bounds.extents.x;
                    // The distance from the left handle to the right side of the bounding box of the collider.
                    float maxX = distance + (a2DCollider ? floatingObjects2D[cIndex].bounds.extents.x : floatingObjects3D[cIndex].bounds.extents.x);

                    if (minX < 0)
                        minX = 0;
                    if (maxX > waterWidth)
                        maxX = waterWidth;

                    // The index of the surface vertex that is closest to the left edge of the colliders bounding box.
                    minIndex = (int)Mathf.Floor(minX * water2D.segmentsPerUnit);
                    // The index of the surface vertex that is closest to the right edge of the colliders bounding box.
                    maxIndex = (int)Mathf.Floor(maxX * water2D.segmentsPerUnit);

                    // We make sure that we don't get out of bounds indexes.
                    if (maxIndex > surfaceVertsCount - 1)
                        maxIndex = surfaceVertsCount - 1;
                    if (minIndex < 0)
                        minIndex = 0;
                }

                if (a2DCollider)
                {
                    // If the object has a BoxCollider2D component, physics forces are applied to this object.
                    if (boxColl2D != null)
                    {
                        boXVertices = GetBoxVerticesGlobalPosition(cIndex, a2DCollider);

                        ApplyForcesToObject(boXVertices, linePoints, minIndex, maxIndex, cIndex, a2DCollider);
                    }

                    // If the object has a CircleCollider2D component, physics forces are applied to this object.
                    if (circleColl2D != null)
                    {
                        Vector2[] polyCorners = new Vector2[polygonCorners];
                        polyCorners = GetPolygonVerticesGlobalPosition(cIndex, polygonCorners, a2DCollider);

                        ApplyForcesToObject(polyCorners, linePoints, minIndex, maxIndex, cIndex, a2DCollider);
                    }

                    // If the object has a PolygonCollider2D component, physics forces are applied to this object.
                    if (polyColl2D != null)
                    {
                        Vector2[] polyPoints = polyColl2D.points;
                        for (int n = 0; n < polyPoints.Length; n++)
                        {
                            polyPoints[n] = floatingObjects2D[cIndex].transform.TransformPoint(polyPoints[n]);
                        }

                        if (Water2D_PolygonClipping.IsClockwise(polyPoints))
                        {
                            System.Array.Reverse(polyPoints);
                        }

                        ApplyForcesToObject(polyPoints, linePoints, minIndex, maxIndex, cIndex, a2DCollider);
                    }
                }
                else
                {
                    if (boxColl != null)
                    {
                        boXVertices = GetBoxVerticesGlobalPosition(cIndex, a2DCollider);

                        ApplyForcesToObject(boXVertices, linePoints, minIndex, maxIndex, cIndex, a2DCollider);
                    }

                    if (sphereColl != null)
                    {
                        Vector2[] polyCorners = new Vector2[polygonCorners];
                        polyCorners = GetPolygonVerticesGlobalPosition(cIndex, polygonCorners, a2DCollider);

                        ApplyForcesToObject(polyCorners, linePoints, minIndex, maxIndex, cIndex, a2DCollider);
                    }

                    if (capsuleColl != null)
                    {
                        Vector2[] polyCorners = new Vector2[polygonCorners];

                        polyCorners = GetCapsuleVerticesGlobalPosition(capsuleColl, polygonCorners);
                        ApplyForcesToObject(polyCorners, linePoints, minIndex, maxIndex, cIndex, a2DCollider);
                    }
                }
            }
        }

        /// <summary>
        /// Applies a buoyant force, drag and lift to an object submerged in the water.
        /// </summary>
        /// <param name="subjPoly"> The polygon of the object in the water.</param>
        /// <param name="clipPoly"> The clipping polygon.</param>
        /// <param name="minIndex"> The min index for a value in the "waterLinePoints" list.</param>
        /// <param name="maxIndex"> The max index for a value in the "waterLinePoints" list.</param>
        /// <param name="cIndex">The index of a collider in the floatingObjects2D or floatingObjects3D list.</param>
        /// <param name="a2DCollider">Is this a 2D collider?.</param>
        private void ApplyForcesToObject(Vector2[] subjPoly, Vector2[] clipPoly, int minIndex, int maxIndex, int cIndex, bool a2DCollider)
        {
            // The segment of the polygon that is below the waterline.
            Vector2[] intersectionPolygon;
            List<List<Vector2>> intersectionPolygons = new List<List<Vector2>>();
            bool isIntersecting = true;

            if (clippingMethod == Water2D_ClippingMethod.Simple)
            {
                intersectionPolygon = Water2D_PolygonClipping.GetIntersectedPolygon(subjPoly, clipPoly, out isIntersecting);

                if (isIntersecting)
                    ApplyPhysicsForces(cIndex, intersectionPolygon, a2DCollider);
            }
            else
            {
                intersectionPolygons = GetIntersectionPolygon(subjPoly, minIndex, maxIndex, out isIntersecting);

                if (isIntersecting)
                {
                    int len = intersectionPolygons.Count;
                    for (int i = 0; i < len; i++)
                    {
                        intersectionPolygon = intersectionPolygons[i].ToArray();
                        ApplyPhysicsForces(cIndex, intersectionPolygon, a2DCollider);
                    }
                }
            }
        }

        /// <summary>
        /// Applies a buoyant force, drag and lift to an object submerged in the water.
        /// </summary>
        /// <param name="subjectPoly"> The polygon of the object in the water.</param>
        /// <param name="minIndex"> The min index for a value in the "waterLinePoints" list. </param>
        /// <param name="maxIndex"> The max index for a value in the "waterLinePoints" list. </param>
        /// <param name="isIntersecting"> Are the subject and clipping polygon intersecting?. </param>
        private List<List<Vector2>> GetIntersectionPolygon(Vector2[] subjectPoly, int minIndex, int maxIndex, out bool isIntersecting)
        {
            Vector2 bottomHandleGlobalPos = transform.TransformPoint(water2D.handlesPosition[1]);
            List<List<Vector2>> intersectionPoly = new List<List<Vector2>>();
            List<Vector2> clipPolygon = new List<Vector2>();
            Clipper clipper = new Clipper();
            Paths solutionPath = new Paths();
            Path subjPath = new Path();
            Path clipPath = new Path();
            int len, len2, min, max;
            isIntersecting = true;

            if (surfaceVertsCount > meshSegmentsPerWaterLineSegment)
            {
                min = (int)Mathf.Floor(minIndex / meshSegmentsPerWaterLineSegment);
                max = (int)Mathf.Floor(maxIndex / meshSegmentsPerWaterLineSegment) + 1;

                if (max > waterLinePoints.Count - 2)
                    max = waterLinePoints.Count - 2;

                for (int i = min; i <= max; i++)
                {
                    clipPolygon.Add(waterLinePoints[i]);
                }

                int last = clipPolygon.Count - 1;
                clipPolygon.Add(new Vector2(clipPolygon[last].x, bottomHandleGlobalPos.y));
                clipPolygon.Add(new Vector2(clipPolygon[0].x, bottomHandleGlobalPos.y));
            }
            else
            {
                Vector2 vertGlobalPos = transform.TransformPoint(vertices[surfaceVertsCount]);
                clipPolygon.Add(vertGlobalPos);
                vertGlobalPos = transform.TransformPoint(vertices[surfaceVertsCount + surfaceVertsCount - 1]);
                clipPolygon.Add(new Vector2(vertGlobalPos.x, vertGlobalPos.y));

                int last = clipPolygon.Count - 1;
                clipPolygon.Add(new Vector2(clipPolygon[last].x, bottomHandleGlobalPos.y));
                clipPolygon.Add(new Vector2(clipPolygon[0].x, bottomHandleGlobalPos.y));
            }

            if (showClippingPlolygon)
            {
                for (int i = 0; i < clipPolygon.Count; i++)
                {
                    if (i < clipPolygon.Count - 1)
                        Debug.DrawLine(clipPolygon[i], clipPolygon[i + 1], Color.green);
                    else
                        Debug.DrawLine(clipPolygon[i], clipPolygon[0], Color.green);
                }
            }

            len = subjectPoly.Length;
            for (int i = 0; i < len; i++)
            {
                subjPath.Add(new IntPoint(subjectPoly[i].x * scaleFactor, subjectPoly[i].y * scaleFactor));
            }

            len = clipPolygon.Count;
            for (int i = 0; i < len; i++)
            {
                clipPath.Add(new IntPoint(clipPolygon[i].x * scaleFactor, clipPolygon[i].y * scaleFactor));
            }

            clipper.AddPath(subjPath, PolyType.ptSubject, true);
            clipper.AddPath(clipPath, PolyType.ptClip, true);
            clipper.Execute(ClipType.ctIntersection, solutionPath, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

            if (solutionPath.Count != 0)
            {
                len = solutionPath.Count;

                for (int i = 0; i < len; i++)
                {
                    len2 = solutionPath[i].Count;
                    List<Vector2> list = new List<Vector2>();

                    for (int j = 0; j < len2; j++)
                    {
                        list.Add(new Vector2(solutionPath[i][j].X / scaleFactor, solutionPath[i][j].Y / scaleFactor));
                    }

                    intersectionPoly.Add(list);
                }
                return intersectionPoly;
            }
            else
            {
                isIntersecting = false;
                return null;
            }
        }

        /// <summary>
        /// Calculates the global position for the players bounding box vertices.
        /// </summary>
        /// <param name="cIndex">The index of a collider in the floatingObjects2D or floatingObjects3D list.</param>
        /// <param name="a2DCollider">Does the Player have a 2D collider.</param>
        /// <returns>Returns the global position of a bounding box vertices.</returns>
        public Vector2[] GetPlayerBoudingBoxVerticesGlobalPos(int cIndex, bool a2DCollider)
        {
            // Bounding box vertices.
            Vector2[] boxVertices = new Vector2[4];
            // The current global position of the object.
            Vector3 objPos = Vector3.zero;

            if (a2DCollider)
                objPos = floatingObjects2D[cIndex].transform.position;
            else
                objPos = floatingObjects3D[cIndex].transform.position;

            // Top left vertex.
            boxVertices[0] = new Vector2(objPos.x - (playerBoundingBoxSize.x / 2f) + playerBoundingBoxCenter.x, objPos.y + (playerBoundingBoxSize.y / 2f) + playerBoundingBoxCenter.y);
            // Bottom left vertex.
            boxVertices[1] = new Vector2(objPos.x - (playerBoundingBoxSize.x / 2f) + playerBoundingBoxCenter.x, objPos.y - (playerBoundingBoxSize.y / 2f) + playerBoundingBoxCenter.y);
            // Bottom right vertex.
            boxVertices[2] = new Vector2(objPos.x + (playerBoundingBoxSize.x / 2f) + playerBoundingBoxCenter.x, objPos.y - (playerBoundingBoxSize.y / 2f) + playerBoundingBoxCenter.y);
            // Top right vertex.
            boxVertices[3] = new Vector2(objPos.x + (playerBoundingBoxSize.x / 2f) + playerBoundingBoxCenter.x, objPos.y + (playerBoundingBoxSize.y / 2f) + playerBoundingBoxCenter.y);

            return boxVertices;
        }

        /// <summary>
        /// Performs the cross product on a vector and a scalar.
        /// </summary>
        /// <param name="a">A scalar value.</param>
        /// <param name="b">A 2D vector.</param>
        /// <returns>Returns new 2D vector.</returns>
        private Vector2 Cross(float a, Vector2 b)
        {
            return new Vector2(a * b.y, -a * b.x);
        }

        /// <summary>
        /// Calculates the global position of a BoxCollider2D vertices.
        /// </summary>
        /// <param name="cIndex">The index of a collider in the floatingObjects2D or floatingObjects3D list.</param>
        /// <param name="a2DCollider">Is this a 2D collider?.</param>
        /// <returns>Returns the global position of a box collider vertices.</returns>
        public Vector2[] GetBoxVerticesGlobalPosition(int cIndex, bool a2DCollider)
        {
            Vector2[] boxVertices = new Vector2[4];
            Vector2 boundsMin = Vector2.zero;
            Vector2 boundsMax = Vector2.zero;
            Vector2 boundsExtents = Vector2.zero;
            // The center of the Box Collider.
            Vector3 boxCollCenterPos = Vector3.zero;
            float angleDegZ = 0f;
            float radians = 0;
            BoxCollider2D boxCollider2D = null;
            BoxCollider boxCollider = null;

            if (a2DCollider)
            {
                // The angle of rotation on the Z axis in radians 
                radians = floatingObjects2D[cIndex].transform.eulerAngles.z * Mathf.Deg2Rad;
                boxCollCenterPos = floatingObjects2D[cIndex].bounds.center;
                angleDegZ = floatingObjects2D[cIndex].transform.eulerAngles.z;
                boundsMin = floatingObjects2D[cIndex].bounds.min;
                boundsMax = floatingObjects2D[cIndex].bounds.max;
                boundsExtents = floatingObjects2D[cIndex].bounds.extents;
                boxCollider2D = floatingObjects2D[cIndex].GetComponent<BoxCollider2D>();
            }
            else
            {
                // The angle of rotation on the Z axis in radians 
                radians = floatingObjects3D[cIndex].transform.eulerAngles.z * Mathf.Deg2Rad;
                boxCollCenterPos = floatingObjects3D[cIndex].bounds.center;
                angleDegZ = floatingObjects3D[cIndex].transform.eulerAngles.z;
                boundsMin = floatingObjects3D[cIndex].bounds.min;
                boundsMax = floatingObjects3D[cIndex].bounds.max;
                boundsExtents = floatingObjects3D[cIndex].bounds.extents;
                boxCollider = floatingObjects3D[cIndex].GetComponent<BoxCollider>();
            }

            // If the angle of rotation on the Z axis is one of the values (0, 90, 180, 270, 360) we can use the bounding box of the collider to calculate 
            // the position of its 4 vertices.
            if (angleDegZ == 0 || angleDegZ == 90 || angleDegZ == 180 || angleDegZ == 270 || angleDegZ == 360)
            {
                // Top left vertex.
                boxVertices[0] = new Vector2(boundsMin.x, boxCollCenterPos.y + boundsExtents.y);
                // Bottom left vertex.
                boxVertices[1] = new Vector2(boundsMin.x, boxCollCenterPos.y - boundsExtents.y);
                // Bottom right vertex.
                boxVertices[2] = new Vector2(boundsMax.x, boxCollCenterPos.y - boundsExtents.y);
                // Top right vertex.
                boxVertices[3] = new Vector2(boundsMax.x, boxCollCenterPos.y + boundsExtents.y);
            }
            else
            {
                float halfWidth = 0f;
                float halfHeight = 0f;

                if (a2DCollider)
                {
                    halfWidth = (boxCollider2D.size.x * boxCollider2D.transform.localScale.x) / 2f;
                    halfHeight = (boxCollider2D.size.y * boxCollider2D.transform.localScale.y) / 2f;
                }
                else
                {
                    halfWidth = (boxCollider.size.x * boxCollider.transform.localScale.x) / 2f;
                    halfHeight = (boxCollider.size.y * boxCollider.transform.localScale.y) / 2f;
                }

                // The global position of the box vertices at 0 degrees on the Z axis.
                // Top left vertex.
                boxVertices[0] = new Vector2(boxCollCenterPos.x - halfWidth, boxCollCenterPos.y + halfHeight);
                // Bottom left vertex.
                boxVertices[1] = new Vector2(boxCollCenterPos.x - halfWidth, boxCollCenterPos.y - halfHeight);
                // Bottom right vertex.
                boxVertices[2] = new Vector2(boxCollCenterPos.x + halfWidth, boxCollCenterPos.y - halfHeight);
                // Top right vertex.
                boxVertices[3] = new Vector2(boxCollCenterPos.x + halfWidth, boxCollCenterPos.y + halfHeight);

                // The global position of the box vertices after rotation around the Z axis.
                for (int i = 0; i < 4; i++)
                {
                    boxVertices[i] = new Vector2((boxVertices[i].x - boxCollCenterPos.x) * Mathf.Cos(radians) - (boxVertices[i].y - boxCollCenterPos.y) * Mathf.Sin(radians) + boxCollCenterPos.x, (boxVertices[i].x - boxCollCenterPos.x) * Mathf.Sin(radians) + (boxVertices[i].y - boxCollCenterPos.y) * Mathf.Cos(radians) + boxCollCenterPos.y);
                }
            }

            return boxVertices;
        }

        /// <summary>
        /// Generates an imaginary regular polygon based on the circle or sphere colliders center and radius.
        /// </summary>
        /// <param name="cIndex">The index of a collider in the floatingObjects2D or floatingObjects3D list.</param>
        /// <param name="pCorners">The number of corners the polygon should have.</param>
        /// <param name="a2DCollider">Is this a 2D collider?.</param>
        /// <returns>Returns the global position of a regular polygon vertices.</returns>
        public Vector2[] GetPolygonVerticesGlobalPosition(int cIndex, int pCorners, bool a2DCollider)
        {
            Vector2[] polygonPoints = new Vector2[pCorners];
            Vector3 collCenterPos = Vector3.zero;
            CircleCollider2D circleCollider2D = null;
            SphereCollider sphereCollider = null;
            Vector3 sphereLocalScale = Vector3.zero;

            float radius = 0;
            float angleDeg = 0;
            float radians = 0;

            if (a2DCollider)
            {
                collCenterPos = floatingObjects2D[cIndex].bounds.center;
                circleCollider2D = floatingObjects2D[cIndex].GetComponent<CircleCollider2D>();
                radius = 0.125f + circleCollider2D.radius * (circleCollider2D.transform.localScale.x > circleCollider2D.transform.localScale.y ? circleCollider2D.transform.localScale.x : circleCollider2D.transform.localScale.y);
                radians = circleCollider2D.transform.eulerAngles.z * Mathf.Deg2Rad;
            }
            else
            {
                collCenterPos = floatingObjects3D[cIndex].bounds.center;
                sphereCollider = floatingObjects3D[cIndex].GetComponent<SphereCollider>();
                sphereLocalScale = sphereCollider.transform.localScale;

                radius = 0.125f + sphereCollider.radius * sphereLocalScale.x;

                if (sphereLocalScale.y > sphereLocalScale.x && sphereLocalScale.y > sphereLocalScale.z)
                    radius = 0.125f + sphereCollider.radius * sphereLocalScale.y;
                if (sphereLocalScale.z > sphereLocalScale.x && sphereLocalScale.z > sphereLocalScale.y)
                    radius = 0.125f + sphereCollider.radius * sphereLocalScale.z;

                radians = sphereCollider.transform.eulerAngles.z * Mathf.Deg2Rad;
            }

            for (int i = 0; i < pCorners; i++)
            {
                angleDeg = (360f / pCorners) * i;
                radians = angleDeg * Mathf.Deg2Rad;

                polygonPoints[i] = new Vector2(collCenterPos.x + radius * Mathf.Cos(radians), collCenterPos.y + radius * Mathf.Sin(radians));

                if (a2DCollider)
                    radians = circleCollider2D.transform.eulerAngles.z * Mathf.Deg2Rad;
                else
                    radians = sphereCollider.transform.eulerAngles.z * Mathf.Deg2Rad;

                polygonPoints[i] = new Vector2((polygonPoints[i].x - collCenterPos.x) * Mathf.Cos(radians) - (polygonPoints[i].y - collCenterPos.y) * Mathf.Sin(radians) + collCenterPos.x, (polygonPoints[i].x - collCenterPos.x) * Mathf.Sin(radians) + (polygonPoints[i].y - collCenterPos.y) * Mathf.Cos(radians) + collCenterPos.y);
            }

            return polygonPoints;
        }

        /// <summary>
        /// Generates an imaginary polygon based on a CapsuleCollider center and radius.
        /// </summary>
        /// <param name="capsuleCollider">A CapsuleCollider around which to create a polygon.</param>
        /// <param name="pCorners">The number of corners the polygon should have.</param>
        /// <returns>Returns the global position of a polygon vertices.</returns>
        public Vector2[] GetCapsuleVerticesGlobalPosition(CapsuleCollider capsuleCollider, int pCorners)
        {
            Vector3 capsuleLocalScale = capsuleCollider.transform.localScale;
            Vector3 capsuleCCPos = capsuleCollider.bounds.center;
            float radius;

            radius = 0.125f + capsuleCollider.radius * capsuleCollider.transform.localScale.x;

            if (capsuleLocalScale.y > capsuleLocalScale.x && capsuleLocalScale.y > capsuleLocalScale.z)
                radius = 0.125f + capsuleCollider.radius * capsuleLocalScale.y;
            if (capsuleLocalScale.z > capsuleLocalScale.x && capsuleLocalScale.z > capsuleLocalScale.y)
                radius = 0.125f + capsuleCollider.radius * capsuleLocalScale.z;

            float angleDeg;
            float radians = capsuleCollider.transform.eulerAngles.z * Mathf.Deg2Rad;
            int direction = capsuleCollider.direction;

            pCorners += 2;
            if (pCorners % 2 != 0)
                pCorners += 1;

            Vector2[] polygonPoints = new Vector2[pCorners];
            int half = pCorners / 2;
            float offset;
            float edgeDeg = 360f / pCorners;

            offset = ((capsuleCollider.height * capsuleCollider.transform.localScale.y) - (2 * radius)) / 2f + 0.1f;

            for (int i = 0; i < pCorners; i++)
            {
                angleDeg = (360f / pCorners) * i;
                angleDeg += edgeDeg / 2f;
                radians = angleDeg * Mathf.Deg2Rad;

                polygonPoints[i] = new Vector2(capsuleCCPos.x + radius * Mathf.Cos(radians), capsuleCCPos.y + (i < half ? offset : -offset) + radius * Mathf.Sin(radians));

                if (direction == 1)
                    radians = capsuleCollider.transform.eulerAngles.z * Mathf.Deg2Rad;
                else
                    radians = (capsuleCollider.transform.eulerAngles.z + 90f) * Mathf.Deg2Rad;

                polygonPoints[i] = new Vector2((polygonPoints[i].x - capsuleCCPos.x) * Mathf.Cos(radians) - (polygonPoints[i].y - capsuleCCPos.y) * Mathf.Sin(radians) + capsuleCCPos.x, (polygonPoints[i].x - capsuleCCPos.x) * Mathf.Sin(radians) + (polygonPoints[i].y - capsuleCCPos.y) * Mathf.Cos(radians) + capsuleCCPos.y);
            }

            return polygonPoints;
        }

        /// <summary>
        /// Applies a buoyant force, drag and lift to an object submerged in the water.
        /// </summary>
        /// <param name="cIndex">The index of a collider in the floatingObjects2D or floatingObjects3D list.</param>
        /// <param name="intersectionPolygon">List of a polygon vertices.</param>
        /// <param name="a2DCollider">Is this a 2D collider?.</param>
        /// <remarks>
        /// Based on:
        /// http://www.iforce2d.net/b2dtut/buoyancy
        /// </remarks>
        private void ApplyPhysicsForces(int index, Vector2[] intersectionPolygon, bool a2DCollider)
        {
            forcePosition = GetPolygonAreaAndCentroid(intersectionPolygon, out area);
            displacedMass = area * waterDensity;

            // The buoyant force direction is always opposite of the direction of the gravity force.
            upLift = -Physics2D.gravity * displacedMass;

            if (a2DCollider)
            {
                if (floatingObjects2D[index].tag == "Player")
                    upLift *= playerBuoyantForceScale;
            }
            else
            {
                if (floatingObjects3D[index].tag == "Player")
                    upLift *= playerBuoyantForceScale;
            }

            if (a2DCollider)
                floatingObjects2D[index].GetComponent<Rigidbody2D>().AddForceAtPosition(upLift, forcePosition);
            else
                floatingObjects3D[index].GetComponent<Rigidbody>().AddForceAtPosition(upLift, forcePosition);


            //// Uncomment this lines to see where the buoyant force is applied.
            //if (showForces)
            //{
            //    // The buoyant force.   
            //    Debug.DrawRay(forcePosition, upLift.normalized, Color.grey);
            //}

            // Here a drag and lift is applied to the object.
            // To simulate a realistic drag and lift we must find the leading edges of the polygon. 
            // These are the edges that resist the movement of an object in the water.
            for (int k = 0; k < intersectionPolygon.Length; k++)
            {
                // 2 vertices that form a polygon edge.
                Vector2 vertex0 = intersectionPolygon[k];
                Vector2 vertex1 = intersectionPolygon[(k + 1) % intersectionPolygon.Length];

                // A point in the middle of the line created by the 2 vertices.
                Vector2 midPoint = 0.5f * (vertex0 + vertex1);

                // The velocity direction at the midPoint position for the current object.
                Vector3 velDir;
                if (a2DCollider)
                    velDir = floatingObjects2D[index].GetComponent<Rigidbody2D>().GetPointVelocity(midPoint);
                else
                    velDir = floatingObjects3D[index].GetComponent<Rigidbody>().GetPointVelocity(midPoint);

                // The magnitude of the velocity.
                float vel = velDir.magnitude;

                // We normalize the velocity direction vector so that we can use it in the future calculations.
                // After normalization the vector velDir will have the same direction, but it's magnitude will be 1. 
                velDir.Normalize();

                // The difference between the two vertices. This is the vector that describes the edge
                // formed by the two vertices.
                Vector2 edge = vertex1 - vertex0;
                // The lenght of the edge vector.
                float edgeLength = edge.magnitude;
                // Edge vector normalization.
                edge.Normalize();

                // Performing the cross product between the value of -1 and the normalized edge vector we get its normal.
                Vector2 normal = Cross(-1, edge);

                // The dot product between the normal of the normalized edge vector and the normalized velocity 
                // vector will tell us if the curent edge is a leading edge.
                float dragDot = Vector2.Dot(normal, velDir);

                if (waterFlow)
                {
                    float angleInRad = 0;

                    if (useAngles)
                    {
                        angleInRad = flowAngle * Mathf.Deg2Rad;
                    }
                    else
                    {
                        switch (flowDirection)
                        {
                            case Water2D_FlowDirection.Up: angleInRad = 270f * Mathf.Deg2Rad;
                                break;
                            case Water2D_FlowDirection.Down: angleInRad = 90f * Mathf.Deg2Rad;
                                break;
                            case Water2D_FlowDirection.Left: angleInRad = 0 * Mathf.Deg2Rad;
                                break;
                            case Water2D_FlowDirection.Right: angleInRad = 180f * Mathf.Deg2Rad;
                                break;
                        }
                    }

                    Vector2 forceDir = new Vector2(Mathf.Cos(angleInRad), Mathf.Sin(angleInRad));

                    float flowDot = Vector2.Dot(normal, forceDir);

                    if (flowDot < 0)
                    {
                        float flowMag = flowDot * edgeLength * waterFlowForce;
                        Vector2 force = flowMag * forceDir;

                        if (a2DCollider)
                            floatingObjects2D[index].GetComponent<Rigidbody2D>().AddForceAtPosition(force, midPoint);
                        else
                            floatingObjects3D[index].GetComponent<Rigidbody>().AddForceAtPosition(force, midPoint);

                        if (showForces)
                        {
                            // The velocity direction.   
                            Debug.DrawRay(midPoint, forceDir, Color.green);
                            // The normal of a polygon edge.
                            Debug.DrawRay(midPoint, normal, Color.white);
                        }
                    }
                }

                // If the dragDot is greater then 0 than the normal of the edge vector has the same direction as the 
                // velDir vector so it is not a leading edge.
                if (dragDot > 0)
                    continue;

                // The magnitude of the drag for the current edge.
                float dragMag = dragDot * dragCoefficient * edgeLength * waterDensity * vel * vel;
                // If the dragMag is greater then the max drag that should be applied to an object, the
                // value of dragMag is set to maxDrag value.
                dragMag = Mathf.Min(dragMag, maxDrag);
                // We multiply the dragMag with the normalized velocity direction vector (velDir) to get the drag 
                // force that should be applied to the current edge.
                Vector2 dragForce = dragMag * velDir;

                // The drag force is applied at midPoint position.
                if (a2DCollider)
                    floatingObjects2D[index].GetComponent<Rigidbody2D>().AddForceAtPosition(dragForce, midPoint);
                else
                    floatingObjects3D[index].GetComponent<Rigidbody>().AddForceAtPosition(dragForce, midPoint);

                // Now we apply lift to the current edge.
                // Lift is the force applied perpendicular to the direction of movement.

                float liftDot = Vector2.Dot(edge, velDir);
                // The magnitude of the lift for the current edge.
                float liftMag = dragDot * liftDot * liftCoefficient * edgeLength * waterDensity * vel * vel;
                liftMag = Mathf.Min(liftMag, maxLift);
                // We perform the cross product between the value of 1 and the normalized velocity
                // vector to get a vector perpendicular to the velocity vector. That is the lift direction vector.
                Vector2 liftDir = Cross(1, velDir);
                // We multiply the liftMag with the normalized velocity direction vector to get the lift force that should be applied to the current edge.
                Vector2 liftForce = liftMag * liftDir;

                // The lift force is applied at midPoint position.
                if (a2DCollider)
                    floatingObjects2D[index].GetComponent<Rigidbody2D>().AddForceAtPosition(liftForce, midPoint);
                else
                    floatingObjects3D[index].GetComponent<Rigidbody>().AddForceAtPosition(liftForce, midPoint);


                // Shows in the Scene View the direction of the forces that make an object float in the water.
                if (showForces)
                {
                    // The velocity direction.   
                    Debug.DrawRay(midPoint, velDir, Color.green);
                    // The Lift direction.
                    Debug.DrawRay(midPoint, liftDir, Color.blue);
                    // The drag direction.
                    Debug.DrawRay(midPoint, -velDir, Color.red);
                    // The normal of a polygon edge.
                    Debug.DrawRay(midPoint, normal, Color.white);
                }
            }

            // Shows the shape of the polygon that is below the waterline.
            if (showPolygon && intersectionPolygon.Length > 2)
            {
                // Draws lines for all polygon edges.
                for (int j = 0; j < intersectionPolygon.Length; j++)
                {
                    if (j < intersectionPolygon.Length - 1)
                        Debug.DrawLine(intersectionPolygon[j], intersectionPolygon[j + 1], Color.blue);
                    else
                        Debug.DrawLine(intersectionPolygon[j], intersectionPolygon[0], Color.blue);

                }
            }
        }

        /// <summary>
        /// Calculates the area of a polygon and its centroid. The vertices order must be counterclockwise.
        /// </summary>
        /// <param name="polygonVertices">The polygon for which to calculate the area and the centroid.</param>
        /// <param name="polygonArea">Link to a variable.</param>
        /// <returns>Returns the polygon centroid.</returns>
        public Vector2 GetPolygonAreaAndCentroid(Vector2[] polygonVertices, out float polygonArea)
        {
            int count = polygonVertices.Length;
            Vector2 centroidPos = new Vector2(0, 0);
            polygonArea = 0;

            for (int i = 0; i < count; i++)
            {
                Vector2 vert1 = polygonVertices[i];
                Vector2 vert2 = i + 1 < count ? polygonVertices[i + 1] : polygonVertices[0];

                float rArea = (vert1.x * vert2.y) - (vert1.y * vert2.x);
                float triangleArea = 0.5f * rArea;

                polygonArea += triangleArea;

                float cX = (vert1.x + vert2.x) * rArea;
                float cY = (vert1.y + vert2.y) * rArea;

                centroidPos += new Vector2(cX, cY);
            }

            centroidPos *= (1f / (6f * polygonArea));

            if (polygonArea < 0)
                polygonArea = 0;

            return centroidPos;
        }

        /// <summary>
        /// Calculates the area of a polygon. The vertices order must be counterclockwise.
        /// </summary>
        /// <param name="polygonVertices">Polygon vertices.</param>
        /// <returns>Returns the polygon area.</returns>
        public float GetPolygonArea(Vector2[] polygonVertices)
        {
            int count = polygonVertices.Length;
            float polygonArea = 0;

            for (int i = 0; i < count; i++)
            {
                Vector2 vert1 = polygonVertices[i];
                Vector2 vert2 = i + 1 < count ? polygonVertices[i + 1] : polygonVertices[0];

                float rArea = (vert1.x * vert2.y) - (vert1.y * vert2.x);
                float triangleArea = 0.5f * rArea;

                polygonArea += triangleArea;
            }

            if (polygonArea < 0)
                polygonArea = 0;

            return polygonArea;
        }

        /// <summary>
        /// Applies an upward force, a simple drag and torque on the objects that are in the water.
        /// </summary>
        private void LinearBuoyantForce()
        {
            // The global position of the left handle.
            leftHandleGlobalPos = transform.TransformPoint(water2D.handlesPosition[2]);

            if (!water2D.use3DCollider)
            {
                int len = floatingObjects2D.Count;
                for (int i = 0; i < len; i++)
                {
                    ApplyLinearBuoyantForce(i, true);
                }
            }
            else
            {
                int len = floatingObjects3D.Count;
                for (int i = 0; i < len; i++)
                {
                    ApplyLinearBuoyantForce(i, false);
                }
            }
        }

        /// <summary>
        /// Applies an upward force, a simple drag and torque on the objects that are in the water.
        /// </summary>
        /// <param name="cIndex">The index of a collider in the floatingObjects2D or floatingObjects3D list.</param>
        /// <param name="a2DCollider">Is this a 2D collider?.</param>
        /// <remarks>
        /// Based on:
        /// https://www.youtube.com/watch?v=mDtnT5fh7Ek
        /// </remarks>
        private void ApplyLinearBuoyantForce(int cIndex, bool a2DCollider)
        {
            // The global position of the closest vertex to the center of the the object. 
            Vector3 globalPosOfVert;

            if (a2DCollider)
                forcePosition = floatingObjects2D[cIndex].bounds.center + floatingObjects2D[cIndex].transform.TransformDirection(forcePositionOffset);
            else
                forcePosition = floatingObjects3D[cIndex].bounds.center + floatingObjects3D[cIndex].transform.TransformDirection(forcePositionOffset);

            // The distance from the left handle to the center of the colider.
            float distance;
            if (a2DCollider)
                distance = Mathf.Abs(leftHandleGlobalPos.x - floatingObjects2D[cIndex].bounds.center.x);
            else
                distance = Mathf.Abs(leftHandleGlobalPos.x - floatingObjects3D[cIndex].bounds.center.x);
            // The index of the surface vertex that is closest to the colliders bounding box center.
            int index = (int)Mathf.Floor(distance / (1f / water2D.segmentsPerUnit));

            // We make sure that we don't get out of bounds indexes.
            if (index > surfaceVertsCount - 1)
                index = surfaceVertsCount - 1;

            globalPosOfVert = transform.TransformPoint(vertices[index + surfaceVertsCount]);

            // The idea here is that we want the object to be in a state of equilibrium at the surface of the water.
            // To achieve this, for an object with the mass of 1kg we must apply a force equal to the force of gravity, 
            // but in the opposite direction. Applying a constant force will not produce a realistic simulation. 
            // so we apply a force based on the position of the object relative the global position of the vertex.
            forceFactor = 1f - ((forcePosition.y - globalPosOfVert.y) / floatHeight);

            // A negative force factor will push the object downwards, something we don't want to happen as gravity does that for us.
            if (forceFactor > 0f && ((a2DCollider ? floatingObjects2D[cIndex].GetComponent<Collider2D>().bounds.min.y : floatingObjects3D[cIndex].GetComponent<Collider>().bounds.min.y) < globalPosOfVert.y))
            {
                // This is the linear buoyant force that is applied to an object.
                if (a2DCollider)
                    upLift = -Physics2D.gravity * (forceFactor - floatingObjects2D[cIndex].GetComponent<Rigidbody2D>().velocity.y * bounceDamping) * forceScale;
                else
                    upLift = -Physics2D.gravity * (forceFactor - floatingObjects3D[cIndex].GetComponent<Rigidbody>().velocity.y * bounceDamping) * forceScale;

                // If the collider has the tag "Player" the force is scaled based on the value of playerBuoyantForceScale.
                if ((a2DCollider ? floatingObjects2D[cIndex].GetComponent<Collider2D>().tag : floatingObjects3D[cIndex].GetComponent<Collider>().tag) == "Player")
                    upLift *= playerBuoyantForceScale;

                // Applies a force that slows the vertical descent of an object and pushes it upwards.
                if (a2DCollider)
                    floatingObjects2D[cIndex].GetComponent<Rigidbody2D>().AddForceAtPosition(upLift, forcePosition);
                else
                    floatingObjects3D[cIndex].GetComponent<Rigidbody>().AddForceAtPosition(upLift, forcePosition);

                if (waterFlow)
                {
                    float angleInRad = 0;

                    if (useAngles)
                    {
                        angleInRad = flowAngle * Mathf.Deg2Rad;
                    }
                    else
                    {
                        switch (flowDirection)
                        {
                            case Water2D_FlowDirection.Up: angleInRad = 90f * Mathf.Deg2Rad;
                                break;
                            case Water2D_FlowDirection.Down: angleInRad = 270f * Mathf.Deg2Rad;
                                break;
                            case Water2D_FlowDirection.Left: angleInRad = 180 * Mathf.Deg2Rad;
                                break;
                            case Water2D_FlowDirection.Right: angleInRad = 0f * Mathf.Deg2Rad;
                                break;
                        }
                    }

                    Vector2 forceDir = new Vector2(Mathf.Cos(angleInRad), Mathf.Sin(angleInRad));
                    Vector2 force = waterFlowForce * forceDir;

                    if (a2DCollider)
                        floatingObjects2D[cIndex].GetComponent<Rigidbody2D>().AddForceAtPosition(force, forcePosition);
                    else
                        floatingObjects3D[cIndex].GetComponent<Rigidbody>().AddForceAtPosition(force, forcePosition);
                }

                // The velocity of the object at the center of the bounding box.
                Vector3 velDir;
                if (a2DCollider)
                    velDir = floatingObjects2D[cIndex].GetComponent<Rigidbody2D>().GetPointVelocity(floatingObjects2D[cIndex].bounds.center);
                else
                    velDir = floatingObjects3D[cIndex].GetComponent<Rigidbody>().GetPointVelocity(floatingObjects3D[cIndex].bounds.center);
                // Velocity magnitude.
                float vel = velDir.magnitude;
                // The velocity direction vector is normalized so that we can use it to calculate the drag magnitude.
                velDir.Normalize();
                // The magnitude of the drag.
                float dragMag = liniarBFDragCoefficient * vel * vel;
                // The drag force direction must be opposite of direction of movement.
                Vector2 dragForce = dragMag * -velDir;

                // A drag is applied in the oposite direction of the objects movement.
                if (a2DCollider)
                    floatingObjects2D[cIndex].GetComponent<Rigidbody2D>().AddForceAtPosition(dragForce, floatingObjects2D[cIndex].bounds.center);
                else
                    floatingObjects3D[cIndex].GetComponent<Rigidbody>().AddForceAtPosition(dragForce, floatingObjects3D[cIndex].bounds.center);

                // A torque is applied to stop the rotation of the object.
                float angularDrag = liniarBFAbgularDragCoefficient * -(a2DCollider ? floatingObjects2D[cIndex].GetComponent<Rigidbody2D>().angularVelocity : floatingObjects3D[cIndex].GetComponent<Rigidbody>().angularVelocity.z);
                if (a2DCollider)
                    floatingObjects2D[cIndex].GetComponent<Rigidbody2D>().AddTorque(angularDrag);
                else
                    floatingObjects3D[cIndex].GetComponent<Rigidbody>().AddTorque(0, angularDrag, 0, ForceMode.Force);
            }
        }

        /// <summary>
        /// Updates different variables after the water mesh was recreated from scratch.
        /// </summary>
        private void UpdateVariables()
        {
            vertices = meshFilter.vertices;
            //surfaceVertsCount = vertices.Length / 2;
            surfaceVertsCount = water2D.surfaceVertsCount / 2;
            UVs = meshFilter.uv;

            waterLinePreviousLocalPos = water2D.handlesPosition[1].y + defaultWaterHeight + waterLineYPosOffset;
            waterLineCurrentGlobalPos = transform.TransformPoint(vertices[surfaceVertsCount + 1]);
            waterLineCurrentLocalPos = water2D.handlesPosition[1].y + defaultWaterHeight + waterLineYPosOffset;

            // If the number of surface vertices changed the lists are updated.
            if (vertYOffsets.Length != vertices.Length)
            {
                // If the total number of vertices increased new values are added to the list so we don't get out of bounds errors.
                if (vertYOffsets.Length < vertices.Length)
                {
                    int vertD = vertices.Length - vertYOffsets.Length;

                    for (int i = 0; i < vertD; i++)
                    {
                        velocities.Add(0.0f);
                        accelerations.Add(0.0f);
                        leftDeltas.Add(0.0f);
                        rightDeltas.Add(0.0f);
                        sineY.Add(0.0f);

                        if (particleSystemInstantiation == Water2D_ParticleSystem.PerVertex && i < vertD - 1)
                            foundObjOnPrevFrame.Add(false);
                    }
                }

                // If the total number of vertices decreased, the list values that no longer reference to a surface vertex are deleted.
                if (vertYOffsets.Length > vertices.Length)
                {
                    int vertD = vertYOffsets.Length - vertices.Length;

                    for (int i = 0; i < vertD; i++)
                    {
                        int last = velocities.Count - 1;

                        velocities.RemoveAt(last);
                        accelerations.RemoveAt(last);
                        leftDeltas.RemoveAt(last);
                        rightDeltas.RemoveAt(last);
                        sineY.RemoveAt(last);

                        if (particleSystemInstantiation == Water2D_ParticleSystem.PerVertex && i < vertD - 1)
                            foundObjOnPrevFrame.RemoveAt(last);
                    }
                }
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!floatingObjects2D.Contains(other))
            {
                // This makes sure only the first collider with the "Player" tag 
                // that is detected is added to the floatingObjects list.
                if (other.tag == "Player" && onTriggerPlayerDetected)
                    return;

                floatingObjects2D.Add(other);
                tempObj2D.Add(other);

                if (other.tag == "Player" && !onTriggerPlayerDetected)
                    onTriggerPlayerDetected = true;
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (floatingObjects2D.Contains(other))
            {
                if (other.tag == "Player")
                    onTriggerPlayerDetected = false;

                floatingObjects2D.Remove(other);

                if (tempObj2D.Contains(other))
                    tempObj2D.Remove(other);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (!floatingObjects3D.Contains(other))
            {
                // This makes sure only the first collider with the "Player" tag 
                // that is detected is added to the floatingObjects list.
                if (other.tag == "Player" && onTriggerPlayerDetected)
                    return;

                floatingObjects3D.Add(other);
                tempObj3D.Add(other);

                if (other.tag == "Player" && !onTriggerPlayerDetected)
                    onTriggerPlayerDetected = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (floatingObjects3D.Contains(other))
            {
                if (other.tag == "Player")
                    onTriggerPlayerDetected = false;

                floatingObjects3D.Remove(other);

                if (tempObj3D.Contains(other))
                    tempObj3D.Remove(other);
            }
        }
        #endregion
    }
}
