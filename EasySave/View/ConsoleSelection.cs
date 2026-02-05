
namespace EasySave.View;

public class ConsoleSelection
{
    protected int _windowsWidth;
    protected int _windowsHeight;
    
    
    protected int _maxLen;
    
    protected string[] _option;
    protected int _selectedOption;
    
    protected bool _isRunning =  true;

    protected string _title;
    
    protected string _easySaveLogo = @"
 /$$$$$$$$                                     /$$$$$$                                
| $$_____/                                    /$$__  $$                               
| $$        /$$$$$$  /$$$$$$$ /$$   /$$      | $$  \__/  /$$$$$$  /$$    /$$ /$$$$$$  
| $$$$$    |____  $$ /$$_____/| $$  | $$      |  $$$$$$  |____  $$|  $$  /$$//$$__  $$
| $$__/     /$$$$$$$|  $$$$$$ | $$  | $$       \____  $$  /$$$$$$$ \  $$/$$/| $$$$$$$$
| $$       /$$__  $$ \____  $$| $$  | $$       /$$  \ $$ /$$__  $$  \  $$$/ | $$_____/
| $$$$$$$$|  $$$$$$$ /$$$$$$$/|  $$$$$$$      |  $$$$$$/|  $$$$$$$   \  $/  |  $$$$$$$
|________/ \_______/|_______/  \____  $$       \______/  \_______/    \_/    \_______/
                               /$$  | $$                                              
                              |  $$$$$$/                                              
                               \______/                                               
";
    

    public ConsoleSelection( string[]  options,string title = "Selection", int width = -1, int height = -1)
    {
        _title = title;
        _windowsWidth = width != -1 ? width : 88;
        _windowsHeight = height  != -1 ? height : options.Length + 2 ;
        _maxLen = options.Max(s => s.Length);
        _option = options;
        _selectedOption = 0;
    }
    


    protected void _printLine(string line,bool select =false)
    {
        
        int leftPadding = Math.Max(0, (_windowsWidth-_maxLen)/2 );
        int rightPadding = Math.Max(0, (_windowsWidth - line.Length - leftPadding));
        Console.Write($"|{new string( ' ',leftPadding)}");

        if (select)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            
        }
        
        Console.Write(line);
        
        Console.ResetColor();
        
        Console.Write($"{new string( ' ',rightPadding)}|\n");
    }

    protected void _printBorder()
    {
        Console.WriteLine(new string('-', _windowsWidth));
    }


    protected void _printCentered(string line, bool select = false)
    {
        int leftPadding = Math.Max(0, (_windowsWidth-line.Length )/2 );
        int rightPadding = Math.Max(0, (_windowsWidth - line.Length - leftPadding));
        Console.Write($"|{new string( ' ',leftPadding)}");
        
        if (select)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            
        }
        Console.Write(line);
        Console.ResetColor();
        Console.Write($"{new string( ' ',rightPadding)}|\n");
        
    }

    protected void _printAppTitle()
    {
        string[] lines = _easySaveLogo.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.None);
        foreach (string line in lines)
        {
            _printCentered(line);
        }
    }
    


    public int Run()
    {
        while (_isRunning)
        {
            _render();   
            _checkInput();
        }
        Console.Clear();

        return _selectedOption;

    }

    protected void _render()
    {
        Console.Clear();
        _printBorder();
        _printAppTitle();
        _printBorder();
        _printCentered(_title);
        _printBorder();
        _printOptions();
       

        for (int i = 0; i < Math.Max(0,_windowsHeight-_option.Length -2); i++)
        {
            _printLine("");
        }

        _printBorder();
        
    }

    protected virtual void _printOptions()
    {
        for (int i = 0; i < _option.Length; i++)
        {
            _printLine(_option[i],i == _selectedOption); 
        }
        
    }

    protected void _checkInput()
    {
        switch (Console.ReadKey(false).Key)
        {
            case ConsoleKey.UpArrow:
                _upArrow();
                break;
            case ConsoleKey.DownArrow:
                _downArrow();
                break;
            
            case ConsoleKey.Escape:
                _isRunning = false;
                break;
            case ConsoleKey.Enter:
                _enter();
                break;
            
            
        }
    }

    protected void _downArrow()
    {
        _selectedOption++;
        if (_selectedOption >= _option.Length)
        {
            _selectedOption = 0;
        }
    }
    
    protected void _upArrow()
    {
        _selectedOption--;
        if (_selectedOption < 0)
        {
            _selectedOption = _option.Length - 1;
        }
    }

    protected virtual void _enter()
    {
        _isRunning = false;
    }
    
}