using System;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Image _mainBar;
    [SerializeField] private Image _yellowBar;
    [SerializeField] private Transform _cam;
    [SerializeField] private Health _healthGameObject;
    
    [SerializeField] private float _loweringRatio;


    private Text _countHP;
    
    private string _currentTag;

    
    public static Action<string> SMBDead;
    private void Start()
    {
        Health.Damaging += TakeDamage;
        _countHP = GetComponentInChildren<Text>();
        
        _currentTag = _healthGameObject.gameObject.tag;
        _countHP.text = _healthGameObject.health.ToString();
    }

    private void Update()
    {
        gameObject.transform.LookAt(_cam); // направляем hp bar в сторону камеры
        _yellowBar.fillAmount = Mathf.Lerp(_yellowBar.fillAmount, _mainBar.fillAmount, Time.deltaTime * _loweringRatio); // лерпим желтую полоску урона
    }
    
    public void TakeDamage()
    {
        if (_healthGameObject.health <= 0)
        {
           // _healthGameObject.gameObject.GetComponent<StickmanCore>().Dead();
            //_mainControl.enabled = false;
            SMBDead.Invoke(_currentTag);
            _countHP.text = "0";
            _mainBar.fillAmount = 0;      // тут не до конца наладил систему смерти и завершения игры, так как понял, что это не требуется?
        }
        else
        {
            _countHP.text = _healthGameObject.health.ToString();  // применяем к hp полученный урон
            _mainBar.fillAmount = _healthGameObject.ConvertHP();  // тут мы конвертируем хп в соответсвии с уровнем ( не реализованно )
        }
    }
}
