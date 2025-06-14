using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Beat {

    internal class BeatCoreContext {

        internal AudioSource audioSource;
        internal FFTWindow fftWindow = FFTWindow.BlackmanHarris;
        readonly internal int sampleSize;
        readonly internal int historySize;

        internal float[] spectrum;
        internal float[] energyHistory;

        int historyIndex = 0;
        public int HistoryIndex => historyIndex;

        internal int idRecorder = 0;
        internal List<BeatBeatModel> beatModels;

        // 频段配置 (单位: Hz)
        internal readonly float[] frequencyBands = new float[] {
            20f, 60f, 250f, 500f, 2000f, 4000f, 6000f, 20000f
        };

        internal BeatCoreContext() {
            sampleSize = 1024;
            historySize = 43;
            spectrum = new float[sampleSize];
            energyHistory = new float[historySize];
            beatModels = new List<BeatBeatModel>();
        }

        internal void HistoryIndex_Set(int index) {
            if (index < 0 || index >= historySize) {
                Debug.LogError("History index out of bounds: " + index);
                return;
            }
            historyIndex = index;
        }

        internal int PickID() {
            return idRecorder++;
        }

        internal void Clear() {
            spectrum = null;
            energyHistory = null;
            beatModels.Clear();
            idRecorder = 0;
        }

    }

}