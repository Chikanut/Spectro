using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishTrigger : LightTriggerListener
{
    [System.Serializable]
    public new class Settings : PoolObjectInfo
    {
        public int NextSceneIndex;
    }

    [SerializeField] private MeshRenderer _mesh;
    [SerializeField] private float _churgeTime;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _churgedColor;
    [SerializeField] private Settings _settings;
    
    private Sequence _color;

    void Start()
    {
        Disactivate();
    }

    public override void Activate()
    {
        _color?.Kill();
        _color = DOTween.Sequence();

        _color.Append(_mesh.material.DOColor(_churgedColor,"_EmissionColor", _churgeTime)).onComplete = OnFinished;
    }

    public override void Disactivate()
    {
        _color?.Kill();
        _color = DOTween.Sequence();

        _color.Append(_mesh.material.DOColor(_defaultColor,"_EmissionColor", 0.5f));
    }
    
    void OnFinished()
    {
        SceneManager.LoadScene(_settings.NextSceneIndex);
    }
    
    public override string SerializeSettings()
    {
        GetDefaultInfo(_settings);

        return Helpers.XMLHelper.Serialize(_settings);
    }

    public override void AcceptSettings(string info)
    {
        _settings = Helpers.XMLHelper.Deserialize<Settings>(info);

        AcceptTransformInfo(_settings);
    }
}
