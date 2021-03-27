using System;
using System.Linq;

namespace LiveSplit.KazeAndTheWildMasks {
    public partial class KazeAndTheWildMasksComponent {

        private readonly RemainingDictionary remainingSplits;

        public override bool Update() => memory.Update();

        public override bool Start() {
            return memory.SceneName.Changed && memory.SceneIs("Cutscene_INTRO");
        }

        public override void OnStart() {
            remainingSplits.Setup(settings.Splits);
        }

        public override bool Split() {
            return remainingSplits.Count() != 0
                && (SplitLevel() || SplitScene());

            bool SplitLevel() {
                return remainingSplits.ContainsKey("Level")
                    && memory.SceneIs("Game") && memory.GameState.Changed && memory.GameState.New == GameState.Finished
                    && remainingSplits.Split("Level", memory.LevelName.New);
            }

            bool SplitScene() {
                return remainingSplits.ContainsKey("Scene")
                    && memory.SceneName.Changed
                    && remainingSplits.Split("Scene", memory.SceneName.New.Replace("(Clone)", ""));
            }
        }

        public override bool Reset() {
            return memory.SceneName.Changed && memory.SceneIs("Menu");
        }

        public override bool Loading() {
            return String.IsNullOrEmpty(memory.SceneName.New) || memory.SceneIs("SceneLoader")
                || (memory.SceneIs("Game") && memory.GameState.New == GameState.None);
        }
    }
}