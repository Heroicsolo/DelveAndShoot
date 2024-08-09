using UnityEngine;

public class VectorUtils
{
    public static float DistanceZX(Vector3 from, Vector3 to)
    {
        from.y = to.y;
        return Vector3.Distance(from, to);
    }

    public static Vector3 GetRandomVector(Vector3 minPosition, Vector3 maxPosition)
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(minPosition.x, maxPosition.x),
            Random.Range(minPosition.y, maxPosition.y),
            Random.Range(minPosition.z, maxPosition.z)
        );
        
        return randomPosition;
    }

    public static Vector3 ClosestPointOnMeshOBB(MeshFilter meshFilter, Vector3 worldPoint)
    {
        // First, we transform the point into the local coordinate space of the mesh.
        var localPoint = meshFilter.transform.InverseTransformPoint(worldPoint);

        // Next, we compare it against the mesh's axis-aligned bounds in its local space.
        var localClosest = meshFilter.sharedMesh.bounds.ClosestPoint(localPoint);

        // Finally, we transform the local point back into world space.
        return meshFilter.transform.TransformPoint(localClosest);
    }

    public static bool IsObjectInCone(Transform targetObj, Transform lookFromTransform, float coneAngle)
    {
        Vector3 dir = (targetObj.position - lookFromTransform.position).normalized;

        Vector3 localDir = lookFromTransform.InverseTransformDirection(dir);

        if (localDir.z > 0f && Mathf.Atan2(localDir.z, localDir.x) < coneAngle)
        {
            return true;
        }

        return false;
    }
}
