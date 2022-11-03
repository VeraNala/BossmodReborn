﻿using BossMod;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UIDev
{
    class ReplayTimeline
    {
        private Replay _replay;
        private Replay.Encounter _encounter;
        private StateMachineTree _stateTree;
        private List<int> _phaseBranches;
        private Timeline _timeline = new();
        private ColumnEnemiesCastEvents _colCastEvents;
        private StateMachineBranchColumn _colStates;
        private ColumnEnemiesDetails _colEnemies;
        //private CooldownPlannerColumns _planner;
        private ColumnPlayersDetails _colPlayers;
        private WindowManager.Window? _config;
        private UITree _configTree = new();

        public ReplayTimeline(Replay replay, Replay.Encounter enc)
        {
            _replay = replay;
            _encounter = enc;
            (_stateTree, _phaseBranches) = BuildStateData(enc);
            _timeline.MaxTime = _stateTree.TotalMaxTime;

            _colCastEvents = _timeline.Columns.Add(new ColumnEnemiesCastEvents(_timeline, _stateTree, _phaseBranches, replay, enc));
            _colStates = _timeline.Columns.Add(new StateMachineBranchColumn(_timeline, _stateTree, _phaseBranches));
            _timeline.Columns.Add(new SeparatorColumn(_timeline));
            _colEnemies = _timeline.Columns.Add(new ColumnEnemiesDetails(_timeline, _stateTree, _phaseBranches, replay, enc));
            _colPlayers = _timeline.Columns.Add(new ColumnPlayersDetails(_timeline, _stateTree, _phaseBranches, replay, enc));

            //// TODO: use cooldown plan selector...
            //_planner = new(new(pcClass, ""), () => _timeline.MaxTime = _stateTree.TotalMaxTime, _timeline, _stateTree, _phaseBranches);
            //if (pc != null)
            //    _casts = new(_timeline, pcClass, _stateTree, _phaseBranches);
        }

        public void Draw()
        {
            if (ImGui.Button(_config == null ? "Show config" : "Hide config"))
            {
                if (_config == null)
                {
                    _config = WindowManager.CreateWindow($"Replay timeline config: {_replay.Path} @ {_encounter.Time.Start:O}", DrawConfig, () => _config = null, () => true);
                    _config.SizeHint = new(600, 600);
                    _config.MinSize = new(100, 100);
                }
                else
                {
                    WindowManager.CloseWindow(_config);
                }
            }
            //ImGui.SameLine();
            //_planner.DrawControls();
            _timeline.Draw();
        }

        public void Close()
        {
            if (_config != null)
                WindowManager.CloseWindow(_config);
        }

        private void DrawConfig()
        {
            UICombo.Enum("State text", ref _colStates.TextDisplay);
            foreach (var n in _configTree.Node("Enemy casts columns"))
                _colCastEvents.DrawConfig(_configTree);
            foreach (var n in _configTree.Node("Enemy details"))
                _colEnemies.DrawConfig(_configTree);
            foreach (var n in _configTree.Node("Player details"))
                _colPlayers.DrawConfig(_configTree);
        }

        private (StateMachineTree, List<int>) BuildStateData(Replay.Encounter enc)
        {
            // build state tree with expected timings
            var m = ModuleRegistry.CreateModuleForTimeline(enc.OID);
            if (m == null)
                throw new Exception($"Encounter module not available");

            Dictionary<uint, (StateMachine.State state, StateMachine.State? pred)> stateLookup = new();
            foreach (var p in m.StateMachine.Phases)
                GatherStates(stateLookup, p.InitialState, null);

            // update state durations to match replay data; we don't touch unvisited states, however we set 'skipped' state durations to 0
            var stateEnter = enc.Time.Start;
            StateMachine.State? pred = null;
            foreach (var s in enc.States)
            {
                var cur = stateLookup[s.ID];
                while (cur.pred != pred && cur.pred != null)
                {
                    cur.pred.Duration = 0;
                    cur.pred = stateLookup[cur.pred.ID].pred;
                }
                stateLookup[s.ID].state.Duration = (float)(s.Exit - stateEnter).TotalSeconds;
                stateEnter = s.Exit;
                pred = cur.state;
            }

            var tree = new StateMachineTree(m.StateMachine);
            var phaseBranches = Enumerable.Repeat(0, m.StateMachine.Phases.Count).ToList();
            var phaseTimings = new StateMachineTimings();
            phaseTimings.PhaseDurations.AddRange(Enumerable.Repeat(0.0f, m.StateMachine.Phases.Count));

            var phaseEnter = enc.Time.Start;
            foreach (var p in enc.Phases)
            {
                phaseBranches[p.ID] = tree.Nodes[p.LastStateID].BranchID - tree.Phases[p.ID].StartingNode.BranchID;
                phaseTimings.PhaseDurations[p.ID] = (float)(p.Exit - phaseEnter).TotalSeconds;
                phaseEnter = p.Exit;
            }

            tree.ApplyTimings(phaseTimings);
            return (tree, phaseBranches);
        }

        private void GatherStates(Dictionary<uint, (StateMachine.State state, StateMachine.State? pred)> res, StateMachine.State start, StateMachine.State? pred)
        {
            res[start.ID] = (start, pred);
            if (start.Next != null)
                GatherStates(res, start.Next, start);
            if (start.PotentialSuccessors != null)
                foreach (var succ in start.PotentialSuccessors)
                    if (succ != start.Next)
                        GatherStates(res, succ, start);
        }
    }
}