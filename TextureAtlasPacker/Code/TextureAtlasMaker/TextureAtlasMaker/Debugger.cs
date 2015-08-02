using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TextureAtlasMaker
{
    public static class Debugger
    {
        public enum AlertType
        {
            Critical,
            None
        };

        private static AlertType alertPriority;
        public static AlertType AlertPriority
        {
            get
            {
                return alertPriority;
            }
            set
            {
                alertPriority = value;
            }
        }

        public static string ErrorMessage = string.Empty;
        public static Label lblError;

        public static void UpdateConsole()
        {
            lblError.Text = ErrorMessage;
        }

        public static void ClearDebugger()
        {
            ErrorMessage = "";
            UpdateConsole();
        }
    }
}
