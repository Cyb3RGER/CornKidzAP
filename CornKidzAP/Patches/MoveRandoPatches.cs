using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using CornKidzAP.Archipelago;
using HarmonyLib;

namespace CornKidzAP.Patches;

[HarmonyPatch(typeof(PlayerCtrl), "Run")]
public class MoveRandoPatches
{
    public struct MovePatchInfo
    {
        public MovePatchInfo(CodeMatch[] matches, int expectedMatches = 1, Action<CodeMatcher, Moves> patchAction = null)
        {
            Matches = matches;
            ExpectedMatches = expectedMatches;
            PatchAction = patchAction ?? DefaultPatchAction;
        }

        public CodeMatch[] Matches { get; set; }
        public int ExpectedMatches { get; set; } = 1;
        public Action<CodeMatcher, Moves> PatchAction { get; set; } = DefaultPatchAction;
    }

    private static bool CanUseMove(Moves move)
    {
        //CornKidzAP.Logger.LogDebug($"Called CanUseMove {move}");

        if (!ArchipelagoClient.Authenticated && ArchipelagoClient.State != APState.InGame)
            return true;


        //CornKidzAP.Logger.LogDebug($"Called CanUseMove {move} {ArchipelagoClient.SlotData.IsMovesanity}");
        if (!ArchipelagoClient.SlotData.IsMovesanity)
            return true;

        return ArchipelagoClient.ArchipelagoData.HasMove(move);
    }

    private static Action<CodeMatcher, Moves> DefaultPatchAction => (cm, move) =>
    {
        var inst = cm.Instruction;
        CornKidzAP.Logger.LogDebug($"inst: {inst.opcode} {inst.operand}");
        var label = inst.operand;
        cm.Advance(1);
        cm.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldc_I4, (int)move),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MoveRandoPatches), nameof(CanUseMove))),
            new CodeInstruction(OpCodes.Brfalse, label)
        );
    };

    private static IEnumerable<CodeInstruction> FindAndPatchMatches(Moves move, IEnumerable<CodeInstruction> instructions, MovePatchInfo info)
        => FindAndPatchMatches(move, instructions, [info]);

    private static IEnumerable<CodeInstruction> FindAndPatchMatches(Moves move, IEnumerable<CodeInstruction> instructions, MovePatchInfo[] infos)
    {
        CornKidzAP.Logger.LogDebug($"Patching PlayerCtrl.Run {move}");
        var matcher = new CodeMatcher(instructions);
        for (var i = 0; i < infos.Length; i++)
        {
            var countMatches = 0;
            var patchAction = infos[i].PatchAction ?? DefaultPatchAction;
            matcher.MatchForward(true, infos[i].Matches)
                .ThrowIfNotMatch($"{move} match {i + 1} failed")
                .Repeat(cm =>
                    {
                        countMatches++;
                        patchAction.Invoke(cm, move);
                    }
                );
            // Trace.Assert(countMatches == infos[i].ExpectedMatches, $"{move} Match {i} did not match the expected times: expected {infos[i].ExpectedMatches} got {countMatches}");
            if (countMatches != infos[i].ExpectedMatches)
                throw new InvalidOperationException($"{move} Match {i + 1} did not match the expected times: expected {infos[i].ExpectedMatches} got {countMatches}");
            matcher.Start();
        }

        //matcher.LogInstrcutions();
        return matcher.InstructionEnumeration();
    }

    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> PatchPunch(IEnumerable<CodeInstruction> instructions)
    {
        // if(PlayerCtrl::bOnFloor && PlayerCtrl::Xdown && !PlayerCtrl::bDrill)
        MovePatchInfo info = new(
            [
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bOnFloor"),
                new(i => i.opcode == OpCodes.Brfalse),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "Xdown"),
                new(i => i.opcode == OpCodes.Brfalse),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bDrill"),
                new(i => i.opcode == OpCodes.Brtrue),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldc_I4_S && (sbyte)i.operand == 25),
                new(i => i.opcode == OpCodes.Stfld && ((FieldInfo)i.operand).Name == "plState"),
            ],
            2,
            (cm, move) =>
            {
                cm.SearchBack(i => i.opcode == OpCodes.Brtrue);
                DefaultPatchAction(cm, move);
            }
        );

        return FindAndPatchMatches(Moves.Punch, instructions, info);
    }


    // [HarmonyTranspiler]
    // public static IEnumerable<CodeInstruction> PatchJump(IEnumerable<CodeInstruction> instructions)
    // {
    //     // var matcher = new CodeMatcher(instructions);
    //     // matcher.LogInstructions();
    //
    //     //if (PlayerCtrl::Adown && (PlayerCtrl::bOnFloor || PlayerCtrl::onFloorTimer > -4) && PlayerCtrl::slipTimer == 0)
    //     MovePatchInfo info = new(
    //         [
    //             new(OpCodes.Ldarg_0),
    //             new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "Adown"),
    //             new(i => i.opcode == OpCodes.Brfalse),
    //             new(OpCodes.Ldarg_0),
    //             new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bOnFloor"),
    //             new(i => i.opcode == OpCodes.Brtrue),
    //             new(OpCodes.Ldarg_0),
    //             new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "onFloorTimer"),
    //             new(i => i.opcode == OpCodes.Ldc_I4_S),
    //             new(i => i.opcode == OpCodes.Ble),
    //             new(OpCodes.Ldarg_0),
    //             new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "slipTimer"),
    //             new(i => i.opcode == OpCodes.Brtrue),
    //         ]
    //     );
    //     return FindAndPatchMatches(Moves.Jump, instructions, info);
    // }

    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> PatchSlam(IEnumerable<CodeInstruction> instructions)
    {
        //if (!PlayerCtrl::bOnFloor && PlayerCtrl::LTTimer == 1 && !PlayerCtrl::bNoAttack)
        MovePatchInfo match = new(
            [
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bOnFloor"),
                new(i => i.opcode == OpCodes.Brtrue),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "LTTimer"),
                new(i => i.opcode == OpCodes.Ldc_I4_1),
                new(i => i.opcode == OpCodes.Bne_Un),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bNoAttack"),
                new(i => i.opcode == OpCodes.Brtrue),
            ]
        );
        return FindAndPatchMatches(Moves.Slam, instructions, match);
    }
    
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> PatchCrouch(IEnumerable<CodeInstruction> instructions)
    {
        MovePatchInfo[] matches =
        [
            //if (PlayerCtrl::bOnFloor && PlayerCtrl::LTTimer > 0 && !PlayerCtrl::bDrill)
            new([
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bOnFloor"),
                new(i => i.opcode == OpCodes.Brfalse),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "LTTimer"),
                new(i => i.opcode == OpCodes.Ldc_I4_0),
                new(i => i.opcode == OpCodes.Ble),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bDrill"),
                new(i => i.opcode == OpCodes.Brtrue),
            ]),
            //from punch
            //PlayerCtrl::plTimer = 3;
            //if (PlayerCtrl::LTTimer > 0)
            new([
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldc_I4_3),
                new(i => i.opcode == OpCodes.Stfld && ((FieldInfo)i.operand).Name == "plTimer"),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "LTTimer"),
                new(i => i.opcode == OpCodes.Ldc_I4_0),
                new(i => i.opcode == OpCodes.Ble),
            ])
        ];
        return FindAndPatchMatches(Moves.Crouch, instructions, matches);
    }
    
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> PatchCrouchStabUp(IEnumerable<CodeInstruction> instructions)
    {
        //crouch
        //if (PlayerCtrl::Adown && PlayerCtrl::plTimer > 1 && !PlayerCtrl::bNoAttack)
        MovePatchInfo[] matches =
        [
            new([
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "Adown"),
                new(i => i.opcode == OpCodes.Brfalse),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "plTimer"),
                new(i => i.opcode == OpCodes.Ldc_I4_1),
                new(i => i.opcode == OpCodes.Ble),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bNoAttack"),
                new(i => i.opcode == OpCodes.Brtrue),
            ])
        ];
        return FindAndPatchMatches(Moves.Headbutt, instructions, matches);
    }
    
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> PatchCrouchStab(IEnumerable<CodeInstruction> instructions)
    {
        MovePatchInfo[] matches =
        [
            //crouch
            //if (PlayerCtrl::plTimer > 3 && PlayerCtrl::Xdown && PlayerCtrl::bOnFloor && !PlayerCtrl::bNoAttack)
            new([
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "plTimer"),
                new(i => i.opcode == OpCodes.Ldc_I4_3),
                new(i => i.opcode == OpCodes.Ble),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "Xdown"),
                new(i => i.opcode == OpCodes.Brfalse),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bOnFloor"),
                new(i => i.opcode == OpCodes.Brfalse),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bNoAttack"),
                new(i => i.opcode == OpCodes.Brtrue),
            ], 1),
            // dash
            // PlayerCtrl::yVel = 0.1f;
            // if (PlayerCtrl::Xdown && PlayerCtrl::bOnFloor)
            new([
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldc_R4 && Math.Abs((float)i.operand - 0.1f) < float.Epsilon),
                new(i => i.opcode == OpCodes.Stfld && ((FieldInfo)i.operand).Name == "yVel"),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "Xdown"),
                new(i => i.opcode == OpCodes.Brfalse),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bOnFloor"),
                new(i => i.opcode == OpCodes.Brfalse),
            ], 1),
        ];

        return FindAndPatchMatches(Moves.Headbutt, instructions, matches);
    }
    
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> PatchButtroll(IEnumerable<CodeInstruction> instructions)
    {
        //crouch
        //if ((double) this.plVel.magnitude > 0.10000000149011612 && this.Xdown && this.bOnFloor && !this.bNoAttack && !(bool) (UnityEngine.Object) this.impaleObj)
        MovePatchInfo[] matches =
        [
            new([
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bNoAttack"),
                new(i => i.opcode == OpCodes.Brtrue),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "impaleObj"),
                new(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "op_Implicit"),
                new(i => i.opcode == OpCodes.Brtrue),
            ])
        ];
        return FindAndPatchMatches(Moves.Headbutt, instructions, matches);
    }
    
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> PatchUWButt(IEnumerable<CodeInstruction> instructions)
    {
        MovePatchInfo[] matches =
        [
            // this.Hurt(((Component) this).transform.forward);
            // (!this.Xdown)
            new([
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_0),
                new (i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "get_transform"),
                new (i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == "get_forward"),
                new(OpCodes.Ldc_I4_0),
                new (i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "Hurt"),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "Xdown"),
                new(i => i.opcode == OpCodes.Brfalse),
            ], 1),
        ];

        return FindAndPatchMatches(Moves.Headbutt, instructions, matches);
    }
    
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> PatchAirbutt(IEnumerable<CodeInstruction> instructions)
    {
        MovePatchInfo[] matches =
        [
            // (!this.bOnFloor && this.Xdown && this.plTimer > 5 && !this.bAirbutt && !this.bNoAttack)
            new([
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bOnFloor"),
                new(i => i.opcode == OpCodes.Brtrue),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "Xdown"),
                new(i => i.opcode == OpCodes.Brfalse),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "plTimer"),
                new(i => i.opcode == OpCodes.Ldc_I4_5),
                new(i => i.opcode == OpCodes.Ble),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bAirbutt"),
                new(i => i.opcode == OpCodes.Brtrue),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bNoAttack"),
                new(i => i.opcode == OpCodes.Brtrue),
            ], 1),
            //if (!PlayerCtrl::bOnFloor && PlayerCtrl::Xdown && !PlayerCtrl::bDrill && !PlayerCtrl::bAirbutt && !PlayerCtrl::bNoAttack)
            new([
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bOnFloor"),
                new(i => i.opcode == OpCodes.Brtrue),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "Xdown"),
                new(i => i.opcode == OpCodes.Brfalse),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bDrill"),
                new(i => i.opcode == OpCodes.Brtrue),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bAirbutt"),
                new(i => i.opcode == OpCodes.Brtrue),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bNoAttack"),
                new(i => i.opcode == OpCodes.Brtrue),
            ], 3),
            //if (this.Xdown && !this.bAirbutt)
            new([
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "Xdown"),
                new(i => i.opcode == OpCodes.Brfalse),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bAirbutt"),
                new(i => i.opcode == OpCodes.Brtrue),
            ], 2)
        ];

        return FindAndPatchMatches(Moves.Headbutt, instructions, matches);
    }
    
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> PatchWalljump(IEnumerable<CodeInstruction> instructions)
    {
        MovePatchInfo[] matches =
        [
            //walljumpslide  
            //if (this.Adown)
            //this.plState = PlayerCtrl.State.walljump;
            new(
                [
                    new(OpCodes.Ldarg_0),
                    new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "Adown"),
                    new(i => i.opcode == OpCodes.Brfalse),
                    new(OpCodes.Ldarg_0),
                    new(i => i.opcode == OpCodes.Ldc_I4_S && (sbyte)i.operand == (sbyte)34),
                    new(i => i.opcode == OpCodes.Stfld && ((FieldInfo)i.operand).Name == "plState"),
                ],
                1,
                (cm, move) =>
                {
                    cm.SearchBack(i => i.opcode == OpCodes.Brfalse);
                    DefaultPatchAction(cm, move);
                }
            ),
            //norm
            //if (this.Adown && !this.bOnFloor && !this.bWallJump && this.wallJumpNorm != Vector3.zero && (double) this.yVel < 0.1599999964237213 && (double) this.yVel > -0.30000001192092896 && (this.lastState != PlayerCtrl.State.airbutt || this.plTimer > 10) && this.lastState != PlayerCtrl.State.slip && (double) this.addVel.magnitude < 0.20000000298023224)
            new([
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldflda && ((FieldInfo)i.operand).Name == "addVel"),
                new(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "get_magnitude"),
                new(i => i.opcode == OpCodes.Ldc_R4 && Math.Abs((float)i.operand - 0.2f) < float.Epsilon),
                new(i => i.opcode == OpCodes.Bge_Un),
            ]),
            //ledge	
            //if (this.Adown && this.plTimer > 2)
            //this.plState = PlayerCtrl.State.walljump;
            // new([
            //         new(OpCodes.Ldarg_0),
            //         new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "Adown"),
            //         new(i => i.opcode == OpCodes.Brfalse),
            //         new(OpCodes.Ldarg_0),
            //         new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "plTimer"),
            //         new(i => i.opcode == OpCodes.Ldc_I4_2),
            //         new(i => i.opcode == OpCodes.Ble),
            //         new(OpCodes.Ldarg_0),
            //         new(i => i.opcode == OpCodes.Ldc_I4_S && (sbyte)i.operand == 34),
            //         new(i => i.opcode == OpCodes.Stfld && ((FieldInfo)i.operand).Name == "plState"),
            //     ],
            //     1,
            //     (cm, move) =>
            //     {
            //         cm.SearchBack(i => i.opcode == OpCodes.Ble);
            //         DefaultPatchAction(cm, move);
            //     }
            // ),
            //clamber
            //this.plState = PlayerCtrl.State.norm;
            //if (this.Adown && !this.bWallJump)
            new([
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldc_I4_0),
                new(i => i.opcode == OpCodes.Stfld && ((FieldInfo)i.operand).Name == "plState"),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "Adown"),
                new(i => i.opcode == OpCodes.Brfalse),
                new(OpCodes.Ldarg_0),
                new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bWallJump"),
                new(i => i.opcode == OpCodes.Brtrue),
            ]),
            //polehang
            //if (this.poleObj.GetComponent<Line>().bHangOnly || this.poleObj.GetComponent<Line>().bWall)
            // new(
            //     [
            //         new(OpCodes.Ldarg_0),
            //         new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "poleObj"),
            //         new(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == "GetComponent"),
            //         new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bHangOnly"),
            //         new(OpCodes.Brtrue),
            //         new(OpCodes.Ldarg_0),
            //         new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "poleObj"),
            //         new(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == "GetComponent"),
            //         new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bWall"),
            //         new(OpCodes.Brfalse),
            //     ],
            //     1,
            //     (cm, move) =>
            //     {
            //         var inst = cm.Instruction;
            //         CornKidzAP.Logger.LogDebug($"inst: {inst.opcode} {inst.operand}");
            //         var label = inst.operand;
            //         cm.SearchBack(i => i.opcode == OpCodes.Ldarg_0);
            //         cm.SearchBack(i => i.opcode == OpCodes.Ldarg_0);
            //         cm.InsertAndAdvance(
            //             new CodeInstruction(OpCodes.Ldc_I4, (int)move),
            //             new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MoveRandoPatches), nameof(CanUseMove))),
            //             new CodeInstruction(OpCodes.Brfalse, label)
            //         );
            //     }
            // ),
            //polehang
            //if (this.Adown && this.plSwitch != 2 && this.bStandPoint && this.poleObj.GetComponent<Line>().bWall && (double) this.ctrlDir.magnitude > 0.5 && ((double) Vector3.Angle(this.transform.right, this.ctrlDir) < 30.0 || (double) Vector3.Angle(-this.transform.right, this.ctrlDir) < 30.0))
            // new(
            //     [
            //         new(OpCodes.Ldarg_0),
            //         new(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "get_transform"),
            //         new(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == "get_right"),
            //         new(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "op_UnaryNegation"),
            //         new(OpCodes.Ldarg_0),
            //         new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "ctrlDir"),
            //         new(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "Angle"),
            //         new(i => i.opcode == OpCodes.Ldc_R4 && Math.Abs((float)i.operand - 30f) < float.Epsilon),
            //         new(i => i.opcode == OpCodes.Bge_Un),
            //     ], 1,
            //     (cm, move) =>
            //     {
            //         var inst = cm.Instruction;
            //         CornKidzAP.Logger.LogDebug($"inst: {inst.opcode} {inst.operand}");
            //         var label = inst.operand;
            //         var pos = cm.Pos; //remember position
            //         cm.SearchBack(i => i.opcode == OpCodes.Ble_Un);
            //         cm.Advance(1);
            //         cm.InsertAndAdvance(
            //             new CodeInstruction(OpCodes.Ldc_I4, (int)move),
            //             new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MoveRandoPatches), nameof(CanUseMove))),
            //             new CodeInstruction(OpCodes.Brfalse, label)
            //         );
            //         //since jump before the code match we need to advance back to the old position,
            //         //so we don't keep matching the same spot
            //         cm.ThrowIfFalse("searched backwards and somehow ended up further ahead.", (matcher) => pos > matcher.Pos);
            //         cm.Advance(pos - cm.Pos);
            //     }
            // ),
            //walljump
            //if (this.Adown && !this.bWallJump && (double) this.yVel < 0.23000000417232513)
            new(
                [
                    new(OpCodes.Ldarg_0),
                    new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "Adown"),
                    new(i => i.opcode == OpCodes.Brfalse),
                    new(OpCodes.Ldarg_0),
                    new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bWallJump"),
                    new(i => i.opcode == OpCodes.Brtrue),
                    new(OpCodes.Ldarg_0),
                    new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "yVel"),
                    new(i => i.opcode == OpCodes.Ldc_R4 && Math.Abs((float)i.operand - 0.23f) < float.Epsilon),
                    new(i => i.opcode == OpCodes.Bge_Un),
                ]
            ),
            //rebound
            //if ((this.Adown || this.Atimer > 0 && this.Atimer < 8) && !this.bWallJump && !(bool) (UnityEngine.Object) this.impaleObj && this.lastState == PlayerCtrl.State.airbutt && this.wallJumpNorm != Vector3.zero)
            new(
                [
                    new(OpCodes.Ldarg_0),
                    new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "lastState"),
                    new(i => i.opcode == OpCodes.Ldc_I4_S && (sbyte)i.operand == 35),
                    new(i => i.opcode == OpCodes.Bne_Un),
                    new(OpCodes.Ldarg_0),
                    new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "wallJumpNorm"),
                    new(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "get_zero"),
                    new(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "op_Inequality"),
                    new(i => i.opcode == OpCodes.Brfalse),
                ]
            ),
            //rebound
            //if (this.Adown && !this.bWallJump && this.wallJumpNorm != Vector3.zero && !(bool) (UnityEngine.Object) this.impaleObj)
            new(
                [
                    new(OpCodes.Ldarg_0),
                    new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "wallJumpNorm"),
                    new(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "get_zero"),
                    new(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "op_Inequality"),
                    new(i => i.opcode == OpCodes.Brfalse),
                    new(OpCodes.Ldarg_0),
                    new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "impaleObj"),
                    new(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "op_Implicit"),
                    new(i => i.opcode == OpCodes.Brtrue),
                ]
            ),
        ];

        return FindAndPatchMatches(Moves.WallJump, instructions, matches);
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerCtrl), "LedgeGrab")]
    public static bool PatchLedgeGrab(ref int __result)
    {
        if (CanUseMove(Moves.Climb))
        {
            return true;
        }
    
        __result = 0;
        return false;
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerCtrl), "GrabPole")]
    public static bool PatchGrabPole()
    {
        return CanUseMove(Moves.Climb);
    }
    
    // [HarmonyTranspiler]
    // public static IEnumerable<CodeInstruction> PatchDive(IEnumerable<CodeInstruction> instructions)
    // {
    //     MovePatchInfo[] matches =
    //     [
    //         //onwater
    //         //if (this.Xdown && !this.bDrill || this.bOnCeiling)
    //         new(
    //             [
    //                 new(OpCodes.Ldarg_0),
    //                 new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "Xdown"),
    //                 new(i => i.opcode == OpCodes.Brfalse),
    //                 new(OpCodes.Ldarg_0),
    //                 new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bDrill"),
    //                 new(i => i.opcode == OpCodes.Brfalse),
    //                 new(OpCodes.Ldarg_0),
    //                 new(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "bOnCeiling"),
    //                 new(i => i.opcode == OpCodes.Brfalse),
    //             ],
    //             1,
    //             (cm, move) =>
    //             {
    //                 cm.SearchBack(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "Xdown");
    //                 cm.Advance(1);
    //                 DefaultPatchAction(cm, move);
    //             }
    //         )
    //     ];
    //
    //     return FindAndPatchMatches(Moves.Swim, instructions, matches);
    // }
}