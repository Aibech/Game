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

        private Timer gameTimer; // Timer do odliczania czasu
        private int remainingTime = 10; // Pozostały czas w sekundach
        private Label timeLabel; // Etykieta do wyświetlania czasu
        private bool isPaused = false; // Czy gra jest w stanie pauzy

        private Label levelNameLabel; // Pole na nazwę poziomu
        private Label levelProgressLabel; // Pole na postęp poziomu
        private int wordsGuessed = 0; // Ile słów odgadnięto
        private int totalWords = 6; // Łączna liczba słów do odgadnięcia w poziomie
        private int currentLevel = 1; // Aktualny poziom gry
        private bool[] unlockedLevels = new bool[3] { true, false, false }; // Poziomy odblokowane


        public Game()
        {
            InitializeComponent();

            levelProgressLabel = new Label
            {
                Text = $"{wordsGuessed}/{totalWords}",
                Location = new Point(200, 10),
                AutoSize = true,
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Black,
                Visible = false // Ukryj, dopóki nie jest używana
            };
            this.Controls.Add(levelProgressLabel);


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

            // Inicjalizacja licznika czasu
            timeLabel = new Label
            {
                Text = "Czas: 10s",
                Location = new Point(this.ClientSize.Width - 100, 10),
                AutoSize = true,
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Black,
                Visible = false // Ukryj licznik na początku
            };
            this.Controls.Add(timeLabel);


            // Inicjalizacja timera
            gameTimer = new Timer();
            gameTimer.Interval = 1000; // 1 sekunda
            gameTimer.Tick += GameTimer_Tick;

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
            if (inGame)
            {
                if (e.KeyCode == Keys.Up)
                {
                    // Zmiana toru w górę
                    selectedTrack = Math.Max(0, selectedTrack - 1);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    // Zmiana toru w dół
                    selectedTrack = Math.Min(2, selectedTrack + 1);
                }
                else if (e.KeyCode == Keys.P && inGame)
                {
                    if (isPaused)
                    {
                        ResumeGame();
                    }
                    else
                    {
                        PauseGame();
                    }
                }
                // Odświeżenie ekranu
                this.Invalidate();
            }
            else
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
        private int selectedTrack = 1; // Wybrany tor (0 - górny, 1 - środkowy, 2 - dolny)
        private bool inGame = false; // Czy jesteśmy w trybie gry

        private void ExecuteLevelSelectionOption(int option)
        {
            if (option == 3) // Wróć
            {
                ShowMainMenu();
                return;
            }

            if (option < 0 || option >= unlockedLevels.Length)
            {
                MessageBox.Show("Nieprawidłowy wybór poziomu!");
                return;
            }

            if (!unlockedLevels[option])
            {
                MessageBox.Show("Ten poziom nie jest jeszcze odblokowany!");
                return;
            }

            currentLevel = option + 1;
            StartGame();
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (inGame)
            {
                Graphics g = e.Graphics;

                // Rysowanie tras
                Pen trackPen = new Pen(Color.White, 2);
                int trackHeight = 150; // Powiększona wysokość trasy
                int trackTop = this.ClientSize.Height - 3 * trackHeight; // Trasy zaczynają się od dołu

                for (int i = 0; i < 3; i++)
                {
                    g.DrawLine(trackPen, 0, trackTop + i * trackHeight, this.ClientSize.Width, trackTop + i * trackHeight);
                }

                // Rysowanie postaci
                int characterWidth = 50;
                int characterHeight = 50;
                int characterX = 100;
                int characterY = trackTop + selectedTrack * trackHeight + (trackHeight - characterHeight) / 2;

                g.FillRectangle(Brushes.Red, characterX, characterY, characterWidth, characterHeight);
            }
        }


        // Rozpoczęcie gry
        private void StartGame()
        {
            wordsGuessed = 0;
            totalWords = 6; // Możesz to zmieniać dla różnych poziomów
            levelProgressLabel.Text = $"{wordsGuessed}/{totalWords}";

            // Pole na nazwę poziomu
            levelNameLabel = new Label
            {
                Text = "Poziom " + currentLevel,
                Location = new Point(10, 10), // Lewy górny róg
                AutoSize = true,
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Black
            };
            this.Controls.Add(levelNameLabel);

            // Pole na postęp poziomu
            levelProgressLabel = new Label
            {
                Text = $"{wordsGuessed}/{totalWords}",
                Location = new Point(200, 10), // Obok wskaźnika poziomu
                AutoSize = true,
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Black
            };
            this.Controls.Add(levelProgressLabel);



            inGame = true; // Ustaw tryb gry
            selectedTrack = 1; // Reset trasy
            remainingTime = 10; // Reset czasu

            // Włącz widoczność licznika czasu
            timeLabel.Visible = true;

            // Ukryj menu poziomu
            foreach (var label in levelSelectionOptions)
            {
                label.Visible = false;
            }

            gameTimer.Start();
            Invalidate(); // Odśwież ekran gry
        }

        private void UpdateProgress()
        {
            wordsGuessed++;
            levelProgressLabel.Text = $"{wordsGuessed}/{totalWords}";

            if (wordsGuessed >= totalWords)
            {
                EndLevel();
            }
        }

        private void EndLevel()
        {
            MessageBox.Show("GRATULACJE! Poziom ukończony!");

            // Odblokowanie następnego poziomu (jeśli istnieje)
            if (currentLevel < unlockedLevels.Length)
            {
                unlockedLevels[currentLevel] = true;
            }

            // Powrót do menu
            ShowMainMenu();
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
        // Funkcja pokazująca menu główne
        private void ShowMainMenu()
        {
            inOptionsMenu = false;
            inLevelSelectionMenu = false;
            inGame = false; // Wyłącz tryb gry
            selectedOption = 0;

            // Ukryj licznik czasu
            timeLabel.Visible = false;

            // Ukryj etykiety poziomu
            if (levelNameLabel != null)
            {
                levelNameLabel.Visible = false;
            }
            if (levelProgressLabel != null)
            {
                levelProgressLabel.Visible = false;
            }

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

            foreach (var control in this.Controls.OfType<Panel>())
            {
                control.Visible = false; // Ukryj elementy planszy
            }

            HighlightOption(mainMenuOptions, 0);
            Invalidate(); // Odśwież ekran
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
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (remainingTime > 0)
            {
                timeLabel.Text = $"Czas: {remainingTime}s";
                remainingTime--;
                
            }
            else
            {
                gameTimer.Stop();
                ShowRetryMenu();
            }
        }
        private void ShowRetryMenu()
        {
            var result = MessageBox.Show("Spróbuj jeszcze raz!", "Koniec gry",
                MessageBoxButtons.RetryCancel, MessageBoxIcon.Information);

            if (result == DialogResult.Retry)
            {
                RestartGame();
            }
            else
            {
                ShowMainMenu();
            }
        }

        // Restartowanie gry
        private void RestartGame()
        {
            remainingTime = 10; // Reset czasu
            selectedTrack = 1; // Reset trasy
            isPaused = false; // Wyłącz pauzę
            gameTimer.Start(); // Ponowne uruchomienie timera
            Invalidate(); // Odśwież ekran
        }
        private void PauseGame()
        {
            isPaused = true;
            gameTimer.Stop(); // Zatrzymanie licznika czasu

            // Wyświetlenie okienka pauzy
            var result = MessageBox.Show("Pauza", "Gra wstrzymana",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            if (result == DialogResult.OK) // Kontynuuj grę
            {
                ResumeGame();
            }
            else // Powrót do menu
            {
                ShowMainMenu(); // Przejdź do menu głównego
            }
        }


        private void ResumeGame()
        {
            isPaused = false;
            gameTimer.Start(); // Wznowienie licznika czasu
        }


    }
}
