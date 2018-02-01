using System.Collections.Generic;
using UnityEngine;

namespace DaanRuiter.CMDPlus {
    public class CMDModel {
        public CMDLogEntryCreatedEventHandler LogEntryCreatedEvent;
        public CMDLogEntry[] Entries { get { return m_logEntries.ToArray(); } }

        private List<CMDLogEntry> m_logEntries;
        private CMDButtonWindow m_buttonWindow;

        public CMDModel() {
            m_logEntries = new List<CMDLogEntry>();
        }

        public CMDLogEntry CreateEntry (LogType type, string message, bool isCommand, string stackTrace = null) {
            CMDLogEntry entry = new CMDLogEntry(type, message, stackTrace, isCommand);

            string maxLogAmountSettingId = "LOG_MAX_AMOUNT";
            if (CMDSettings.Has(maxLogAmountSettingId, typeof(int)))
            {
                int max = CMDSettings.Get<int>(maxLogAmountSettingId, false);
                if(m_logEntries.Count >= max)
                {
                    if(m_logEntries.Count > 0)
                    {
                        m_logEntries.RemoveAt(0);
                    }
                }
            }
            m_logEntries.Add(entry);
            ApplyLogTypeColor(entry);

            if (LogEntryCreatedEvent != null) {
                LogEntryCreatedEvent(entry);
            }

            return entry;
        }
        
        /// <summary>
        /// Cleares the list of all log entries
        /// </summary>
        public void ClearEntries() {
            m_logEntries.Clear();
        }

        /// <summary>
        /// Removes a specific entry
        /// </summary>
        public void RemoveEntry(CMDLogEntry entry) {
            if (m_logEntries.Contains(entry)) {
                m_logEntries.Remove(entry);
            }
        }

        private void ApplyLogTypeColor(CMDLogEntry entry) {
            Color entryColor = Color.white;
            if (entry.Type == LogType.Error || entry.Type == LogType.Exception) {
                CMDHelper.TryParseColor(CMDSettings.Get<string>("COLOR_ERROR", false), out entryColor);
            } else if (entry.Type == LogType.Warning) {
                CMDHelper.TryParseColor(CMDSettings.Get<string>("COLOR_WARNING", false), out entryColor);
            } else {
                CMDHelper.TryParseColor(CMDSettings.Get<string>("COLOR_LOG", false), out entryColor);
            }
            entry.SetColor(entryColor);
        }
    }
}