using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibraryBase.Wpf.ViewModel;

namespace ZicoresTradingPostNotifier.Model
{
    public enum MessageType
    {
        Info,
        Warning,
        Error
    }

    public class Message : BindableBase
    {
        public Message()
        {

        }

        public static Message Create(String title, String message, MessageType type)
        {
            Message msg = new Message()
            {
                Title = title,
                Text = message,
                MessageType = type
            };
            return msg;
        }

        public static Message CreateInfo(String title, String message)
        {
            return Create(title, message, MessageType.Info);
        }

        public static Message CreateWarning(String title, String message)
        {
            return Create(title, message, MessageType.Warning);
        }

        public static Message CreateError(String title, String message)
        {
            return Create(title, message, MessageType.Error);
        }

        String _title;
        public String Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        String _text;
        public String Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        MessageType _messageType = MessageType.Info;
        public MessageType MessageType
        {
            get { return _messageType; }
            set
            {
                _messageType = value;
                OnPropertyChanged("MessageType");
            }
        }
    }
}
