using UnityEngine;

public class BirdSpawner : Spawner
{
    private ScalesModel _scales;
    protected BirdSpawner(float respawnTime, float currentRespawnTime) : base(respawnTime, currentRespawnTime) { }

    protected override void Start()
    {
        _scales = GetComponentInParent<ScalesModel>();
        base.Start();
    }

    protected override void Respawn()
    {
        if (CanSpawn())
        {
            SpawnFirstPrefabOnly();
            _scales.MassUpdated();
            CurrentRespawnTime = RespawnTime;
        }
        else if (!HasChild() && CurrentRespawnTime > 0)
        {
            CurrentRespawnTime -= Time.deltaTime;
        }
    }
}
