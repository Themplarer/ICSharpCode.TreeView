using System;
using System.Timers;
using Avalonia.Threading;

namespace ICSharpCode.TreeView;

public class ClickHandler<T>
{
    private readonly int _delay;
    private Timer _timer;
    private int _click;
    private Action<T> _action;
    private T _context;

    public ClickHandler(int delay = 300) => _delay = delay;

    public void UpdateContext(T context) => _context = context;

    public void MouseDown(T context)
    {
        _context = context;
        _click = _timer == null ? 1 : _click + 1;

        if (_click == 1)
        {
            _timer = new Timer { Interval = _delay };
            _action = null;
            _timer.Elapsed += (_, _) => RunAction();
            _timer?.Start();
        }
    }

    public void MouseUp(Action<T> singleClickAction, Action<T> doubleClickAction)
    {
        _action = _click == 1 ? singleClickAction : doubleClickAction;

        if (_timer == null)
            _action(_context);

        if (_timer != null && _click == 2)
            RunAction();
    }

    private void RunAction() => Dispatcher.UIThread.Post(() =>
    {
        _timer?.Stop();
        _timer = null;
        _action?.Invoke(_context);
        _action = null;
    });
}