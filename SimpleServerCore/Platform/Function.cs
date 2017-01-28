namespace SimpleServer.Core.Platform
{
    public interface Function
    {
        /// <summary>
        /// Gets the path to this Function
        /// </summary>
        /// <returns>the path to this Function relative to the webroot</returns>
        string GetPath();
        /// <summary>
        /// Gets the Function data, most commonly represented as DLF data
        /// </summary>
        /// <returns>this Function represented in a string</returns>
        string GetData();
    }
}
