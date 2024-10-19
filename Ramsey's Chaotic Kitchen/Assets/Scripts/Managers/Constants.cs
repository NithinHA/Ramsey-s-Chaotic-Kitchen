using System.Collections.Generic;

public static class Constants
{
    public class GameConstants
    {
        // The keys of this dictionary must be in ascending order.
        public static Dictionary<int, int> CustomerServDelayTipMap = new Dictionary<int, int>()
        {
            {60, 20},
            {100, 10},
            {150, 5}
        };
    }

    public class Audio
    {
        public const string AmbientBg = "ambient_bg";
        public const string MainMenuBg = "main_menu_bg";

        public const string ListenerOn= "listener_on";
        public const string ListenerOff= "listener_off";
        
        public const string WashBasin = "wash_basin";
        public const string GetSupplies = "get_supplies";
        public const string CoinsEarn= "coins_earn";
        public const string ClockAlert = "clock_alert";
        public const string ClockCountdownAlert = "clock_countdown";
        public const string TimeUp = "time_up";     // unused

        public const string InventoryAdd = "inventory_add";
        public const string InventoryRemove = "inventory_remove";
    }

    public class Keywords
    {
#region General
        public const string Inventory = "Inventory";
        public const string Orders = "Orders";
        public const string Help = "Help";
        public const string Restart = "Restart";
        public const string MainMenu = "Main menu";
#endregion
    }
}
