using Assets.Scripts.Runtime.Character;
using UnityEditor;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] public Minion minionPrefab;
    [SerializeField] public MeshRenderer litMesh;
    [SerializeField] public Material notLitMaterial;
    [SerializeField] public Material litMaterial;

    static Mesh capsuleMesh;

    public void Interact(Hero h)
    {
        if (h.canGetAnotherMinion()) {
            var m = Instantiate<Minion>(minionPrefab, getSpawnPosition(), Quaternion.identity);
            h.addMinion(m);
        }
    }

    internal void SetSelected(bool selected)
    {
        Material material = selected ? litMaterial : notLitMaterial;
        var materials = new System.Collections.Generic.List<Material>() { material };
        litMesh.SetMaterials(materials);
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
