using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace DomainShell.Infrastructure
{
    public class PropertyIntercepter<TModel>
    {
        public PropertyIntercepter(TModel model)
            : this(model, null)
        {

        }

        public PropertyIntercepter(TModel model, PropertyChangedEventHandler handler)
        {
            _model = model;

            _handler += (sender, args) =>
            {
                NotifyNonPublicBaseProperty(
                    sender,
                    args,
                    baseArgs =>
                    {
                        baseArgs.BasePropertyValue = baseArgs.PropertyValue;
                    });
            };
            

            if (handler != null)
            {
                _handler += handler;
            }
        }

        private TModel _model;
        private event PropertyChangedEventHandler _handler;

        private Dictionary<string, object> _propertyMap = new Dictionary<string, object>();
        private Dictionary<string, NotifyBasePropertyEventArgs> _notifyBasePropertyEventArgsMap = new Dictionary<string, NotifyBasePropertyEventArgs>();

        public T Get<T>([CallerMemberName] string propertyName = "")
        {
            object value;

            if (_propertyMap.TryGetValue(propertyName, out value))
            {
                return (T)value;
            }

            return default(T);
        }

        public void Set<T>(T value, [CallerMemberName] string propertyName = "")
        {
            _propertyMap[propertyName] = value;

            _handler(_model, new PropertyChangedEventArgs(propertyName));
        }

        private void NotifyNonPublicBaseProperty(object sender, PropertyChangedEventArgs args, Action<NotifyBasePropertyEventArgs> action)
        {
            NotifyBasePropertyEventArgs notifyBasePropertyEventArgs = null;

            if (!_notifyBasePropertyEventArgsMap.TryGetValue(args.PropertyName, out notifyBasePropertyEventArgs))
            {
                PropertyInfo baseProperty = typeof(TModel).BaseType.GetProperty(
                args.PropertyName,
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

                bool valid = true;

                if (baseProperty == null || !baseProperty.CanWrite)
                {
                    valid = false;
                }

                PropertyInfo property = typeof(TModel).GetProperty(args.PropertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | BindingFlags.DeclaredOnly);

                if (valid && (property == null || !property.CanRead))
                {
                    valid = false;
                }

                if (valid && (property.PropertyType != baseProperty.PropertyType))
                {
                    if (property.PropertyType.IsPrimitive)
                    {
                        valid = false;
                    }
                    else if (!property.PropertyType.IsSubclassOf(baseProperty.PropertyType))
                    {
                        valid = false;
                    }
                }

                if (valid)
                {
                    notifyBasePropertyEventArgs = new NotifyBasePropertyEventArgs(
                    sender,
                    args,
                    baseProperty,
                    property
                    );
                }

                _notifyBasePropertyEventArgsMap[args.PropertyName] = notifyBasePropertyEventArgs;
            }

            if (notifyBasePropertyEventArgs == null)
            {
                return;
            }

            action(notifyBasePropertyEventArgs);
        }

        private class NotifyBasePropertyEventArgs
        {
            public NotifyBasePropertyEventArgs(object sender, PropertyChangedEventArgs args, PropertyInfo baseProperty, PropertyInfo property)
            {
                _sender = sender;
                _args = args;
                _baseProperty = baseProperty;
                _property = property;
            }

            private object _sender;
            private PropertyChangedEventArgs _args;
            private PropertyInfo _baseProperty;
            private PropertyInfo _property;

            public string PropertyName
            {
                get
                {
                    return _args.PropertyName;
                }
            }

            public object BasePropertyValue
            {
                set
                {   
                    _baseProperty.SetValue(_sender, value);
                }
            }

            public object PropertyValue
            {
                get
                {
                    return _property.GetValue(_sender);
                }
            }

        }
    }    
}
