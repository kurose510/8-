// IAnomalyObject.cs
public interface IAnomalyObject
{
    // 異変の状態をセットアップする共通の命令
    void SetupAnomaly(bool isAnomaly);
}