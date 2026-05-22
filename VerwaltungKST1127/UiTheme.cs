using System;
using System.Drawing;
using System.Windows.Forms;

namespace VerwaltungKST1127
{
    // ===================================================================================================
    // Zentrale Design-Definition fuer das gesamte Projekt – Konzept "Sanfte Salbei".
    //
    // Idee: Statt in 30 Designer-Dateien die Farben/Schriften haendisch anzupassen, wird das Aussehen
    // hier EINMAL definiert und zur Laufzeit per UiTheme.Apply(this) auf eine Form angewendet.
    // Das haelt das Design einheitlich – neue Formulare uebernehmen den Look mit einer einzigen Zeile.
    //
    // PILOT-Hinweis: Aktuell nur in Form_Start und Form_Materiallager eingebunden, um den Look
    // zu beurteilen, bevor er auf alle Formulare ausgerollt wird.
    // ===================================================================================================
    public static class UiTheme
    {
        // ---- Farbpalette (Hex aus dem Mockup) -----------------------------------------------------
        public static readonly Color Bg          = ColorTranslator.FromHtml("#F2F5F1"); // Fenster-Hintergrund
        public static readonly Color Surface     = ColorTranslator.FromHtml("#FFFFFF"); // Karten/Flaechen
        public static readonly Color Surface2    = ColorTranslator.FromHtml("#F6F8F4"); // sekundaere Flaeche
        public static readonly Color Primary     = ColorTranslator.FromHtml("#4B8B7B"); // Salbeigruen (Aktion)
        public static readonly Color PrimaryDark = ColorTranslator.FromHtml("#3C7568"); // Hover/gedrueckt
        public static readonly Color OnPrimary   = Color.White;                          // Text auf Primaer
        public static readonly Color TextColor   = ColorTranslator.FromHtml("#2E3A33"); // Haupttext
        public static readonly Color Muted       = ColorTranslator.FromHtml("#73807A"); // gedaempfter Text
        public static readonly Color BorderColor = ColorTranslator.FromHtml("#E4E9E2"); // Rahmen/Linien
        public static readonly Color TableHeader = ColorTranslator.FromHtml("#EDF2EC"); // Tabellen-Kopf
        public static readonly Color SelectBg    = ColorTranslator.FromHtml("#DCEDE5"); // Auswahl-Hintergrund
        public static readonly Color SelectText  = ColorTranslator.FromHtml("#2F6B58"); // Auswahl-Text
        public static readonly Color Success     = ColorTranslator.FromHtml("#2E9E6B"); // Erfolg
        public static readonly Color Warning     = ColorTranslator.FromHtml("#E0A33E"); // Warnung
        public static readonly Color Danger      = ColorTranslator.FromHtml("#D9645B"); // Gefahr/Loeschen
        public static readonly Color DangerSoft  = ColorTranslator.FromHtml("#FBE7E4"); // heller Loesch-Hintergrund

        // ---- Schrift ------------------------------------------------------------------------------
        public const string FontName = "Segoe UI";

        public static Font MakeFont(float size, FontStyle style = FontStyle.Regular)
        {
            return new Font(FontName, size, style);
        }

        // ===========================================================================================
        // Einstiegspunkt: auf ein komplettes Formular anwenden.
        // ===========================================================================================
        public static void Apply(Form form)
        {
            if (form == null) return;
            form.BackColor = Bg;
            form.ForeColor = TextColor;
            ApplyToChildren(form);
        }

        // Geht rekursiv durch alle Steuerelemente. Eltern werden vor ihren Kindern bearbeitet,
        // damit ein Kind (z. B. Label) den bereits gesetzten Hintergrund des Eltern kennt.
        private static void ApplyToChildren(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                StyleControl(c);
                if (c.HasChildren) ApplyToChildren(c);
            }
        }

        private static void StyleControl(Control c)
        {
            // Opt-out: Steuerelemente mit Tag "notheme" werden in Ruhe gelassen.
            if (c.Tag is string tag && tag.IndexOf("notheme", StringComparison.OrdinalIgnoreCase) >= 0)
                return;

            // Bilder/Logos und Diagramme NICHT umfaerben.
            if (c is PictureBox) return;
            if (IsChart(c)) return;

            if (c is Button b)            { StyleButton(b); return; }
            if (c is DataGridView dgv)    { StyleGrid(dgv); return; }
            if (c is TextBoxBase)         { StyleInput(c); return; }
            if (c is ComboBox combo)      { StyleCombo(combo); return; }
            if (c is ListBox lb)          { StyleList(lb); return; }
            if (c is Label lbl)           { StyleLabel(lbl); return; }
            if (c is ButtonBase)          { StyleCheckLike(c); return; } // CheckBox/RadioButton
            if (c is GroupBox)            { StyleGroupBox(c); return; }
            if (c is Panel || c is TableLayoutPanel || c is FlowLayoutPanel || c is TabPage)
            {
                StyleContainer(c); return;
            }

            // Alles andere: zumindest die Schriftart vereinheitlichen.
            RemapFont(c);
        }

        // ---- Buttons ------------------------------------------------------------------------------
        private static void StyleButton(Button b)
        {
            b.FlatStyle = FlatStyle.Flat;
            b.UseVisualStyleBackColor = false;
            b.Cursor = Cursors.Hand;
            b.Font = new Font(FontName, Math.Max(b.Font.Size, 9f), FontStyle.Regular);

            string role = RoleOf(b);
            switch (role)
            {
                case "primary":
                    b.BackColor = Primary; b.ForeColor = OnPrimary;
                    b.FlatAppearance.BorderSize = 0;
                    b.FlatAppearance.MouseOverBackColor = PrimaryDark;
                    b.FlatAppearance.MouseDownBackColor = PrimaryDark;
                    break;
                case "success":
                    b.BackColor = Success; b.ForeColor = Color.White;
                    b.FlatAppearance.BorderSize = 0;
                    b.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(Success, 0.05f);
                    break;
                case "danger":
                    b.BackColor = DangerSoft; b.ForeColor = Danger;
                    b.FlatAppearance.BorderSize = 1;
                    b.FlatAppearance.BorderColor = ControlPaint.Light(Danger, 0.4f);
                    b.FlatAppearance.MouseOverBackColor = ControlPaint.Light(Danger, 0.65f);
                    break;
                default: // secondary
                    b.BackColor = Surface; b.ForeColor = TextColor;
                    b.FlatAppearance.BorderSize = 1;
                    b.FlatAppearance.BorderColor = BorderColor;
                    b.FlatAppearance.MouseOverBackColor = Surface2;
                    b.FlatAppearance.MouseDownBackColor = BorderColor;
                    break;
            }
        }

        // Rolle aus Tag ("primary"/"success"/"danger"/"secondary") oder per Namens-Heuristik ableiten.
        private static string RoleOf(Button b)
        {
            if (b.Tag is string t)
            {
                string tl = t.ToLowerInvariant();
                if (tl.Contains("primary"))   return "primary";
                if (tl.Contains("success"))   return "success";
                if (tl.Contains("danger"))    return "danger";
                if (tl.Contains("secondary")) return "secondary";
            }

            string key = ((b.Name ?? "") + " " + (b.Text ?? "")).ToLowerInvariant();
            if (key.Contains("lösch") || key.Contains("loesch") || key.Contains("entfern"))
                return "danger";
            if (key.Contains("speich") || key.Contains("anleg") || key.Contains("erstell")
                || key.Contains("hinzu") || key.Contains("übernehm") || key.Contains("uebernehm")
                || key.Contains("suchen"))
                return "primary";
            return "secondary";
        }

        // ---- Tabellen (DataGridView) --------------------------------------------------------------
        private static void StyleGrid(DataGridView g)
        {
            g.EnableHeadersVisualStyles = false;
            g.BorderStyle = BorderStyle.None;
            g.BackgroundColor = Surface;
            g.GridColor = BorderColor;
            g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            g.RowHeadersVisible = false;
            g.AllowUserToResizeRows = false;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersHeight = 38;
            g.RowTemplate.Height = 30;
            g.Font = MakeFont(9.75f);

            DataGridViewCellStyle head = g.ColumnHeadersDefaultCellStyle;
            head.BackColor = TableHeader;
            head.ForeColor = Muted;
            head.SelectionBackColor = TableHeader;
            head.SelectionForeColor = Muted;
            head.Font = MakeFont(9.75f, FontStyle.Bold);
            head.Alignment = DataGridViewContentAlignment.MiddleLeft;
            head.Padding = new Padding(8, 0, 8, 0);

            DataGridViewCellStyle cell = g.DefaultCellStyle;
            cell.BackColor = Surface;
            cell.ForeColor = TextColor;
            cell.SelectionBackColor = SelectBg;
            cell.SelectionForeColor = SelectText;
            cell.Font = MakeFont(9.75f);
            cell.Padding = new Padding(8, 4, 8, 4);

            g.AlternatingRowsDefaultCellStyle.BackColor = Surface2;
            g.AlternatingRowsDefaultCellStyle.SelectionBackColor = SelectBg;
            g.AlternatingRowsDefaultCellStyle.SelectionForeColor = SelectText;
        }

        // ---- Eingabefelder ------------------------------------------------------------------------
        private static void StyleInput(Control c)
        {
            c.BackColor = Surface;
            c.ForeColor = TextColor;
            RemapFont(c);
            if (c is TextBox tb) tb.BorderStyle = BorderStyle.FixedSingle;
            if (c is RichTextBox rtb) rtb.BorderStyle = BorderStyle.FixedSingle;
            if (c is MaskedTextBox mtb) mtb.BorderStyle = BorderStyle.FixedSingle;
        }

        private static void StyleCombo(ComboBox cb)
        {
            cb.FlatStyle = FlatStyle.Flat;
            cb.BackColor = Surface;
            cb.ForeColor = TextColor;
            RemapFont(cb);
        }

        private static void StyleList(ListBox lb)
        {
            lb.BorderStyle = BorderStyle.FixedSingle;
            RemapFont(lb);
            if (IsLegacyNeutral(lb.BackColor)) lb.BackColor = Surface;
            if (IsLegacyFore(lb.ForeColor)) lb.ForeColor = TextColor;
        }

        // ---- Beschriftungen -----------------------------------------------------------------------
        private static void StyleLabel(Label l)
        {
            RemapFont(l);

            Color bg = EffectiveBack(l);
            bool darkBg = bg.GetBrightness() < 0.5f;

            if (IsLegacyFore(l.ForeColor))
            {
                l.ForeColor = darkBg ? Color.White : TextColor;
            }
            else if (l.ForeColor.ToArgb() == Color.White.ToArgb() && !darkBg)
            {
                // Weisser Text auf jetzt heller Flaeche waere unlesbar -> abdunkeln.
                l.ForeColor = TextColor;
            }
            // Bewusst gesetzte Signalfarben (Rot/Blau/Gruen ...) bleiben erhalten.

            if (IsLegacyNeutral(l.BackColor))
                l.BackColor = Color.Transparent;
        }

        private static void StyleCheckLike(Control c)
        {
            RemapFont(c);
            if (IsLegacyFore(c.ForeColor)) c.ForeColor = TextColor;
            if (IsLegacyNeutral(c.BackColor)) c.BackColor = Color.Transparent;
        }

        // ---- Container ----------------------------------------------------------------------------
        private static void StyleContainer(Control c)
        {
            if (!IsLegacyNeutral(c.BackColor)) return; // bewusste Farben (z. B. dunkelgruene Kopfzeile) behalten

            // Vollflaechige Container = Fenster-Hintergrund; sonst = weisse "Karte".
            c.BackColor = (c.Dock == DockStyle.Fill) ? Bg : Surface;
        }

        private static void StyleGroupBox(Control c)
        {
            RemapFont(c);
            if (IsLegacyFore(c.ForeColor)) c.ForeColor = TextColor;
            if (IsLegacyNeutral(c.BackColor)) c.BackColor = Surface;
        }

        // ---- Hilfsfunktionen ----------------------------------------------------------------------

        // Schriftart auf Segoe UI umstellen, Groesse und Stil beibehalten.
        private static void RemapFont(Control c)
        {
            Font f = c.Font;
            if (f == null) return;
            if (!string.Equals(f.Name, FontName, StringComparison.OrdinalIgnoreCase))
                c.Font = new Font(FontName, f.Size, f.Style);
        }

        // Erste sichtbare (nicht transparente) Hintergrundfarbe in der Eltern-Kette.
        private static Color EffectiveBack(Control c)
        {
            Control p = c;
            while (p != null)
            {
                if (p.BackColor.A != 0 && p.BackColor != Color.Transparent)
                    return p.BackColor;
                p = p.Parent;
            }
            return Bg;
        }

        private static bool IsChart(Control c)
        {
            string full = c.GetType().FullName;
            return full != null && full.IndexOf("DataVisualization", StringComparison.Ordinal) >= 0;
        }

        // "Alt-Look"-Hintergruende (Grau/Silber/Weiss/System), die ersetzt werden duerfen.
        private static bool IsLegacyNeutral(Color col)
        {
            if (col.A == 0 || col == Color.Transparent) return false;
            int rgb = col.ToArgb();
            return rgb == SystemColors.Control.ToArgb()
                || rgb == SystemColors.ControlLight.ToArgb()
                || rgb == SystemColors.ControlLightLight.ToArgb()
                || rgb == SystemColors.ButtonFace.ToArgb()
                || rgb == SystemColors.Window.ToArgb()
                || rgb == SystemColors.Menu.ToArgb()
                || rgb == SystemColors.ActiveCaption.ToArgb()
                || rgb == SystemColors.ActiveCaptionText.ToArgb()
                || rgb == SystemColors.GradientActiveCaption.ToArgb()
                || rgb == SystemColors.GradientInactiveCaption.ToArgb()
                || rgb == SystemColors.InactiveCaption.ToArgb()
                || rgb == SystemColors.Info.ToArgb()
                || rgb == Color.White.ToArgb()
                || rgb == Color.WhiteSmoke.ToArgb()
                || rgb == Color.Gainsboro.ToArgb()
                || rgb == Color.LightGray.ToArgb()
                || rgb == Color.Silver.ToArgb()
                || rgb == Color.DarkGray.ToArgb()
                || rgb == Color.FromArgb(224, 224, 224).ToArgb()
                || rgb == Color.FromArgb(240, 240, 240).ToArgb()
                || rgb == Color.FromArgb(236, 233, 216).ToArgb();
        }

        // Standard-Textfarben (Schwarz/System), die durch das Theme-Textgrau ersetzt werden duerfen.
        private static bool IsLegacyFore(Color col)
        {
            int rgb = col.ToArgb();
            return rgb == Color.Black.ToArgb()
                || rgb == SystemColors.ControlText.ToArgb()
                || rgb == SystemColors.WindowText.ToArgb()
                || rgb == SystemColors.GrayText.ToArgb();
        }
    }
}
