# Beat  
Beat, a real-time audio rhythm detection library.<br/>
**Beat，实时音频节拍检测库，取名自"拍子"。**  

Beat provides beat detection models, frequency band analysis, and event callbacks, which can be used for rhythm games, audio visualization, and interactive systems.<br/> 
**Beat 提供节拍检测模型、频段分析和事件回调，可用于节奏游戏/音频可视化/交互系统。**  

# Readiness  
Stable and available.  <br/>
**稳定可用。**  

# Sample
```
BeatCore beat;
AudioSource audioSource;

void Start() {
    // 1. 初始化
    beat = new BeatCore();
    beat.SetAudioSource(GetComponent<AudioSource>());
    
    // 2. 创建节拍检测模型（低频段）
    int bassBeatId = beat.CreateBeatModel(
        beatThreshold: 1.5f, 
        beatCooldown: 0.2f,
        frequencyBand: BeatFrequencyBandType.SubBass
    );

    // 3. 注册回调
    beat.AddBeatListener(id => {
        if(id == bassBeatId) transform.localScale = Vector3.one * 1.5f;
    });
}

void Update() {
    // 4. 每帧更新
    beat.Tick(Time.deltaTime, Time.time);
}

void OnDestroy() {
    // 5. 资源清理
    beat.TearDown();
}
```

# UPM URL  
`ssh://git@github.com/onovich/Beat.git?path=/Assets/com.mortise.beat#main`  