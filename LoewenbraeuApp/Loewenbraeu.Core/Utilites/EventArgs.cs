using System;


namespace Loewenbraeu.Core
{
	public class EventArgs<T> : System.EventArgs
    {
       private T _value;
		
       public T Value
       { 
           get
           {
               return _value;
           }
           private set
           {
               _value = value;
           }
       }
       
        public EventArgs() : base()
        {
        }

        public EventArgs(T value) : this()
        {
            _value = value;
        }
    }
}



