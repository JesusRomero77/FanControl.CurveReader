using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CurveReaderConfigurator
{
    public partial class ConfiguratorForm : Form
    {
        // Puntos que se resta al tamaño de la fuente de letra del desplegable del umbral respecto a la del formulario.
        private const float RowFontReduction = 1F;

        // Carpeta de FanControl elegida por el usuario; null mientras no se haya configurado.
        private string _fanControlFolder;

        // Ajustes que el usuario está editando para el perfil seleccionado; null si no hay ninguno.
        private ProfileConfig _profileConfig;

        // Ajustes de cada perfil ya abierto en esta sesión, con el nombre de archivo del perfil como clave.
        // Así, al volver a un perfil, se recuperan los cambios que el usuario hizo y aún no guardó.
        private readonly Dictionary<string, ProfileConfig> _openedProfileConfigs =
            new Dictionary<string, ProfileConfig>(StringComparer.OrdinalIgnoreCase);

        public ConfiguratorForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.CurveReaderConfigurator;
        }

        private void ConfiguratorForm_Load(object sender, EventArgs e)
        {
            // Idioma guardado por el usuario; si no hay ninguno, se mantiene el de por defecto (inglés).
            Translator.SetLanguage(AppSettings.LoadLanguage(Translator.CurrentLanguage));
            ApplyLanguage();

            // Si el usuario ya eligió antes la ruta de FanControl, se recupera para no pedirla de nuevo.
            // Se comprueba que siga siendo válida por si FanControl se ha movido o desinstalado.
            string savedFolder = AppSettings.LoadFanControlFolder();
            if (savedFolder != null && File.Exists(FanControlLocator.GetCachePath(savedFolder)))
            {
                _fanControlFolder = savedFolder;
                LoadProfiles();
            }
        }
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Menú Configuración > Especificar Ruta Fan Control: el usuario elige FanControl.exe
        // (o un acceso directo), se comprueba que junto a él exista Configurations\CACHE
        // y se cargan los perfiles en el desplegable.
        private void FanControlPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Selecciona el ejecutable de FanControl (o un acceso directo)";
                dialog.Filter = "FanControl.exe o acceso directo|*.exe;*.lnk";

                if (dialog.ShowDialog(this) != DialogResult.OK) return;

                string folder;
                if (!FanControlLocator.TryGetFanControlFolder(dialog.FileName, out folder))
                {
                    MessageBox.Show(this,
                        "No se ha encontrado la carpeta Configurations con el archivo CACHE junto a ese ejecutable. " +
                        "Comprueba que has elegido FanControl.exe.",
                        "Ruta no válida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _fanControlFolder = folder;
                AppSettings.SaveFanControlFolder(folder);
                LoadProfiles();
            }
        }

        // Rellena el desplegable con los perfiles de la carpeta que indica el CACHE y deja
        // seleccionado el perfil activo de FanControl.
        private void LoadProfiles()
        {
            profilesComboBox.Items.Clear();
            if (_fanControlFolder == null) return;

            string error;
            ProfileLocation location = ProfileLocator.Locate(_fanControlFolder, out error);
            if (location == null)
            {
                MessageBox.Show(this, "No se ha podido leer el CACHE de FanControl:\r\n" + error,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (string path in ProfileLocator.GetProfileFiles(location.ProfilesFolder))
            {
                ProfileItem item = new ProfileItem(path);
                int index = profilesComboBox.Items.Add(item);

                if (string.Equals(item.FileName, location.ActiveProfileFileName, StringComparison.OrdinalIgnoreCase))
                    profilesComboBox.SelectedIndex = index;
            }
        }

        // Escribe en cada menú y etiqueta el texto del idioma actual y marca cuál está activo.
        private void ApplyLanguage()
        {
            fileToolStripMenuItem.Text = Translator.Translate(TextId.File);
            saveToolStripMenuItem.Text = Translator.Translate(TextId.Save);
            exitToolStripMenuItem.Text = Translator.Translate(TextId.Exit);

            configurationToolStripMenuItem.Text = Translator.Translate(TextId.Settings);
            languageToolStripMenuItem.Text = Translator.Translate(TextId.Language);
            fanControlPathToolStripMenuItem.Text = Translator.Translate(TextId.FanControlPath);

            helpToolStripMenuItem.Text = Translator.Translate(TextId.Help);
            viewHelpToolStripMenuItem.Text = Translator.Translate(TextId.ViewHelp);
            aboutToolStripMenuItem.Text = Translator.Translate(TextId.About);

            profileLabel.Text = Translator.Translate(TextId.Profile);

            // El texto emergente del botón de recarga también cambia con el idioma.
            mainToolTip.SetToolTip(refreshProfilesButton, Translator.Translate(TextId.RefreshProfiles));

            // Los nombres de los idiomas no se traducen: cada uno aparece en su propio idioma,
            // así quien se encuentre la app en un idioma que no entiende puede encontrar el suyo.
            englishToolStripMenuItem.Text = "English";
            spanishToolStripMenuItem.Text = "Español";
            englishToolStripMenuItem.Checked = Translator.CurrentLanguage == AppLanguage.English;
            spanishToolStripMenuItem.Checked = Translator.CurrentLanguage == AppLanguage.Spanish;

            // Los textos del panel (títulos y notas) también cambian con el idioma.
            BuildProfileConfigPanel();
        }

        // Menú Configuración > Idioma > English: cambia la interfaz a inglés.
        private void EnglishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Translator.SetLanguage(AppLanguage.English);
            AppSettings.SaveLanguage(AppLanguage.English);
            ApplyLanguage();
        }

        // Menú Configuración > Idioma > Español: cambia la interfaz a español.
        private void SpanishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Translator.SetLanguage(AppLanguage.Spanish);
            AppSettings.SaveLanguage(AppLanguage.Spanish);
            ApplyLanguage();
        }

        // Al elegir un perfil: si ya se abrió antes en esta sesión, se recuperan sus ajustes con los cambios
        // pendientes de guardar. Si no, se lee su contenido, se cruzan sus controles y sensores con los
        // ajustes guardados (o los valores por defecto) y se recuerda el resultado. Después se reconstruye el panel.
        private void ProfilesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProfileItem selected = profilesComboBox.SelectedItem as ProfileItem;
            if (selected == null)
            {
                _profileConfig = null;
                BuildProfileConfigPanel();
                return;
            }

            ProfileConfig openedConfig;
            if (_openedProfileConfigs.TryGetValue(selected.FileName, out openedConfig))
            {
                _profileConfig = openedConfig;
                BuildProfileConfigPanel();
                return;
            }

            string error;
            ProfileContent content = ProfileReader.Read(selected.FilePath, out error);
            if (content == null)
            {
                MessageBox.Show(this, error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Si el archivo de configuración existe pero no se puede leer, se avisa y se siguen
            // mostrando los valores por defecto.
            LoggerConfig storedConfig = LoggerConfigFile.Load(out error);
            if (storedConfig == null)
            {
                MessageBox.Show(this, error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                storedConfig = new LoggerConfig();
            }

            _profileConfig = ProfileConfigBuilder.Build(
                selected.FileName, content, storedConfig.FindProfile(selected.FileName));
            _openedProfileConfigs[selected.FileName] = _profileConfig;
            BuildProfileConfigPanel();
        }

        // Menú Archivo > Guardar: escribe en el archivo de configuración del plugin los ajustes de todos los
        // perfiles abiertos en esta sesión. Parte de lo que ya hay guardado, para no borrar los perfiles que el
        // usuario no ha abierto, y sustituye (o añade) los abiertos. Si algo falla, avisa.
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_openedProfileConfigs.Count == 0) return;

            string error;
            LoggerConfig storedConfig = LoggerConfigFile.Load(out error);
            if (storedConfig == null)
            {
                MessageBox.Show(this, error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Cada perfil abierto reemplaza en su sitio al guardado con el mismo nombre de archivo,
            // o se añade al final si aún no tenía configuración.
            foreach (ProfileConfig openedConfig in _openedProfileConfigs.Values)
            {
                ProfileConfig existingConfig = storedConfig.FindProfile(openedConfig.FileName);
                if (existingConfig == null)
                    storedConfig.Profiles.Add(openedConfig);
                else
                    storedConfig.Profiles[storedConfig.Profiles.IndexOf(existingConfig)] = openedConfig;
            }

            if (!LoggerConfigFile.Save(storedConfig, out error))
                MessageBox.Show(this, error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // Reconstruye el panel desde cero a partir de _profileConfig: una tabla con la sección de
        // controles, la de curvas y la de sensores, en el mismo orden que FanControl.
        private void BuildProfileConfigPanel()
        {
            profileConfigPanel.SuspendLayout();

            // Se eliminan los controles anteriores (al eliminar uno sale solo de la lista).
            while (profileConfigPanel.Controls.Count > 0)
                profileConfigPanel.Controls[0].Dispose();

            if (_profileConfig != null)
            {
                TableLayoutPanel table = new TableLayoutPanel
                {
                    Dock = DockStyle.Top,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    ColumnCount = 4
                };
                table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));       // casilla de registro
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));  // nombre
                table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));       // intervalo
                table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));       // umbral

                AddSection(table, Translator.Translate(TextId.Controls), _profileConfig.Controls);
                AddCurvesSection(table);
                AddSection(table, Translator.Translate(TextId.Sensors), _profileConfig.Sensors);

                profileConfigPanel.Controls.Add(table);
            }

            profileConfigPanel.ResumeLayout();
        }

        // Añade una sección con filas: la cabecera y una fila por cada control o sensor.
        private void AddSection(TableLayoutPanel table, string title, List<ItemConfig> items)
        {
            AddSectionHeader(table, title);
            foreach (ItemConfig item in items)
                AddItemRow(table, item);
        }

        // Añade la sección de curvas: la misma cabecera que las demás, para dejarla preparada para
        // cuando las curvas se registren, y una nota indicando que de momento no se registran.
        private void AddCurvesSection(TableLayoutPanel table)
        {
            AddSectionHeader(table, Translator.Translate(TextId.Curves));

            Label note = new Label
            {
                Text = Translator.Translate(TextId.CurvesNotRecorded),
                AutoSize = true,
                UseMnemonic = false,
                ForeColor = SystemColors.GrayText
            };
            AddFullWidthRow(table, note);
        }

        // Añade la cabecera de una sección: encabezado de la casilla de registro, título de la sección
        // en negrita, encabezados del intervalo y del umbral, y una línea fina debajo.
        private void AddSectionHeader(TableLayoutPanel table, string title)
        {
            int headerRow = table.RowCount;
            table.RowCount = headerRow + 1;

            Label logHeaderLabel = new Label
            {
                Text = Translator.Translate(TextId.Log),
                AutoSize = true,
                UseMnemonic = false,
                Margin = new Padding(3, 12, 3, 3),
                Anchor = AnchorStyles.Top
            };
            Label titleLabel = new Label
            {
                Text = title,
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                UseMnemonic = false,
                Margin = new Padding(3, 12, 3, 3)
            };
            Label intervalHeaderLabel = new Label
            {
                Text = Translator.Translate(TextId.Interval),
                AutoSize = true,
                UseMnemonic = false,
                Margin = new Padding(3, 12, 3, 3),
                Anchor = AnchorStyles.Top
            };
            Label thresholdHeaderLabel = new Label
            {
                Text = Translator.Translate(TextId.ChangeThreshold),
                AutoSize = true,
                UseMnemonic = false,
                Margin = new Padding(3, 12, 3, 3),
                Anchor = AnchorStyles.Top
            };
            table.Controls.Add(logHeaderLabel, 0, headerRow);
            table.Controls.Add(titleLabel, 1, headerRow);
            table.Controls.Add(intervalHeaderLabel, 2, headerRow);
            table.Controls.Add(thresholdHeaderLabel, 3, headerRow);

            Label line = new Label { BorderStyle = BorderStyle.Fixed3D, AutoSize = false, Height = 2, Dock = DockStyle.Fill };
            AddFullWidthRow(table, line);
        }

        // Añade la fila de un control o sensor: casilla de registro, nombre, intervalo y umbral.
        // Cada cambio del usuario se escribe directamente en 'item'.
        private void AddItemRow(TableLayoutPanel table, ItemConfig item)
        {
            int row = table.RowCount;
            table.RowCount = row + 1;

            // La casilla va sola en su columna; Anchor = None la centra bajo el encabezado.
            CheckBox includeCheckBox = new CheckBox
            {
                Checked = item.IncludedInLog,
                AutoSize = true,
                Anchor = AnchorStyles.None
            };

            // UseMnemonic = false para que un "&" en el nombre se muestre tal cual.
            Label nameLabel = new Label
            {
                Text = item.Name,
                AutoSize = true,
                UseMnemonic = false,
                Anchor = AnchorStyles.Left
            };

            NumericUpDown intervalNumeric = CreateIntervalNumericUpDown(item.IntervalSeconds);
            ComboBox thresholdComboBox = CreateValueComboBox(
                Enumerable.Range(ItemConfig.MinThreshold, ItemConfig.MaxThreshold - ItemConfig.MinThreshold + 1),
                item.Threshold);

            // Refleja el estado de la casilla en el resto de la fila: si el elemento no se registra,
            // su nombre y el control del intervalo se sombrean y los dos controles se deshabilitan.
            Action updateRowAppearance = () =>
            {
                bool included = includeCheckBox.Checked;
                nameLabel.BackColor = included ? Color.Transparent : SystemColors.ControlDark;
                intervalNumeric.BackColor = included ? SystemColors.Window : SystemColors.ControlDark;
                intervalNumeric.Enabled = included;
                thresholdComboBox.Enabled = included;
            };
            updateRowAppearance();

            // Al marcar o desmarcar la casilla se guarda el cambio y se actualiza el aspecto de la fila.
            includeCheckBox.CheckedChanged += (sender, e) =>
            {
                item.IncludedInLog = includeCheckBox.Checked;
                updateRowAppearance();
            };

            // Al cambiar el intervalo o el umbral, se guarda el valor elegido.
            intervalNumeric.ValueChanged += (sender, e) =>
            {
                item.IntervalSeconds = (int)intervalNumeric.Value;
            };
            thresholdComboBox.SelectedIndexChanged += (sender, e) =>
            {
                item.Threshold = (int)thresholdComboBox.SelectedItem;
            };

            // Pulsar sobre el nombre también marca o desmarca la casilla, como si fuera su texto.
            nameLabel.Click += (sender, e) => includeCheckBox.Checked = !includeCheckBox.Checked;

            table.Controls.Add(includeCheckBox, 0, row);
            table.Controls.Add(nameLabel, 1, row);
            table.Controls.Add(intervalNumeric, 2, row);
            table.Controls.Add(thresholdComboBox, 3, row);
        }

        // Añade una fila con un único control que ocupa todo el ancho de la tabla.
        private static void AddFullWidthRow(TableLayoutPanel table, Control control)
        {
            int row = table.RowCount;
            table.RowCount = row + 1;
            table.Controls.Add(control, 0, row);
            table.SetColumnSpan(control, table.ColumnCount);
        }

        // Dibuja cada elemento del desplegable del umbral. Si el desplegable está deshabilitado, el
        // fondo es ControlDark, igual que el del nombre del elemento; si no, se usan los colores
        // normales de Windows, incluido el resaltado al pasar el ratón por la lista.
        private void RowComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (e.Index < 0) return;

            bool enabled = comboBox.Enabled;
            bool highlighted = enabled && (e.State & DrawItemState.Selected) != 0;

            Color backColor = !enabled ? SystemColors.ControlDark
                            : highlighted ? SystemColors.Highlight
                            : SystemColors.Window;
            Color textColor = !enabled ? SystemColors.ControlText
                            : highlighted ? SystemColors.HighlightText
                            : SystemColors.WindowText;

            using (SolidBrush brush = new SolidBrush(backColor))
                e.Graphics.FillRectangle(brush, e.Bounds);

            TextRenderer.DrawText(e.Graphics, comboBox.Items[e.Index].ToString(), e.Font, e.Bounds,
                textColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

            if (enabled) e.DrawFocusRectangle();
        }

        // Crea el control numérico del intervalo, con el mismo estilo de letra que el desplegable:
        // negrita y algo más pequeña que la del formulario.
        private NumericUpDown CreateIntervalNumericUpDown(int value)
        {
            Font numericFont = new Font(Font.FontFamily, Math.Max(Font.Size - RowFontReduction, 6F), FontStyle.Bold);

            // Minimum y Maximum se asignan antes que Value, porque Value debe estar dentro del rango.
            NumericUpDown numeric = new NumericUpDown
            {
                Font = numericFont,
                Minimum = ItemConfig.MinIntervalSeconds,
                Maximum = ItemConfig.MaxIntervalSeconds,
                Value = value,
                Width = 60,
                Anchor = AnchorStyles.None
            };
            numeric.Disposed += (sender, e) => numericFont.Dispose();

            return numeric;
        }

        // Crea un desplegable de fila con los valores indicados y uno seleccionado, con el estilo común:
        // letra en negrita algo más pequeña que la del formulario y fondo ControlDark al deshabilitarse.
        private ComboBox CreateValueComboBox(IEnumerable<int> values, int selectedValue)
        {
            Font comboFont = new Font(Font.FontFamily, Math.Max(Font.Size - RowFontReduction, 6F), FontStyle.Bold);

            ComboBox comboBox = new ComboBox
            {
                Font = comboFont,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = comboFont.Height + 2,
                Width = 60,
                Anchor = AnchorStyles.None
            };
            comboBox.DrawItem += RowComboBox_DrawItem;
            comboBox.Disposed += (sender, e) => comboFont.Dispose();

            foreach (int value in values)
                comboBox.Items.Add(value);
            comboBox.SelectedItem = selectedValue;

            return comboBox;
        }

        // Botón de recargar junto al desplegable de perfiles. Si ya se eligió la ruta de FanControl,
        // vuelve a leer el CACHE y recarga la lista de perfiles (y el perfil activo). Si aún no se ha
        // elegido, lanza el mismo clic del menú Configuración > Especificar ruta de FanControl, para
        // que el usuario la indique.
        private void RefreshProfilesButton_Click(object sender, EventArgs e)
        {
            if (_fanControlFolder == null)
            {
                fanControlPathToolStripMenuItem.PerformClick();
                return;
            }

            LoadProfiles();
        }

        // Menú Ayuda > Acerca de: abre la ventana con la información del programa.
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutForm aboutForm = new AboutForm())
                aboutForm.ShowDialog(this);
        }

        // Menú Ayuda > Ver la ayuda: abre la ventana con las instrucciones de uso.
        private void ViewHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (HelpForm helpForm = new HelpForm())
                helpForm.ShowDialog(this);
        }
    }
}
