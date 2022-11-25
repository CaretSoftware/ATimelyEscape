using System.Collections;
using UnityEngine;

public class TravelPathScript : MonoBehaviour
{
    [SerializeField] private float delay = 5f;
    [SerializeField] private GameObject objectToTravel;

    private LineRenderer line;
    private float travelTime;
    private float startTime;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        travelTime = delay / line.positionCount;
    }

    public void TravelPath(int path)
    {
        StartCoroutine(TravelPathCoroutine(path));
    }

    private int current, next;
    private IEnumerator TravelPathCoroutine(int path)
    {
        yield return new WaitForSeconds(delay);

        current = path == 0 ? 0 : line.positionCount - 1;
        next = path == 0 ? current + 1 : current - 1;
        startTime = Time.time;
        while (Time.time - startTime <= travelTime)
        {
            if (path == 0 && next < line.positionCount)
                transform.position = Vector3.Lerp(line.GetPosition(current++), line.GetPosition(next++), Time.time - startTime);
            else if(path != 0 && next >= 0)
                transform.position = Vector3.Lerp(line.GetPosition(current--), line.GetPosition(next--), Time.time - startTime);
            yield return 1;
        }
    }
}
