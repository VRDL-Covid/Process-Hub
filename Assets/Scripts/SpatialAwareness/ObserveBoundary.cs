using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

public class ObserveBoundary : MonoBehaviour
{
    SurfaceObserver surfaceObserver;
    [Tooltip("Sets boundary for volume to observe, \rNOTE: the larger it is the higher the impact on framerate")]
    public Vector3 extents = new Vector3(10, 10, 10);
    Dictionary<SurfaceId, GameObject> spatialMeshObjects = new Dictionary<SurfaceId, GameObject>();
    // Start is called before the first frame update
    void Start()
    {
#if WINDOWS_UWP
        surfaceObserver = new SurfaceObserver();
        surfaceObserver.SetVolumeAsAxisAlignedBox(Vector3.zero, extents);
        StartCoroutine(UpdateLoop());
#endif
    }

        // Update is called once per frame
        void Update()
    {
        
    }

    IEnumerator UpdateLoop()
    {
        var wait = new WaitForSeconds(2.5f);
        while (true)
        {
            surfaceObserver.Update(OnSurfaceChanged);
            yield return wait;
        }
    }


    private void OnSurfaceChanged(SurfaceId surfaceId, SurfaceChange changeType, Bounds bounds, System.DateTime updateTime)
    {
        switch (changeType)
        {
            case SurfaceChange.Added:
            case SurfaceChange.Updated:
                if (!spatialMeshObjects.ContainsKey(surfaceId))
                {
                    spatialMeshObjects[surfaceId] = new GameObject("spatial-mapping-" + surfaceId);
                    spatialMeshObjects[surfaceId].transform.parent = this.transform;
                    spatialMeshObjects[surfaceId].AddComponent<MeshRenderer>();
                }
                GameObject target = spatialMeshObjects[surfaceId];
                SurfaceData sd = new SurfaceData(
                    //the surface id returned from the system
                    surfaceId,
                    //the mesh filter that is populated with the spatial mapping data for this mesh
                    target.GetComponent<MeshFilter>() ?? target.AddComponent<MeshFilter>(),
                    //the world anchor used to position the spatial mapping mesh in the world
                    target.GetComponent<WorldAnchor>() ?? target.AddComponent<WorldAnchor>(),
                    //the mesh collider that is populated with collider data for this mesh, if true is passed to bakeMeshes below
                    target.GetComponent<MeshCollider>() ?? target.AddComponent<MeshCollider>(),
                    //triangles per cubic meter requested for this mesh
                    1000,
                    //bakeMeshes - if true, the mesh collider is populated, if false, the mesh collider is empty.
                    true
                    );

                // TODO: 
                //SurfaceObserver.RequestMeshAsync(sd, OnDataReady);
                break;
            case SurfaceChange.Removed:
                var obj = spatialMeshObjects[surfaceId];
                spatialMeshObjects.Remove(surfaceId);
                if (obj != null)
                {
                    GameObject.Destroy(obj);
                }
                break;
            default:
                break;
        }

        void OnDataReady(SurfaceData sd, bool outputWritten, float elapsedBakeTimeSeconds)
        {
            if (outputWritten)
            {
                // Request completed successfully
            }
            else
            {
                // Request has failed.
            }
        }
    }
}

