﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenMeteo
{
    public class DailyOptions : ICollection<DailyOptionsParameter>
    {
        /// <summary>
        /// Gets a new object containing every parameter
        /// </summary>
        /// <returns></returns>
        public static DailyOptions All => new DailyOptions((DailyOptionsParameter[])Enum.GetValues(typeof(DailyOptionsParameter)));

        /// <summary>
        /// Gets a copy of elements contained in the List.
        /// </summary>
        /// <typeparam name="DailyOptionsParameter"></typeparam>
        /// <returns>A copy of elements contained in the List</returns>
        public List<DailyOptionsParameter> Parameter => new List<DailyOptionsParameter>(_parameter);

        public int Count => _parameter.Count;

        public bool IsReadOnly => false;

        private readonly List<DailyOptionsParameter> _parameter = new List<DailyOptionsParameter>();

        public DailyOptions()
        {

        }

        private DailyOptions(DailyOptionsParameter[] parameter)
        {
            Add(parameter);
        }

        /// <summary>
        /// Index the collection
        /// </summary>
        /// <param name="index"></param>
        /// <returns><see cref="string"/> DailyOptionsType as string representation at index</returns>
        public DailyOptionsParameter this[int index]
        {
            get => _parameter[index];
            set => _parameter[index] = value;
        }

        public void Add(DailyOptionsParameter param)
        {
            // Check that the parameter isn't already added
            if (_parameter.Contains(param)) 
                return;

            _parameter.Add(param);
        }

        private void Add(DailyOptionsParameter[] param)
        {
            foreach (var paramToAdd in param)
            {
                Add(paramToAdd);
            }
        }

        public void Clear()
        {
            _parameter.Clear();
        }

        public bool Contains(DailyOptionsParameter item) 
            => _parameter.Contains(item);

        public bool Remove(DailyOptionsParameter item) 
            => _parameter.Remove(item);

        public void CopyTo(DailyOptionsParameter[] array, int arrayIndex)
        {
            _parameter.CopyTo(array, arrayIndex);
        }

        public IEnumerator<DailyOptionsParameter> GetEnumerator() 
            => _parameter.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();
    }

    public enum DailyOptionsParameter
    {
        weathercode,
        temperature_2m_max,
        temperature_2m_min,
        apparent_temperature_max,
        apparent_temperature_min,
        sunrise,
        sunset,
        precipitation_sum,
        rain_sum,
        showers_sum,
        snowfall_sum,
        precipitation_hours,
        windspeed_10m_max,
        windgusts_10m_max,
        winddirection_10m_dominant,
        shortwave_radiation_sum,
        et0_fao_evapotranspiration
    }
}
