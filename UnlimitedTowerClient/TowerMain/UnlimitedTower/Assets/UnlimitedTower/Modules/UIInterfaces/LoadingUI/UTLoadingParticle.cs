using System.Collections;
using UnityEngine;

public class UTLoadingParticle : MonoBehaviour {

    [Header("Particles")]
    public GameObject[] particlePrefabs;

    public void Begin()
    {
        StartCoroutine(KeepGenerateParticle());
    }

    public void End()
    {
        foreach (Transform child in transform)
            child.GetComponent<MoveGameObject>()?.End();
        StopAllCoroutines();
    }

    IEnumerator KeepGenerateParticle()
    {
        //if prefab cnt is less than 1, will not work
        while (particlePrefabs.Length > 0)
        {
            int radIdx = Random.Range(0, particlePrefabs.Length);

            var go = Instantiate(particlePrefabs[radIdx], transform);

            go.transform.position =
                transform.position +
                Vector3.right * Random.Range(-150, 150) +
                Vector3.up * Random.Range(-150, 150);

            yield return new WaitForSecondsRealtime(0.2f);
        }
    }
}
