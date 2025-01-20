// Awake()：在插件启动时会直接调用Awake()方法；
// Start()：在所有插件全部执行完成后会调用Start()方法，执行顺序在Awake()后面；
// Update()：插件启动后会一直循环执行Update()方法，可用于监听事件或判断键盘按键，执行顺序在Start()后面；
// OnDestroy()：在插件关闭时会调用OnDestroy()方法，可处理前面提到的“ScriptEngine”插件重启时需要做的操作。
public class SigeerBaseLoad : BaseUnityPlugin {
    internal static BepInEx.Logging.ManualLogSource Log { get; private set; }
    internal static SigeerBaseLoad Instance { get; private set; }
    public void Awake() {
        Instance = this;
        Log = this.Logger;
    }

    public void Unload() {
        Logger.LogInfo("Unloading SigeerBaseLoad");
    }
}