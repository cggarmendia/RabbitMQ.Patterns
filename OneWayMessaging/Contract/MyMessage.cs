﻿using System;

namespace Contract
{
    [Serializable]
    public class MyMessage : ICustomMessage
    {
        public string Message { get; set; }
    }
}
