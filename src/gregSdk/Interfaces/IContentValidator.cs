namespace gregCoreSDK.Sdk.Interfaces;

public interface IContentValidator<T>
{
    bool Validate(T definition, out string error);
}
