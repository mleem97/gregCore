namespace gregCoreSDK.Sdk;

public interface IGregModelOverrideService
{
    void ReplaceModel(string contentId, string modelPath, string fallbackPath);
}
