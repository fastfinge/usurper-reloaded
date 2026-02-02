using System;
using System.Threading.Tasks;

namespace UsurperRemake.BBS
{
    /// <summary>
    /// Interface for terminal I/O implementations (socket, serial, etc.)
    /// </summary>
    public interface ITerminalIO : IDisposable
    {
        /// <summary>
        /// Session information from the drop file
        /// </summary>
        BBSSessionInfo SessionInfo { get; }

        /// <summary>
        /// Initialize the terminal connection
        /// </summary>
        bool Initialize();

        /// <summary>
        /// Set the current text color using ANSI codes
        /// </summary>
        void SetColor(string colorName);

        /// <summary>
        /// Write text without newline
        /// </summary>
        void Write(string text);

        /// <summary>
        /// Write text with newline
        /// </summary>
        void WriteLine(string text = "");

        /// <summary>
        /// Write raw bytes/string without conversion
        /// </summary>
        void WriteRaw(string text);

        /// <summary>
        /// Clear the screen
        /// </summary>
        void ClearScreen();

        /// <summary>
        /// Read a line of input
        /// </summary>
        Task<string> ReadLineAsync();

        /// <summary>
        /// Read a single key
        /// </summary>
        Task<char> ReadKeyAsync();

        /// <summary>
        /// Check if data is available to read
        /// </summary>
        bool DataAvailable();
    }
}
