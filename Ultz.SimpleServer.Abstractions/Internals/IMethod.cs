namespace Ultz.SimpleServer.Internals
{
    /// <summary>
    /// Represents a protocol-defined method
    /// </summary>
    public interface IMethod
    {
        /// <summary>
        /// An identifier for this method
        /// </summary>
        byte[] Id { get; }
    }
}