using System;
using System.Collections.Generic;
using System.Reflection;

namespace DaanRuiter.CMDPlus {
    public class CMDUnkownCommandException : Exception {
        private string m_command;

        public CMDUnkownCommandException(string command) {
            m_command = command;
        }

        public override string ToString() {
            return CMDStrings.UnknownCommandException(m_command);
        }
    }
    public class CMDParameterMismatchException : Exception {
        public MethodInfo MethodInfo { get; private set; }
        public KeyValuePair<Type, Type>[] ParameterTypes { get; private set; }

        public CMDParameterMismatchException(MethodInfo methodInfo, KeyValuePair<Type, Type>[] parameterTypes) {
            MethodInfo = methodInfo;
            ParameterTypes = parameterTypes;
        }

        public override string ToString() {
            return CMDStrings.ParameterMismatchException(this, true);
        }
    }
}