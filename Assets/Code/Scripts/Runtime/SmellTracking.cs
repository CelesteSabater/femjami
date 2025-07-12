using UnityEngine;
using StarterAssets;
using System.Linq;
using System.Collections;
using UnityEngine.AI;

public class SmellTracking : MonoBehaviour
{
    public Transform[] targets;
    public float distanceBetweenParticles = 1.0f;
    public float timeBetweenInstantiates = 1.0f;
    public float multiplierToDestroy = 10;
    private float timeCounter = 0;
    public GameObject smellParticles;

    void Update()
    {
        Smell();
    }

    private void Smell()
    {
        if (timeCounter > 0)
        {
            timeCounter -= Time.deltaTime;
            return;
        }

        if (!StarterAssetsInputs.Instance.smell) return;

        for (int i = 0; i < targets.Count(); i++)
        {
            if (targets[i] != null)
                GenerateSmeellPath(targets[i]);
        }
            

        timeCounter = timeBetweenInstantiates;
    }

    private void GenerateSmeellPath(Transform target)
    {
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
        {
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Vector3 start = path.corners[i];
                Vector3 end = path.corners[i + 1];
                float segmentDistance = Vector3.Distance(start, end);
                int particlesInSegment = (int)(segmentDistance / distanceBetweenParticles);

                for (int j = 0; j < particlesInSegment; j++)
                {
                    Vector3 point = Vector3.Lerp(start, end, j / (float)particlesInSegment);
                    GameObject go = Instantiate(smellParticles, point, Quaternion.identity);
                    go.transform.LookAt(target);
                    StartCoroutine(Destroy(go, timeBetweenInstantiates * multiplierToDestroy));
                }
            }
        }
    }

    IEnumerator Destroy(GameObject go, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(go);
    }
}
