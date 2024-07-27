using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenMeteo
{
    public class CurrentOptions : ICollection<CurrentOptionsParameter>
    {
        /// <summary>
        /// Gets a new object containing every parameter
        /// </summary>
        /// <returns></returns>
        public static CurrentOptions All => new CurrentOptions((CurrentOptionsParameter[])Enum.GetValues(typeof(CurrentOptionsParameter)));

        /// <summary>
        /// Gets a copy of elements contained in the List.
        /// </summary>
        /// <typeparam name="CurrentOptionsParameter"></typeparam>
        /// <returns>A copy of elements contained in the List</returns>
        public List<CurrentOptionsParameter> Parameter => new List<CurrentOptionsParameter>(_parameter);

        public int Count => _parameter.Count;

        public bool IsReadOnly => false;

        private readonly List<CurrentOptionsParameter> _parameter = new List<CurrentOptionsParameter>();

        public CurrentOptions()
        {

        }

        private CurrentOptions(CurrentOptionsParameter[] parameter)
        {
            Add(parameter);
        }

        /// <summary>
        /// Index the collection
        /// </summary>
        /// <param name="index"></param>
        /// <returns><see cref="string"/> CurrentOptionsType as string representation at index</returns>
        public CurrentOptionsParameter this[int index]
        {
            get => _parameter[index];
            set => _parameter[index] = value;
        }

        public void Add(CurrentOptionsParameter param)
        {
            // Check that the parameter isn't already added
            if (_parameter.Contains(param)) return;

            _parameter.Add(param);
        }

        private void Add(CurrentOptionsParameter[] param)
        {
            foreach (CurrentOptionsParameter paramToAdd in param)
            {
                Add(paramToAdd);
            }
        }

        public void Clear()
        {
            _parameter.Clear();
        }

        public bool Contains(CurrentOptionsParameter item) 
            => _parameter.Contains(item);

        public bool Remove(CurrentOptionsParameter item) 
            => _parameter.Remove(item);

        public void CopyTo(CurrentOptionsParameter[] array, int arrayIndex)
        {
            _parameter.CopyTo(array, arrayIndex);
        }

        public IEnumerator<CurrentOptionsParameter> GetEnumerator() 
            => _parameter.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();
    }

    public enum CurrentOptionsParameter
    {
        temperature_2m,
        relativehumidity_2m,
        apparent_temperature,
        is_day, precipitation,
        rain,
        showers,
        snowfall,
        weathercode,
        cloudcover,
        pressure_msl,
        surface_pressure,
        windspeed_10m,
        winddirection_10m,
        windgusts_10m
    }
}
