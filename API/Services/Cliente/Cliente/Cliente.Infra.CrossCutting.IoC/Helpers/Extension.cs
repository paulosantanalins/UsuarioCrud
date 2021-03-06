﻿using System;
using System.Linq;

namespace Cliente.Infra.CrossCutting.IoC.Helpers
{
    public static class Extension
    {
        public static bool Filtrar(this string str, string[] toFilter)
        {
            foreach (var s in toFilter)
            {
                if (str.Contains(s))
                {
                    return true;
                }
            }
            return false;
        }
        public static IQueryable<T> WhereAtLeastOneProperty<T, PropertyType>(this IQueryable<T> source, Predicate<PropertyType> predicate)
        {
            var properties = typeof(T).GetProperties().Where(prop => prop.CanRead && prop.PropertyType == typeof(PropertyType)).ToArray();
            return source.Where(item => properties.Any(prop => PropertySatisfiesPredicate(predicate, item, prop)));
        }

        private static bool PropertySatisfiesPredicate<T, PropertyType>(Predicate<PropertyType> predicate, T item, System.Reflection.PropertyInfo prop)
        {
            try
            {
                return predicate((PropertyType)prop.GetValue(item));
            }
            catch
            {
                return false;
            }
        }

    }

}
