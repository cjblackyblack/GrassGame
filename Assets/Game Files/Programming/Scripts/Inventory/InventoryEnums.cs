public enum ItemType {
    Planter,
    Harvester,
    WateringCan,
    Collector,
    FullPlant,
    PlantBit,
    Seed,
    Other
}

public static class InventoryEnum {

    public static bool IsTool(this ItemType item) {
        return item switch {
            ItemType.Planter => true,
            ItemType.Harvester => true,
            ItemType.WateringCan => true,
            ItemType.Collector => true,
            _ => false
        };
    }

    public static bool IsOrganic(this ItemType item) {
        return item switch {
            ItemType.FullPlant => true,
            ItemType.PlantBit => true,
            ItemType.Seed => true,
            _ => false
        };
    }

}