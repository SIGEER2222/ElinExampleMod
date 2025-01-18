using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace ExampleMod;

[BepInPlugin("SigeerTools", "Example Mod", "1.0.0.0")]
public class Plugin : BaseUnityPlugin
{
    private void Start()
    {
        System.Console.WriteLine("Hello World from Elin Example Mod!");
        Logger.LogInfo("mod loaded");
        var harmony = new Harmony("SigeerTools");
        harmony.PatchAll();
    }
}