using FairyGUI;
using HarmonyLib;
using XiaWorld;
using System;
using System.Collections.Generic;

namespace ChatPlus {
    public class ChatPlus {
        [HarmonyPatch(typeof(Wnd_JianghuTalk), "OnInit")]
        class ChatPlusPatch {
            static void Postfix(ref Wnd_JianghuTalk __instance) {
                try {
                    var talkWindow = __instance;
                    GButton pryButton;
                    if (talkWindow.UIInfo.GetChild("ChatPlus.Pry") == null) {
                        pryButton = (GButton)UIPackage.CreateObjectFromURL("ui://ncbwb41mv9j6ah");
                        pryButton.name = "ChatPlus.Pry";
                        pryButton.title = "Pry";
                        pryButton.text = "Pry";
                        pryButton.x = 94;
                        pryButton.y = 204;
                        talkWindow.UIInfo.AddChild(pryButton);
                        pryButton.onClick.Add(delegate () {
                            int NPCid = (int)Traverse.Create(talkWindow).Field("targetseed").GetValue();
                            List<int[]> prylist = new List<int[]>();
                            foreach (KeyValuePair<int, JianghuMgr.JHNpcData> KnownNPC in JianghuMgr.Instance.KnowNpcData) {
                                g_emJHNpcDataType known = JianghuMgr.Instance.CheckNpcKnowOtherOnly(NPCid, KnownNPC.Key, g_emJHNpcDataType.Feature, g_emJHNpcDataType.Secret3);
                                if (known != g_emJHNpcDataType.None) {
                                    prylist.Add(new int[] {
                                           KnownNPC.Key,
                                           (int)known
                                       });
                                }
                            }
                            if (prylist.Count > 0) {
                                int[] result = prylist[World.RandomRange(0, prylist.Count, GMathUtl.RandomType.emNone)];
                                Npc player = (Npc)Traverse.Create(talkWindow).Field("player").GetValue();
                                Npc target = (Npc)Traverse.Create(talkWindow).Field("target").GetValue();
                                g_emJHNpcDataType key = GameDefine.JHNpcDataMainType[(g_emJHNpcDataType)result[1]];
                                talkWindow.AddTalkData(JianghuMgr.Instance.GetRandomTalk("Know", player, target), result[0], GameDefine.JHDataTxts[key], 0);
                            } else {
                                talkWindow.SetTxt("(You already know everything this person knows about everyone you've met.)");
                            }
                        });
                    } else {
                        pryButton = (GButton)talkWindow.UIInfo.GetChild("ChatPlus.Pry");
                    }

                    GButton gossipButton;
                    if (talkWindow.UIInfo.GetChild("ChatPlus.Gossip") == null) {
                        gossipButton = (GButton)UIPackage.CreateObjectFromURL("ui://ncbwb41mv9j6ah");
                        gossipButton.name = "ChatPlus.Gossip";
                        gossipButton.title = "Gossip";
                        gossipButton.text = "Gossip";
                        gossipButton.x = 28;
                        gossipButton.y = 204;
                        talkWindow.UIInfo.AddChild(gossipButton);
                        gossipButton.onClick.Add(delegate () {
                            int NPCid = (int)Traverse.Create(talkWindow).Field("targetseed").GetValue();
                            List<int> gossiplist = new List<int>();
                            foreach (KeyValuePair<int, JianghuMgr.JHNpcData> KnownNPC in JianghuMgr.Instance.KnowNpcData) {
                                if (KnownNPC.Key != NPCid && JianghuMgr.Instance.CheckNpcInterestOther(NPCid, KnownNPC.Key)) {
                                    gossiplist.Add(KnownNPC.Key);
                                }
                            }
                            if (gossiplist.Count > 0) {
                                int result = gossiplist[World.RandomRange(0, gossiplist.Count, GMathUtl.RandomType.emNone)];
                                Npc player = (Npc)Traverse.Create(talkWindow).Field("player").GetValue();
                                Npc target = (Npc)Traverse.Create(talkWindow).Field("target").GetValue();
                                talkWindow.AddTalkData(JianghuMgr.Instance.GetRandomTalk("Interest", player, target), result, null, 0);
                            } else {
                                talkWindow.SetTxt("(You haven't met anyone that this person is interested in.)");
                            }
                        });
                    } else {
                        gossipButton = (GButton)talkWindow.UIInfo.GetChild("ChatPlus.Gossip");
                    }
                }
                catch (Exception e) {
                    KLog.Dbg("[CHAT+] error" + e.ToString(), new object[0]);
                }
            }
        }
    }
}
