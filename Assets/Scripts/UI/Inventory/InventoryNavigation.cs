using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryNavigation : MonoBehaviour
{
    private InventoryManager _manager;
    private XPSystem _xpSys;
    private CurMenu _curMenu = CurMenu.Inventory;

    [SerializeField]
    [Header("Level Up Slots")]
    private List<Image> _levelUpSlots;

    [SerializeField] private Color lvlDefaultColor;
    [SerializeField] private Color lvlSelectedColor;
    
    
    private int _lvlUpCurSlot;
    private enum CurMenu
    {
        LevelUpMenu,
        Inventory,
        BackToMenuButton
    }

    [Header("Back To Menu Slots")] [SerializeField]
    private Image backToMenuImage;
    
    [SerializeField] private Color backToMenuDefaultColor;
    [SerializeField] private Color backToMenuSelectedColor;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Action()
    {
        if (_curMenu == CurMenu.Inventory)
        {
            _manager.ItemAction();
        }
        else if (_curMenu == CurMenu.BackToMenuButton)
        {
            // Exit game
            SceneManager.LoadScene("MainMenu");
        }
        else if (_curMenu == CurMenu.LevelUpMenu)
        {
            _xpSys.LevelUpStat(_xpSys.StatOrderLvlUp[_lvlUpCurSlot]);
        }
    }

    public void SetXpSystem(XPSystem sys)
    {
        _xpSys = sys;
    }
    
    public void SetManager(InventoryManager mng)
    {
        _manager = mng;
    }

    public void MoveCursor(bool left, bool right, bool up, bool down)
    {
        switch (_curMenu)
        {
            case CurMenu.Inventory:
                MoveCursorInv(left, right, up, down);
                break;
            case CurMenu.LevelUpMenu:
                MoveInLevelUp(left, right, up, down);
                UpdateCursorLvlUp();
                break;
            case CurMenu.BackToMenuButton:
                MoveInBackToMenuSlot(left, right, up, down);
                UpdateBackToMenu();
                break;
        }

        if (_curMenu == CurMenu.LevelUpMenu)
        {
            UpdateCursorLvlUp();
        }

        if (_curMenu == CurMenu.BackToMenuButton)
        {
            UpdateBackToMenu();
        }
    }
    
    private void MoveCursorInv(bool left, bool right, bool up, bool down)
    {
        int row = _manager.Equipped ? _manager.EquipmentSlotsPerRow : _manager.SlotsPerRow;
        int total = _manager.Equipped ? _manager.EquipmentDisplays.Count : _manager.InventorySlot;

        if (left)
        {
            _manager.CurrentSlot -= 1;
            if (_manager.CurrentSlot < 0 || _manager.CurrentSlot % row == row - 1)
            {
                if (_xpSys.LevelUpsRemaining > 0)
                {
                    _manager.CurrentSlot += 1;
                    _curMenu = CurMenu.LevelUpMenu;
                }
                else
                    _manager.CurrentSlot += row;
            }
        }

        if (right)
        {
            _manager.CurrentSlot += 1;
            if (_manager.CurrentSlot % row == 0)
            {
                Debug.Log(_xpSys.LevelUpsRemaining);
                if (_xpSys.LevelUpsRemaining > 0)
                {
                    _manager.CurrentSlot -= 1;
                    _curMenu = CurMenu.LevelUpMenu;
                }
                else
                {
                    _manager.CurrentSlot -= row;
                }
            }
        }

        if (up)
        {
            _manager.CurrentSlot -= row;
            if (_manager.CurrentSlot < 0)
            {
                _manager.CurrentSlot += row;
                _curMenu = CurMenu.BackToMenuButton;
            }
        }

        if (down)
        {
            _manager.CurrentSlot = (_manager.CurrentSlot + row) % total;
        }

        if (left || right || up || down)
        {
            UpdateCursorInv();
        }
    }

    public void MoveInLevelUp(bool left, bool right, bool up, bool down)
    {
        if (up)
        {
            _lvlUpCurSlot -= 1;
            if (_lvlUpCurSlot < 0)
            {
                _lvlUpCurSlot = _levelUpSlots.Count - 1;
            }
        }
        if (down)
        {
            _lvlUpCurSlot += 1;
            _lvlUpCurSlot %= _levelUpSlots.Count;
        }
        if (right)
        {
            _curMenu = CurMenu.Inventory;
            _manager.CurrentSlot -= _manager.CurrentSlot % _manager.SlotsPerRow;
            UpdateCursorInv();
        }
        if (left)
        {
            _curMenu = CurMenu.Inventory;
            _manager.CurrentSlot -= _manager.CurrentSlot % _manager.SlotsPerRow;
            _manager.CurrentSlot += _manager.SlotsPerRow - 1;
            UpdateCursorInv();
        }
    }

    private void MoveInBackToMenuSlot(bool left, bool right, bool up, bool down)
    {
        if (down)
        {
            _curMenu = CurMenu.Inventory;
        }
        if (up)
        {
            _curMenu = CurMenu.Inventory;
            
            _manager.CurrentSlot = _manager.CurrentSlot % _manager.SlotsPerRow + _manager.InventorySlot - _manager.SlotsPerRow;
        }
    }

    private void UpdateCursorInv()
    {
        _manager.UpdateCursorPosition();
        _manager.UpdateItemDisplay();
    }

    private void UpdateCursorLvlUp()
    {
        SelectLevelUpSlot();
    }

    private void UpdateBackToMenu()
    {
        if (_curMenu == CurMenu.BackToMenuButton)
        {
            backToMenuImage.color = backToMenuSelectedColor;
        }
        else
        {
            backToMenuImage.color = backToMenuDefaultColor;
        }
            
    }
    
    private void SelectLevelUpSlot()
    {
        for (int i = 0; i < _levelUpSlots.Count; i++)
        {
            if (_lvlUpCurSlot == i && _curMenu == CurMenu.LevelUpMenu)
            {
                _levelUpSlots[i].color = lvlSelectedColor;
            }
            else
            {
                _levelUpSlots[i].color = lvlDefaultColor;
            }
        }
    }
}
