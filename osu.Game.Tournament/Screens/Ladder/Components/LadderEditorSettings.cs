// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Game.Overlays.Settings;
using osu.Game.Screens.Play.PlayerSettings;
using osu.Game.Tournament.Components;
using osu.Game.Tournament.Models;

namespace osu.Game.Tournament.Screens.Ladder.Components
{
    public class LadderEditorSettings : PlayerSettingsGroup
    {
        private const int padding = 10;

        private SettingsDropdown<TournamentRound> roundDropdown;
        private PlayerCheckbox losersCheckbox;
        private DateTextBox dateTimeBox;
        private SettingsTeamDropdown team1Dropdown;
        private SettingsTeamDropdown team2Dropdown;

        [Resolved]
        private LadderEditorInfo editorInfo { get; set; }

        [Resolved]
        private LadderInfo ladderInfo { get; set; }

        public LadderEditorSettings()
            : base("ladder")
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                team1Dropdown = new SettingsTeamDropdown(ladderInfo.Teams) { LabelText = "Team 1" },
                team2Dropdown = new SettingsTeamDropdown(ladderInfo.Teams) { LabelText = "Team 2" },
                roundDropdown = new SettingsRoundDropdown(ladderInfo.Rounds) { LabelText = "Round" },
                losersCheckbox = new PlayerCheckbox { LabelText = "Losers Bracket" },
                dateTimeBox = new DateTextBox { LabelText = "Match Time" },
            };

            editorInfo.Selected.ValueChanged += selection =>
            {
                // ensure any ongoing edits are committed out to the *current* selection before changing to a new one.
                GetContainingInputManager().TriggerFocusContention(null);

                roundDropdown.Current = selection.NewValue?.Round;
                losersCheckbox.Current = selection.NewValue?.Losers;
                dateTimeBox.Current = selection.NewValue?.Date;

                team1Dropdown.Current = selection.NewValue?.Team1;
                team2Dropdown.Current = selection.NewValue?.Team2;
            };

            roundDropdown.Current.ValueChanged += round =>
            {
                if (editorInfo.Selected.Value?.Date.Value < round.NewValue?.StartDate.Value)
                {
                    editorInfo.Selected.Value.Date.Value = round.NewValue.StartDate.Value;
                    editorInfo.Selected.TriggerChange();
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            this.FadeIn();
        }

        protected override bool OnHover(HoverEvent e)
        {
            return false;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
        }

        private class SettingsRoundDropdown : SettingsDropdown<TournamentRound>
        {
            public SettingsRoundDropdown(BindableList<TournamentRound> rounds)
            {
                Current = new Bindable<TournamentRound>();

                foreach (var r in rounds.Prepend(new TournamentRound()))
                    add(r);

                rounds.CollectionChanged += (_, args) =>
                {
                    switch (args.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            args.NewItems.Cast<TournamentRound>().ForEach(add);
                            break;

                        case NotifyCollectionChangedAction.Remove:
                            args.OldItems.Cast<TournamentRound>().ForEach(i => Control.RemoveDropdownItem(i));
                            break;
                    }
                };
            }

            private readonly List<IUnbindable> refBindables = new List<IUnbindable>();

            private T boundReference<T>(T obj)
                where T : IBindable
            {
                obj = (T)obj.GetBoundCopy();
                refBindables.Add(obj);
                return obj;
            }

            private void add(TournamentRound round)
            {
                Control.AddDropdownItem(round);
                boundReference(round.Name).BindValueChanged(_ =>
                {
                    Control.RemoveDropdownItem(round);
                    Control.AddDropdownItem(round);
                });
            }
        }
    }
}
