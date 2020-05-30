using System;

namespace Contract
{
    [Serializable]
    public class MyOtherMessage : ICustomMessage
    {
        public string Message { get; set; }
    }
}
