using System;
using UnityEngine;

public class UiNotify : MonoBehaviour
{
    [SerializeField] private GameObject _dangerZone;

    [SerializeField] private float _correctionAngle;

    private SpriteRenderer _spriteRenderer;
    void Start()
    {
        SuperJump.UiDangerZone += SetDangerZone;
        SuperPunch.UiDangerZone += SetDangerZone;
        _spriteRenderer = _dangerZone.GetComponent<SpriteRenderer>();
    }
    

    private void SetDangerZone(bool enable, float angle)
    {
        _spriteRenderer.material.SetFloat("_FieldOfView", angle + _correctionAngle);  // тут мы получаем угол для зоны действия суперсилы и включаем/выключаем ее
        _dangerZone.SetActive(enable);
    }
}
