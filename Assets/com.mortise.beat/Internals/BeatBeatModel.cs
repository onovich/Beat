
namespace MortiseFrame.Beat {

    internal class BeatBeatModel {
        internal int id;
        internal float beatThreshold;// = 1.5f; // 动态阈值乘数
        internal float beatCooldown;// = 0.4f; // 节拍最小间隔
        internal float lastBeatTime;// = 0f;
        internal BeatFrequencyBandType frequencyBand;
    }

}