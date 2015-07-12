﻿namespace OpenInput
{
    using System;
    using DI_Keyboard = SharpDX.DirectInput.Keyboard;

    /// <summary>
    /// 
    /// </summary>
    public class Keyboard : IKeyboard
    {
        /// <inheritdoc />
        public string Name => "Keyboard";

        /// <inheritdoc />
        public ITextInput TextInput => textInput;
        private TextInput textInput;

        internal readonly DI_Keyboard keyboard;

        /// <summary>
        /// 
        /// </summary>
        public Keyboard()
        {
            var directInput = DeviceService.Service.Value.directInput;

            this.keyboard = new DI_Keyboard(directInput);
            this.keyboard.Acquire();
        }

        /// <inheritdoc />
        public KeyboardState GetCurrentState()
        {
            if (keyboard.IsDisposed)
                return new KeyboardState();

            keyboard.Poll();

            var state = keyboard.GetCurrentState();

            Keys[] keys = new Keys[state.PressedKeys.Count];
            for (int i = 0; i < state.PressedKeys.Count; i++)
                keys[i] = SharpDXConverters.Convert(state.PressedKeys[i]);

            return new KeyboardState(keys);
        }
    }

    class TextInput : ITextInput
    {
        public bool Capture
        {
            get { return capture; }
            set { capture = value; }
        }

        public string Result
        {
            get { return result; }
            set { result = value; }
        }

        public bool AllowNewLine
        {
            get { return allowNewLine; }
            set { allowNewLine = value; }
        }

        private bool capture;
        private string result;
        private bool allowNewLine;

        public TextInput()
        {
            this.Capture = false;
            this.Result = string.Empty;
            this.allowNewLine = false;
        }

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