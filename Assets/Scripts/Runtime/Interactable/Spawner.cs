using Assets.Scripts.Runtime.Character;
using Assets.Scripts.Runtime.ScriptableObjects;
using Assets.Scripts.Runtime.UI;
using TMPro;
using UnityEngine;

public class MinionSpawner : MonoBehaviour
{
    [SerializeField] private MeshRenderer   _litMesh;
    [SerializeField] private TextMeshProUGUI _textLabel;
    [SerializeField] private MinionType _minionType;
    [SerializeField] private MinonConfigurationData _minionsConfiguration;

    private MinionSpawnerConfigEntry _thisSpawnerConfig;


    private static Mesh capsuleMesh;

    public void Awake()
    {
        _thisSpawnerConfig = _minionsConfiguration.SpawnerConfig.Find(c => c.Type == _minionType);
        _textLabel.text = _thisSpawnerConfig.Cost.ToString();
    }

    public void Interact(Hero h)
    {
        var minionCost = _thisSpawnerConfig.Cost;
        if (minionCost > h.GetGoldAmount()) {
            HUD.Instance.ShowNotEnoughCash(minionCost);
            AudioManager.Instance.PlayFailSound();
            return;
        }
        if (!h.canGetAnotherMinion())
        {
            HUD.Instance.ShowMinionsMax(h);
            AudioManager.Instance.PlayFailSound();
            return;
        }
        h.TryPayGoldAmount(minionCost);
        var m = Instantiate<Minion>(_thisSpawnerConfig.Prefab, getSpawnPosition(), Quaternion.identity);
        h.addMinion(m);
        AudioManager.Instance.PlayMinionSummoned(m);
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

    private void OnValidate()
    {
        var config = _minionsConfiguration.SpawnerConfig.Find(c => c.Type == _minionType);
        if (config == null)
        {
            Debug.LogError("Spawner has invalid minion type", this);
        }
        else
        {
            Material material = config.notLitMaterial;
            var materials = new System.Collections.Generic.List<Material>() { material };
            _litMesh.SetMaterials(materials);
        }
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
