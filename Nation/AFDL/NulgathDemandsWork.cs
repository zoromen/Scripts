/*
name: Nulgath Demands Work
description: This bot will do the Nulgath Demands Work quest untill you have a uni35 and the equipment.
tags: archfiend, doomlord, ADFL, nulgath, demands, work, unidentified, uni, 35, essence, fragment
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Nation/AFDL/WillpowerExtraction.cs
//cs_include Scripts/Nation/Various/GoldenHanzoVoid.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class NulgathDemandsWork
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    public CoreNation Nation = new();
    public GoldenHanzoVoid GHV = new();
    public WillpowerExtraction WillpowerExtraction = new();
    public CoreAdvanced Adv = new();

    public string[] NDWItems =
    {
        "DoomLord's War Mask",
        "ShadowFiend Cloak",
        "Locks of the DoomLord",
        "Doomblade of Destruction",
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(Nation.bagDrops);
        Core.BankingBlackList.AddRange(NDWItems);
        Core.BankingBlackList.AddRange(new[] { "Archfiend Essence Fragment", "Unidentified 35" });
        Core.SetOptions();

        DoNulgathDemandsWork();

        Core.SetOptions(false);
    }

    public void DoNulgathDemandsWork()
    {
        NDWQuest(new[] { "Unidentified 35" }, 300);
        NDWQuest(NDWItems);
    }

    /// <summary>
    /// Complets "Nulgath Demands Work" until the Desired Items are gotten. 
    /// <param name="string[] items">The List of items to Get from the Quest</param>
    /// <param name="quant">Amount of the "item" [Mostly the Archfiend Ess and Uni 35]</param>
    /// </summary>
    public void NDWQuest(string[]? items = null, int quant = 1)
    {
        items ??= NDWItems;

        if (Core.CheckInventory(items, quant))
            return;

        ItemBase[] rewards5259 = Core.EnsureLoad(5259).Rewards.ToArray();

        Core.AddDrop(Nation.bagDrops);
        Core.AddDrop(rewards5259.Select(x => x.Name).ToArray());

        foreach (string item in items)
        {
            if (Core.CheckInventory(item, quant))
                continue;

            ItemBase _item = rewards5259.FirstOrDefault(reward => reward.Name.Equals(item, StringComparison.OrdinalIgnoreCase))!;

            Core.FarmingLogger(item, quant);

            int i = 0;
            while (!Bot.ShouldExit && !Core.CheckInventory(item, quant))
            {
                Core.EnsureAccept(5259);

                WillpowerExtraction.Unidentified34(10);
                Nation.FarmUni13(2);
                Nation.FarmBloodGem(2);
                Nation.FarmDiamondofNulgath(60);
                Nation.FarmDarkCrystalShard(45);
                Uni27();
                Nation.FarmVoucher(true);
                Nation.FarmGemofNulgath(15);
                Nation.SwindleBulk(50);
                GHV.GetGHV();

                if (_item.Name == "Unidentified 35")
                {
                    while (!Bot.ShouldExit && Core.CheckInventory("Archfiend Essence Fragment", 9) && !Core.CheckInventory("Unidentified 35", quant))
                        Adv.BuyItem("tercessuinotlim", 1951, _item.ID, shopItemID: 7912);
                    if (!Core.CheckInventory(_item.ID, quant))
                        Core.EnsureComplete(5259, _item.ID);
                }
                else Core.EnsureCompleteChoose(5259, Core.QuestRewards(5259));

                Core.Logger($"Completed x{++i}");
                i++;
            }
        }
    }

    public void Uni27()
    {
        if (Core.CheckInventory("Unidentified 27"))
            return;

        Core.AddDrop("Unidentified 27");
        Nation.Supplies("Unidentified 26", 1);
        Core.EnsureAccept(584);
                Nation.ResetSindles();
        string[] locations = new[] { "tercessuinotlim", Core.IsMember ? "Nulgath" : "evilmarsh" };
        string location = locations[new Random().Next(locations.Length)];
        string cell = location == "tercessuinotlim" ? (new Random().Next(2) == 0 ? "m1" : "m2") : "Field1";
        Core.KillMonster(location, cell, "Left", "Dark Makai", "Dark Makai Sigil", log: false);
        Core.EnsureComplete(584);
        Bot.Wait.ForPickup("Unidentified 27");
        Core.Logger("Uni 27 acquired");

    }
}
