using System.Collections;
using UnityEngine;

public class LaserEmitter : MonoBehaviour
{
    [SerializeField] private Transform otherEmitter;
    [SerializeField] private float startDelay = 0f;
    [SerializeField] private float onDuration = 2f;
    [SerializeField] private float offDuration = 1f;
    [SerializeField] private LaserBeam laserBeam;

    private void Start()
    {
        StartCoroutine(LaserCycle());
    }

    private IEnumerator LaserCycle()
    {
        yield return new WaitForSeconds(startDelay);
        while (true)
        {
            laserBeam.Activate(transform.position, otherEmitter.position);
            yield return new WaitForSeconds(onDuration);

            laserBeam.Deactivate();
            yield return new WaitForSeconds(offDuration);
        }
    }
}
