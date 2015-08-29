﻿using System;
using PokeD.Core.Wrappers;

namespace PokeD.Server.Windows.WrapperInstances
{
    public class InputWrapperInstance : IInputWrapper
    {
        public event OnKeys OnKey;

        public InputWrapperInstance() { }

        public void ShowKeyboard() { }

        public void HideKeyboard() { }

        public void ConsoleWrite(string message)
        {
            Console.WriteLine(message);
        }
    }
}
