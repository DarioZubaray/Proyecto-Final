using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using BE.Properties;

namespace TrabajoFinal_DarioZubaray
{
    public static class ThemeHelper
    {
        #region Constantes
        public const string System = "System";
        public const string Light = "Light";
        public const string Dark = "Dark";
        public const string DefaultTheme = System;

        private const string RegistryKeyPath =
            @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string AppsUseLightThemeValue = "AppsUseLightTheme";
        #endregion

        #region Colores
        private static readonly Color LightBack = Color.White;
        private static readonly Color LightFore = Color.FromArgb(30, 30, 30);
        private static readonly Color LightControl = SystemColors.Control;
        private static readonly Color LightTextBack = Color.White;
        private static readonly Color LightGridHeader = Color.FromArgb(221, 221, 221);

        private static readonly Color DarkBack = Color.FromArgb(30, 30, 30);
        private static readonly Color DarkFore = Color.FromArgb(220, 220, 220);
        private static readonly Color DarkControl = Color.FromArgb(45, 45, 48);
        private static readonly Color DarkTextBack = Color.FromArgb(63, 63, 70);
        private static readonly Color DarkGridHeader = Color.FromArgb(62, 62, 66);
        #endregion

        #region Clase de opción
        public class ThemeItem
        {
            public string Code { get; set; }
            public string DisplayName { get; set; }

            public override string ToString()
            {
                return DisplayName;
            }
        }
        #endregion

        #region Métodos Públicos
        public static List<ThemeItem> GetSupportedThemes()
        {
            return new List<ThemeItem>
            {
                new ThemeItem { Code = System, DisplayName = Resources.Theme_System },
                new ThemeItem { Code = Light, DisplayName = Resources.Theme_Light },
                new ThemeItem { Code = Dark, DisplayName = Resources.Theme_Dark }
            };
        }

        public static string ResolveTheme(string themeCode)
        {
            if (string.IsNullOrEmpty(themeCode))
            {
                themeCode = DefaultTheme;
            }

            if (themeCode.Equals(Light, StringComparison.OrdinalIgnoreCase))
            {
                return Light;
            }

            if (themeCode.Equals(Dark, StringComparison.OrdinalIgnoreCase))
            {
                return Dark;
            }

            return IsSystemDark() ? Dark : Light;
        }

        public static void ApplyTheme(Control root, string themeCode)
        {
            bool isDark = ResolveTheme(themeCode) == Dark;
            ApplyToControl(root, isDark);
        }

        public static void ApplyThemeToAllOpenForms(string themeCode)
        {
            bool isDark = ResolveTheme(themeCode) == Dark;

            foreach (Form form in Application.OpenForms)
            {
                ApplyToControl(form, isDark);
            }
        }
        #endregion

        #region Métodos Privados
        private static bool IsSystemDark()
        {
            try
            {
                object value = Microsoft.Win32.Registry.GetValue(
                    RegistryKeyPath, AppsUseLightThemeValue, 1);

                if (value == null)
                {
                    return false;
                }

                int lightTheme = Convert.ToInt32(value);
                return lightTheme == 0;
            }
            catch
            {
                return false;
            }
        }

        private static void ApplyToControl(Control control, bool isDark)
        {
            if (control == null)
            {
                return;
            }

            ApplyColors(control, isDark);

            foreach (Control child in control.Controls)
            {
                ApplyToControl(child, isDark);
            }
        }

        private static void ApplyColors(Control control, bool isDark)
        {
            Color back = isDark ? DarkBack : LightBack;
            Color fore = isDark ? DarkFore : LightFore;
            Color controlBack = isDark ? DarkControl : LightControl;

            if (control is DataGridView grid)
            {
                ApplyDataGridView(grid, isDark);
                return;
            }

            if (control is MenuStrip || control is StatusStrip || control is ToolStrip)
            {
                ApplyToolStrip((ToolStrip)control, isDark);
                return;
            }

            control.BackColor = (control is TextBox || control is ComboBox || control is CheckBox)
                ? (isDark ? DarkTextBack : LightTextBack)
                : controlBack;
            control.ForeColor = fore;
        }

        private static void ApplyToolStrip(ToolStrip strip, bool isDark)
        {
            strip.BackColor = isDark ? DarkControl : LightControl;
            strip.ForeColor = isDark ? DarkFore : LightFore;

            foreach (ToolStripItem item in strip.Items)
            {
                item.ForeColor = isDark ? DarkFore : LightFore;

                if (item is ToolStripMenuItem menuItem)
                {
                    ApplyDropDownItems(menuItem.DropDownItems, isDark);
                }
            }
        }

        private static void ApplyDropDownItems(ToolStripItemCollection items, bool isDark)
        {
            foreach (ToolStripItem item in items)
            {
                item.BackColor = isDark ? DarkControl : LightControl;
                item.ForeColor = isDark ? DarkFore : LightFore;

                if (item is ToolStripMenuItem menuItem)
                {
                    ApplyDropDownItems(menuItem.DropDownItems, isDark);
                }
            }
        }

        private static void ApplyDataGridView(DataGridView grid, bool isDark)
        {
            Color back = isDark ? DarkBack : LightBack;
            Color fore = isDark ? DarkFore : LightFore;
            Color controlBack = isDark ? DarkControl : LightControl;
            Color header = isDark ? DarkGridHeader : LightGridHeader;
            Color line = isDark ? Color.FromArgb(80, 80, 80) : SystemColors.ControlDark;

            grid.BackgroundColor = isDark ? DarkBack : SystemColors.Window;
            grid.GridColor = line;
            grid.DefaultCellStyle.BackColor = back;
            grid.DefaultCellStyle.ForeColor = fore;
            grid.DefaultCellStyle.SelectionBackColor = isDark ? Color.FromArgb(0, 120, 215) : SystemColors.Highlight;
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = header;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = fore;
            grid.RowHeadersDefaultCellStyle.BackColor = header;
            grid.RowHeadersDefaultCellStyle.ForeColor = fore;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.BackgroundColor = controlBack;
        }
        #endregion
    }
}
