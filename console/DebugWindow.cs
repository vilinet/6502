using NES;
using NES.Display.SFML;
using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;
using System.Text;

namespace console
{
    public class DebugWindow : SfmlGeneralDisplay
    {
        private DebugView _actualView = DebugView.None;

        private enum DebugView
        {
             Nametable, PpuMemory, Oam, Cpu, None
        }

        private readonly Nes _nes;
        private readonly PPU _ppu;
        private readonly List<string> _cpuOperations = new List<string>();
        private string _cpuState = "";
        private readonly string[] _stackState = new string[20];
        private string _memory;
        private readonly StringBuilder _sb = new StringBuilder(5000);
        private int _bank1Palette, _bank2Palette;

        public DebugWindow(string title, uint width, uint height, Nes nes) : base(title, width, height)
        {
            _nes = nes;
            _ppu = _nes.PPU;
        }

        private void BeforeOperationExecuted(emulator6502.Cpu sender, emulator6502.OpcodeEventArgs e)
        {
            lock (this)
            {
                if (_cpuOperations.Count >= 20)
                    _cpuOperations.RemoveAt(0);

                _cpuOperations.Add(e.Full.ToString(e.Full.Parameter));

                _cpuState = $"C: {R(_nes.Cpu.Status.Carry)} N:{R(_nes.Cpu.Status.Negative)} O:{R(_nes.Cpu.Status.Overflow)} Z:{R(_nes.Cpu.Status.Zero)}\n";
                _cpuState += $"A: {_nes.Cpu.A:X2} X:{_nes.Cpu.X:X2} Y:{_nes.Cpu.Y:X2}\n";
                _cpuState += $"SP: {_nes.Cpu.SP:X2} PC: {_nes.Cpu.PC:X4}";

                for (int i = 0; i < 20; i++)
                    _stackState[i] = $"{255 - i:X2}: {_nes.Bus.Read((ushort)(0x0100 + (255 - i))):X2}";

                _sb.Clear();

                for (int i = 0; i < 32; i++)
                {
                    _sb.Append($"{((byte)(i * 8)):X2}: ");
                    for (int j = 0; j < 8; j++)
                    {
                        _sb.Append(_nes.Bus.Read((ushort)(i * 32 + j)).ToString("X2") + " ");
                        if (j == 3) _sb.Append(" | ");
                    }
                    _sb.Append("\n");
                    _memory = _sb.ToString();
                }
            }
        }


        public override void Render()
        {
            HandleEvents();
            ClearPixels();

            switch (_actualView)
            {
                case DebugView.Nametable:
                    DebugNametable(false);
                    break;
                case DebugView.PpuMemory:
                    DebugPpuMemory(false);
                    break;
                case DebugView.Oam:
                    DebugOam(false);
                    break;
            }

            _texture.Update(Pixels);
            Draw(_sprite);

            switch (_actualView)
            {
                case DebugView.PpuMemory:
                    DebugPpuMemory(true);
                    break;
                case DebugView.Nametable:
                    DebugNametable(true);
                    break;
                case DebugView.Oam:
                    DebugOam(true);
                    break;
                case DebugView.Cpu:
                    DebugCpu();
                    break;
            }

            Display();
        }

        public void DebugCpu()
        {
            string[] list;
            lock (this)
            {
                list = _cpuOperations.ToArray();
            }

            DrawText(0, 0, _memory, 22);
            for (int i = 0; i < list.Length; i++) DrawText(260, i * 10, list[i], 22, color: i == list.Length - 1 ? Color.Yellow : default);
            DrawText(260, 220, _cpuState, 22);

            for (int i = 0; i < _stackState.Length; i++) DrawText(550, i * 10, _stackState[i], 22);
        }

        private string R(bool val)
        {
            return val ? "1" : "0";
        }

        private void DrawRectangle(int x, int y, int width, int height, uint color)
        {
            for (int i = x; i < x + width; i++)
            {
                for (int j = y; j < y + height; j++)
                {
                    DrawPixel(i, j, color);
                }
            }
        }

        public void DebugNametable(bool text)
        {
            for (var n = 0; n < 4; n++)
            {
                var startPos = PPU.NAMETABLE + n * PPU.NAMETABLE_FULL_LENGTH;
                var offsetX = (n % 2 == 0 ? 0 : 30 * 8);
                var offsetY = n < 2 ? 0 : 240;
                var startAttributes = startPos + PPU.NAMETABLE_TILES_LENGTH;

                for (var j = 0; j < 30; j++)
                {
                    for (var i = 0; i < 32; i++)
                    {
                        var ind = _ppu.ReadPpu(startPos++);
                        if(!text)  DrawSprite(GetSprite(1, ind), i * 8 + offsetX, j * 8 + offsetY);
                        else DrawText(i * 8 + offsetX, j * 8 + offsetY, ind.ToString("X2"), 12);
                    }
                }
          

                for (var j = 0; j < 30/4; j++)
                {
                    for (var i = 0; i < 32/4; i++)
                    {
                        var attr = _ppu.ReadPpu(startAttributes++);
                        if (text)
                        {
                            //DrawText(i * 32+16 + offsetX, j * 32+16 + offsetY, attr.ToString("X2"), 14, Color.Magenta);
                        }
                     
                    }
                }
               
            }
            
            if (!text)
            {
                for (var i = 0; i < 32; i++)
                {
                    DrawHorzontalLine(0, i*32, 1000);
                    if(i<18)
                    DrawVerticalLine(0, i*32, 1000);
                }
            }
        }

        private void DrawHorzontalLine(int x1, int y, int x2, uint color = 0xFFFFFF)
        {
            for (; x1 < x2; x1++)
            {
                DrawPixel(x1, y, color);
            }
        }
        
        private void DrawVerticalLine(int y1, int x, int y2, uint color = 0xFFFFFF)
        {
            for (; y1 < y2; y1++)
            {
                DrawPixel(x, y1, color);
            }
        }

        private void DebugPpuMemory(bool text)
        {
            int x = 0;
            int y = 0;
            for (int i = 0; i < 256; i++)
            {
                if (i % 16 == 0 && i != 0)
                {
                    x = 0;
                    y += 8;
                }

                if (!text) DrawSprite(GetSprite(0, i, _bank1Palette, bg: true), x, y);
                else DrawText(x, y, i.ToString("X2"), 10);
                x += 8;
            }

            x = 0;
            y = 140;

            for (int i = 0; i < 256; i++)
            {
                if (i % 16 == 0 && i != 0)
                {
                    x = 0;
                    y += 8;
                }
                if (!text) DrawSprite(GetSprite(1, i, _bank2Palette, bg: true), x, y);
                else DrawText(x, y, i.ToString("X2"), 10);
                x += 8;
            }

            int ind = 0;
            for (int t = 0; t < 8; t++)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (!text) DrawRectangle(i * 8 + 150, t * 8, 8, 8, PpuColors.Colors[_ppu.Palettes[ind] % 64]);

                    ind++;
                }
            }
        }

        private void DebugOam(bool text)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    var oam = _ppu.Oam[i * 32 + j];
                    if (!text) DrawSprite(GetSprite(oam), i * 200, j * 10 + 1);
                    else DrawText(i * 200 + 8, j * 10, $"{oam.Id:X2}: X:{oam.X:X2} Y:{oam.X:X2} A: {oam.Attributes}");
                }
            }

        }

        private void DrawSprite(NesSprite sprite, int x, int y, int scale = 2)
        {
            int index = 0;
            for (int j = y; j < y + 8; j++)
            {
                for (int i = x; i < x + 8; i++)
                {
                    DrawPixel(i, j, sprite.data[index++]);
                }
            }
        }

        private NesSprite GetSprite(Oam oam)
        {
            bool flipHor = (oam.Attributes & 0b01000000) != 0;
            bool flipVer = (oam.Attributes & 0b10000000) != 0;
            return GetSprite(0, oam.Id, oam.Attributes & 3, flipVer, flipHor);
        }

        private NesSprite GetSprite(int bankIndex, int spriteIndex, int paletteIndex = -1, bool bg = false, bool flipVertical = false, bool flipHorizontal = false)
        {
            if (paletteIndex == -1) paletteIndex = 1;
            var sprite = new NesSprite();

            var paletteBase = (!bg ? PPU.PALETTE_SPRITE : PPU.PALETTE) + paletteIndex * 4;

            var addr = spriteIndex * 16 + bankIndex * 0x1000;
            var index = 0;

            for (int j = 0; j < 8; j++)
            {
                int value1 = _ppu.ReadPpu(addr);
                int value2 = _ppu.ReadPpu(addr + 8);

                for (int x = 0; x < 8; x++)
                {
                    var palette = ((value2 & 1) << 1) + (value1 & 1);

                    sprite.data[index + (7 - x)] = PpuColors.Colors[_ppu.ReadPpu(paletteBase + palette) % 64];

                    value1 >>= 1;
                    value2 >>= 1;

                }

                index += 8;
                addr++;
            }

            return sprite;
        }


        protected override void HandleEvents()
        {
            while (PollEvent(out Event ev))
            {
                bool? pressed = null;

                switch (ev.Type)
                {
                    case EventType.Closed:
                        Close();
                        break;
                    case EventType.KeyPressed:
                        pressed = true;
                        break;
                    case EventType.KeyReleased:
                        pressed = false;

                        OnKeyRelease(ev.Key.Code);
                        break;
                }

                if (pressed.HasValue)
                {
                    var c = ev.Key.Code;
                    switch (c)
                    {
                        case Keyboard.Key.Escape:
                            Close();
                            break;
                        default: break;
                    }

                    if (c >= Keyboard.Key.Numpad1 && c <= Keyboard.Key.Numpad5)
                    {
                        _actualView = (DebugView)(c - Keyboard.Key.Numpad1);
                        if (_actualView == DebugView.Cpu)
                        {
                            _nes.Cpu.BeforeOperationExecuted -= BeforeOperationExecuted;
                            _nes.Cpu.BeforeOperationExecuted += BeforeOperationExecuted;
                            _cpuOperations.Clear();
                        }
                        else
                        {
                            _nes.Cpu.BeforeOperationExecuted -= BeforeOperationExecuted;
                        }
                    }


                    if (c >= Keyboard.Key.Num0 && c <= Keyboard.Key.Num3)
                    {
                        _bank1Palette = c - Keyboard.Key.Num0;
                    }
                    else if (c >= Keyboard.Key.Num4 && c <= Keyboard.Key.Num8)
                    {
                        _bank2Palette = c - Keyboard.Key.Num4;
                    }
                }
            }
        }
    }
}
