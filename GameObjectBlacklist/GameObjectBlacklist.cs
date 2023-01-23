using Il2CppSystem.Runtime.InteropServices;
using MelonLoader;
using System;
using System.Reflection;
using System.Linq;
using UnityEngine;
using VRC;

namespace GameObjectBlacklist
{
    // This Is Not Nearly As Effective As An Anti Crash, But Can Be Used Nicely To Anti Avatars You Made Yourself or By Friends //
    // Im Sure There Are Better Ways To Have This More Optimized, But This Is An Alternative For Anti Crash, For People Who Can't Get There Hands On One Or Don't Know How To Make One //
    // Thanks To Toast For Originally Helping Me Make The Way It Checks Objects As I Was Confused At The Time //
    public class GameObjectBlacklist : MelonMod
    {
        public static bool logs;
        private static MethodInfo s_joinMethod { get; set; }
        private delegate IntPtr userJoined(IntPtr _instance, IntPtr _user, IntPtr _methodinfo);
        private static userJoined s_userJoined;
        public override void OnApplicationStart()
        {
            NativeHook();
            logs = true;
            MelonLogger.Msg(ConsoleColor.Cyan, "RightCTRL + Backspace To Disable/Enable Logging Of Objects That Get Destroyed");
        }

        public override void OnUpdate()
        {
            // Originally Just Did Space Personally, But Added The Others To Have It Check More Frequently // - You Could Just Not Have It On A Input Like The Line Below, But If Your List Becomes Large This Can Get Unoptimized Extremely Fast
            // Feel Free To Remove Any Of These // - Recommend Keeping Atleast The One For Space Unless You Plan On Having It Check Another Way
            // If You Want To Add A Input For Being In VR Then Go For It //

            /*StuffToDestroy();*/

            if (Input.GetKeyDown(KeyCode.Space))
            {
                StuffToDestroy();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StuffToDestroy();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                StuffToDestroy();
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                StuffToDestroy();
            }
        }

        public override void OnLateUpdate()
        {
            if (logs == true)
            {
                if (Input.GetKey(KeyCode.RightControl) & Input.GetKeyDown(KeyCode.Backspace))
                {
                    logs = false;
                    MelonLogger.Msg(ConsoleColor.Red, "Logs Off");
                }
            }

            if (logs == false)
            {
                if (Input.GetKey(KeyCode.RightControl) & Input.GetKeyDown(KeyCode.Backspace))
                {
                    logs = true;
                    MelonLogger.Msg(ConsoleColor.Green, "Logs On");
                }
            }
        }

        public static void StuffToDestroy()
        {
            // List Of Object Path Names To Check
            string[] All = {

                // Pretty Much Just Put Any Object Names With The Correct Paths To Have It Removed //
                // Example If I Wanted To Remove Peoples Heads For Whatever Reason - "Armature/Hips/Spine/Chest/Neck/Head" or If They Rename Every Object Of There Avatar For Example It Might Be Something Like "FewdysArmature/FewdyHips/FewdSpine/FewChest/FeNeck/FHead" lol //

                // Possible Malicious Objects
                "Crash",
                "Boom",
                "Brrr",
                "Clap",
                "GameObject",
                "Capsule",
                "Line",
                "Trail",
                "Cube",
                "Sphere",

                //////////////////////////////////////////////

                // Malicious Amature
                "Armature/Crash",
                "Armature/Boom",
                "Armature/Brrr",
                "Armature/Clap",

                //////////////////////////////////////////////

                // Malicious Hips
                "Armature/Hips/Crash",
                "Armature/Hips/Boom",
                "Armature/Hips/Brrr",
                "Armature/Hips/Clap",

                //////////////////////////////////////////////

                // Malicious Head
                "Armature/Hips/Spine/Chest/Neck/Head/Crash",
                "Armature/Hips/Spine/Chest/Neck/Head/Boom",
                "Armature/Hips/Spine/Chest/Neck/Head/Brrr",
                "Armature/Hips/Spine/Chest/Neck/Head/Clap",
                "Armature/Hips/Spine/Chest/Neck/Head/Laser",
                "Armature/Hips/Spine/Chest/Neck/Head/Lazer",

                ////////////////////////////////////////////////////////////////////////////////////

                // Malicious Right Wrist
                "Armature/Hips/Spine/Chest/Right shoulder/Right arm/Right elbow/Right wrist/Crash",
                "Armature/Hips/Spine/Chest/Right shoulder/Right arm/Right elbow/Right wrist/Boom",
                "Armature/Hips/Spine/Chest/Right shoulder/Right arm/Right elbow/Right wrist/Brrr",
                "Armature/Hips/Spine/Chest/Right shoulder/Right arm/Right elbow/Right wrist/Clap",

                ////////////////////////////////////////////////////////////////////////////////////

                // Malicious Left Wrist
                "Armature/Hips/Spine/Chest/Left shoulder/Left arm/Left elbow/Left wrist/Crash",
                "Armature/Hips/Spine/Chest/Left shoulder/Left arm/Left elbow/Left wrist/Boom",
                "Armature/Hips/Spine/Chest/Left shoulder/Left arm/Left elbow/Left wrist/Brrr",
                "Armature/Hips/Spine/Chest/Left shoulder/Left arm/Left elbow/Left wrist/Clap",

                ////////////////////////////////////////////////////////////////////////////////////

                // Normal Object Names To Remove // - This Is To Just For Default Object Names

                // Normal Armature
                "Armature/Capsule",
                "Armature/GameObject",
                "Armature/Line",
                "Armature/Trail",
                "Armature/Cube",
                "Armature/Sphere",
                "Armature/Particle System",

                // Normal Hips
                "Armature/Hips/Capsule",
                "Armature/Hips/GameObject",
                "Armature/Hips/Line",
                "Armature/Hips/Trail",
                "Armature/Hips/Cube",
                "Armature/Hips/Sphere",
                "Armature/Hips/Particle System",

                // Normal Head
                "Armature/Hips/Spine/Chest/Neck/Head/Capsule",
                "Armature/Hips/Spine/Chest/Neck/Head/GameObject",
                "Armature/Hips/Spine/Chest/Neck/Head/Line",
                "Armature/Hips/Spine/Chest/Neck/Head/Trail",
                "Armature/Hips/Spine/Chest/Neck/Head/Cube",
                "Armature/Hips/Spine/Chest/Neck/Head/Sphere",
                "Armature/Hips/Spine/Chest/Neck/Head/Particle System",

                // Normal Left Wrist
                "Armature/Hips/Spine/Chest/Left shoulder/Left arm/Left elbow/Left wrist/Capsule",
                "Armature/Hips/Spine/Chest/Left shoulder/Left arm/Left elbow/Left wrist/GameObject",
                "Armature/Hips/Spine/Chest/Left shoulder/Left arm/Left elbow/Left wrist/Line",
                "Armature/Hips/Spine/Chest/Left shoulder/Left arm/Left elbow/Left wrist/Trail",
                "Armature/Hips/Spine/Chest/Left shoulder/Left arm/Left elbow/Left wrist/Cube",
                "Armature/Hips/Spine/Chest/Left shoulder/Left arm/Left elbow/Left wrist/Sphere",
                "Armature/Hips/Spine/Chest/Left shoulder/Left arm/Left elbow/Left wrist/Particle System",

                // Normal Right Wrist
                "Armature/Hips/Spine/Chest/Right shoulder/Right arm/Right elbow/Right wrist/Capsule",
                "Armature/Hips/Spine/Chest/Right shoulder/Right arm/Right elbow/Right wrist/GameObject",
                "Armature/Hips/Spine/Chest/Right shoulder/Right arm/Right elbow/Right wrist/Line",
                "Armature/Hips/Spine/Chest/Right shoulder/Right arm/Right elbow/Right wrist/Trail",
                "Armature/Hips/Spine/Chest/Right shoulder/Right arm/Right elbow/Right wrist/Cube",
                "Armature/Hips/Spine/Chest/Right shoulder/Right arm/Right elbow/Right wrist/Sphere",
                "Armature/Hips/Spine/Chest/Right shoulder/Right arm/Right elbow/Right wrist/Particle System",
            };

            // Feel Free To Edit or Add As You Want //
            try
            {
                foreach (Player player in PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0)
                {
                    if (player.field_Private_APIUser_0.id == VRC.Player.prop_Player_0.field_Private_APIUser_0.id) /*if the player is local*/
                    {
                        if (player == null) return;

                        foreach (var go in All)
                        {
                            GameObject avobj = player.gameObject.transform.Find("ForwardDirection/Avatar").gameObject;
                            GameObject avmirror = player.gameObject.transform.Find("ForwardDirection/_AvatarMirrorClone").gameObject;
                            GameObject avshadow = player.gameObject.transform.Find("ForwardDirection/_AvatarShadowClone").gameObject;
                            if (avobj.transform.Find(go))
                            {
                                GameObject.DestroyImmediate(avobj.transform.Find(go).gameObject);
                                if (logs == true)
                                {
                                    MelonLogger.Msg(ConsoleColor.DarkYellow, $"Destroyed Possible Bad Object: '{go}' On Self");
                                }
                            }
                            if (avmirror.transform.Find(go))
                            {
                                GameObject.DestroyImmediate(avmirror.transform.Find(go).gameObject);
                            }
                            if (avshadow.transform.Find(go))
                            {
                                GameObject.DestroyImmediate(avshadow.transform.Find(go).gameObject);
                            }
                        }
                    }
                    else
                    {
                        foreach (var go in All)
                        {
                            GameObject obj = player._vrcplayer.field_Internal_GameObject_0.gameObject;
                            if (obj.transform.Find(go))
                            {
                                GameObject.DestroyImmediate(obj.transform.Find(go).gameObject);
                                if (logs == true)
                                {
                                    MelonLogger.Msg(ConsoleColor.DarkYellow, $"Destroyed Possible Bad Object: '{go}' On Player {player.field_Private_APIUser_0.displayName}");
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        // Native Hook For OnPlayerJoined //
        private unsafe void NativeHook()
        {
            var JoinmethodInfos = typeof(NetworkManager).GetMethods().FirstOrDefault(x => x.Name == "Method_Public_Void_Player_1");
            var JoinmethodPointer = *(IntPtr*)(IntPtr)UnhollowerBaseLib.UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(JoinmethodInfos).GetValue(null);
            MelonUtils.NativeHookAttach((IntPtr)(&JoinmethodPointer), typeof(GameObjectBlacklist).GetMethod(nameof(OnJoin), BindingFlags.Static | BindingFlags.NonPublic)!.MethodHandle.GetFunctionPointer());
            s_userJoined = Marshal.GetDelegateForFunctionPointer<userJoined>(JoinmethodPointer);
        }

        // OnPlayerJoined //
        private static void OnJoin(IntPtr _instance, IntPtr _user, IntPtr _methodInfo)
        {
            s_userJoined(_instance, _user, _methodInfo);
            var vrcPlayer = UnhollowerSupport.Il2CppObjectPtrToIl2CppObject<VRC.Player>(_user);
            if (vrcPlayer != null)
            {
                try
                {
                    StuffToDestroy();
                }
                catch { }
            }
        }
    }
}
