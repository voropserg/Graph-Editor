using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace GraphEditor
{
    [Serializable]
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

    }
}
