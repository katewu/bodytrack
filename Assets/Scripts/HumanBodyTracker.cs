using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class HumanBodyTracker : MonoBehaviour
{

        [SerializeField]
        [Tooltip("The Skeleton prefab to be controlled.")]
        GameObject m_SkeletonPrefab;

        [SerializeField]
        [Tooltip("The ARHumanBodyManager which will produce body tracking events.")]
        ARHumanBodyManager m_HumanBodyManager;

    private List<Vector3> LeftHandPos;
    private List<Vector3> RightHandPos;

    private void Start()
    {
        LeftHandPos = new List<Vector3>();
        RightHandPos = new List<Vector3>();
    }

    /// <summary>
    /// Get/Set the <c>ARHumanBodyManager</c>.
    /// </summary>
    public ARHumanBodyManager humanBodyManager
        {
            get { return m_HumanBodyManager; }
            set { m_HumanBodyManager = value; }
        }

        /// <summary>
        /// Get/Set the skeleton prefab.
        /// </summary>
        public GameObject skeletonPrefab
        {
            get { return m_SkeletonPrefab; }
            set { m_SkeletonPrefab = value; }
        }

        Dictionary<TrackableId, BoneController> m_SkeletonTracker = new Dictionary<TrackableId, BoneController>();

        void OnEnable()
        {
            Debug.Assert(m_HumanBodyManager != null, "Human body manager is required.");
            m_HumanBodyManager.humanBodiesChanged += OnHumanBodiesChanged;
        }

        void OnDisable()
        {
            if (m_HumanBodyManager != null)
                m_HumanBodyManager.humanBodiesChanged -= OnHumanBodiesChanged;
        }

        void OnHumanBodiesChanged(ARHumanBodiesChangedEventArgs eventArgs)
        {
            BoneController boneController;

            foreach (var humanBody in eventArgs.added)
            {
                if (!m_SkeletonTracker.TryGetValue(humanBody.trackableId, out boneController))
                {
                    Debug.Log($"Adding a new skeleton [{humanBody.trackableId}].");
                    var newSkeletonGO = Instantiate(m_SkeletonPrefab, humanBody.transform);
                    boneController = newSkeletonGO.GetComponent<BoneController>();
                    m_SkeletonTracker.Add(humanBody.trackableId, boneController);
                }

                boneController.InitializeSkeletonJoints();
                boneController.ApplyBodyPose(humanBody);
            }

            foreach (var humanBody in eventArgs.updated)
            {
                if (m_SkeletonTracker.TryGetValue(humanBody.trackableId, out boneController))
                {
                    boneController.ApplyBodyPose(humanBody);

                    LeftHandPos.Add(humanBody.joints[27].anchorPose.position);
                    RightHandPos.Add(humanBody.joints[71].anchorPose.position);

                    print("LeftHand Mean: " + Mean(MinimumPos(LeftHandPos, 8)) + " SD: " + StandardDeviation(MinimumPos(LeftHandPos, 8)));
                    print("RightHand Mean: " + Mean(MinimumPos(RightHandPos, 8)) + " SD: " + StandardDeviation(MinimumPos(RightHandPos, 8)));
                }
            }

            foreach (var humanBody in eventArgs.removed)
            {
                Debug.Log($"Removing a skeleton [{humanBody.trackableId}].");
                if (m_SkeletonTracker.TryGetValue(humanBody.trackableId, out boneController))
                {
                    Destroy(boneController.gameObject);
                    m_SkeletonTracker.Remove(humanBody.trackableId);
                }
            }
        }

    private List<Vector3> MinimumPos(List<Vector3> coords, int maxNum)
    {
        List<Vector3> mins = new List<Vector3>();
        int i = coords.Count - 2;

        while(mins.Count < maxNum && i > 0)
        {
            if(coords[i+1].y > coords[i].y && coords[i-1].y > coords[i].y)
            {
                mins.Add(coords[i]);
            }
            i--;
        }

        return mins;
    }

    private Vector3 Mean(List<Vector3> coords)
    {
        Vector3 sum = Vector3.zero;
        foreach(Vector3 pos in coords)
        {
            sum += pos;
        }

        int n = coords.Count;
        return new Vector3(sum.x/n, sum.y/n, sum.x/n);
    }

    private Vector3 StandardDeviation(List<Vector3> coords)
    {
        Vector3 mean = Mean(coords);
        Vector3 sum = Vector3.zero;

        foreach(Vector3 pos in coords)
        {
            sum.x = Mathf.Pow(pos.x - mean.x, 2);
            sum.y = Mathf.Pow(pos.y - mean.y, 2);
            sum.z = Mathf.Pow(pos.z - mean.z, 2);
        }

        int n = coords.Count;

        sum.x /= n;
        sum.y /= n;
        sum.z /= n;

        sum.x = Mathf.Sqrt(sum.x);
        sum.y = Mathf.Sqrt(sum.y);
        sum.z = Mathf.Sqrt(sum.z);

        return sum;
    }

   }

