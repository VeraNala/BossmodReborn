﻿// CONTRIB: made by dhoggpt, improvements by Malediktus, not checked
using System;
using System.Collections.Generic;

namespace BossMod.Endwalker.Dungeon.D12Aetherfont.D122Arkas
{
    public enum OID : uint
    {
        Boss = 0x3EEA, //R=5.1
        Helper = 0x233C,
    };

    public enum AID : uint
    {
        AutoAttack = 870, // Boss->player, no cast, single-target
        BattleCry1 = 33364, // Boss->self, 5.0s cast, range 40 circle
        BattleCry2 = 34605, // Boss->self, 5.0s cast, range 40 circle
        ElectricEruption = 33615, // Boss->self, 5.0s cast, range 40 circle
        Electrify = 33367, // Helper->location, 5.5s cast, range 10 circle
        LightningClaw1 = 33366, // Boss->location, no cast, range 6 circle
        LightningClaw2 = 34712, // Boss->player, 5.0s cast, single-target 
        LightningLeapA = 33358, // Boss->location, 4.0s cast, single-target
        LightningLeapB = 33359, // Boss->location, 5.0s cast, single-target
        LightningLeap1 = 33360, // Helper->location, 6.0s cast, range 10 circle
        LightningLeap2 = 34713, // Helper->location, 5.0s cast, range 10 circle
        LightningRampageA = 34318, // Boss->location, 4.0s cast, single-target
        LightningRampageB = 34319, // Boss->location, 2.0s cast, single-target
        LightningRampageC = 34320, // Boss->location, 2.0s cast, single-target
        LightningRampage1 = 34321, // Helper->location, 5.0s cast, range 10 circle
        LightningRampage2 = 34714, // Helper->location, 5.0s cast, range 10 circle
        RipperClaw = 33368, // Boss->player, 5.0s cast, single-target
        Shock = 33365, // Helper->location, 3.5s cast, range 6 circle
        SpinningClaw = 33362, // Boss->self, 3.5s cast, range 10 circle
        ForkedFissures = 33361, // Helper->location, 1.0s cast, width 4 rect charge
        SpunLightning = 33363, // Helper->self, 3.5s cast, range 30 width 8 rect
    };

    public enum IconID : uint
    {
        Tankbuster = 218, // player
        Stackmarker = 161, // 39D7/3DC2
    };

    class Voidzone : BossComponent
    {
        private bool active;

        public override void OnEventEnvControl(BossModule module, byte index, uint state)
        {
            if (index == 0x00)
                if (state == 0x00020001)
                    active = true;
                if (state == 0x00080004)
                    active = false;
        }

        public override void Update(BossModule module)
        {
            if (!active)
                module.Arena.Bounds = new ArenaBoundsCircle(new(425, -440), 14.5f);
            if (active)
                module.Arena.Bounds = new ArenaBoundsCircle(new(425, -440), 10);
        }
    }


    class SpunLightning : Components.SelfTargetedAOEs
    {
        public SpunLightning() : base(ActionID.MakeSpell(AID.SpunLightning), new AOEShapeRect(30, 4)) { }
    }

    class LightningClaw : Components.StackWithIcon
    {
        public LightningClaw() : base((uint)IconID.Stackmarker, ActionID.MakeSpell(AID.LightningClaw2), 6, 5.2f, 4) { }
    }

    class ForkedFissures : Components.GenericAOEs
    {
        private DateTime _activation;
        private static WPos[] patternIndex04Start = [new(419.293f, -445.661f), new(422.919f, -448.675f), new(419.359f, -445.715f), new(419.333f, -437.25f), new(432.791f, -434.82f), new(423.239f, -442.489f), new(426.377f, -437.596f), new(419.335f, -445.663f), new(417.162f, -442.421f), new(427.274f, -448.618f), new(430.839f, -441.877f), new(419.292f, -445.596f), new(427.482f, -448.548f), new(419.101f, -434.242f), new(424.274f, -433.427f), new(419.326f, -445.681f)];
        private static WPos[] patternIndex04End = [new(420.035f, -454.124f), new(427.42f, -448.692f), new(412.039f, -447.562f), new(417.533f, -427.085f), new(433.860f, -427.97f), new(426.993f, -437.034f), new(433.646f, -433.433f), new(411.276f, -434.165f), new(419.394f, -436.118f), new(430.442f, -453.971f), new(439.872f, -438.59f), new(423.667f, -442.039f), new(431.815f, -441.032f), new(425.437f, -432.547f), new(428.824f, -425.528f), new(424.002f, -448.966f)];
        private static WPos[] patternIndex03Start = [new(419.343f, -434.343f), new(416.325f, -437.829f), new(419.304f, -434.353f), new(427.97f, -434.253f), new(430.244f, -447.772f), new(422.523f, -438.223f), new(427.408f, -441.363f), new(419.274f, -434.245f), new(422.582f, -432.152f), new(416.35f, -442.222f), new(423.09f, -445.755f), new(419.412f, -434.285f), new(419.294f, -434.309f), new(416.47f, -442.448f), new(430.633f, -433.69f), new(431.389f, -439.114f)];
        private static WPos[] patternIndex03End = [new(410.880f, -435.019f), new(416.312f, -442.557f), new(417.441f, -427.085f), new(437.949f, -432.547f), new(436.942f, -448.875f), new(428.031f, -442.039f), new(431.571f, -448.63f), new(430.9f, -426.261f), new(428.916f, -434.379f), new(411.032f, -445.457f), new(426.413f, -454.917f), new(422.934f, -438.59f), new(416.037f, -438.926f), new(423.941f, -446.738f), new(432.364f, -440.177f), new(439.475f, -443.809f)];
        private static WPos[] patternIndex02Start = [new(430.635f, -434.592f), new(430.708f, -434.484f), new(434.518f, -440.005f), new(424.457f, -445.105f), new(430.834f, -434.374f), new(431.156f, -439.05f), new(430.599f, -434.383f), new(434.571f, -440.454f), new(423.033f, -437.371f), new(422.069f, -437.329f), new(419.287f, -441.464f), new(430.513f, -434.548f), new(417.501f, -435.027f), new(431.252f, -446.301f), new(430.458f, -434.36f), new(425.28f, -430.49f)];
        private static WPos[] patternIndex02End = [new(439.139f, -435.325f), new(422.232f, -437.583f), new(431.083f, -446.983f), new(420.279f, -454.215f), new(429.831f, -440.299f), new(424.063f, -445.762f), new(434.379f, -440.269f), new(439.811f, -441.733f), new(411.612f, -433.28f), new(418.936f, -441.794f), new(412.07f, -447.532f), new(424.308f, -430.228f), new(410.025f, -440.269f), new(430.594f, -453.88f), new(429.587f, -425.834f), new(414.298f, -429.557f)];
        private static WPos[] patternIndex01Start = [new(430.357f, -445.557f), new(434.507f, -440.159f), new(430.357f, -445.557f), new(424.561f, -449.554f), new(425.887f, -446.107f), new(423.516f, -434.294f), new(430.346f, -445.616f), new(419.902f, -439.485f), new(430.357f, -445.557f), new(430.404f, -445.54f), new(429.973f, -432.501f), new(427.648f, -437.101f), new(430.357f, -445.557f), new(427.648f, -438.04f), new(424.713f, -449.483f), new(418.756f, -446.251f)];
        private static WPos[] patternIndex01End = [new(439.2f, -444.602f), new(435.416f, -429.313f), new(429.618f, -454.246f), new(423.24f, -454.887f), new(419.303f, -439.078f), new(417.441f, -427.054f), new(424.704f, -444.846f), new(410.757f, -435.294f), new(424.643f, -449.577f), new(427.451f, -437.247f), new(424.796f, -425.132f), new(423.148f, -433.951f), new(434.867f, -439.109f), new(431.693f, -426.627f), new(418.051f, -446.036f), new(411.063f, -445.579f)];

        private List<WPos> _patternStart = new();
        private List<WPos> _patternEnd = new();
        private List<(WPos source, AOEShape shape, Angle direction)> _casters = new();

        public override IEnumerable<AOEInstance> ActiveAOEs(BossModule module, int slot, Actor actor)
        {
            if (_casters.Count > 0)
                for (int i = 0; i < 16; ++i)
                    yield return new(_casters[i].shape, _casters[i].source, _casters[i].direction, _activation);
        }

        public override void OnEventEnvControl(BossModule module, byte index, uint state)
        {
            if (state == 0x00200010)
            {
                _activation = module.WorldState.CurrentTime.AddSeconds(6);
                if (index == 0x04)
                {
                    _patternStart.AddRange(patternIndex04Start);
                    _patternEnd.AddRange(patternIndex04End);
                }
                if (index == 0x03)
                {
                    _patternStart.AddRange(patternIndex03Start);
                    _patternEnd.AddRange(patternIndex03End);
                }
                if (_casters.Count < 16 && _patternStart.Count > 0 && _patternEnd.Count > 0)
                    for (int i = 0; i < 16; ++i)
                        _casters.Add((_patternStart[i], new AOEShapeRect((_patternEnd[i] - _patternStart[i]).Length(), 2), Angle.FromDirection(_patternEnd[i] - _patternStart[i])));
                if (_casters.Count >= 16 && _patternStart.Count > 16 && _patternEnd.Count > 16)
                    for (int i = 16; i < 32; ++i)
                        _casters.Add((_patternStart[i], new AOEShapeRect((_patternEnd[i] - _patternStart[i]).Length(), 2), Angle.FromDirection(_patternEnd[i] - _patternStart[i])));
            }
            if (state == 0x00020001)
            {
                _activation = module.WorldState.CurrentTime.AddSeconds(6);
                if (index == 0x02)
                {
                    _patternStart.AddRange(patternIndex02Start);
                    _patternEnd.AddRange(patternIndex02End);
                }
                if (index == 0x01)
                {
                    _patternStart.AddRange(patternIndex01Start);
                    _patternEnd.AddRange(patternIndex01End);
                }
                if (_casters.Count < 16 && _patternStart.Count > 0 && _patternEnd.Count > 0)
                    for (int i = 0; i < 16; ++i)
                        _casters.Add((_patternStart[i], new AOEShapeRect((_patternEnd[i] - _patternStart[i]).Length(), 2), Angle.FromDirection(_patternEnd[i] - _patternStart[i])));
                if (_casters.Count >= 16 && _patternStart.Count > 16 && _patternEnd.Count > 16)
                    for (int i = 16; i < 32; ++i)
                        _casters.Add((_patternStart[i], new AOEShapeRect((_patternEnd[i] - _patternStart[i]).Length(), 2), Angle.FromDirection(_patternEnd[i] - _patternStart[i])));
            }
        }
        public override void AddHints(BossModule module, int slot, Actor actor, TextHints hints, MovementHints? movementHints)
        {
                    hints.Add($"casters {_casters.Count}, start {_patternStart.Count}, end {_patternEnd.Count}");
        }

        public override void OnCastFinished(BossModule module, Actor caster, ActorCastInfo spell)
        {
            if ((AID)spell.Action.ID == AID.ForkedFissures && _casters.Count > 0 && _patternStart.Count > 0 && _patternEnd.Count > 0)
            {
                _casters.RemoveAt(0);
                _patternStart.RemoveAt(0);
                _patternEnd.RemoveAt(0);
            }
        }
    }

    class ElectricEruption : Components.RaidwideCast
    {
        public ElectricEruption() : base(ActionID.MakeSpell(AID.ElectricEruption)) { }
    }

    class Electrify : Components.LocationTargetedAOEs
    {
        public Electrify() : base(ActionID.MakeSpell(AID.Electrify), 10) { }
    }

    class LightningLeap1 : Components.LocationTargetedAOEs
    {
        public LightningLeap1() : base(ActionID.MakeSpell(AID.LightningLeap1), 10) { }
    }

    class LightningLeap2 : Components.LocationTargetedAOEs
    {
        public LightningLeap2() : base(ActionID.MakeSpell(AID.LightningLeap2), 10) { }
    }

    class LightningRampage1 : Components.LocationTargetedAOEs
    {
        public LightningRampage1() : base(ActionID.MakeSpell(AID.LightningRampage1), 10) { }
    }

    class LightningRampage2 : Components.LocationTargetedAOEs
    {
        public LightningRampage2() : base(ActionID.MakeSpell(AID.LightningRampage2), 10) { }
    }

    class RipperClaw : Components.SingleTargetCast
    {
        public RipperClaw() : base(ActionID.MakeSpell(AID.RipperClaw)) { }
    }

    class Shock : Components.LocationTargetedAOEs
    {
        public Shock() : base(ActionID.MakeSpell(AID.Shock), 6) { }
    }

    class SpinningClaw : Components.SelfTargetedAOEs
    {
        public SpinningClaw() : base(ActionID.MakeSpell(AID.SpinningClaw), new AOEShapeCircle(10)) { }
    }

    class BattleCry1 : Components.RaidwideCast
    {
        public BattleCry1() : base(ActionID.MakeSpell(AID.BattleCry1)) { }
    }

    class BattleCry2 : Components.RaidwideCast
    {
        public BattleCry2() : base(ActionID.MakeSpell(AID.BattleCry2)) { }
    }

    class D122ArkasStates : StateMachineBuilder
    {
        public D122ArkasStates(BossModule module) : base(module)
        {
            TrivialPhase()
                .ActivateOnEnter<Voidzone>()
                .ActivateOnEnter<LightningClaw>()
                .ActivateOnEnter<SpunLightning>()
                .ActivateOnEnter<ForkedFissures>()
                .ActivateOnEnter<ElectricEruption>()
                .ActivateOnEnter<Electrify>()
                .ActivateOnEnter<LightningLeap1>()
                .ActivateOnEnter<LightningLeap2>()
                .ActivateOnEnter<LightningRampage1>()
                .ActivateOnEnter<LightningRampage2>()
                .ActivateOnEnter<RipperClaw>()
                .ActivateOnEnter<Shock>()
                .ActivateOnEnter<SpinningClaw>()
                .ActivateOnEnter<BattleCry1>()
                .ActivateOnEnter<BattleCry2>();
        }
    }

    [ModuleInfo(CFCID = 822, NameID = 12337)]
    public class D122Arkas : BossModule
    {
        public D122Arkas(WorldState ws, Actor primary) : base(ws, primary, new ArenaBoundsCircle(new(425, -440), 14.5f)) { }
    }
}
