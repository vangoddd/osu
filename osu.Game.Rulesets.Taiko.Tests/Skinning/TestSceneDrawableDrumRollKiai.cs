﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Beatmaps;

namespace osu.Game.Rulesets.Taiko.Tests.Skinning
{
    [TestFixture]
    public class TestSceneDrawableDrumRollKiai : TestSceneDrawableDrumRoll
    {
        [SetUp]
        public void SetUp() => Schedule(() =>
        {
            var controlPointInfo = new ControlPointInfo();

            controlPointInfo.Add(0, new TimingControlPoint { BeatLength = 500 });
            controlPointInfo.Add(0, new EffectControlPoint { KiaiMode = true });

            Beatmap.Value = CreateWorkingBeatmap(new Beatmap
            {
                ControlPointInfo = controlPointInfo
            });

            // track needs to be playing for BeatSyncedContainer to work.
            Beatmap.Value.Track.Start();
        });
    }
}
