using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(StatsDictionary))]
[CustomPropertyDrawer(typeof(ItemPickupDictionary))]
public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }

[Serializable]
public struct InventoryData
{
    public SerializableDictionary<string, uint> Consumables;
    public SerializableDictionary<EquipmentSlot, List<string>> Equipment;
    public SerializableDictionary<EquipmentSlot, string> Equipped;
}


public static class GameData
{
    private static List<string> _pickupsPicked = new List<string>();
    public static List<string> PickupsPicked
    {
        get => _pickupsPicked;
        set => _pickupsPicked = value;
    }
    
    private static List<string> _pressurePlatesActivated = new List<string>();
    public static List<string> PressurePlatesActivated
    {
        get => _pressurePlatesActivated;
        set => _pressurePlatesActivated = value;
    }

    public static int LevelNumber = 1;
    public static int CheckpointNumber = 1;
    private static InventoryData _inventoryData = new InventoryData()
    {
        Consumables = new SerializableDictionary<string, uint>(),
        Equipment = new SerializableDictionary<EquipmentSlot, List<string>>(),
        Equipped = new SerializableDictionary<EquipmentSlot, string>()
    };

    public static InventoryData InventoryData
    {
        get => _inventoryData;
        set => _inventoryData = value;
    }

    private static void GetInventoryData()
    {
        InventoryData = new InventoryData()
        {
            Consumables = new SerializableDictionary<string, uint>(),
            Equipment = new SerializableDictionary<EquipmentSlot, List<string>>(),
            Equipped = new SerializableDictionary<EquipmentSlot, string>()
        };
        var newConsumables = new SerializableDictionary<string, uint>();
        var newEquipment = new SerializableDictionary<EquipmentSlot, List<string>>();
        var newEquipped = new SerializableDictionary<EquipmentSlot, string>();

        var inventory = GameObject.FindWithTag("Player").GetComponent<Inventory>();
        if (inventory == null) return;

        foreach (var consumable in inventory.Consumables)
        {
            newConsumables.Add(consumable.Key.name, consumable.Value);
        }
        
        foreach (var equipment in inventory.Equipment)
        {
            var piecesList = new List<string>();
            foreach (var equipmentPiece in equipment.Value)
            {
                piecesList.Add(equipmentPiece.name);
            }
            newEquipment.Add(equipment.Key, piecesList);
        }

        foreach (var equipped in inventory.Equipped)
        {
            if (equipped.Value != null)
                newEquipped.Add(equipped.Key, equipped.Value.name);
        }
        
        InventoryData = new InventoryData()
        {
            Consumables = newConsumables,
            Equipment = newEquipment,
            Equipped = newEquipped
        };

    }

    public static bool InCheckpoint = false;
    
    public static void AddPickUp(GameObject pickupObject)
    {
        PickupsPicked = new List<string> (PickupsPicked) {GameObjectToHash(pickupObject)};
    }

    public static void SetCheckpoint(int checkpointNumber)
    {
        CheckpointNumber = checkpointNumber;
    }

    public static void AddActivatedPressurePlate(PressurePlate pressurePlate)
    {
        PressurePlatesActivated = new List<string> (PressurePlatesActivated) 
            {GameObjectToHash(pressurePlate.gameObject)};
    }

    public static string GameObjectToHash(GameObject gameObject)
    {
        var hash = new Hash128();
        hash.Append(gameObject.name);
        hash.Append(gameObject.tag);

        var position = gameObject.transform.position;
        hash.Append(position.x);
        hash.Append(position.y);
        hash.Append(position.z);

        var rotation = gameObject.transform.rotation;
        hash.Append(rotation.x);
        hash.Append(rotation.y);
        hash.Append(rotation.z);

        var localScale = gameObject.transform.localScale;
        hash.Append(localScale.x);
        hash.Append(localScale.y);
        hash.Append(localScale.z);

        return hash.ToString();
    }

    public static void SetSaveData(Save save)
    {
        LevelNumber = save.levelNumber;
        CheckpointNumber = save.checkpointNumber;
        _pressurePlatesActivated = save.pressurePlatesActivated;
        _pickupsPicked = save.pickupsPicked;
        InventoryData = save.InventoryData;
    }

    public static Save GetSaveData()
    {
        GetInventoryData();
        return new Save();
    }
}
