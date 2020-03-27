namespace ACA.Domain
{
    /// <summary>
    /// All implementations of this interface will
    /// be validated at startup to ensure we catch
    /// configuration/setup type errors at startup
    /// and don't have to wait for a certain
    /// condition to find them
    /// </summary>
    public interface IValidatable
    {
        void Validate();
    }
}