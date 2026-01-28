using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic; 

public class WeaponSelector:MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown dropdown;
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TextMeshProUGUI weaponName;
    [SerializeField]
    private WeaponDB weaponDB;

    public void ChangeWeapon(int index)
    {
        if (weaponDB == null)
        {
            return;
        }
        
        bool isValid = index >= 0 && index < weaponDB.weaponDatas.Length;
        icon.sprite = isValid? weaponDB.weaponDatas[index].icon : null;
        weaponName.text = isValid? weaponDB.weaponDatas[index].weaponName : "(≡^ ^≡)";
        GraphEditorManager.Instance.graphData.weapon = index;
    }

    public void OnChangeWeapon()
    {
        ChangeWeapon(dropdown.value);
    }

    private void Init()
    {
        if (weaponDB == null)
        {
            Debug.LogError("データベース入れて");
            return;
        }

        dropdown.options.Clear();
        List<TMP_Dropdown.OptionData> options = new() { };
        foreach (var weapon in weaponDB.weaponDatas)
        {
            string weaponLabel = weapon?.weaponName;
            Sprite iconSprite = weapon?.icon? weapon.icon: null;

            options.Add(new TMP_Dropdown.OptionData(weaponLabel, iconSprite, Color.white));
        }
        dropdown.AddOptions(options);
        ChangeWeapon(GraphEditorManager.Instance.graphData.weapon);
    }

    public void Start()
    {
        Init();
        GraphEditorManager.Instance.onLoardGraph += Init;
    }
}