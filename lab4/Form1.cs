using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Text;

namespace lab4
{
    public partial class Form1 : Form
    {
        // Main text editor control
        private RichTextBox textBox;
        
        // Layout panel
        private TableLayoutPanel mainLayout;
        
        // File handling variables
        private string currentFilePath = "";
        private bool isModified = false;
        private bool isNewFile = true;
        
        // Find and replace variables
        private string searchString = "";
        private string replaceString = "";
        private Form findDialog = null;
        private Form replaceDialog = null;
        
        // Status bar
        private StatusStrip statusBar;
        private ToolStripStatusLabel statusLabel;
        private ToolStripStatusLabel lineColumnLabel;
        
        // Menu and toolbar
        private MenuStrip menuStrip;
        private ToolStrip toolbar;
        
        // Panel for menu and toolbar
        private Panel topPanel;
        
        // Context menu
        private ContextMenuStrip contextMenu;

        public Form1()
        {
            InitializeComponent();
            CreateCustomComponents();
            SetupEventHandlers();
            UpdateTitle();
            UpdateStatus();
        }

        private void CreateCustomComponents()
        {
            // Form properties
            this.Text = "Безымянный — Блокнот";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.MinimumSize = new Size(400, 300);
            
            // Main layout panel
            mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.RowCount = 2;
            mainLayout.ColumnCount = 1;
            mainLayout.RowStyles.Clear();
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            
            // Top panel for menu and toolbar
            topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.AutoSize = true;
            topPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            
            // Create menu, toolbar, editor, status bar
            CreateMenu();
            CreateToolbar();
            CreateContextMenu();
            CreateTextEditor();
            CreateStatusBar();
            
            // Assemble top panel
            topPanel.Controls.Add(menuStrip);
            topPanel.Controls.Add(toolbar);
            menuStrip.Dock = DockStyle.Top;
            toolbar.Dock = DockStyle.Top;
            
            // Add to main layout
            mainLayout.Controls.Add(topPanel, 0, 0);
            mainLayout.Controls.Add(textBox, 0, 1);
            
            // Status bar at bottom
            statusBar.Dock = DockStyle.Bottom;
            this.Controls.Add(mainLayout);
            this.Controls.Add(statusBar);
            
            mainLayout.BringToFront();
            statusBar.BringToFront();
        }

        private void CreateMenu()
        {
            menuStrip = new MenuStrip();
            menuStrip.TabIndex = 0;
            
            // File menu
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("Файл");
            fileMenu.DropDownItems.AddRange(new ToolStripMenuItem[] {
                new ToolStripMenuItem("Новый", null, NewFile_Click) { ShortcutKeys = Keys.Control | Keys.N, ShortcutKeyDisplayString = "Ctrl+N" },
                new ToolStripMenuItem("Открыть...", null, OpenFile_Click) { ShortcutKeys = Keys.Control | Keys.O, ShortcutKeyDisplayString = "Ctrl+O" },
                new ToolStripMenuItem("Сохранить", null, SaveFile_Click) { ShortcutKeys = Keys.Control | Keys.S, ShortcutKeyDisplayString = "Ctrl+S" },
                new ToolStripMenuItem("Сохранить как...", null, SaveAsFile_Click),
                new ToolStripMenuItem("-"),
                new ToolStripMenuItem("Параметры страницы...", null, PageSetup_Click),
                new ToolStripMenuItem("Печать...", null, Print_Click) { ShortcutKeys = Keys.Control | Keys.P, ShortcutKeyDisplayString = "Ctrl+P" },
                new ToolStripMenuItem("-"),
                new ToolStripMenuItem("Выход", null, Exit_Click)
            });
            
            // Edit menu
            ToolStripMenuItem editMenu = new ToolStripMenuItem("Правка");
            editMenu.DropDownItems.AddRange(new ToolStripMenuItem[] {
                new ToolStripMenuItem("Отменить", null, Undo_Click) { ShortcutKeys = Keys.Control | Keys.Z, ShortcutKeyDisplayString = "Ctrl+Z" },
                new ToolStripMenuItem("-"),
                new ToolStripMenuItem("Вырезать", null, Cut_Click) { ShortcutKeys = Keys.Control | Keys.X, ShortcutKeyDisplayString = "Ctrl+X" },
                new ToolStripMenuItem("Копировать", null, Copy_Click) { ShortcutKeys = Keys.Control | Keys.C, ShortcutKeyDisplayString = "Ctrl+C" },
                new ToolStripMenuItem("Вставить", null, Paste_Click) { ShortcutKeys = Keys.Control | Keys.V, ShortcutKeyDisplayString = "Ctrl+V" },
                new ToolStripMenuItem("Удалить", null, Delete_Click),
                new ToolStripMenuItem("-"),
                new ToolStripMenuItem("Найти...", null, Find_Click) { ShortcutKeys = Keys.Control | Keys.F, ShortcutKeyDisplayString = "Ctrl+F" },
                new ToolStripMenuItem("Найти далее", null, FindNext_Click) { ShortcutKeys = Keys.F3, ShortcutKeyDisplayString = "F3" },
                new ToolStripMenuItem("Заменить...", null, Replace_Click) { ShortcutKeys = Keys.Control | Keys.H, ShortcutKeyDisplayString = "Ctrl+H" },
                new ToolStripMenuItem("Перейти...", null, GoTo_Click) { ShortcutKeys = Keys.Control | Keys.G, ShortcutKeyDisplayString = "Ctrl+G" },
                new ToolStripMenuItem("-"),
                new ToolStripMenuItem("Выделить всё", null, SelectAll_Click) { ShortcutKeys = Keys.Control | Keys.A, ShortcutKeyDisplayString = "Ctrl+A" },
                new ToolStripMenuItem("Время и дата", null, TimeDate_Click) { ShortcutKeys = Keys.F5, ShortcutKeyDisplayString = "F5" }
            });
            
            // Format menu
            ToolStripMenuItem formatMenu = new ToolStripMenuItem("Формат");
            formatMenu.DropDownItems.AddRange(new ToolStripMenuItem[] {
                new ToolStripMenuItem("Перенос по словам", null, WordWrap_Click) { CheckOnClick = true, Checked = true },
                new ToolStripMenuItem("-"),
                new ToolStripMenuItem("Шрифт...", null, Font_Click),
                new ToolStripMenuItem("Цвет текста...", null, TextColor_Click)
            });
            
            // View menu
            ToolStripMenuItem viewMenu = new ToolStripMenuItem("Вид");
            ToolStripMenuItem statusBarMenuItem = new ToolStripMenuItem("Строка состояния", null, StatusBar_Click);
            statusBarMenuItem.CheckOnClick = true;
            statusBarMenuItem.Checked = true;
            viewMenu.DropDownItems.Add(statusBarMenuItem);
            
            // Help menu
            ToolStripMenuItem helpMenu = new ToolStripMenuItem("Справка");
            helpMenu.DropDownItems.AddRange(new ToolStripMenuItem[] {
                new ToolStripMenuItem("Просмотр справки", null, ViewHelp_Click),
                new ToolStripMenuItem("О программе", null, About_Click)
            });
            
            menuStrip.Items.AddRange(new ToolStripMenuItem[] { fileMenu, editMenu, formatMenu, viewMenu, helpMenu });
        }

        private void CreateToolbar()
        {
            toolbar = new ToolStrip();
            toolbar.GripStyle = ToolStripGripStyle.Hidden;
            toolbar.TabIndex = 1;
            
            ToolStripButton newBtn = new ToolStripButton("Новый", null, NewFile_Click) { ToolTipText = "Создать новый файл (Ctrl+N)", DisplayStyle = ToolStripItemDisplayStyle.Text };
            ToolStripButton openBtn = new ToolStripButton("Открыть", null, OpenFile_Click) { ToolTipText = "Открыть существующий файл (Ctrl+O)", DisplayStyle = ToolStripItemDisplayStyle.Text };
            ToolStripButton saveBtn = new ToolStripButton("Сохранить", null, SaveFile_Click) { ToolTipText = "Сохранить текущий файл (Ctrl+S)", DisplayStyle = ToolStripItemDisplayStyle.Text };
            ToolStripSeparator sep1 = new ToolStripSeparator();
            ToolStripButton cutBtn = new ToolStripButton("Вырезать", null, Cut_Click) { ToolTipText = "Вырезать выделенный текст (Ctrl+X)", DisplayStyle = ToolStripItemDisplayStyle.Text };
            ToolStripButton copyBtn = new ToolStripButton("Копировать", null, Copy_Click) { ToolTipText = "Копировать выделенный текст (Ctrl+C)", DisplayStyle = ToolStripItemDisplayStyle.Text };
            ToolStripButton pasteBtn = new ToolStripButton("Вставить", null, Paste_Click) { ToolTipText = "Вставить текст из буфера обмена (Ctrl+V)", DisplayStyle = ToolStripItemDisplayStyle.Text };
            ToolStripSeparator sep2 = new ToolStripSeparator();
            ToolStripButton undoBtn = new ToolStripButton("Отменить", null, Undo_Click) { ToolTipText = "Отменить последнее действие (Ctrl+Z)", DisplayStyle = ToolStripItemDisplayStyle.Text };
            ToolStripSeparator sep3 = new ToolStripSeparator();
            ToolStripButton findBtn = new ToolStripButton("Найти", null, Find_Click) { ToolTipText = "Найти текст (Ctrl+F)", DisplayStyle = ToolStripItemDisplayStyle.Text };
            
            toolbar.Items.AddRange(new ToolStripItem[] { newBtn, openBtn, saveBtn, sep1, cutBtn, copyBtn, pasteBtn, sep2, undoBtn, sep3, findBtn });
        }

        private void CreateContextMenu()
        {
            contextMenu = new ContextMenuStrip();
            contextMenu.Items.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("Отменить", null, Undo_Click),
                new ToolStripMenuItem("-"),
                new ToolStripMenuItem("Вырезать", null, Cut_Click),
                new ToolStripMenuItem("Копировать", null, Copy_Click),
                new ToolStripMenuItem("Вставить", null, Paste_Click),
                new ToolStripMenuItem("Удалить", null, Delete_Click),
                new ToolStripMenuItem("-"),
                new ToolStripMenuItem("Выделить всё", null, SelectAll_Click),
                new ToolStripMenuItem("Найти...", null, Find_Click)
            });
        }

        private void CreateTextEditor()
        {
            textBox = new RichTextBox();
            textBox.Dock = DockStyle.Fill;
            textBox.Font = new Font("Consolas", 11);
            textBox.ForeColor = Color.Black;
            textBox.AcceptsTab = true;
            textBox.WordWrap = true;
            textBox.Multiline = true;
            textBox.ScrollBars = RichTextBoxScrollBars.ForcedBoth;
            textBox.BorderStyle = BorderStyle.None;
            textBox.ContextMenuStrip = contextMenu;
            textBox.HideSelection = false;   // keep selection highlighted when dialog is open
            textBox.TextChanged += TextBox_TextChanged;
            textBox.SelectionChanged += TextBox_SelectionChanged;
            textBox.KeyDown += TextBox_KeyDown;
        }

        private void CreateStatusBar()
        {
            statusBar = new StatusStrip();
            statusLabel = new ToolStripStatusLabel("Готово");
            lineColumnLabel = new ToolStripStatusLabel("Строка 1, столбец 1");
            statusBar.Items.Add(statusLabel);
            statusBar.Items.Add(new ToolStripStatusLabel(" "));
            statusBar.Items.Add(lineColumnLabel);
        }

        private void SetupEventHandlers()
        {
            this.FormClosing += NotepadClone_FormClosing;
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (!isModified) { isModified = true; UpdateTitle(); }
            UpdateStatus();
        }

        private void TextBox_SelectionChanged(object sender, EventArgs e) => UpdateStatus();

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z) { Undo_Click(sender, e); e.SuppressKeyPress = true; }
            else if (e.Control && e.KeyCode == Keys.X) { Cut_Click(sender, e); e.SuppressKeyPress = true; }
            else if (e.Control && e.KeyCode == Keys.C) { Copy_Click(sender, e); e.SuppressKeyPress = true; }
            else if (e.Control && e.KeyCode == Keys.V) { Paste_Click(sender, e); e.SuppressKeyPress = true; }
            else if (e.Control && e.KeyCode == Keys.A) { SelectAll_Click(sender, e); e.SuppressKeyPress = true; }
            else if (e.KeyCode == Keys.F3) { FindNext_Click(sender, e); e.SuppressKeyPress = true; }
            else if (e.KeyCode == Keys.F5) { TimeDate_Click(sender, e); e.SuppressKeyPress = true; }
        }

        private void NotepadClone_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isModified)
            {
                DialogResult result = MessageBox.Show($"Сохранить изменения в файле \"{GetDisplayFileName()}\"?", "Блокнот",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes) SaveFile();
                else if (result == DialogResult.Cancel) e.Cancel = true;
            }
        }

        // ============ FILE ACTIONS ============
        private void NewFile_Click(object sender, EventArgs e)
        {
            if (CheckSaveChanges())
            {
                textBox.Clear();
                currentFilePath = "";
                isModified = false;
                isNewFile = true;
                searchString = replaceString = "";
                UpdateTitle(); UpdateStatus();
            }
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {
            if (!CheckSaveChanges()) return;
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                dlg.Title = "Открыть";
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Read as UTF-8 with BOM support
                        string content = File.ReadAllText(dlg.FileName, Encoding.UTF8);
                        textBox.Text = content;
                        currentFilePath = dlg.FileName;
                        isModified = false; isNewFile = false;
                        searchString = replaceString = "";
                        UpdateTitle();
                        statusLabel.Text = $"Открыт: {Path.GetFileName(currentFilePath)}";
                    }
                    catch (Exception ex) { MessageBox.Show($"Не удается открыть файл.\n\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
        }

        private void SaveFile_Click(object sender, EventArgs e) => SaveFile();
        private void SaveAsFile_Click(object sender, EventArgs e) => SaveFileAs();

        private bool SaveFile()
        {
            if (string.IsNullOrEmpty(currentFilePath)) return SaveFileAs();
            try
            {
                // Save as UTF-8 with BOM (better Windows Notepad compatibility)
                File.WriteAllText(currentFilePath, textBox.Text, new UTF8Encoding(true));
                isModified = false;
                UpdateTitle();
                statusLabel.Text = $"Сохранено: {Path.GetFileName(currentFilePath)}";
                return true;
            }
            catch (Exception ex) { MessageBox.Show($"Не удается сохранить файл.\n\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
        }

        private bool SaveFileAs()
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                dlg.Title = "Сохранить как";
                dlg.FileName = string.IsNullOrEmpty(currentFilePath) ? "Безымянный.txt" : Path.GetFileName(currentFilePath);
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(dlg.FileName, textBox.Text, new UTF8Encoding(true));
                        currentFilePath = dlg.FileName;
                        isModified = false; isNewFile = false;
                        UpdateTitle();
                        statusLabel.Text = $"Сохранено: {Path.GetFileName(currentFilePath)}";
                        return true;
                    }
                    catch (Exception ex) { MessageBox.Show($"Не удается сохранить файл.\n\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
                }
            }
            return false;
        }

        private void PageSetup_Click(object sender, EventArgs e) => MessageBox.Show("Функция настройки страницы будет доступна в следующей версии.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        private void Print_Click(object sender, EventArgs e) => MessageBox.Show("Функция печати будет доступна в следующей версии.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        private void Exit_Click(object sender, EventArgs e) => this.Close();

        // ============ EDIT ACTIONS ============
        private void Undo_Click(object sender, EventArgs e) { if (textBox.CanUndo) { textBox.Undo(); textBox.ClearUndo(); } }
        private void Cut_Click(object sender, EventArgs e) { if (textBox.SelectedText.Length > 0) textBox.Cut(); }
        private void Copy_Click(object sender, EventArgs e) { if (textBox.SelectedText.Length > 0) textBox.Copy(); }
        private void Paste_Click(object sender, EventArgs e) => textBox.Paste();
        private void Delete_Click(object sender, EventArgs e) { if (textBox.SelectedText.Length > 0) textBox.SelectedText = ""; }
        private void Find_Click(object sender, EventArgs e) => ShowFindDialog();
        private void FindNext_Click(object sender, EventArgs e) { if (!string.IsNullOrEmpty(searchString)) FindNext(true); else ShowFindDialog(); }
        private void Replace_Click(object sender, EventArgs e) => ShowReplaceDialog();
        private void GoTo_Click(object sender, EventArgs e) { if (textBox.Lines.Length > 0) ShowGoToDialog(); else MessageBox.Show("В документе нет текста.", "Блокнот", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void SelectAll_Click(object sender, EventArgs e) => textBox.SelectAll();
        private void TimeDate_Click(object sender, EventArgs e) => textBox.SelectedText = DateTime.Now.ToString("HH:mm dd.MM.yyyy");

        // ============ FORMAT ACTIONS ============
        private void WordWrap_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            textBox.WordWrap = item.Checked;
            textBox.ScrollBars = item.Checked ? RichTextBoxScrollBars.Vertical : RichTextBoxScrollBars.ForcedBoth;
        }

        private void Font_Click(object sender, EventArgs e)
        {
            using (FontDialog dlg = new FontDialog())
            {
                if (textBox.SelectionLength > 0 && textBox.SelectionFont != null)
                    dlg.Font = textBox.SelectionFont;
                else
                    dlg.Font = textBox.Font;
                dlg.ShowColor = false;
                dlg.ShowEffects = false;
                if (dlg.ShowDialog() == DialogResult.OK)
                    textBox.SelectionFont = dlg.Font;
            }
        }

        private void TextColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog dlg = new ColorDialog())
            {
                if (textBox.SelectionLength > 0)
                    dlg.Color = textBox.SelectionColor;
                else
                    dlg.Color = textBox.ForeColor;
                if (dlg.ShowDialog() == DialogResult.OK)
                    textBox.SelectionColor = dlg.Color;
            }
        }

        // ============ VIEW ============
        private void StatusBar_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            statusBar.Visible = item.Checked;
        }

        // ============ HELP ============
        private void ViewHelp_Click(object sender, EventArgs e)
        {
            string msg = "=== Сочетания клавиш ===\n\n" +
                "Ctrl+N   - Новый файл\nCtrl+O   - Открыть файл\nCtrl+S   - Сохранить файл\n" +
                "Ctrl+P   - Печать\nCtrl+Z   - Отменить\nCtrl+X   - Вырезать\nCtrl+C   - Копировать\n" +
                "Ctrl+V   - Вставить\nCtrl+A   - Выделить всё\nCtrl+F   - Найти\nCtrl+H   - Заменить\n" +
                "Ctrl+G   - Перейти\nF3       - Найти далее\nF5       - Время и дата\n\n" +
                "=== Форматирование ===\n\n" +
                "• Шрифт и цвет применяются К ВЫДЕЛЕННОМУ ТЕКСТУ.\n" +
                "• Если ничего не выделено – изменения будут применены к новому тексту (курсор).";
            MessageBox.Show(msg, "Справка", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void About_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Блокнот\nВерсия 2.0\n\nПолнофункциональный клон Windows Notepad\n" +
                "Разработано для лабораторной работы №4\n\nОсобенности:\n" +
                "• Полная поддержка UTF-8\n• Поиск и замена с учётом регистра\n" +
                "• Настраиваемый шрифт и цвет текста (только для выделенного фрагмента)\n" +
                "• Перенос по словам\n• Строка состояния\n\n© 2024", "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ============ FIND/REPLACE DIALOGS ============
        private void ShowFindDialog()
        {
            if (findDialog == null || findDialog.IsDisposed)
            {
                findDialog = new Form { Text = "Найти", Size = new Size(500, 150), FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false, StartPosition = FormStartPosition.CenterParent, Font = new Font("Segoe UI", 9) };
                var tlp = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Padding = new Padding(10) };
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60));
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                var label = new Label { Text = "Что:", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill };
                var searchBox = new TextBox { Dock = DockStyle.Fill, Text = searchString, Font = new Font("Segoe UI", 10) };
                var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0, 5, 0, 0) };
                var findNext = new Button { Text = "Найти далее", Size = new Size(100, 30), FlatStyle = FlatStyle.System };
                var findPrev = new Button { Text = "Найти назад", Size = new Size(100, 30), FlatStyle = FlatStyle.System };
                var cancel = new Button { Text = "Отмена", Size = new Size(100, 30), FlatStyle = FlatStyle.System };
                buttonPanel.Controls.AddRange(new Control[] { cancel, findPrev, findNext });
                tlp.Controls.Add(label, 0, 0);
                tlp.Controls.Add(searchBox, 1, 0);
                tlp.Controls.Add(buttonPanel, 1, 1);
                findNext.Click += (s, ev) => { searchString = searchBox.Text; if (!string.IsNullOrEmpty(searchString)) FindNext(true); else MessageBox.Show("Введите текст для поиска.", "Найти", MessageBoxButtons.OK, MessageBoxIcon.Information); };
                findPrev.Click += (s, ev) => { searchString = searchBox.Text; if (!string.IsNullOrEmpty(searchString)) FindNext(false); else MessageBox.Show("Введите текст для поиска.", "Найти", MessageBoxButtons.OK, MessageBoxIcon.Information); };
                cancel.Click += (s, ev) => findDialog.Close();
                searchBox.KeyDown += (s, ev) => { if (ev.KeyCode == Keys.Enter) { searchString = searchBox.Text; if (!string.IsNullOrEmpty(searchString)) FindNext(true); } };
                findDialog.Controls.Add(tlp);
            }
            findDialog.ShowDialog(this);
        }

        private void ShowReplaceDialog()
        {
            if (replaceDialog == null || replaceDialog.IsDisposed)
            {
                replaceDialog = new Form { Text = "Заменить", Size = new Size(500, 200), FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false, StartPosition = FormStartPosition.CenterParent, Font = new Font("Segoe UI", 9) };
                var tlp = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3, Padding = new Padding(10) };
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60));
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                var findLabel = new Label { Text = "Что:", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill };
                var findBox = new TextBox { Dock = DockStyle.Fill, Text = searchString, Font = new Font("Segoe UI", 10) };
                var replaceLabel = new Label { Text = "Чем:", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill };
                var replaceBox = new TextBox { Dock = DockStyle.Fill, Text = replaceString, Font = new Font("Segoe UI", 10) };
                var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0, 5, 0, 0) };
                var findNext = new Button { Text = "Найти далее", Size = new Size(100, 30), FlatStyle = FlatStyle.System };
                var findPrev = new Button { Text = "Найти назад", Size = new Size(100, 30), FlatStyle = FlatStyle.System };
                var replace = new Button { Text = "Заменить", Size = new Size(100, 30), FlatStyle = FlatStyle.System };
                var replaceAll = new Button { Text = "Заменить все", Size = new Size(100, 30), FlatStyle = FlatStyle.System };
                var cancel = new Button { Text = "Отмена", Size = new Size(100, 30), FlatStyle = FlatStyle.System };
                buttonPanel.Controls.AddRange(new Control[] { cancel, replaceAll, replace, findPrev, findNext });
                tlp.Controls.Add(findLabel, 0, 0);
                tlp.Controls.Add(findBox, 1, 0);
                tlp.Controls.Add(replaceLabel, 0, 1);
                tlp.Controls.Add(replaceBox, 1, 1);
                tlp.Controls.Add(buttonPanel, 1, 2);
                findNext.Click += (s, ev) => { searchString = findBox.Text; if (!string.IsNullOrEmpty(searchString)) FindNext(true); else MessageBox.Show("Введите текст для поиска.", "Заменить", MessageBoxButtons.OK, MessageBoxIcon.Information); };
                findPrev.Click += (s, ev) => { searchString = findBox.Text; if (!string.IsNullOrEmpty(searchString)) FindNext(false); };
                replace.Click += (s, ev) => { searchString = findBox.Text; replaceString = replaceBox.Text; if (!string.IsNullOrEmpty(searchString)) ReplaceCurrent(); };
                replaceAll.Click += (s, ev) => { searchString = findBox.Text; replaceString = replaceBox.Text; if (!string.IsNullOrEmpty(searchString)) ReplaceAll(); };
                cancel.Click += (s, ev) => replaceDialog.Close();
                replaceDialog.Controls.Add(tlp);
            }
            replaceDialog.ShowDialog(this);
        }

        private void ShowGoToDialog()
        {
            var goToDialog = new Form { Text = "Перейти", Size = new Size(350, 130), FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false, StartPosition = FormStartPosition.CenterParent, Font = new Font("Segoe UI", 9) };
            var tlp = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Padding = new Padding(10) };
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            var label = new Label { Text = "Номер строки:", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill };
            var num = new NumericUpDown { Dock = DockStyle.Fill, Minimum = 1, Maximum = Math.Max(1, textBox.Lines.Length), Value = Math.Min(textBox.GetLineFromCharIndex(textBox.SelectionStart) + 1, textBox.Lines.Length) };
            // Update maximum when the user types – just in case lines were added while dialog is open
            num.Enter += (s, ev) => { num.Maximum = Math.Max(1, textBox.Lines.Length); };
            var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            var go = new Button { Text = "Перейти", Size = new Size(90, 30), FlatStyle = FlatStyle.System };
            var cancel = new Button { Text = "Отмена", Size = new Size(90, 30), FlatStyle = FlatStyle.System };
            buttonPanel.Controls.Add(cancel);
            buttonPanel.Controls.Add(go);
            tlp.Controls.Add(label, 0, 0);
            tlp.Controls.Add(num, 1, 0);
            tlp.Controls.Add(buttonPanel, 1, 1);
            go.Click += (s, ev) => { int line = (int)num.Value - 1; if (line >= 0 && line < textBox.Lines.Length) { int pos = textBox.GetFirstCharIndexFromLine(line); textBox.Select(pos, 0); textBox.ScrollToCaret(); goToDialog.Close(); } };
            cancel.Click += (s, ev) => goToDialog.Close();
            goToDialog.Controls.Add(tlp);
            goToDialog.ShowDialog(this);
        }

        private void FindNext(bool forward)
        {
            if (string.IsNullOrEmpty(searchString)) return;
            int start = forward ? textBox.SelectionStart + textBox.SelectionLength : textBox.SelectionStart;
            if (start >= textBox.Text.Length) start = 0;
            if (start < 0) start = 0;
            int idx = forward ? textBox.Text.IndexOf(searchString, start, StringComparison.OrdinalIgnoreCase)
                              : textBox.Text.LastIndexOf(searchString, start, StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                textBox.Select(idx, searchString.Length);
                textBox.ScrollToCaret();
                statusLabel.Text = $"Найдено: \"{searchString}\"";
            }
            else
            {
                MessageBox.Show($"Не удается найти \"{searchString}\"", "Блокнот", MessageBoxButtons.OK, MessageBoxIcon.Information);
                statusLabel.Text = "Поиск завершен";
            }
        }

        private void ReplaceCurrent()
        {
            if (string.IsNullOrEmpty(searchString)) return;
            if (textBox.SelectedText.Equals(searchString, StringComparison.OrdinalIgnoreCase))
                textBox.SelectedText = replaceString;
            FindNext(true);
        }

        private void ReplaceAll()
        {
            if (string.IsNullOrEmpty(searchString)) return;
            int originalStart = textBox.SelectionStart;
            int count = 0;
            string content = textBox.Text;
            int idx = 0;
            while ((idx = content.IndexOf(searchString, idx, StringComparison.OrdinalIgnoreCase)) >= 0)
            {
                count++;
                idx += searchString.Length;
            }
            if (count > 0)
            {
                string newContent = content.Replace(searchString, replaceString);
                textBox.Text = newContent;
                MessageBox.Show($"Заменено вхождений: {count}", "Блокнот", MessageBoxButtons.OK, MessageBoxIcon.Information);
                statusLabel.Text = $"Заменено: {count} вхождений";
                if (originalStart <= newContent.Length) textBox.Select(originalStart, 0);
            }
            else MessageBox.Show($"Не найдено \"{searchString}\"", "Блокнот", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ============ HELPER METHODS ============
        private bool CheckSaveChanges()
        {
            if (isModified)
            {
                DialogResult res = MessageBox.Show($"Сохранить изменения в файле \"{GetDisplayFileName()}\"?", "Блокнот", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (res == DialogResult.Yes) return SaveFile();
                else if (res == DialogResult.Cancel) return false;
            }
            return true;
        }

        private void UpdateTitle()
        {
            string name = GetDisplayFileName();
            this.Text = (isModified ? "*" : "") + name + " — Блокнот";
        }

        private string GetDisplayFileName() => string.IsNullOrEmpty(currentFilePath) ? "Безымянный" : Path.GetFileName(currentFilePath);

        private void UpdateStatus()
        {
            if (statusBar?.Visible == true && textBox != null)
            {
                try
                {
                    int line = textBox.GetLineFromCharIndex(textBox.SelectionStart) + 1;
                    int col = textBox.SelectionStart - textBox.GetFirstCharIndexFromLine(line - 1) + 1;
                    lineColumnLabel.Text = $"Строка {line}, столбец {col}";
                    int sel = textBox.SelectionLength;
                    statusLabel.Text = sel > 0 ? $"Выделено: {sel} символов" : "Готово";
                }
                catch { lineColumnLabel.Text = "Строка 1, столбец 1"; statusLabel.Text = "Готово"; }
            }
        }
    }
}