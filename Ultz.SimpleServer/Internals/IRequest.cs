namespace Ultz.SimpleServer.Internals
{
    public interface IRequest
    {
        /// <summary>
        /// Represents a protocol identifier for the action the server should take with the type of request. Null if the protocol doesn't provide such an identifier.
        /// </summary>
        IMethod Method { get; }
        /// <summary>
        /// The request payload.
        /// </summary>
        byte[] Data { get; }
    }
}