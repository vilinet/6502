using emulator6502;
using NES;
using NES.Display.SFML;
using SFML.Window;
using System;
using NES.Interfaces;

namespace console
{
    public class MySFMLNesApp : SFMLGeneralDisplay
    {
        private readonly Nes _nes;
        private bool _debug;
        private bool _debugText = false;

        public MySFMLNesApp() : base("NES",1900 , 1080)
        {
            _nes = new Nes(this,  Controller1);
            _nes.LoadRom("./smb.nes");
            _nes.RunOnThread();
        }

        protected override void OnPostDraw(IDrawText texter)
        {
            _nes.RenderDebug(texter);
            texter.DrawText(0, 0, _nes.ActualFps.ToString());
        }

        protected override void OnKeyRelease(Keyboard.Key key)
        {
            switch (key)
            {
                case Keyboard.Key.Space:
                    if (_nes.State == NesState.Running)
                        _nes.Pause();
                    else _nes.Resume();
                    break;
                case Keyboard.Key.R:
                    _nes.Reset();
                    break;
                case Keyboard.Key.Add:
                    _nes.Speed += 0.1f;
                    break;
                case Keyboard.Key.Subtract:
                    _nes.Speed -= 0.1f;
                    break;
                case Keyboard.Key.Multiply:
                    _nes.Speed = Math.Min(_nes.Speed + 0.25f, 3);
                    break;
                case Keyboard.Key.Divide:
                    _nes.Speed = Math.Max(_nes.Speed - 0.25f, 0);
                    break;
                case Keyboard.Key.D:
                    _debug = !_debug;
                    ClearPixels();
                    break;
                case Keyboard.Key.T:
                    _debugText = !_debugText;
                    ClearPixels();
                    break;
            }
            _nes.DebugInput((char)key);
        }

        void Cpu_BeforeOperationExecuted(Cpu cpu, OpcodeEventArgs e)
        {
            Console.WriteLine(e.Full.ToString(e.Full.Parameter));
        }
    }

    static class Program
    {
        static void Main(string[] args)
        {
            // new Terminal().Run();
           var app =  new MySFMLNesApp();
          //var app = new MyNesApp(true);
          
            while (app.IsOpen)
            {
                app.Render();
            }
        }
    }
}