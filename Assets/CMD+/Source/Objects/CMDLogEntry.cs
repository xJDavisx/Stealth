using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DaanRuiter.CMDPlus {
    public class CMDLogEntry {
        private LogType m_type;
        private Color m_color;
        private DateTime m_postTime;
        private string m_message;
        private string m_string;
        private bool m_command;
        private int m_stackSize = 0;
        private int m_extraLines;

        /// <summary>
        /// The type of log. Is either: Log, Warning, Error or Exception
        /// </summary>
        public LogType Type {
            get {
                return m_type;
            }
        }
        /// <summary>
        /// When was the log entry created
        /// </summary>
        public DateTime PostTime {
            get {
                return m_postTime;
            }
        }
        /// <summary>
        /// The message to display in the console
        /// </summary>
        public string Message {
            get {
                return m_message;
            }
            set {
                m_message = value;
            }
        }
        /// <summary>
        /// The log message casted in a easily serializable format
        /// </summary>
        public string SerializableMessage {
            get {
                return Regex.Replace(m_message, "<.*?>", string.Empty);
            }
        }
        /// <summary>
        /// The string to the where the log was called from
        /// </summary>
        public string StackTrace {
            get {
                return m_string;
            }
        }
        /// <summary>
        /// Was this a command (not used ATM)
        /// </summary>
        public bool IsCommand {
            get {
                return m_command;
            }
        }
        /// <summary>
        /// The size of the string
        /// </summary>
        public int StackSize {
            get {
                return m_stackSize;
            }
        }
        /// <summary>
        /// The color if a custom color was set
        /// </summary>
        public Color Color {
            get {
                return m_color;
            }
        }
        /// <summary>
        /// The amount of lines in the message
        /// </summary>
        public int ExtraLines {
            get {
                return m_extraLines;
            }
        }

        /// <summary>
        /// Construcs the ConsolePrintInfo
        /// </summary>
        /// <param name="type">The type of log. Is either: Log, Warning, Error or Exception</param>
        /// <param name="message">The message to display in the console</param>
        /// <param name="string">The string to the where the log was called from</param>
        /// <param name="command">Is this a command (not used ATM)</param>
        /// <param name="color">Custom color of the message</param>
        public CMDLogEntry(LogType type = LogType.Log, string message = "", string stackTrace = null, bool command = false, Color color = default(Color)) {
            m_type = type;
            m_message = message;
            m_string = stackTrace;
            m_command = command;
            if(color == default(Color)) {
                m_color = Color.white;
            } else {
                m_color = color;
            }

            m_extraLines = message.Split('\n').Length;
            if (m_extraLines == 1) {
                m_extraLines = 0;
            }

            if (m_string != null) {
                m_stackSize = stackTrace.Split('\r').Length;
            }

            m_postTime = DateTime.Now;
        }

        /// <summary>
        /// Set the color of the log entry
        /// </summary>
        /// <param name="color"></param>
        public CMDLogEntry SetColor (Color color) {
            m_color = color;
            return this;
        }

    }
}