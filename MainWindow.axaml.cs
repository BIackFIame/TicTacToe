using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyAvaloniaApp
{
    
    public partial class MainWindow : Window
    {
        private List<Button> buttons = new List<Button>();
        private string tag = "X";
#pragma warning disable CS8632 // Аннотацию для ссылочных типов, допускающих значения NULL, следует использовать в коде только в контексте аннотаций "#nullable".
        private Window? dialog;
#pragma warning restore CS8632 // Аннотацию для ссылочных типов, допускающих значения NULL, следует использовать в коде только в контексте аннотаций "#nullable".
        private bool isPlayingWithComputer;



        public MainWindow()
        {
            InitializeComponent(); 
            GenerateTag();
            Init();

            this.Opened += (sender, args) =>
            {
                UpdateGameStatusText(); // Обновить элементы UI после загрузки окна
                ShowGameModeDialog(); // Показать диалог выбора режима игры
            };
        }

        private void ShowGameModeDialog()
{
    var dialog = new Window
    {
        Width = 400,
        Height = 200,
        SystemDecorations = SystemDecorations.BorderOnly,
        Title = "Выбор режима игры"
    };

    var stackPanel = new StackPanel
    {
        Orientation = Orientation.Vertical,
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
    };

    var playWithPersonButton = new Button { Content = "Играть с человеком", Width = 200 };
    playWithPersonButton.Click += (_, __) =>
    {   
        isPlayingWithComputer = false;
        dialog.Close();
    };
    stackPanel.Children.Add(playWithPersonButton);

    var playWithComputerButton = new Button { Content = "Играть с компьютером", Width = 200 };
    playWithComputerButton.Click += (_, __) =>
    {
        isPlayingWithComputer = true;
        dialog.Close();
    };
    stackPanel.Children.Add(playWithComputerButton);

    dialog.Content = stackPanel;
    dialog.ShowDialog(this); // Показать диалоговое окно
}               
        private void ComputerMove()
        {
            var freeButtons = buttons.Where(b => b.Content == null).ToList();
            if (freeButtons.Any())
            {
                var random = new Random();
                var randomButton = freeButtons[random.Next(freeButtons.Count)];
                randomButton.Content = "O";
                if (CheckWin())
                {
                    ShowEndGameDialog("Победил компьютер!");
                }
                else if (IsDraw())
                {
                    ShowEndGameDialog("НИЧЬЯ!");
                }
            }
        }

        private void UpdateGameStatusText()
        {
            if (GameStatusTextBlock != null) // Проверка на null
            {
                if (isPlayingWithComputer)
                {
                    GameStatusTextBlock.Text = "Вы (X) против Компьютера (O)";
                }
                else
                {
                    GameStatusTextBlock.Text = "Игрок 1 (X) против Игрока 2 (O)";
                }
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            AddButtonSafe("Button0");
            AddButtonSafe("Button1");
            AddButtonSafe("Button2");
            AddButtonSafe("Button3");
            AddButtonSafe("Button4");
            AddButtonSafe("Button5");
            AddButtonSafe("Button6");
            AddButtonSafe("Button7");
            AddButtonSafe("Button8");
        }
        private void AddButtonSafe(string buttonName)
        {
            var button = this.FindControl<Button>(buttonName);
            if (button != null)
            {
                buttons.Add(button);
            }
        }

        private bool CheckWin()
        {
            for (int i = 0; i < 9; i += 3) 
            {
                if (buttons[i].Content == buttons[i + 1].Content && buttons[i].Content == buttons[i + 2].Content && buttons[i].Content != null) 
                {
                    return true;
                }
            }
            for (int i = 0; i < 3; i++)
            {
                if (buttons[i].Content == buttons[i + 3].Content && buttons[i].Content == buttons[i + 6].Content && buttons[i].Content != null)
                {
                    return true;
                }
            }
            if (buttons[0].Content == buttons[4].Content && buttons[4].Content == buttons[8].Content && buttons[4].Content != null)
            {
                return true;
            }
            if (buttons[2].Content == buttons[4].Content && buttons[4].Content == buttons[6].Content && buttons[4].Content != null)
            {
                return true;
            }
            return false;
        }

        private bool IsDraw()
        {
            foreach (var button in buttons)
            {
                if (button.Content == null)
                {
                    return false; // Есть еще пустые клетки
                }
            }
            return !CheckWin(); // Ничья, если все клетки заполнены и нет победителя
        }
        
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Content == null)
            {
                button.Content = tag;
                if (CheckWin())
                {
                    ShowEndGameDialog($"Победил {tag}!");
                }
                else if (IsDraw())
                {
                    ShowEndGameDialog("НИЧЬЯ!");
                }
                else
                {
                    if (isPlayingWithComputer && tag == "X")
                    {
                        ComputerMove(); // Ход компьютера, если выбран соответствующий режим
                    }
                    else
                    {
                        tag = tag == "X" ? "O" : "X";
                    }
                }
            }
        }


        private void ShowEndGameDialog(string message)
{
    if (dialog != null)
    {
        dialog.Close();
    }

    dialog = new Window
    {
        Width = 300,
        Height = 200,
        SystemDecorations = SystemDecorations.None,
        Background = Brushes.White, // Белый фон для окна
        WindowStartupLocation = WindowStartupLocation.CenterOwner
    };

    var stackPanel = new StackPanel
    {
        Orientation = Orientation.Vertical,
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
    };

    var textBlock = new TextBlock 
    { 
        Text = message, 
        FontSize = 20, 
        Foreground = Brushes.Black, // Черный текст
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center 
    };
    stackPanel.Children.Add(textBlock);

    // Функция для создания кнопки с заданными стилями
    Button CreateStyledButton(string content)
    {
        return new Button 
        { 
            Content = content, 
            Width = 200, 
            Margin = new Thickness(10),
            Background = Brushes.LightGray, // Серый фон для кнопки
            Foreground = Brushes.Black, // Черный текст для кнопки
        };
    }

    var continueButton = CreateStyledButton("Продолжить играть");
    continueButton.Click += (_, __) =>
    {
        dialog.Close();
        ResetGame();
    };
    stackPanel.Children.Add(continueButton);

    var changeModeButton = CreateStyledButton("Изменить режим игры");
    changeModeButton.Click += (_, __) =>
    {
        ResetGame(); // Сброс игрового поля до начального состояния
        isPlayingWithComputer = !isPlayingWithComputer; // Переключение режима игры
        ShowGameModeDialog(); // Показать диалог выбора режима игры
    };
    stackPanel.Children.Add(changeModeButton);

    var exitButton = CreateStyledButton("Выйти");
    exitButton.Click += (_, __) =>
    {
        this.Close();
    };
    stackPanel.Children.Add(exitButton);

    dialog.Content = stackPanel;
    dialog.ShowDialog(this); // Показать диалоговое окно
}




        // private void PositionDialog()
        // {
        //     if (dialog != null)
        //     {
        //         var mainWindowPosition = this.Position;
        //         dialog.Position = new PixelPoint(mainWindowPosition.X + 100, mainWindowPosition.Y + 100);
        //     }
        // }

        private void GenerateTag()
    {
        var r = new Random();
        tag = r.Next(0, 2) == 0 ? "X" : "O";
    }

        private void ResetGame()
        {
            foreach (var button in buttons)
            {
                if (button != null)
                {
                    button.Content = null;
                    button.IsEnabled = true;
                }
            }
            GenerateTag();
            if (dialog != null)
            {
                dialog.Close();
                dialog = null;
            }
            if (winOverlay != null)
            {
                winOverlay.IsVisible = false;
            }

            UpdateGameStatusText(); // Обновить текст статуса игры
            if (isPlayingWithComputer)
            {
                tag = "X"; // Пользователь всегда начинает первым при игре с компьютером
            }
        }


        private void Init()
        {
            GenerateTag();
            this.Closed += (s, e) =>
            {
                if (dialog != null)
                {
                    dialog.Close();
                }
            };
        }
    }
}