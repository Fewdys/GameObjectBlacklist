using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using HarmonyLib;
using VRC.SDKBase;

namespace GameObjectBlacklist
{
    internal class OnPlayer
    {
        public static HarmonyLib.Harmony Harmony { get; set; }
        public static HarmonyLib.Harmony Instance = new HarmonyLib.Harmony("Few");

        [Obsolete]
        public static Harmony.HarmonyMethod GetLocalPatch(Type type, string methodName)
        {
            return new Harmony.HarmonyMethod(type.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic));
        }
        public static void InitPatches()
        {
            try
            {
                _OnPlayer.InitOnPlayer();
            }
            catch (Exception ERR) { MelonLoader.MelonLogger.Msg(ERR.Message); }

        }
    }

    class _OnPlayer
    {
        public static void InitOnPlayer()
        {
            try
            {
                //OnPlayer.Instance.Patch(typeof(NetworkManager).GetMethod(nameof(NetworkManager.Method_Public_Void_Player_0)), new HarmonyMethod(AccessTools.Method(typeof(_OnPlayer), nameof(JoinLog))));
                OnPlayer.Instance.Patch(typeof(NetworkManager).GetMethod(nameof(NetworkManager.Method_Public_Void_Player_2)), new HarmonyMethod(AccessTools.Method(typeof(_OnPlayer), nameof(OnPlayerJoin))));
                OnPlayer.Instance.Patch(typeof(NetworkManager).GetMethod(nameof(NetworkManager.Method_Public_Void_Player_1)), new HarmonyMethod(AccessTools.Method(typeof(_OnPlayer), nameof(OnPlayerLeave))));
                MelonLogger.Msg(ConsoleColor.Cyan, "[Patch] OnPlayer Patched!");
            }
            catch (Exception ex)
            {
                MelonLogger.Msg(ConsoleColor.Red, "[Patch] Patching OnPlayer Failed!\n" + ex);
            }
        }

        public static void JoinLog(ref VRC.Player __0)
        {
            if (__0.field_Private_APIUser_0 == null) return;
            MelonLogger.Msg(ConsoleColor.DarkGreen, __0.field_Private_APIUser_0.displayName + " Has Joined");
            //Task.Run(() => { Anti.Anticrashv2(); });
            return;
        }

        public static void OnPlayerJoin(ref VRC.Player __0)
        {
            if (__0.prop_APIUser_0 == null) return;
            try
            {
                if (VRCPlayer.field_Internal_Static_VRCPlayer_0 != null)
                {
                    try
                    {
                        GameObjectBlacklist.StuffToDestroy();
                    }
                    catch { }
                }
            }
            catch { }
            return;
        }

        public static void OnPlayerLeave(ref VRC.Player __0)
        {
            if (__0.prop_APIUser_0 == null) return;
            return;
        }

    }

}