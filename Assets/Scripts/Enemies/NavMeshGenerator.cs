using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.AI.Navigation;

public class NavMeshGenerator : MonoBehaviour
{
    private NavMeshSurface navMeshSurface;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        GenerateNavMesh();
    }

    void GenerateNavMesh()
    {
        // Check if there is already a NavMeshSurface component
        navMeshSurface = GetComponent<NavMeshSurface>();
        if (navMeshSurface == null)
        {
            navMeshSurface = gameObject.AddComponent<NavMeshSurface>();
        }

        // Find all objects with the "Ground" tag
        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        GameObject[] groundObjects = GameObject.FindGameObjectsWithTag("Ground");

        foreach (GameObject ground in groundObjects)
        {
            MeshFilter meshFilter = ground.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                NavMeshBuildSource source = new NavMeshBuildSource
                {
                    shape = NavMeshBuildSourceShape.Mesh,
                    sourceObject = meshFilter.sharedMesh,
                    transform = ground.transform.localToWorldMatrix,
                    area = 0
                };
                sources.Add(source);
            }
        }

        // Build the NavMesh
        navMeshSurface.collectObjects = CollectObjects.Volume;
        navMeshSurface.overrideTileSize = true;
        navMeshSurface.tileSize = 256;
        navMeshSurface.overrideVoxelSize = true;
        navMeshSurface.voxelSize = 0.1f;
        navMeshSurface.BuildNavMesh();
    }
}
