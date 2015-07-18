﻿namespace OpenInput.DirectInput
{
    using System;
    using CooperativeLevel = SharpDX.DirectInput.CooperativeLevel;
    using DirectInputKeyboard = SharpDX.DirectInput.Keyboard;

    /// <summary>
    /// DirectInput Keyboard.
    /// </summary>
    /// <remarks>
    /// * Bad for GUI-style text input.
    /// * You have to detect caps lock / shift.
    /// * No support for keymaps other than US-Englis.
    /// http://www.gamedev.net/blog/233/entry-1567278-reasons-not-to-use-directinput-for-keyboard-input/
    /// </remarks>
    public class Keyboard : IKeyboard
    {
        /// <summary>
        /// [SharpDX.DirectInput] ApiCode: [E_NOTIMPL/Not implemented], Message: Not implemented
        /// </summary>
        public string Name => "Keyboard";

        /// <inheritdoc />
        public TextInput TextInput => textInput;

        private TextInput textInput;
        private KeyboardState previusState;

        internal readonly DirectInputKeyboard keyboard;

        /// <summary>
        /// Initializes a new instance of the <see cref="Keyboard"/> class.
        /// </summary>
        public Keyboard()
        {
            var directInput = DeviceService.Service.Value.directInput;

            this.keyboard = new DirectInputKeyboard(directInput);
            this.keyboard.Acquire();

            this.textInput = new DirectInput_TextInput();
        }

        /// <inheritdoc />
        public KeyboardState GetCurrentState()
        {
            if (keyboard.IsDisposed)
                return previusState = new KeyboardState(new Keys[] { });
            
            keyboard.Poll();
                
            var state = keyboard.GetCurrentState();
                
            Keys[] keys = new Keys[state.PressedKeys.Count];
            for (int i = 0; i < state.PressedKeys.Count; i++)
                keys[i] = KeyMapper.Convert(state.PressedKeys[i]);

            var currentState = new KeyboardState(keys);
                
            if (TextInput.Capture) // I could pull from keyboard.GetBufferedData()
            {
                var shift = currentState.IsKeyDown(Keys.LeftShift) | currentState.IsKeyDown(Keys.RightShift);

                var compare = currentState.Compare(previusState);
                foreach (var key in compare.Item1)
                {
                    if (InputHelper.IsLetter(key))
                    {
                        var keyChar = InputHelper.ToText(key);
                        TextInput.Result += shift ? keyChar[0] : (char)(keyChar[0] + 32);
                    }

                    if (key == Keys.Space)
                    {
                        TextInput.Result += " ";
                    }

                    if (TextInput.Result.Length > 0 && key == Keys.Back)
                    {
                        TextInput.Result = TextInput.Result.Remove(TextInput.Result.Length - 1);
                    }

                    if (TextInput.AllowNewLine && key == Keys.Enter)
                    {
                        TextInput.Result += Environment.NewLine;
                    }
                }
            }

            return previusState = currentState;
        }
    }

    public class DirectInput_TextInput : TextInput
    {
        /*
        
            // TODO: I would like to add support to other letters/symbols like åäö and symbols from other languages
            if (InputHelper.IsLetter(e.Key))
                Result += e.KeyChar;

            if (e.Key == Keys.Back && Result.Length > 0)
            {
                Result = Result.Remove(Result.Length - 1);
            }

            if (AllowNewLine && e.Key == Keys.Enter)
            {
                Result += Environment.NewLine;
            }
        */
    }
}