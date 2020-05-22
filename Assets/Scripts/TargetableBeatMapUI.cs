using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Beat Map UI class responsible for tracking a target on the screen, also responsible for displaying / updating beat sample UI
/// </summary>
public class TargetableBeatMapUI : MonoBehaviour
{

    [SerializeField] UICircle uiCircle;

    [SerializeField] TargetableBeatSampleUI targetBeatSampleUIPrefab;

    LockableTarget lockOnTarget;
    private RectTransform rectTransform;
    private Animator UIAnimator;
    [SerializeField] Canvas renderCanvas;

    [Range(0.15f,1.0f)]
    [SerializeField] float maxCanvasSizeRatio = 0.5f;

    [Header("Beat Sync Color Settings")]
    [SerializeField] Color perfectColor;
    [SerializeField] Color goodColor;
    [SerializeField] Color okColor;
    [SerializeField] Color missColor;
    [SerializeField] Color neutralColor;


    private float maxRadiusSize = float.MaxValue;
    private float lastScreenSizeOnRadiusUpdate;
    private void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
        UIAnimator = this.GetComponent<Animator>();
        maxRadiusSize = maxCanvasSizeRatio * (renderCanvas.pixelRect.width / 2.0f);
        lastScreenSizeOnRadiusUpdate = renderCanvas.pixelRect.width;
    }

    private void Start()
    {
        if(BeatSyncReceiver.BeatReceiver != null)
        {
            BeatSyncReceiver.BeatReceiver.QueuedBeatToSync += OnBeatQeueud;
        }
    }

    private void OnDestroy()
    {
        if (BeatSyncReceiver.BeatReceiver != null)
        {
            BeatSyncReceiver.BeatReceiver.QueuedBeatToSync -= OnBeatQeueud;
        }
    }

    /// <summary>
    /// Method called when a beat is queued by the beat sync receiver
    /// </summary>
    /// <param name="beatQueued"></param>
    private void OnBeatQeueud(BeatSyncData beatQueued)
    {
        // Get a beat sample UI for the next beat sync queued
        TargetableBeatSampleUI prefabCreated = GetNextBeatSampleObject();
        // Animate the beat sample to the beat data
        prefabCreated.AnimateBeatData(beatQueued, missColor);
    }

    /// <summary>
    /// Whether or not the target was visible in the last frame
    /// </summary>
    private bool wasVisibleLastFrame = false;


    // Update is called once per frame
    void LateUpdate()
    {
        // If there is no target to lock onto, then make this beat sync UI invisible
        if (lockOnTarget == null)
        {
            TargetUIVisible(false);
            wasVisibleLastFrame = false;
            return;
        }

        // If the target was visible in the last frame, reposition this beat UI at the target's position in UI space
        if (wasVisibleLastFrame)
        {
            // Get the viewport position of the target
            Vector3 newScreenPosition = PlayerController.PlayerCamera.WorldToViewportPoint(lockOnTarget.TargetRenderer.bounds.center);
            // Reposition this beat UI to the viewport position on the target
            float xPosition = newScreenPosition.x * renderCanvas.pixelRect.width;
            float yPosition = newScreenPosition.y * renderCanvas.pixelRect.height;
            newScreenPosition = new Vector3(xPosition, yPosition, 0.0f);
            rectTransform.anchoredPosition = newScreenPosition;
        }
        
        // If the target is within the camera's view, then get the size of the object in screen space (pixel space)
        if (IsTargetWithinCameraView())
        {
            Vector2 sizeInPixels = GetObjectSizeInPixels();
            // Cut the size of the object in pixels in half
            // We need to do this because the circle script is scaled by radius (half the sized of a circle)
            float suggestedRadius = sizeInPixels.magnitude / 2.0f;
            UpdateMaxRadiusIfRequired();
            // If the suggested radius size is smaller than the max radius size, then
            // use the suggested radius size
            if (suggestedRadius < maxRadiusSize)  uiCircle.radius = suggestedRadius;
            // Render this UI visible
            TargetUIVisible(true);
            // The target is visible this frame, store this variable for the next frame
            wasVisibleLastFrame = true;
        }
        else
        {
            // If the target is not visible, then toggle off this UI
            TargetUIVisible(false);
            // The target is not visible in this frame, store this variable for the next frame
            wasVisibleLastFrame = false;
        }
    }

    /// <summary>
    /// Sets this targetable BeatSyncUI to follow an object's renderer
    /// </summary>
    /// <param name="targetToLockOnto"></param>
    public void SetLockableTarget(LockableTarget targetToLockOnto)
    {
        if(lockOnTarget != null)
        {
            lockOnTarget.AssignTargetableBeatMap(null);
        }

        // Set the lock on target renderer
        lockOnTarget = targetToLockOnto;
        
        
        // If the target is null, then turn off this UI
        if (lockOnTarget == null)
        {
            TargetUIVisible(false);
        }
        else
        {
            lockOnTarget.AssignTargetableBeatMap(this);
            // Reset the "wasVisibleLastFrame"
            wasVisibleLastFrame = IsTargetWithinCameraView();
        }
    }

    /// <summary>
    /// Method called to toggle the beat Map UI
    /// </summary>
    /// <param name="visible"></param>
    private void TargetUIVisible(bool visible)
    {
        if(UIAnimator != null) UIAnimator.SetBool("visible", visible);
    }


    Vector3 lockOnTargetBoundsCenter;
    Vector3 frontFaceBottomRightPos;
    Vector3 frontFaceTopRightPos;
    Vector3 frontFaceBottomLeftPos;
    Vector3 frontFaceTopLeftPos;
    Vector3 backFaceBottomRightPos;
    Vector3 backFaceTopRightPos;
    Vector3 backFaceBottomLeftPos;
    Vector3 backFaceTopLeftPos;

    /// <summary>
    /// Returns the size of the target renderer in pixels on the screen
    /// </summary>
    /// <returns></returns>
    private Vector3 GetObjectSizeInPixels()
    {
        lockOnTargetBoundsCenter = lockOnTarget.TargetRenderer.bounds.center;

        frontFaceBottomRightPos = lockOnTargetBoundsCenter;
        frontFaceBottomRightPos.x += lockOnTarget.TargetRenderer.bounds.extents.x;
        frontFaceBottomRightPos.z += lockOnTarget.TargetRenderer.bounds.extents.z;
        frontFaceBottomRightPos.y -= lockOnTarget.TargetRenderer.bounds.extents.y;
        //frontFaceBottomRightPos = lockOnTarget.transform.TransformPoint(frontFaceBottomRightPos);

        frontFaceTopRightPos = lockOnTargetBoundsCenter;
        frontFaceTopRightPos.x += lockOnTarget.TargetRenderer.bounds.extents.x;
        frontFaceTopRightPos.z += lockOnTarget.TargetRenderer.bounds.extents.z;
        frontFaceTopRightPos.y += lockOnTarget.TargetRenderer.bounds.extents.y;
        //frontFaceTopRightPos = lockOnTarget.transform.TransformPoint(frontFaceTopRightPos);

        frontFaceBottomLeftPos = lockOnTargetBoundsCenter;
        frontFaceBottomLeftPos.x -= lockOnTarget.TargetRenderer.bounds.extents.x;
        frontFaceBottomLeftPos.z += lockOnTarget.TargetRenderer.bounds.extents.z;
        frontFaceBottomLeftPos.y -= lockOnTarget.TargetRenderer.bounds.extents.y;
        //frontFaceBottomLeftPos = lockOnTarget.transform.TransformPoint(frontFaceBottomLeftPos);

        frontFaceTopLeftPos = lockOnTargetBoundsCenter;
        frontFaceTopLeftPos.x -= lockOnTarget.TargetRenderer.bounds.extents.x;
        frontFaceTopLeftPos.z += lockOnTarget.TargetRenderer.bounds.extents.z;
        frontFaceTopLeftPos.y += lockOnTarget.TargetRenderer.bounds.extents.y;
        //frontFaceTopLeftPos = lockOnTarget.transform.TransformPoint(frontFaceTopLeftPos);

        backFaceBottomRightPos = lockOnTargetBoundsCenter;
        backFaceBottomRightPos.x += lockOnTarget.TargetRenderer.bounds.extents.x;
        backFaceBottomRightPos.z -= lockOnTarget.TargetRenderer.bounds.extents.z;
        backFaceBottomRightPos.y -= lockOnTarget.TargetRenderer.bounds.extents.y;
        //backFaceBottomRightPos = lockOnTarget.transform.TransformPoint(backFaceBottomRightPos);

        backFaceTopRightPos = lockOnTargetBoundsCenter;
        backFaceTopRightPos.x += lockOnTarget.TargetRenderer.bounds.extents.x;
        backFaceTopRightPos.z -= lockOnTarget.TargetRenderer.bounds.extents.z;
        backFaceTopRightPos.y += lockOnTarget.TargetRenderer.bounds.extents.y;
        //backFaceTopRightPos = lockOnTarget.transform.TransformPoint(backFaceTopRightPos);

        backFaceBottomLeftPos = lockOnTargetBoundsCenter;
        backFaceBottomLeftPos.x -= lockOnTarget.TargetRenderer.bounds.extents.x;
        backFaceBottomLeftPos.z -= lockOnTarget.TargetRenderer.bounds.extents.z;
        backFaceBottomLeftPos.y -= lockOnTarget.TargetRenderer.bounds.extents.y;
        //backFaceBottomLeftPos = lockOnTarget.transform.TransformPoint(backFaceBottomLeftPos);

        backFaceTopLeftPos = lockOnTargetBoundsCenter;
        backFaceTopLeftPos.x -= lockOnTarget.TargetRenderer.bounds.extents.x;
        backFaceTopLeftPos.z -= lockOnTarget.TargetRenderer.bounds.extents.z;
        backFaceTopLeftPos.y += lockOnTarget.TargetRenderer.bounds.extents.y;
        //backFaceTopLeftPos = lockOnTarget.transform.TransformPoint(backFaceTopLeftPos);

        frontFaceBottomRightPos = PlayerController.PlayerCamera.WorldToScreenPoint(frontFaceBottomRightPos);
        frontFaceBottomLeftPos = PlayerController.PlayerCamera.WorldToScreenPoint(frontFaceBottomLeftPos);
        frontFaceTopLeftPos = PlayerController.PlayerCamera.WorldToScreenPoint(frontFaceTopLeftPos);
        frontFaceTopRightPos = PlayerController.PlayerCamera.WorldToScreenPoint(frontFaceTopRightPos);

        backFaceBottomRightPos = PlayerController.PlayerCamera.WorldToScreenPoint(backFaceBottomRightPos);
        backFaceBottomLeftPos = PlayerController.PlayerCamera.WorldToScreenPoint(backFaceBottomLeftPos);
        backFaceTopRightPos = PlayerController.PlayerCamera.WorldToScreenPoint(backFaceTopRightPos);
        backFaceTopLeftPos = PlayerController.PlayerCamera.WorldToScreenPoint(backFaceTopLeftPos);



        Vector3[] screenPoints = { frontFaceBottomRightPos , frontFaceBottomLeftPos , frontFaceTopLeftPos , frontFaceTopRightPos ,
        backFaceBottomRightPos, backFaceBottomLeftPos, backFaceTopRightPos, backFaceTopLeftPos};

        float width = CalculateWidth(screenPoints);
        float height = CalculateHeight(screenPoints);

        //Debug.Log($"New Size In Pixels = w: {width}, h: {height} ");
        return new Vector3(width, height, 0.0f);
    }

    /// <summary>
    /// Calculates the height of a renderer's volume in screen space
    /// </summary>
    /// <param name="vectors"></param>
    /// <returns></returns>
    float CalculateHeight(Vector3[] vectors)
    {
        float yMin = float.MaxValue;
        float yMax = float.MinValue;

        foreach (Vector3 vector in vectors)
        {
            yMin = Mathf.Min(yMin, vector.y);
            yMax = Mathf.Max(yMax, vector.y);
        }

        return yMax - yMin;
    }

    /// <summary>
    /// Calculates the width of a renderer's volume in screen space
    /// </summary>
    /// <param name="vectors"></param>
    /// <returns></returns>
    float CalculateWidth(Vector3[] vectors)
    {
        float xMin = float.MaxValue;
        float xMax = float.MinValue;

        foreach (Vector3 vector in vectors)
        {
            xMin = Mathf.Min(xMin, vector.x);
            xMax = Mathf.Max(xMax, vector.x);
        }

        return xMax - xMin;
    }

    /// <summary>
    /// Returns whether or not the target renderer is visible to a given camera
    /// </summary>
    /// <returns></returns>
    private bool IsTargetWithinCameraView()
    {
        return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(PlayerController.PlayerCamera), lockOnTarget.TargetRenderer.bounds);
    }


    /// <summary>
    /// Our beat sample UI object pool queue
    /// </summary>
    private Queue<TargetableBeatSampleUI> beatSampleObjectPool = new Queue<TargetableBeatSampleUI>();

    /// <summary>
    /// Returns a beat sample UI sample (this uses an object pooling method)
    /// </summary>
    /// <returns></returns>
    private TargetableBeatSampleUI GetNextBeatSampleObject()
    {
        // Cache the next beat sample to be returned
        TargetableBeatSampleUI nextBeatSample;
        // If there's a beat sample in our object pool queue, then use it
        if (beatSampleObjectPool.Count > 0)
        {
            // Dequeue the object from our pool
            nextBeatSample = beatSampleObjectPool.Dequeue();
            // Set the color to its neutral color
            nextBeatSample.SetBeatSampleColor(neutralColor);
            // Add it to our list of beat samples playing
            beatSamplesPlaying.Add(nextBeatSample);
            // Return it
            return nextBeatSample;
            
        }
        else
        {
            // Instantaite a beat sample prefab
            nextBeatSample = Instantiate(targetBeatSampleUIPrefab, targetBeatSampleUIPrefab.transform.position, targetBeatSampleUIPrefab.transform.rotation, transform);
            // Set the beat sample UI to the neutral color
            nextBeatSample.SetBeatSampleColor(neutralColor);
            // Set its target parent reference to this
            nextBeatSample.SetBeatSampleTargetParent(this);
            // Add it to our list of beat samples playing
            beatSamplesPlaying.Add(nextBeatSample);
            // Return it
            return nextBeatSample;
        }
    }

    

    /// <summary>
    /// List of currently playing beat samples
    /// </summary>
    List<TargetableBeatSampleUI> beatSamplesPlaying = new List<TargetableBeatSampleUI>();

    /// <summary>
    /// Updates a displayed beat sample with an input synchronization type
    /// </summary>
    /// <param name="beatData">The beat sync data</param>
    /// <param name="syncType">The beat input sync type</param>
    public void UpdateDisplayedBeatSample(BeatSyncData beatData, BeatInputSync syncType)
    {
        // Loop through our beat samples playing
        foreach (TargetableBeatSampleUI playingSample in beatSamplesPlaying)
        {
            // Check if the beat sample isn't null
            if(playingSample != null)
            {
                // Check if the beat sample UI is playing the beat data we're looking for
                // If it is, set it's color displayed to the respective color assigned
                if (playingSample.PlayingBeatSync == beatData)
                {

                    if(syncType == BeatInputSync.PERFECT)
                    {
                        playingSample.SetBeatSampleColor(perfectColor);
                    }
                    else if(syncType == BeatInputSync.GOOD)
                    {
                        playingSample.SetBeatSampleColor(goodColor);
                    }
                    else if (syncType == BeatInputSync.OK)
                    {
                        playingSample.SetBeatSampleColor(okColor);
                    }

                    break;
                }
            }
        }
    }

    /// <summary>
    /// Enqueues a beat sample UI to the beat sample object pool
    /// </summary>
    /// <param name="beatSampleToQueue"></param>
    public void EnqueueBeatSample(TargetableBeatSampleUI beatSampleToQueue)
    {
        // If the beat to queue isn't null, then pool it
        if(beatSampleToQueue != null)
        {
            // If the beat is in our list of beat samples playing, then remove it from our list
            if (beatSamplesPlaying.Contains(beatSampleToQueue)) beatSamplesPlaying.Remove(beatSampleToQueue);
            // Enqueue the beat UI sample
            beatSampleObjectPool.Enqueue(beatSampleToQueue);
        }
    }

    private void UpdateMaxRadiusIfRequired()
    {
        if(lastScreenSizeOnRadiusUpdate != renderCanvas.pixelRect.width)
        {
            maxRadiusSize = maxCanvasSizeRatio * (renderCanvas.pixelRect.width / 2.0f);
            lastScreenSizeOnRadiusUpdate = renderCanvas.pixelRect.width;
        }
    }

}
