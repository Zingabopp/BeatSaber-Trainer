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
        public BeatmapObjectSpawnController _spawnController { get; private set; }
        public StandardLevelSceneSetupDataSO _levelData { get; private set; }
        public GameEnergyCounter _energyCounter { get; private set; }
        public ScoreController _scoreController { get; private set; }


        public NJSSettings _originalSettings { get; private set; }
        public NJSSettings _modifiedSettings { get; private set; }
        private bool NoFail
        {
            get
            {
                if (_energyCounter != null)
                    return _energyCounter.noFail;
                else
                    DebugMessage("Unable to determine NoFail status, _energyCounter not found");
                return false;
            }
        }
        //public static GameObject NjsSettingsObject { get; private set; }
        //public static GameObject SpawnOffsetSettingsObject { get; private set; }


        private void Start()
        {
            if (Plugin.enableNjsOverride)
            {
                DebugMessage("NJSController Spawned");
                _originalSettings = new NJSSettings(Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().FirstOrDefault(),
                    Resources.FindObjectsOfTypeAll<StandardLevelSceneSetupDataSO>().FirstOrDefault());
                StartCoroutine(SetupController());
                AdjustNJS();
                StartCoroutine(GetEnergyCounter());
            }
        }

        public void AdjustNJS()
        {
            BS_Utils.Gameplay.ScoreSubmission.DisableSubmission(Plugin.PluginName);
            StartCoroutine(CallAdjustNJS());
        }

        public void OnNoteWasCut(NoteData noteData, NoteCutInfo noteCutInfo, int multiplier)
        {
            // Event order: combo, multiplier, scoreController.noteWasCut, (LateUpdate) scoreController.scoreDidChange, afterCut, (LateUpdate) scoreController.scoreDidChange
            if (noteCutInfo.allIsOK)
            {
                // Good cut
            }
        }
        public void OnNoteWasMissed(NoteData noteData, int multiplier)
        {
            // Event order: combo, multiplier, scoreController.noteWasMissed, (LateUpdate) scoreController.scoreDidChange
        }
        public void OnComboDidChange(int combo)
        {
            //statusManager.gameStatus.combo = combo;
            // public int ScoreController#maxCombo
            //statusManager.gameStatus.maxCombo = scoreController.maxCombo;
        }

        public void EchoEnergy(float energy)
        {
            DebugMessage($"Energy changed: {energy.ToString("0.00")}");
        }

        private IEnumerator<WaitForSeconds> GetEnergyCounter()
        {

            while (_energyCounter == null)
            {

                DebugMessage("_energyCounter is null");
                _energyCounter = Resources.FindObjectsOfTypeAll<GameEnergyCounter>().FirstOrDefault();

                yield return new WaitForSeconds(0.5f);
            }
            _energyCounter.gameEnergyDidChangeEvent += EchoEnergy;


        }

        private IEnumerator<WaitForEndOfFrame> SetupController()
        {
            yield return new WaitForEndOfFrame();
            _levelData = Resources.FindObjectsOfTypeAll<StandardLevelSceneSetupDataSO>().FirstOrDefault();
            _spawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().FirstOrDefault();
            _energyCounter = Resources.FindObjectsOfTypeAll<GameEnergyCounter>().FirstOrDefault();
            _scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().FirstOrDefault();

            if (_energyCounter == null)
                DebugMessage("_energyCounter is null");
            if (_levelData == null)
            {
                DebugMessage("_levelData is null");
            }
            if (_spawnController == null)
            {
                DebugMessage("_spawnController is null");
            }
            if (_energyCounter == null)
            {
                DebugMessage("_energyCounter is null");
            }
            if (_scoreController != null)
            {
                if (Plugin.dynamicNJS)
                {

                }
            }
            else
            {
                DebugMessage("_scoreController is null");
            }
        }

        private IEnumerator<WaitForEndOfFrame> CallAdjustNJS()
        {
            DebugMessage("Entered CallAdjustNJS()");
            yield return new WaitForEndOfFrame();
            var njs = (float) Plugin.NjsOverride;
            DebugMessage($"Trying to adjust to new njs of {njs.ToString("0.00")}");


            if (_modifiedSettings == null)
            {
                DebugMessage($"_modifiedSettings is null, creating a new one with njs: {njs.ToString("0.00")}");
                _modifiedSettings = new NJSSettings(_spawnController, _levelData, njs);
            }
            else
                _modifiedSettings.ChangeNJS(njs);
            if (Plugin.njsAsMin)
            {
                if (_modifiedSettings.njs > _originalSettings.njs)
                    _modifiedSettings.ApplySettings();
                else
                    DebugMessage($"Ignoring modified NJS of {_modifiedSettings.njs.ToString("0.00")}, lower than the map's ({_originalSettings.njs.ToString("0.00")})");
            }
            else
                _modifiedSettings.ApplySettings();


        }

        public bool HasValidGameObjects
        {
            get
            {
                bool isValid = true;
                if (_levelData == null)
                {
                    DebugMessage("_levelData is null");
                    isValid = false;
                }
                if (_spawnController == null)
                {
                    DebugMessage("_spawnController is null");
                    isValid = false;
                }
                if (_energyCounter == null)
                {
                    DebugMessage("_energyCounter is null");
                    isValid = false;
                }
                if (_scoreController == null)
                {
                    DebugMessage("_scoreController is null");
                    isValid = false;
                }
                return isValid;
            }
        }
    }

    public class NJSSettings
    {
        public BeatmapObjectSpawnController _spawnController;
        public StandardLevelSceneSetupDataSO _levelData { get; private set; }
        public float halfJumpDur, maxHalfJump, noteJumpStartBeatOffset, moveSpeed, moveDir, jumpDis, spawnAheadTime, moveDis, bpm, njs, num;
        private bool isReady = false;
        /// <summary>
        /// Creates an NJSSettings object with the current settings.
        /// </summary>
        /// <param name="spawnController"></param>
        /// <param name="levelData"></param>
        public NJSSettings(BeatmapObjectSpawnController spawnController, StandardLevelSceneSetupDataSO levelData)
        {
            _spawnController = spawnController;
            _levelData = levelData;
            if (HasValidGameObjects)
            {
                halfJumpDur = _spawnController.GetPrivateField<float>("_halfJumpDurationInBeats");
                maxHalfJump = _spawnController.GetPrivateField<float>("_maxHalfJumpDistance");
                noteJumpStartBeatOffset = _levelData.difficultyBeatmap.noteJumpStartBeatOffset;
                moveSpeed = _spawnController.GetPrivateField<float>("_moveSpeed");
                moveDir = _spawnController.GetPrivateField<float>("_moveDurationInBeats");
                jumpDis = _spawnController.GetPrivateField<float>("_jumpDistance");
                spawnAheadTime = _spawnController.GetPrivateField<float>("_spawnAheadTime");
                moveDis = _spawnController.GetPrivateField<float>("_moveDistance");
                bpm = _spawnController.GetPrivateField<float>("_beatsPerMinute");
                njs = _spawnController.GetPrivateField<float>("_noteJumpMovementSpeed");
                num = 60f / bpm;
                isReady = true;
            }
            else
            {
                DebugMessage("Unable to create NJSSetting object, missing game objects");
                isReady = false;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spawnController"></param>
        /// <param name="levelData"></param>
        /// <param name="newNjs"></param>
        public NJSSettings(BeatmapObjectSpawnController spawnController, StandardLevelSceneSetupDataSO levelData, float newNjs)
        {
            _spawnController = spawnController;
            _levelData = levelData;
            njs = newNjs;
            if (HasValidGameObjects)
            {
                halfJumpDur = 4f;
                maxHalfJump = _spawnController.GetPrivateField<float>("_maxHalfJumpDistance");
                noteJumpStartBeatOffset = _levelData.difficultyBeatmap.noteJumpStartBeatOffset;
                moveSpeed = _spawnController.GetPrivateField<float>("_moveSpeed");
                moveDir = _spawnController.GetPrivateField<float>("_moveDurationInBeats");

                bpm = _spawnController.GetPrivateField<float>("_beatsPerMinute");
                num = 60f / bpm;
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
                isReady = true;
            }
            else
            {
                DebugMessage("Unable to create NJSSetting object, missing game objects");
                isReady = false;
            }
        }

        public void ChangeNJS(float newNjs)
        {
            njs = newNjs;
            if (HasValidGameObjects)
            {
                halfJumpDur = 4f;
                maxHalfJump = _spawnController.GetPrivateField<float>("_maxHalfJumpDistance");
                noteJumpStartBeatOffset = _levelData.difficultyBeatmap.noteJumpStartBeatOffset;
                moveSpeed = _spawnController.GetPrivateField<float>("_moveSpeed");
                moveDir = _spawnController.GetPrivateField<float>("_moveDurationInBeats");

                bpm = _spawnController.GetPrivateField<float>("_beatsPerMinute");
                num = 60f / bpm;
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
                isReady = true;
            }
            else
            {
                DebugMessage("Unable to change NJS, missing game objects");
                isReady = false;
            }
        }

        public void ApplySettings()
        {
            if (isReady && HasValidGameObjects)
            {
                DebugMessage($"Adjusting NJS to: {njs}");
                _spawnController.SetPrivateField("_halfJumpDurationInBeats", halfJumpDur);
                _spawnController.SetPrivateField("_spawnAheadTime", spawnAheadTime);
                _spawnController.SetPrivateField("_jumpDistance", jumpDis);
                _spawnController.SetPrivateField("_noteJumpMovementSpeed", njs);
                _spawnController.SetPrivateField("_moveDistance", moveDis);
            }
            else
            {
                DebugMessage("Can't apply NJS settings, not ready");
                isReady = false;
            }
        }

        public bool HasValidGameObjects
        {
            get
            {
                bool isValid = true;
                if (_levelData == null)
                {
                    DebugMessage("_levelData is null");
                    isValid = false;
                }
                if (_spawnController == null)
                {
                    DebugMessage("_spawnController is null");
                    isValid = false;
                }
                return isValid;
            }
        }
    }
}
