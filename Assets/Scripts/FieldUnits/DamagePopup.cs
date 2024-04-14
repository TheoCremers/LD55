using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro _tmp;
    private float _lifetime = 0.5f;
    private Color _fontColor;
    private static int _sortOrder;
    public float spawnOffset = 0.5f;
    public float floatSpeed = 0.6f;
    void Awake()
    {
        _tmp = transform.GetComponent<TextMeshPro>();
    }
    
    public static void Create(Transform target, int amount, bool isHealing = false)
    {
        var instance = Instantiate(GameAssets.Instance.DamagePopup, 
            target.position, Quaternion.identity);
        var dmgPopup = instance.GetComponent<DamagePopup>();
        dmgPopup.Initialize(amount, isHealing);
    }

    private void Initialize(int amount, bool isHealing)
    {
        _tmp.SetText(amount.ToString());
        if (isHealing)
        {
            _tmp.color = Color.green;
        }

        transform.position += new Vector3(0, spawnOffset); // zzz
        _fontColor = _tmp.color;
        _sortOrder++;
        _tmp.sortingOrder = _sortOrder;
    }
    // Update is called once per frame
    private void Update()
    {
        transform.position += new Vector3(0, floatSpeed) * Time.deltaTime;
        _lifetime -= Time.deltaTime;
        if (_lifetime < 0)
        {
            _fontColor.a -= 2f * Time.deltaTime;
            _tmp.color = _fontColor;
            if (_fontColor.a < 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
