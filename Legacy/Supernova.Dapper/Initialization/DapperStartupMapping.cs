using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dapper;
using Supernova.Dapper.Core.Attributes;

namespace Supernova.Dapper.Initialization
{
    public class DapperStartupMapping
    {
        /// <summary>
        /// Scans the provided namespaces for properties of the database entity models decorated with named column attributes and generates Dapper column type maps, in order to support entities, which have properties with different names than their Database table counterparts.
        /// </summary>
        /// <param name="namespacesToScan">Parameter list of the namespaces that contain your database entity models</param>
        public static void RegisterCustomMaps(params string[] namespacesToScan)
        {
            List<Type> typesInNamespaces = ExtractTypes(namespacesToScan);
            IDictionary<Type, CustomPropertyTypeMap> typeMaps = GetCustomTypeMaps(typesInNamespaces);

            foreach (KeyValuePair<Type, CustomPropertyTypeMap> typeMap in typeMaps)
            {
                SqlMapper.SetTypeMap(typeMap.Key, typeMap.Value);
            }
        }

        private static List<Type> ExtractTypes(string[] namespacesToScan)
        {
            List<Type> typesInNamespaces = new List<Type>();
            foreach (string @namespace in namespacesToScan)
            {
                IEnumerable<Type> typesToRegister = AppDomain
                    .CurrentDomain
                    .GetAssemblies()
                    .SelectMany(t => t.GetTypes())
                    .Where(t => !string.IsNullOrWhiteSpace(t.Namespace) && t.IsClass &&
                                t.Namespace.Contains(@namespace));

                typesInNamespaces.AddRange(typesToRegister);
            }

            return typesInNamespaces;
        }

        private static IDictionary<Type, CustomPropertyTypeMap> GetCustomTypeMaps(IEnumerable<Type> types)
        {
            Dictionary<Type, CustomPropertyTypeMap> customTypeMaps = 
                new Dictionary<Type, CustomPropertyTypeMap>();

            foreach (Type type in types)
            {
                Dictionary<string, string> columnMaps = new Dictionary<string, string>();
                PropertyInfo[] allProperties = type.GetProperties();
                foreach (PropertyInfo property in allProperties)
                {
                    BaseAttribute namedAttribute = property.GetCustomAttribute<BaseAttribute>();
                    if (!string.IsNullOrWhiteSpace(namedAttribute?.Name))
                    {
                        columnMaps.Add(namedAttribute.Name, property.Name);
                    }
                }

                CustomPropertyTypeMap currentTypeMap = new CustomPropertyTypeMap(type, (inputType, columnName) =>
                {
                    if (columnMaps.ContainsKey(columnName))
                    {
                        return inputType.GetProperty(columnMaps[columnName]);
                    }
                    else
                    {
                        return type.GetProperty(columnName);
                    }
                });

                customTypeMaps.Add(type, currentTypeMap);
            }

            return customTypeMaps;
        }
    }
}
