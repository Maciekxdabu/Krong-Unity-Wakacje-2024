using Assets.Scripts.Runtime.Character;
using Assets.Scripts.Runtime.ScriptableObjects;
using UnityEngine;

public class MinionSpawner : MonoBehaviour
{
    [SerializeField] private MeshRenderer   _litMesh;
    [SerializeField] private MinionType _minionType;
    [SerializeField] private MinonConfigurationData _minionsConfiguration;

    private MinionSpawnerConfigEntry _thisSpawnerConfig;


    private static Mesh capsuleMesh;

    public void Awake()
    {
        _thisSpawnerConfig = _minionsConfiguration.SpawnerConfig.Find(c => c.Type == _minionType);
    }

    public void Interact(Hero h)
    {
        if (h.canGetAnotherMinion()) {
            var m = Instantiate<Minion>(_thisSpawnerConfig.Prefab, getSpawnPosition(), Quaternion.identity);
            h.addMinion(m);
        }
    }

    internal void SetSelected(bool selected)
    {
        Material material = selected ? _thisSpawnerConfig.litMaterial : _thisSpawnerConfig.notLitMaterial;
        var materials = new System.Collections.Generic.List<Material>() { material };
        _litMesh.SetMaterials(materials);
    }

    private Vector3 getSpawnPosition()
    {
        return transform.position + transform.forward * -1.0f;
    }

    private void OnDrawGizmos()
    {
        if (capsuleMesh == null)
        {
            capsuleMesh = Resources.GetBuiltinResource<Mesh>("Capsule.fbx");
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireMesh(capsuleMesh, getSpawnPosition(), Quaternion.identity, 0.25f * Vector3.one);
    }
}
