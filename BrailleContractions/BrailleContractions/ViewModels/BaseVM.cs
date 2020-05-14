using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BrailleContractions.ViewModels
{
    /// <summary>
    /// Base class for view models.
    /// </summary>
    public class BaseVM : INotifyPropertyChanged
    {
        /// <summary>
        /// Sets a property and invokes PropertyChanged if the value changed.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="backingField">The backing field of the property.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="onChanged">Optional additional action to invoke if the value changed.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>A boolean indicating whether the value changed.</returns>
        protected bool SetProperty<T>(ref T backingField, T value, Action onChanged = null, [CallerMemberName] string propertyName = null)
        {
            bool changed = !EqualityComparer<T>.Default.Equals(backingField, value);

            if (changed)
            {
                backingField = value;
                onChanged?.Invoke();
                OnPropertyChanged(propertyName);
            }

            return changed;
        }

        /// <summary>
        /// Implements <see cref="INotifyPropertyChanged"/>.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invokes <see cref="PropertyChanged"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
