﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.IO;


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
        private int remainingTime = 1000; // Pozostały czas w sekundach
        private Label timeLabel; // Etykieta do wyświetlania czasu
        private bool isPaused = false; // Czy gra jest w stanie pauzy

        private Label levelNameLabel; // Pole na nazwę poziomu
        private Label levelProgressLabel; // Pole na postęp poziomu
        private int wordsGuessed = 0; // Ile słów odgadnięto
        private int totalWords = 6; // Łączna liczba słów do odgadnięcia w poziomie
        private int currentLevel = 1; // Aktualny poziom gry
        private bool[] unlockedLevels = new bool[3] { true, false, false }; // Poziomy odblokowane

        private List<Rectangle> letters = new List<Rectangle>(); // Lista prostokątów liter
        private List<char> letterValues = new List<char>(); // Lista odpowiadających im liter
        private Random random = new Random(); // Generator losowy dla liter
        private Timer letterTimer; // Timer do przesuwania liter
        private int letterSpeed = 20; // Prędkość liter
        private int letterWidth = 50; // Szerokość prostokąta litery
        private int letterHeight = 50; // Wysokość prostokąta litery

        // Zmienne globalne
        private List<string> wordList = new List<string> { "KROWA", "PIES", "KOT", "KURA", "KACZKA", "KOGUT" };
        private string currentWord;
        private int currentIndex = 0;
        private int currentPoints = 0;
        private int targetPoints;

        private int hpGracza = 3;

        private int maxHealth = 3; // Maksymalne zdrowie gracza
        private int currentHealth;
        private string targetWord = "example"; // Słowo do ułożenia
        private int currentLetterIndex = 0; // Indeks aktualnie oczekiwanej litery
        private List<char> healthIcons = new List<char>(); // Lista ikon zdrowia
        private int Interval = 16; // Interwał co 100 ms

        private SoundPlayer successSoundPlayer;

        private int letterGenerationCount = 0; // Licznik generowanych liter

        // Dodatkowe elementy do wyświetlania zebranych liter
        private Label collectedWordLabel;

        private int timeElapsed = 0; // Czas od ostatniej litery w milisekundach

        string hpPath = Path.Combine(Application.StartupPath, "img", "hp_icon3.png");

        private int currentSoundIndex = 0; // Indeks aktualnego dźwięku
        private string audioDirectory = "audio"; // Katalog z plikami audio
        private List<string> audioFiles = new List<string> { Path.Combine(Application.StartupPath, "audio", "0cow.wav"),
            Path.Combine(Application.StartupPath, "audio", "1dog.wav"),
            Path.Combine(Application.StartupPath, "audio", "2cat.wav"),
            Path.Combine(Application.StartupPath, "audio", "3chicken.wav"),
            Path.Combine(Application.StartupPath, "audio", "4duck.wav"),
            Path.Combine(Application.StartupPath, "audio", "5cock.wav"),
    }; // Lista plików audio

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

            // Ustawienie pierwszego słowa
            currentWord = wordList[0];
            collectedWordLabel = new Label
            {
                Text = "",
                ForeColor = Color.White,
                Font = new Font("Arial", 20, FontStyle.Bold),
                Location = new Point(10, 80), // Pozycja w oknie
                AutoSize = true
            };
            this.Controls.Add(collectedWordLabel);

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

            // Inicjalizacja timera liter
            letterTimer = new Timer();
            letterTimer.Interval = 16; // Interwał co 100 ms
            letterTimer.Tick += LetterTimer_Tick;
            letterTimer.Start();


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

            // Inicjalizacja zmiennych
            currentWord = wordList[0];
            targetPoints = wordList.Count;

            // Ładowanie dźwięku sukcesu
            successSoundPlayer = new SoundPlayer(Path.Combine(Application.StartupPath, "audio", "success.wav"));

        }

        private void LetterTimer_Tick(object sender, EventArgs e)
        {
            if (isPaused) return; // Jeśli gra jest wstrzymana, nie wykonujemy aktualizacji liter

            timeElapsed += Interval; // Dodajemy czas interwału zegara

            // Sprawdzamy, czy minęła 1 sekunda (1000 ms)
            if (timeElapsed >= 500)
            {
                timeElapsed = 0; // Resetujemy licznik czasu
                int trackY = this.ClientSize.Height - 3 * 150 + random.Next(0, 3) * 150; // Y w zależności od trasy
                char newLetter;

                // Co druga litera jest prawidłowa
                if (letterGenerationCount % 2 == 0)
                {
                    // Pobieramy aktualną oczekiwaną literę z currentWord
                    if (currentLetterIndex < currentWord.Length)
                    {
                        newLetter = currentWord[currentLetterIndex]; // Następna litera w słowie
                    }
                    else
                    {
                        // Jeśli całe słowo zostało już ułożone, generujemy losową literę
                        newLetter = (char)random.Next('A', 'Z' + 1);
                    }
                }
                else
                {
                    // Generujemy losową literę
                    newLetter = (char)random.Next('A', 'Z' + 1);
                }

                // Zwiększamy licznik generowanych liter
                letterGenerationCount++;

                // Dodaj literę do listy
                Rectangle newRectangle = new Rectangle(this.ClientSize.Width, trackY + 50, letterWidth, letterHeight); // Prostokąt litery
                letters.Add(newRectangle);
                letterValues.Add(newLetter);
            }

            // Przesuwanie liter w lewo
            for (int i = 0; i < letters.Count; i++)
            {
                Rectangle rect = letters[i];
                rect.X -= letterSpeed; // Zwiększona prędkość liter
                letters[i] = rect;
            }

            // Usuwanie liter, które dotarły do lewej krawędzi ekranu
            for (int i = letters.Count - 1; i >= 0; i--)
            {
                if (letters[i].X + letterWidth < 0)
                {
                    letters.RemoveAt(i);
                    letterValues.RemoveAt(i);
                }
            }

            // Sprawdzenie kolizji z bohaterem
            CheckCollisions();

            // Odświeżanie ekranu
            Invalidate();
        }

        private void CheckCollisions()
        {
            Rectangle characterRect = new Rectangle(100, this.ClientSize.Height - 3 * 150 + selectedTrack * 150 + 50, 50, 50);

            for (int i = letters.Count - 1; i >= 0; i--)
            {
                if (letters[i].IntersectsWith(characterRect))
                {
                    char collidedLetter = letterValues[i]; // Litera, która została zebrana
                    letters.RemoveAt(i);
                    letterValues.RemoveAt(i);

                    // Sprawdzenie, czy litera jest poprawna, z uwzględnieniem wielkości liter
                    char expectedLetter = char.ToUpper(currentWord[currentLetterIndex]); // Konwertujemy oczekiwaną literę na wielką literę
                    char collidedLetterUpper = char.ToUpper(collidedLetter); // Konwertujemy zderzoną literę na wielką literę

                    if (collidedLetterUpper == expectedLetter)
                    {
                        collectedWordLabel.Text += currentWord[currentLetterIndex]; // Dodanie oryginalnej litery w odpowiedniej wielkości
                        currentLetterIndex++; // Przechodzimy do następnej litery

                        // Sprawdzenie, czy całe słowo zostało ułożone
                        if (currentLetterIndex == currentWord.Length)
                        {
                           // MessageBox.Show($"Gratulacje! Ułożyłeś słowo: {currentWord}", "Brawo!");
                            UpdateProgress();
                            PlayCorrectSound(); // Odtwórz dźwięk
                            // Zmiana słowa na następne
                            currentIndex++;
                            if (currentIndex >= wordList.Count)
                            {
                                MessageBox.Show("Ułożyłeś wszystkie słowa! Gra zakończona!", "Koniec gry");
                                
                            }
                            else
                            {
                                currentWord = wordList[currentIndex];
                                collectedWordLabel.Text = ""; // Reset wyświetlacza zebranych liter
                                currentLetterIndex = 0; // Reset indeksu liter
                            }
                        }
                    }
                    else
                    {
                        // Litera niepoprawna - można tu dodać logikę np. utraty zdrowia
                        hpLoss(hpPath);
                    }
                }
            }
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
        private Label hpLabel;
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
                int characterWidth = 80;
                int characterHeight = 80;
                int characterX = 100;
                int characterY = trackTop + selectedTrack * trackHeight + (trackHeight - characterHeight) / 2;

                Image characterImage = Image.FromFile("img/stick.png");
                g.DrawImage(characterImage, characterX, characterY, characterWidth, characterHeight);


                // Rysowanie liter
                for (int i = 0; i < letters.Count; i++)
                {
                    Rectangle letterRect = letters[i];
                    g.FillRectangle(Brushes.Blue, letterRect);
                    g.DrawString(letterValues[i].ToString(), new Font("Arial", 16, FontStyle.Bold), Brushes.White, letterRect.X + 10, letterRect.Y + 10);
                }
            }
        }


        // Rozpoczęcie gry
        private void StartGame()
        {

            currentSoundIndex = 0;

            // Inicjalizacja listy plików audio
            audioFiles = Directory.GetFiles(audioDirectory, "*.wav").ToList(); // Wczytanie plików WAV z katalogu

            // Ustawienie właściwości formularza
            //this.BackColor = Color.Black; // Tło formularza
            //this.TransparencyKey = this.BackColor; // Czarny staje się przezroczysty

            // Wczytanie obrazu do tła
            Bitmap backgroundImage = new Bitmap(Path.Combine(Application.StartupPath, "img", "farm.png")); // Ścieżka do obrazu
            this.BackgroundImage = backgroundImage; // Ustawienie obrazu jako tła
            this.BackgroundImageLayout = ImageLayout.Stretch; // Dopasowanie obrazu do rozmiaru okna


            wordsGuessed = 0;
            totalWords = 6; // Możesz to zmieniać dla różnych poziomów
            levelProgressLabel.Text = $"{wordsGuessed}/{totalWords}";
            hpGracza = 3;


            currentLetterIndex = 0;
            InitializeHealthIcons();

            DisplayHealthIcons();
            MessageBox.Show(wordList[currentIndex]);
            PlayCorrectSound();
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

            string hpPath = Path.Combine(Application.StartupPath, "img", "hp_icon3.png");

            if (File.Exists(hpPath))
            {
                hpLabel = new Label
                {
                    Location = new Point(this.ClientSize.Width - 250, 10),
                    BackColor = Color.Transparent,
                    Image = Image.FromFile(hpPath),
                    ImageAlign = ContentAlignment.MiddleCenter
                };
                this.Controls.Add(hpLabel);
            }
            else
            {
                Console.WriteLine(hpPath);
                MessageBox.Show("Plik obrazu HP nie został znaleziony: " + hpPath, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Controls.Add(hpLabel);

            inGame = true; // Ustaw tryb gry
            selectedTrack = 1; // Reset trasy
            remainingTime = 1000; // Reset czasu

            // Włącz widoczność licznika czasu
            timeLabel.Visible = true;

            // Ukryj menu poziomu
            foreach (var label in levelSelectionOptions)
            {
                label.Visible = false;
            }

            gameTimer.Start();
            letterTimer.Start();
            Invalidate(); // Odśwież ekran gry
        }
        private void hpLoss(string hpPath)
        {
            hpGracza--;
            if(hpGracza == 2)//if(hpPath == Path.Combine(Application.StartupPath, "img", "hp_icon3.png"))
            {
               
                hpPath = Path.Combine(Application.StartupPath, "img", "hp_icon2.png");
            }
            if(hpGracza == 1)
            {
                hpPath = Path.Combine(Application.StartupPath, "img", "hp_icon1.png");
            }
            hpLabel.Image = Image.FromFile(hpPath);
            if(hpGracza == 0)
            {
                ShowRetryMenu();
            }
            
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
            gameTimer.Stop();
            letterTimer.Stop();
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
            ClearLetters(); // Czyszczenie liter

            inOptionsMenu = false;
            inLevelSelectionMenu = false;
            inGame = false; // Wyłącz tryb gry
            selectedOption = 0;

            timeLabel.Visible = false; // Ukryj licznik czasu

            if (levelNameLabel != null) levelNameLabel.Visible = false;
            if (levelProgressLabel != null) levelProgressLabel.Visible = false;

            foreach (var label in optionsMenuOptions) label.Visible = false;
            foreach (var label in levelSelectionOptions) label.Visible = false;
            foreach (var label in mainMenuOptions) label.Visible = true;

            foreach (var control in this.Controls.OfType<Panel>())
            {
                control.Visible = false; // Ukryj elementy planszy
            }
            hpLabel.Visible = false;
            collectedWordLabel.Visible = false;
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
            gameTimer.Stop();
            letterTimer.Stop();
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

        private void ClearLetters()
        {
            letters.Clear(); // Usuwanie wszystkich prostokątów liter
            letterValues.Clear(); // Usuwanie wszystkich wartości liter
            Invalidate(); // Odświeżenie ekranu
        }


        // Restartowanie gry
        private void RestartGame()
        {
            wordsGuessed = 0;
            totalWords = 6; // Możesz to zmieniać dla różnych poziomów
            levelProgressLabel.Text = $"{wordsGuessed}/{totalWords}";
            hpGracza = 3;
            ClearLetters(); // Czyszczenie liter

            wordsGuessed = 0; // Reset postępu
            levelProgressLabel.Text = $"{wordsGuessed}/{totalWords}"; // Aktualizacja postępu

            remainingTime = 1000; // Reset czasu
            selectedTrack = 1; // Reset trasy
            isPaused = false; // Wyłącz pauzę

            gameTimer.Start(); // Ponowne uruchomienie timera
            letterTimer.Start();
            Invalidate(); // Odśwież ekran
        }

        private void PauseGame()
        {
            isPaused = true;
            gameTimer.Stop(); // Zatrzymanie licznika czasu
            letterTimer.Stop(); // Zatrzymanie ruchu liter

            var result = MessageBox.Show("Pauza", "Gra wstrzymana", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            if (result == DialogResult.OK) // Kontynuuj grę
            {
                ResumeGame();
            }
            else // Powrót do menu
            {
                ShowMainMenu();
            }
        }


        private void ResumeGame()
        {
            isPaused = false;
            gameTimer.Start(); // Wznowienie licznika czasu
            letterTimer.Start(); // Wznowienie ruchu liter
        }

        private void InitializeHealthIcons()
        {
            healthIcons.Clear();
            for (int i = 0; i < maxHealth; i++)
            {
                healthIcons.Add('♥'); // Dodanie serduszek jako HP
            }
        }

        private void DisplayHealthIcons()
        {
            Console.WriteLine(string.Join(" ", healthIcons));
        }

        private void PlayCorrectSound()
        {
            if (audioFiles.Count == 0) return; // Jeśli brak plików, wyjdź

            try
            {
                // Wczytaj aktualny plik dźwiękowy
                string currentSoundPath = audioFiles[currentSoundIndex];
                using (SoundPlayer player = new SoundPlayer(currentSoundPath))
                {
                    player.Play(); // Odtwórz dźwięk
                }

                // Przejdź do kolejnego dźwięku
                currentSoundIndex++;
                if (currentSoundIndex >= audioFiles.Count) currentSoundIndex = 0; // Wracamy na początek listy
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas odtwarzania dźwięku: {ex.Message}");
            }
        }
    }
}
