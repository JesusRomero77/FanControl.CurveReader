using System;
using System.Drawing;
using System.Windows.Forms;

namespace CurveReaderConfigurator
{
    /// <summary>
    /// Ventana de ayuda: una serie de secciones (título en negrita + texto explicativo) con el
    /// funcionamiento básico del programa, dentro de un recuadro con fondo algo más oscuro que
    /// el resto de la ventana. Se muestra siempre en el idioma que tenga activo el formulario
    /// principal en ese momento.
    /// </summary>
    public class HelpForm : Form
    {
        // Cuánto se oscurece el fondo de la ventana para el recuadro del texto de ayuda.
        // Mismo valor que usa el recuadro de la descripción en AboutForm.
        private const int ContentPanelDarkening = 15;

        // Cuántos puntos se suma al tamaño de letra del formulario para el texto de ayuda.
        private const float HelpFontIncrease = 2F;

        public HelpForm()
        {
            BuildLayout();
        }

        // Construye el recuadro con las cinco secciones y, debajo, el botón OK. Cada control se
        // añade al formulario (o al panel) justo después de crearlo y antes de usar su tamaño
        // para colocar el siguiente: un control con AutoSize no tiene su tamaño real hasta que
        // está en un contenedor.
        private void BuildLayout()
        {
            Text = Translator.Translate(TextId.HelpWindowTitle);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Icon = Properties.Resources.CurveReaderConfigurator;

            const int margin = 20;
            const int contentWidth = 420;

            Panel contentPanel = BuildContentPanel(margin, margin, contentWidth);
            Controls.Add(contentPanel);

            Button okButton = new Button
            {
                Text = Translator.Translate(TextId.Ok),
                AutoSize = true,
                DialogResult = DialogResult.OK
            };
            Controls.Add(okButton);
            okButton.Location = new Point(contentPanel.Right - okButton.Width, contentPanel.Bottom + 20);

            AcceptButton = okButton;
            ClientSize = new Size(contentPanel.Right + margin, okButton.Bottom + margin);
        }

        // Crea el recuadro con las cinco secciones dentro, con fondo algo más oscuro que el de
        // la ventana. El ancho es fijo (igual que el recuadro de AboutForm), y el alto se calcula
        // a partir de dónde termina la última sección.
        private Panel BuildContentPanel(int left, int top, int textWidth)
        {
            const int padding = 10;

            Panel panel = new Panel
            {
                BackColor = Darken(BackColor, ContentPanelDarkening),
                Location = new Point(left, top)
            };

            Font titleFont = new Font(Font.FontFamily, Font.Size + HelpFontIncrease, FontStyle.Bold);
            Font bodyFont = new Font(Font.FontFamily, Font.Size + HelpFontIncrease);
            panel.Disposed += (sender, e) => { titleFont.Dispose(); bodyFont.Dispose(); };

            int sectionTop = padding;
            sectionTop = AddSection(panel, TextId.HelpRequirementsTitle, TextId.HelpRequirementsText,
                titleFont, bodyFont, padding, sectionTop, textWidth) + 15;
            sectionTop = AddSection(panel, TextId.HelpGettingStartedTitle, TextId.HelpGettingStartedText,
                titleFont, bodyFont, padding, sectionTop, textWidth) + 15;
            sectionTop = AddSection(panel, TextId.HelpSelectProfileTitle, TextId.HelpSelectProfileText,
                titleFont, bodyFont, padding, sectionTop, textWidth) + 15;
            sectionTop = AddSection(panel, TextId.HelpConfigureItemsTitle, TextId.HelpConfigureItemsText,
                titleFont, bodyFont, padding, sectionTop, textWidth) + 15;
            int lastBottom = AddSection(panel, TextId.HelpSaveTitle, TextId.HelpSaveText,
                titleFont, bodyFont, padding, sectionTop, textWidth);

            panel.Size = new Size(textWidth + padding * 2, lastBottom + padding);
            return panel;
        }

        // Añade una sección (título en negrita + texto normal debajo) dentro del panel indicado
        // y devuelve la posición Y de su borde inferior, para poder colocar la siguiente sección.
        private int AddSection(Panel panel, TextId titleId, TextId textId, Font titleFont, Font bodyFont,
            int left, int top, int width)
        {
            Label titleLabel = new Label
            {
                Text = Translator.Translate(titleId),
                Font = titleFont,
                AutoSize = true,
                UseMnemonic = false,
                Location = new Point(left, top)
            };
            panel.Controls.Add(titleLabel);

            Label bodyLabel = new Label
            {
                Text = Translator.Translate(textId),
                Font = bodyFont,
                AutoSize = true,
                UseMnemonic = false,
                MaximumSize = new Size(width, 0),
                Location = new Point(left, titleLabel.Bottom + 5)
            };
            panel.Controls.Add(bodyLabel);

            return bodyLabel.Bottom;
        }

        // Resta 'amount' a cada componente de color, sin bajar de 0, para obtener un tono
        // ligeramente más oscuro que el original.
        private static Color Darken(Color baseColor, int amount)
        {
            return Color.FromArgb(
                Math.Max(baseColor.R - amount, 0),
                Math.Max(baseColor.G - amount, 0),
                Math.Max(baseColor.B - amount, 0));
        }
    }
}