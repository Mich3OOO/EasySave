namespace EasySave.View;

public class ConsoleForm:ConsoleSelection
{

    private string[] _returnData;
    public ConsoleForm(string[] options, string title = "Selection", int width = -1, int height = -1) : base(options,
        title, width, height)
    {
        _returnData = new string[options.Length];

        for (int i = 0; i < options.Length; i++)
        {
            _returnData[i] = new string(' ', _maxLen);
        }
        
    }

    protected override void _printOptions()
    {
        for (int i = 0; i < _option.Length; i++)
        {
            _printLine(_option[i]);
            _printCentered(_returnData[i],i == _selectedOption);
        }
    }

    protected override void _enter()
    {
        _returnData[_selectedOption] = _editText(_returnData[_selectedOption] != new string(' ', _maxLen) ? _returnData[_selectedOption]:"");
        
    }

    private string _editText(string text)
    {
        char[] allowed = { ' ','_','/',':','-' };
        int pos = Console.CursorLeft;
        Console.Write(text);
        ConsoleKeyInfo info;
        List<char> chars = new List<char> ();
        if (string.IsNullOrEmpty(text) == false) {
            chars.AddRange(text.ToCharArray());
        }
        
        

        while (true)
        {
            info = Console.ReadKey(true);
            if (info.Key == ConsoleKey.Backspace && Console.CursorLeft > pos)
            {
                chars.RemoveAt(chars.Count - 1);
                Console.CursorLeft -= 1;
                Console.Write(' ');
                Console.CursorLeft -= 1;

            }
            else if (info.Key == ConsoleKey.Enter) { Console.Write(Environment.NewLine); break; }
            
            else if (char.IsLetterOrDigit(info.KeyChar) || allowed.Contains(info.KeyChar))
            {
                Console.Write(info.KeyChar);
                chars.Add(info.KeyChar);
            }

            
        }

        string returnValue = new string(chars.ToArray());
        return  returnValue != string.Empty ? returnValue : new string(' ', _maxLen);
    }
    
}