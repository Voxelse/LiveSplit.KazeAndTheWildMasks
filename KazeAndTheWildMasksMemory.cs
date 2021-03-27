using System;
using Voxif.AutoSplitter;
using Voxif.Helpers.Unity;
using Voxif.IO;
using Voxif.Memory;

namespace LiveSplit.KazeAndTheWildMasks {
    public class KazeAndTheWildMasksMemory : Memory {

        protected override string[] ProcessNames => new string[] { "Kaze and the Wild Masks" };

        public StringPointer SceneName { get; private set; }
        public Pointer<GameState> GameState { get; private set; }
        public StringPointer LevelName { get; private set; }

        private UnityHelperTask unityTask;

        public KazeAndTheWildMasksMemory(Logger logger) : base(logger) {
            OnHook += () => {
                unityTask = new UnityHelperTask(game, logger);
                unityTask.Run(InitPointers);
            };

            OnExit += () => {
                if(unityTask != null) {
                    unityTask.Dispose();
                    unityTask = null;
                }
            };
        }

        private void InitPointers(IMonoHelper unity) {
            MonoNestedPointerFactory ptrFactory = new MonoNestedPointerFactory(game, unity);

            var scene = ptrFactory.Make<IntPtr>("SceneCore", "_instance", 0x10, 0x18, 0x30);

            SceneName = ptrFactory.MakeString(scene, 0x10, 0x30, 0x60, 0x0);
            SceneName.StringType = EStringType.UTF8;

            GameState = ptrFactory.Make<GameState>(scene, 0x80);

            LevelName = ptrFactory.MakeString(scene, 0x90, 0x10, 0x18, ptrFactory.StringHeaderSize);
            LevelName.StringType = EStringType.AutoSized;

            logger.Log(ptrFactory.ToString());

            unityTask = null;
        }

        public override bool Update() => base.Update() && unityTask == null;

        public bool SceneIs(string value) {
            return SceneName.New.StartsWith(value, StringComparison.Ordinal);
        }
    }

    public enum GameState {
        Playing,
        Paused,
        None,
        Finished
    }
}