using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using IllusionPlugin;
using static BeatSaberTrainer.Util;
using BS_Utils;

namespace BeatSaberTrainer
{
    public class NJSController : MonoBehaviour
    {
        public static BeatmapObjectSpawnController _spawnController { get; private set; }
        public static StandardLevelSceneSetupDataSO _levelData { get; private set; }
        public static GameObject NjsSettingsObject { get; private set; }
        public static GameObject SpawnOffsetSettingsObject { get; private set; }


        private void Start()
        {
            if(Plugin.enableNjsOverride)
            {
                DebugMessage("NJSController Spawned");
                AdjustNJS();
            }
        }

        public void AdjustNJS()
        {
            BS_Utils.Gameplay.ScoreSubmission.DisableSubmission(Plugin.PluginName);
            StartCoroutine(CallAdjustNJS());
        }

        private IEnumerator<WaitForEndOfFrame> CallAdjustNJS()
        {
            DebugMessage("Entered CallAdjustNJS()");
            yield return new WaitForEndOfFrame();
            var njs = (float) Plugin.NjsOverride;
            _levelData = Resources.FindObjectsOfTypeAll<StandardLevelSceneSetupDataSO>().FirstOrDefault();
            _spawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().FirstOrDefault();
            if (_levelData == null)
            {
                DebugMessage("_levelData is null");
                //if (_levelData == null) return;
                //_levelData.didFinishEvent += MainGameSceneSetupDataOnDidFinishEvent;
            }


            if (_spawnController == null)
            {
                DebugMessage("_spawnController is null");

            }
            DebugMessage($"Adjusting NJS to: {njs}");
            float halfJumpDur = 4f;
            float maxHalfJump = _spawnController.GetPrivateField<float>("_maxHalfJumpDistance");
            float noteJumpStartBeatOffset = _levelData.difficultyBeatmap.noteJumpStartBeatOffset;
            float moveSpeed = _spawnController.GetPrivateField<float>("_moveSpeed");
            float moveDir = _spawnController.GetPrivateField<float>("_moveDurationInBeats");
            float jumpDis;
            float spawnAheadTime;
            float moveDis;
            float bpm = _spawnController.GetPrivateField<float>("_beatsPerMinute");
            float num = 60f / bpm;
            moveDis = moveSpeed * num * moveDir;
            while (njs * num * halfJumpDur > maxHalfJump)
            {
                halfJumpDur /= 2f;
            }
            halfJumpDur += noteJumpStartBeatOffset;
            if (halfJumpDur < 1f) halfJumpDur = 1f;
            //        halfJumpDur = spawnController.GetPrivateField<float>("_halfJumpDurationInBeats");
            jumpDis = njs * num * halfJumpDur * 2f;
            spawnAheadTime = moveDis / moveSpeed + jumpDis * 0.5f / njs;
            _spawnController.SetPrivateField("_halfJumpDurationInBeats", halfJumpDur);
            _spawnController.SetPrivateField("_spawnAheadTime", spawnAheadTime);
            _spawnController.SetPrivateField("_jumpDistance", jumpDis);
            _spawnController.SetPrivateField("_noteJumpMovementSpeed", njs);
            _spawnController.SetPrivateField("_moveDistance", moveDis);
        }
    }
}
