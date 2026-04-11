namespace gregSdk.Interfaces;

public interface IContentValidator<T>
{
    bool Validate(T definition, out string error);
}
