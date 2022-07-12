using UnityEngine;

public class PlayerPartsParticlesController : MonoBehaviour
{
    [SerializeField] private GameObject _target;

    private ParticleSystem _partSystem;
    private ParticleSystem.ShapeModule _partSystemShape;

    private float _distance;

    private delegate void ParticlesActive(Transform target);
    private ParticlesActive ParticlesAct;

    private void OnEnable()
    {
        _partSystem = GetComponentInChildren<ParticleSystem>();
        
        SuperMagnetPunch.ActivateMagnetingParticles += PlayerAssigning;
    }

    private void OnDestroy()
    {
        SuperMagnetPunch.ActivateMagnetingParticles -= PlayerAssigning;
    }

    private void PlayerAssigning(GameObject player)
    {
        if (player != null)
        {
            _target = player;
            ParticlesAct = ChasingPlayer;
        }
        else
        {
            ParticlesAct = null;
        }
    }

    void Update()
    {
        ParticlesAct?.Invoke(_target.transform);
    }

    private void ChasingPlayer(Transform target)
    {
        _partSystemShape = _partSystem.shape;
        
        _distance = (target.position - transform.position).magnitude;
        _partSystemShape.length = _distance;
    }
}
