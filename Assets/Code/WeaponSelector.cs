using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelector : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown dropdown;
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TextMeshProUGUI weaponName;
    [SerializeField]
    private WeaponDB weaponDB;
    [SerializeField]
    private List<int> indexes = new() { };
    public void ChangeWeapon(int index)
    {
        if (weaponDB == null)
        {
            return;
        }
        if(indexes.Count <= index)
        {
            index = 0;
        }
        if(dropdown.value != index)
        {
            dropdown.value = index;
        }
        index = indexes[index];
        bool isValid = index >= 0 && index < weaponDB.weaponDatas.Length;
        icon.sprite = isValid ? weaponDB.weaponDatas[index].icon : null;
        weaponName.text = isValid ? weaponDB.weaponDatas[index].weaponName : "(≡^ ^≡)";
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
        indexes = new();
        for (int i = 0; i < weaponDB.weaponDatas.Length; ++i)
        {
            var weapon = weaponDB.weaponDatas[i];
            bool isValid = weapon.usableStage.reverseStages.Contains(StageSelector.sceneName);
            bool isDefault = weapon.usableStage.defaultUsable;
            if (!(isValid ^ isDefault))
            {
                continue;
            }
            indexes.Add(i);
            string weaponLabel = weapon?.weaponName;
            Sprite iconSprite = weapon?.icon ? weapon.icon : null;

            options.Add(new TMP_Dropdown.OptionData(weaponLabel, iconSprite, Color.white));
        }
        dropdown.AddOptions(options);
        ChangeWeapon(GraphEditorManager.Instance.graphData.weapon);
    }

    public void Start()
    {
        Init();
        GraphEditorManager.Instance.onLoardGraph += Init;
        ChangeWeapon(0);
    }
}