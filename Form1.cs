using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game
{
    public partial class Game : Form
    {
        private int selectedOption = 0; // Numer aktualnie wybranej opcji
        private Label[] mainMenuOptions; // Opcje menu głównego
        private Label[] optionsMenuOptions; // Opcje menu opcji
        private Label[] levelSelectionOptions; // Opcje wyboru poziomu
        private bool inOptionsMenu = false; // Czy jesteśmy w menu opcji
        private bool inLevelSelectionMenu = false; // Czy jesteśmy w ekranie wyboru poziomu

        public Game()
        {
            InitializeComponent();

            // Tworzenie menu głównego
            mainMenuOptions = new Label[3];
            mainMenuOptions[0] = CreateLabel("Start Game", 100, 50);
            mainMenuOptions[1] = CreateLabel("Options", 100, 100);
            mainMenuOptions[2] = CreateLabel("Exit", 100, 150);

            // Tworzenie menu opcji
            optionsMenuOptions = new Label[3];
            optionsMenuOptions[0] = CreateLabel("Dźwięk: Włączony", 100, 50);
            optionsMenuOptions[1] = CreateLabel("Jasność: Normalna", 100, 100);
            optionsMenuOptions[2] = CreateLabel("Wróć", 100, 150);

            // Tworzenie ekranu wyboru poziomu
            levelSelectionOptions = new Label[4];
            levelSelectionOptions[0] = CreateLabel("Poziom 1", 100, 50);
            levelSelectionOptions[1] = CreateLabel("Poziom 2", 100, 100);
            levelSelectionOptions[2] = CreateLabel("Poziom 3", 100, 150);
            levelSelectionOptions[3] = CreateLabel("Wróć", 100, 200);

            // Dodanie menu głównego do formularza
            foreach (var label in mainMenuOptions)
            {
                this.Controls.Add(label);
            }

            // Podświetlenie pierwszej opcji
            HighlightOption(mainMenuOptions, 0);

            // Obsługa klawiszy
            this.KeyDown += new KeyEventHandler(Game_KeyDown);
        }

        // Funkcja tworząca etykiety menu
        private Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Black
            };
        }

        // Podświetlenie wybranej opcji
        private void HighlightOption(Label[] menu, int index)
        {
            for (int i = 0; i < menu.Length; i++)
            {
                menu[i].ForeColor = (i == index) ? Color.Yellow : Color.White;
            }
        }

        // Obsługa klawiszy
        private void Game_KeyDown(object sender, KeyEventArgs e)
        {
            Label[] currentMenu = inLevelSelectionMenu
                ? levelSelectionOptions
                : inOptionsMenu
                ? optionsMenuOptions
                : mainMenuOptions;

            if (e.KeyCode == Keys.Down) // Strzałka w dół
            {
                selectedOption = (selectedOption + 1) % currentMenu.Length;
                HighlightOption(currentMenu, selectedOption);
            }
            else if (e.KeyCode == Keys.Up) // Strzałka w górę
            {
                selectedOption = (selectedOption - 1 + currentMenu.Length) % currentMenu.Length;
                HighlightOption(currentMenu, selectedOption);
            }
            else if (e.KeyCode == Keys.Enter) // Enter
            {
                if (inLevelSelectionMenu)
                {
                    ExecuteLevelSelectionOption(selectedOption);
                }
                else if (inOptionsMenu)
                {
                    ExecuteOptionsMenuOption(selectedOption);
                }
                else
                {
                    ExecuteMainMenuOption(selectedOption);
                }
            }
        }

        // Funkcja wykonująca akcję w zależności od wybranej opcji w menu głównym
        private void ExecuteMainMenuOption(int option)
        {
            switch (option)
            {
                case 0: // Start Game
                    ShowLevelSelectionMenu();
                    break;
                case 1: // Options
                    ShowOptionsMenu();
                    break;
                case 2: // Exit
                    Application.Exit();
                    break;
            }
        }

        // Funkcja wykonująca akcję w zależności od wybranej opcji w menu opcji
        private void ExecuteOptionsMenuOption(int option)
        {
            switch (option)
            {
                case 0: // Dźwięk
                    MessageBox.Show("Opcja 'Dźwięk' jeszcze nie jest aktywna.");
                    break;
                case 1: // Jasność
                    MessageBox.Show("Opcja 'Jasność' jeszcze nie jest aktywna.");
                    break;
                case 2: // Wróć
                    ShowMainMenu();
                    break;
            }
        }

        // Funkcja wykonująca akcję w zależności od wybranego poziomu
        private void ExecuteLevelSelectionOption(int option)
        {
            switch (option)
            {
                case 0: // Poziom 1
                    MessageBox.Show("Wybrano Poziom 1!");
                    break;
                case 1: // Poziom 2
                    MessageBox.Show("Wybrano Poziom 2!");
                    break;
                case 2: // Poziom 3
                    MessageBox.Show("Wybrano Poziom 3!");
                    break;
                case 3: // Wróć
                    ShowMainMenu();
                    break;
            }
        }

        // Funkcja pokazująca menu opcji
        private void ShowOptionsMenu()
        {
            inOptionsMenu = true;
            selectedOption = 0;

            foreach (var label in mainMenuOptions)
            {
                label.Visible = false;
            }
            foreach (var label in optionsMenuOptions)
            {
                this.Controls.Add(label);
                label.Visible = true;
            }

            HighlightOption(optionsMenuOptions, 0);
        }

        // Funkcja pokazująca menu główne
        private void ShowMainMenu()
        {
            inOptionsMenu = false;
            inLevelSelectionMenu = false;
            selectedOption = 0;

            foreach (var label in optionsMenuOptions)
            {
                label.Visible = false;
            }
            foreach (var label in levelSelectionOptions)
            {
                label.Visible = false;
            }
            foreach (var label in mainMenuOptions)
            {
                label.Visible = true;
            }

            HighlightOption(mainMenuOptions, 0);
        }

        // Funkcja pokazująca ekran wyboru poziomu
        private void ShowLevelSelectionMenu()
        {
            inLevelSelectionMenu = true;
            selectedOption = 0;

            foreach (var label in mainMenuOptions)
            {
                label.Visible = false;
            }
            foreach (var label in levelSelectionOptions)
            {
                this.Controls.Add(label);
                label.Visible = true;
            }

            HighlightOption(levelSelectionOptions, 0);
        }
    }
}
