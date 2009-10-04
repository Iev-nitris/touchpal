/*
 * This file is part of the TouchPal project hosted on Google Code
 * (http://code.google.com/p/touchpal). See the accompanying license.txt file for 
 * applicable licenses.
 */

/*
 * (c) Copyright Craig Courtney 2009 All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TouchPal
{
    class KeyAction : IAction
    {
        private static Dictionary<string, ushort> keycodes = new Dictionary<string, ushort>{
            {"BACKSPACE", 0x08},
            {"TAB", 0x09},
            {"CLEAR", 0x0C},
            {"RETURN", 0x0D},
            {"LSHIFT", 0xA0},
            {"RSHIFT", 0xA1},
            {"LCONTROL", 0xA2},
            {"RCONTROL", 0xA3},
            {"LALT", 0xA4},
            {"RALT", 0xA5},
            {"PAUSE", 0x13},
            {"CAPSLOCK", 0x14},
            {"ESCAPE", 0x1B},
            {"SPACE", 0x20},
            {"PAGEUP", 0x21},
            {"PAGEDOWN", 0x22},
            {"END", 0x23},
            {"HOME", 0x24},
            {"LEFT", 0x25},
            {"UP", 0x26},
            {"RIGHT", 0x27},
            {"DOWN", 0x28},
            {"PRINTSCREEN", 0x2C},
            {"INSERT", 0x2D},
            {"DELETE", 0x2E},
            {"LWIN", 0x5B},
            {"RWIN", 0x5C},
            {"APPS", 0x5D},
            {"NUMPAD0", 0x60},
            {"NUMPAD1", 0x61},
            {"NUMPAD2", 0x62},
            {"NUMPAD3", 0x63},
            {"NUMPAD4", 0x64},
            {"NUMPAD5", 0x65},
            {"NUMPAD6", 0x66},
            {"NUMPAD7", 0x67},
            {"NUMPAD8", 0x68},
            {"NUMPAD9", 0x69},
            {"MULTIPLY", 0x6A},
            {"ADD", 0x6B},
            {"SEPARATOR", 0x6C},
            {"SUBTRACT", 0x6D},
            {"DECIMAL", 0x6E},
            {"DIVIDE", 0x6F},
            {"F1", 0x70},
            {"F2", 0x71},
            {"F3", 0x72},
            {"F4", 0x73},
            {"F5", 0x74},
            {"F6", 0x75},
            {"F7", 0x76},
            {"F8", 0x77},
            {"F9", 0x78},
            {"F10", 0x79},
            {"F11", 0x7A},
            {"F12", 0x7B},
            {"F13", 0x7C},
            {"F14", 0x7D},
            {"F15", 0x7E},
            {"F16", 0x7F},
            {"F17", 0x80},
            {"F18", 0x81},
            {"F19", 0x82},
            {"F20", 0x83},
            {"F21", 0x84},
            {"F22", 0x85},
            {"F23", 0x86},
            {"F24", 0x87},
            {"NUMLOCK", 0x90},
            {"SCROLLLOCK", 0x91}
        };

        const int INPUT_MOUSE = 0;
        const int INPUT_KEYBOARD = 1;
        const int INPUT_HARDWARE = 2;
        const uint KEY_EXTENDED = 0x0001;
        const uint KEY_UP = 0x0002;
        const uint KEY_UNICODE = 0x0004;
        const uint KEY_SCANCODE = 0x0008;

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBOARD_INPUT
        {
            public uint type;
            public ushort vk;
            public ushort scanCode;
            public uint flags;
            public uint time;
            public uint extrainfo;
            public uint padding1;
            public uint padding2;
        }

        [DllImport("User32.dll")]
        private static extern uint SendInput(uint numberOfInputs, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] KEYBOARD_INPUT[] input, int structSize);

        [DllImport("User32.dll")]
        static extern short VkKeyScan(char ch);

        [DllImport("User32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);

        private KEYBOARD_INPUT[] keyEvents;

        public KeyAction(string keys, bool keyDown)
        {
            List<KEYBOARD_INPUT> eventList = new List<KEYBOARD_INPUT>();
            int length = keys.Length;
            for (int index = 0; index < length; index++)
            {
                char character = keys[index];
                if (character.Equals('{'))
                {
                    int endIndex = keys.IndexOf('}', index + 1);
                    if (endIndex > -1)
                    {
                        string keycode = keys.Substring(index + 1, endIndex - index - 1);
                        if (keycodes.ContainsKey(keycode))
                        {
                            eventList.Add(CreateInput(keycodes[keycode], keyDown));
                        }
                        else
                        {
                            TouchPal.Warn("Bad keycode value:" + keycode);
                        }
                        index = endIndex;
                    }
                    else
                    {
                        TouchPal.Warn("Unterminated keycode!");
                        index = length+1;
                    }
                }
                eventList.Add(CreateInput((ushort)VkKeyScan(character), keyDown));
            }
            keyEvents = eventList.ToArray();
        }

        private static KEYBOARD_INPUT CreateInput(ushort virtualKeyCode, bool keyDown)
        {
            KEYBOARD_INPUT input = new KEYBOARD_INPUT();
            input.type = INPUT_KEYBOARD;
            input.flags = KEY_SCANCODE;

            uint scanCode = MapVirtualKey(virtualKeyCode, 0);
            if ((virtualKeyCode >= 33 && virtualKeyCode <= 46) || (virtualKeyCode >= 91 && virtualKeyCode <= 93) ||
                virtualKeyCode == 0xA1 || virtualKeyCode == 0xA3 || virtualKeyCode == 0xA5)
            {
                input.flags |= KEY_EXTENDED;
            }
            if (keyDown)
            {
                input.scanCode = (ushort)(scanCode & 0xFF);
            }
            else
            {
                input.scanCode = (ushort)scanCode;
                input.flags |= KEY_UP;
            }

            return input;
        }

        #region IAction Members

        void IAction.Execute(ControlManager manager)
        {
            uint result = SendInput((uint)keyEvents.Length, keyEvents, Marshal.SizeOf(keyEvents[0]));
            if (result != (uint)keyEvents.Length)
            {
                TouchPal.Error("SendInput did not process all key events.");
            }
        }

        #endregion
    }
}
