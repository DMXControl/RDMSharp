using RDMSharp.Metadata.JSON;
using RDMSharp.Metadata.JSON.OneOfTypes;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

[assembly: InternalsVisibleTo("RDMSharpTests")]
namespace RDMSharp.Metadata
{
    public readonly struct DataTreeBranch : IEquatable<DataTreeBranch>
    {
        public static readonly DataTreeBranch Empty = new DataTreeBranch();
        public static readonly DataTreeBranch Unset = new DataTreeBranch(true);

        public readonly DataTree[] Children;
        public readonly bool IsEmpty;
        public readonly bool IsUnset;

        public readonly object ParsedObject;
        public DataTreeBranch()
        {
            IsEmpty = true;
        }
        private DataTreeBranch(bool isUnset)
        {
            IsUnset = true;
        }
        public DataTreeBranch(params DataTree[] children)
        {
            if (children.Length == 0)
                IsEmpty = true;

            Children = children;
            if (Children.Count(c => c.Index == 0) > 1)
                for (uint i = 0; i < Children.Length; i++)
                    Children[i] = new DataTree(Children[i], i);
        }
        private DataTreeBranch(object parsedObject, params DataTree[] children): this(children)
        {
            ParsedObject = parsedObject;
        }
        public DataTreeBranch(MetadataJSONObjectDefine define, Command.ECommandDublicte commandType, params DataTree[] children): this(children)
        {
            if (define == null)
                throw new ArgumentNullException();

            ParsedObject = this.getParsedObject(define, commandType);
        }

        private object getParsedObject(MetadataJSONObjectDefine define, Command.ECommandDublicte commandType)
        {
            ushort pid = define.PID;
            var definedDataTreeObjectType = MetadataFactory.GetDefinedDataTreeObjectType(define, commandType);
            return getParsedObject(pid, definedDataTreeObjectType, commandType);
        }
        private object getParsedObject(ushort pid, Type definedDataTreeObjectType, Command.ECommandDublicte commandType)
        {
            if (IsEmpty || IsUnset)
                return null;
            try
            {
                if (definedDataTreeObjectType != null)
                {
                    if (definedDataTreeObjectType.IsEnum)
                    {
                        var enumAttribute = definedDataTreeObjectType.GetCustomAttributes<DataTreeEnumAttribute>().FirstOrDefault(a => (ushort)a.Parameter == pid && a.Command == commandType);

                        var eChildren = getChildrenUsingPath(enumAttribute, Children);
                        if (enumAttribute.IsArray)
                        {
                            var array = Array.CreateInstance(definedDataTreeObjectType, eChildren.Length);
                            var aa = eChildren.Select(eC => Enum.ToObject(definedDataTreeObjectType, eC.Value)).ToArray();
                            Array.Copy(aa, array, eChildren.Length);
                            return array;
                        }
                        else
                            return Enum.ToObject(definedDataTreeObjectType, eChildren.Single().Value);
                    }

                    ConstructorInfo[] constructors = definedDataTreeObjectType.GetConstructors();
                    var objectAttribute = definedDataTreeObjectType.GetCustomAttributes<DataTreeObjectAttribute>().FirstOrDefault(a => (ushort)a.Parameter == pid && a.Command == commandType);


                    var children = getChildrenUsingPath(objectAttribute, Children);
                    DataTree[] getChildrenUsingPath(DataTreeObjectAttribute objectAttribute, DataTree[] children)
                    {
                        if (!string.IsNullOrWhiteSpace(objectAttribute.Path))
                        {
                            string[] path = objectAttribute.Path.Split('/');
                            while (path.Length >= 1)
                            {
                                children = children.FirstOrDefault(c => string.Equals(c.Name, path[0])).Children;
                                path = path.Skip(1).ToArray();
                            }
                        }
                        return children;
                    }


                    foreach (var constructor in constructors)
                    {

                        if (constructor.GetCustomAttribute<DataTreeObjectConstructorAttribute>() is DataTreeObjectConstructorAttribute cAttribute)
                        {
                            if (!children.All(c => c.IsCompound))
                                return createObjectFromDataTree(children);
                            else
                            {
                                var array = Array.CreateInstance(definedDataTreeObjectType, children.Length);
                                foreach (var comp in children)
                                    array.SetValue(createObjectFromDataTree(comp.Children), comp.Index);
                                return array;
                            }


                            object createObjectFromDataTree(DataTree[] children)
                            {
                                var parameters = new List<object>();
                                foreach (var param in constructor.GetParameters())
                                    if (param.GetCustomAttribute<DataTreeObjectParameterAttribute>() is DataTreeObjectParameterAttribute pAttribute)
                                    {
                                        var children2 = children;

                                        string name = pAttribute.Name;
                                        string[] path = name.Split('/');
                                        while (path.Length > 1)
                                        {
                                            children2 = children2.FirstOrDefault(c => string.Equals(c.Name, path[0])).Children;
                                            path = path.Skip(1).ToArray();
                                            if (path.Length == 1)
                                                name = path[0];
                                        }
                                        if (!pAttribute.IsArray && children2.FirstOrDefault(c => string.Equals(c.Name, name)) is DataTree child)
                                            parameters.Add(child.Value);
                                        else if (pAttribute.IsArray && children2.Where(c => string.Equals(c.Name, pAttribute.Name)).OfType<DataTree>() is IEnumerable<DataTree> childenum)
                                        {
                                            Type targetType = children2.First().Value.GetType();
                                            var array = Array.CreateInstance(targetType, children2.Length);
                                            Array.Copy(children2.Select(c => c.Value).ToArray(), array, children2.Length);
                                            parameters.Add(array);
                                        }
                                        else
                                            throw new ArgumentException($"No matching Value found for '{pAttribute.Name}'");
                                    }

                                var instance = constructor.Invoke(parameters.ToArray());
                                return instance;
                            }
                        }
                    }
                }

                if (Children.Length == 1)
                {
                    DataTree dataTree = Children[0];

                    if (dataTree.Value != null)
                        return dataTree.Value;

                    if (dataTree.Children.GroupBy(c => c.Name).Count() == 1)
                    {
                        var list = dataTree.Children.Select(c => c.Value).ToList();
                        Type targetType = list.First().GetType();
                        var array = Array.CreateInstance(targetType, list.Count);
                        Array.Copy(list.ToArray(), array, list.Count);
                        //for (int i = 0; i < list.Count; i++)
                        //    array.SetValue(Convert.ChangeType(list[i], targetType), i);

                        return array;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            throw new NotImplementedException();
        }

        public static DataTreeBranch FromObject(object obj, object key, ParameterBag parameterBag, ERDM_Command command)
        {
            var result = FromObject(obj, key, command, parameterBag.PID);
            if (result.IsUnset)
            {
                var define = MetadataFactory.GetDefine(parameterBag);
                if (define == null)
                    return result;
                Command? cmd = null;
                switch (command)
                {
                    case ERDM_Command.GET_COMMAND:
                        if (define.GetRequest.HasValue)
                            cmd = define.GetRequest;
                        break;
                    case ERDM_Command.GET_COMMAND_RESPONSE:
                        if (define.GetResponse.HasValue)
                            cmd = define.GetResponse;
                        break;
                    case ERDM_Command.SET_COMMAND:
                        if (define.SetRequest.HasValue)
                            cmd = define.SetRequest;
                        break;
                    case ERDM_Command.SET_COMMAND_RESPONSE:
                        if (define.SetResponse.HasValue)
                            cmd = define.SetResponse;
                        break;
                }
                if(!cmd.HasValue)
                    return result;
                List<DataTree> children = new List<DataTree>();

                if (cmd.Value.SingleField.HasValue)
                    children.Add(getChildren(cmd.Value.SingleField.Value, obj));
                if (cmd.Value.ListOfFields.Length > 0)
                {
                    if (cmd.Value.ListOfFields.Length > 1)
                        throw new NotImplementedException();


                    children.Add(getChildren(cmd.Value.ListOfFields[0], obj));

                }
                DataTree getChildren(OneOfTypes oneOf, object o)
                {
                    if (oneOf.ObjectType != null)
                        return new DataTree(oneOf.ObjectType.Name, 0, o);

                    throw new NotImplementedException();
                }
                if (children.Count != 0)
                    return new DataTreeBranch(children: children.ToArray());
            }
            return result;
        }

        public static DataTreeBranch FromObject(object obj, object key, ERDM_Command command, ERDM_Parameter parameter)
        {
            Type type = obj.GetType();

            if (type.IsGenericType && typeof(IDictionary).IsAssignableFrom(type.GetGenericTypeDefinition()))
            {
                Type[] genericArguments = type.GetGenericArguments();
                Type keyType = genericArguments[0];
                Type valueType = genericArguments[1];

                var tryGetValueMethod = type.GetMethod("TryGetValue");
                object[] parameters = { key, null };

                bool found = (bool)tryGetValueMethod.Invoke(obj, parameters);

                if (found)
                {
                    // Der Wert wird im zweiten Parameter (Index 1) gespeichert
                    object value = parameters[1];
                    obj = value;
                    type = value.GetType();
                }
            }

            bool isArray = type.IsArray;

            if (isArray)
                type = type.GetElementType();

            if (type.GetCustomAttributes<DataTreeObjectAttribute>().FirstOrDefault(a => a.Parameter == parameter && a.Command == Tools.ConvertCommandDublicteToCommand(command) && a.IsArray == isArray) is not DataTreeObjectAttribute dataTreeObjectAttribute)
                return DataTreeBranch.Unset;

            List<DataTree> children = new List<DataTree>();
            bool isCompound = false;
            if (!type.IsEnum)
            {
                var properties = type.GetProperties().Where(p => p.GetCustomAttributes<DataTreeObjectPropertyAttribute>().Count() != 0).ToArray();
                isCompound = properties.Length > 1;

                if (isArray)
                {
                    Array array = (Array)obj;
                    for (uint i = 0; i < array.Length; i++)
                        children.Add(new DataTree(null, i, convertToDataTree(array.GetValue(i), properties, parameter), isCompound: isCompound));
                }
                else
                    children.AddRange(convertToDataTree(obj, properties, parameter));
            }
            else
            {
                DataTreeEnumAttribute enumAttribute = dataTreeObjectAttribute as DataTreeEnumAttribute;
                if (isArray)
                {
                    Array array = (Array)obj;
                    for (uint i = 0; i < array.Length; i++)
                        children.Add(new DataTree(enumAttribute.Name, i, getUnderlyingValue(array.GetValue(i))));
                }
                else
                    children.Add(new DataTree(enumAttribute.Name, 0, getUnderlyingValue(obj)));
            }

            if (!string.IsNullOrWhiteSpace(dataTreeObjectAttribute.Path))
            {
                string[] path = dataTreeObjectAttribute.Path.Split('/');
                DataTree? route = null;
                for (int i = path.Length; i != 0; i--)
                {
                    if (route.HasValue)
                        route = new DataTree(path[i - 1], 0, route);
                    else
                        route = new DataTree(path[i - 1], 0, children.ToArray());
                }

                return new DataTreeBranch(obj, route.Value);
            }

            return new DataTreeBranch(obj, children.ToArray());

            static DataTree[] convertToDataTree(object value, PropertyInfo[] properties, ERDM_Parameter parameter)
            {
                List<DataTree> innetChildren = new List<DataTree>();
                Dictionary<string, List<DataTree>> deeperChildren = new Dictionary<string, List<DataTree>>();
                foreach (var property in properties)
                {
                    var attributes = property.GetCustomAttributes<DataTreeObjectPropertyAttribute>();
                    DataTreeObjectPropertyAttribute attribute = attributes.FirstOrDefault();
                    if (attributes.Count() != 1)
                        attribute = attributes.FirstOrDefault(a => a.Parameter == parameter);


                    if (attribute != null)
                    {
                        var val = property.GetValue(value);
                        if (val is Enum)
                            val = getUnderlyingValue(val);
                        if (attribute.Name.Contains("/"))
                        {
                            string[] path = attribute.Name.Split('/');
                            if (!deeperChildren.TryGetValue(path[0], out List<DataTree> ddc))
                            {
                                ddc = new List<DataTree>();
                                deeperChildren.TryAdd(path[0], ddc);
                            }
                            ddc.Add(new DataTree(path[1], attribute.Index, val));
                        }
                        else
                            innetChildren.Add(new DataTree(attribute.Name, attribute.Index, val));
                    }
                }
                foreach (var dC in deeperChildren)
                {
                    var index = FindMissingNumbers(innetChildren.Select(ic => (int)ic.Index)).FirstOrDefault();
                    innetChildren.Add(new DataTree(dC.Key, (uint)index, children: dC.Value.OrderBy(c => c.Index).ToArray()));
                }
                return innetChildren.OrderBy(iC => iC.Index).ToArray();

                static IEnumerable<int> FindMissingNumbers(IEnumerable<int> numbers)
                {
                    // Liste sortieren
                    var sortedNumbers = numbers.OrderBy(n => n).ToList();

                    // Bereich (Range) bestimmen
                    int min = sortedNumbers.First();
                    int max = sortedNumbers.Last();

                    // Alle erwarteten Zahlen im Bereich erstellen
                    var fullRange = Enumerable.Range(min, max - min + 1);

                    // Fehlende Zahlen durch Differenz finden
                    return fullRange.Except(sortedNumbers);
                }
            }
            static object getUnderlyingValue(object enumValue)
            {
                // Ermitteln des zugrunde liegenden Typs
                Type underlyingType = Enum.GetUnderlyingType(enumValue.GetType());

                // Konvertierung des Enum-Werts in den zugrunde liegenden Typ
                return Convert.ChangeType(enumValue, underlyingType);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is DataTreeBranch branch && Equals(branch);
        }

        public bool Equals(DataTreeBranch other)
        {
            for (int i = 0; i < Children.Length; i++)
            {
                DataTree me = Children[i];
                if ((other.Children?.Length ?? 0) <= i)
                    return false;
                DataTree ot = other.Children[i];
                if (!me.Equals(ot))
                    return false;
            }
            return true;
           // return EqualityComparer<DataTree[]>.Default.Equals(Children, other.Children); // is not dooing its job
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Children);
        }

        public static bool operator ==(DataTreeBranch left, DataTreeBranch right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DataTreeBranch left, DataTreeBranch right)
        {
            return !(left == right);
        }
    }
}