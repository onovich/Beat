using System;
using System.Collections.Generic;
using MortiseFrame.Beat;
using UnityEngine;

namespace MortiseFrame.Beat {

    public class BeatCore {

        BeatCoreContext ctx;
        Action<int> OnBeatDetectedHandler;

        public BeatCore() {
            ctx = new BeatCoreContext();
        }

        public void AddBeatListener(Action<int> listener) {
            OnBeatDetectedHandler += listener;
        }

        public void RemoveBeatListener(Action<int> listener) {
            OnBeatDetectedHandler -= listener;
        }

        public void SetAudioSource(AudioSource source) {
            ctx.audioSource = source;
        }

        public int CreateBeatModel(float beatThreshold, float beatCooldown, BeatFrequencyBandType frequencyBand) {
            var model = new BeatBeatModel {
                id = ctx.PickID(),
                beatThreshold = beatThreshold,
                beatCooldown = beatCooldown,
                lastBeatTime = 0f,
                frequencyBand = frequencyBand
            };
            ctx.beatModels.Add(model);
            return model.id;
        }

        public void RemoveBeatModel(int id) {
            for (int i = 0; i < ctx.beatModels.Count; i++) {
                if (ctx.beatModels[i].id == id) {
                    ctx.beatModels.RemoveAt(i);
                    return;
                }
            }
        }

        public void Tick(float dt, float time) {
            if (ctx.audioSource == null || !ctx.audioSource.isPlaying) return;
            ctx.audioSource.GetSpectrumData(ctx.spectrum, 0, ctx.fftWindow);

            for (int i = 0; i < ctx.beatModels.Count; i++) {
                var model = ctx.beatModels[i];
                int id = model.id;
                float beatThreshold = model.beatThreshold;
                float beatCooldown = model.beatCooldown;
                ref float lastBeatTime = ref model.lastBeatTime;
                BeatFrequencyBandType frequencyBand = model.frequencyBand;

                float currentEnergy = GetBandEnergy(frequencyBand);
                UpdateEnergyHistory(currentEnergy);
                DetectBeat(id, currentEnergy, beatThreshold, beatCooldown, ref lastBeatTime, time);
            }
        }

        float GetBandEnergy(BeatFrequencyBandType band) {
            int sampleRate = AudioSettings.outputSampleRate;
            int bandIndex = (int)band;

            // 获取频段范围
            int minIndex = FrequencyToSpectrumIndex(ctx.frequencyBands[bandIndex], sampleRate);
            int maxIndex = FrequencyToSpectrumIndex(ctx.frequencyBands[bandIndex + 1], sampleRate);

            // 计算能量平均值
            float sum = 0f;
            for (int i = minIndex; i <= maxIndex; i++) {
                sum += ctx.spectrum[i];
            }
            return sum / (maxIndex - minIndex + 1);
        }

        void UpdateEnergyHistory(float currentEnergy) {
            ctx.energyHistory[ctx.HistoryIndex] = currentEnergy;
            ctx.HistoryIndex_Set((ctx.HistoryIndex + 1) % ctx.historySize);
        }

        void DetectBeat(int id, float currentEnergy, float beatThreshold, float beatCooldown, ref float lastBeatTime, float time) {
            // 1. 计算动态阈值(基于历史平均能量)
            float averageEnergy = 0f;
            foreach (var energy in ctx.energyHistory) {
                averageEnergy += energy;
            }
            averageEnergy /= ctx.energyHistory.Length;
            float threshold = averageEnergy * beatThreshold;

            // 2. 节拍检测条件
            float currentTime = time;
            bool isAboveThreshold = currentEnergy > threshold;
            bool isCooldownOver = (currentTime - lastBeatTime) > beatCooldown;

            // 3. 触发节拍事件
            if (isAboveThreshold && isCooldownOver) {
                lastBeatTime = currentTime;
                OnBeatDetectedHandler?.Invoke(id);
            }
        }

        int FrequencyToSpectrumIndex(float frequency, int sampleRate) {
            return Mathf.FloorToInt(frequency * ctx.sampleSize / (sampleRate / 2f));
        }

        public void TearDown() {
            if (ctx.audioSource != null) {
                ctx.audioSource = null;
            }
            ctx.Clear();
            OnBeatDetectedHandler = null;
        }

    }

}