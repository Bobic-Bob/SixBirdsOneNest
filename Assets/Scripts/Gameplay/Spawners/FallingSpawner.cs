using UnityEngine;

public class FallingSpawner : Spawner
{
    protected FallingSpawner(float respawnTime, float currentRespawnTime) : base(respawnTime, currentRespawnTime){}

    protected override void Start()
    {
        AdaptivePosition();
        base.Start();
    }

    private void AdaptivePosition()
    {
        Vector3 newSpawnPos = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth * 0.5f, Camera.main.pixelHeight + 100f));
        gameObject.transform.position = newSpawnPos;
    }
}