using Microsoft.Extensions.Logging;
using RDMSharp.Metadata.JSON;
using RDMSharp.Metadata.JSON.OneOfTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RDMSharpTests")]
namespace RDMSharp.Metadata;

public readonly struct DataTreeBranch : IEquatable<DataTreeBranch>
{
    private static ILogger Logger = Logging.CreateLogger<DataTreeBranch>();
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
    private DataTreeBranch(object parsedObject, params DataTree[] children) : this(children)
    {
        ParsedObject = parsedObject;
    }
    public DataTreeBranch(MetadataJSONObjectDefine define, Command.ECommandDublicate commandType, params DataTree[] children) : this(children)
    {
        if (define == null)
            throw new ArgumentNullException();

        try
        {
            ParsedObject = this.getParsedObject(define, commandType);
        }
        catch (Exception e)
        {
            Logger?.LogWarning($"Can't get a Parsed Object for {define.Name} (0x{define.PID:X4})", e);
        }
    }

    private object getParsedObject(MetadataJSONObjectDefine define, Command.ECommandDublicate commandType)
    {
        ushort pid = define.PID;
        var definedDataTreeObjectType = MetadataFactory.GetDefinedDataTreeObjectType(define, commandType);
        return getParsedObject(define.ManufacturerID, pid, definedDataTreeObjectType, commandType);
    }
    private object getParsedObject(ushort manufacturerID, ushort pid, Type definedDataTreeObjectType, Command.ECommandDublicate commandType)
    {
        if (IsEmpty || IsUnset)
            return null;
        try
        {
            bool checkManufacturer(ushort attrManuId)
            {
                if (attrManuId == (ushort)EManufacturer.ESTA)
                    return true;

                return attrManuId == manufacturerID;
            }

            if (definedDataTreeObjectType != null)
            {
                if (definedDataTreeObjectType.IsEnum)
                {
                    var enumAttribute = definedDataTreeObjectType.GetCustomAttributes<DataTreeEnumAttribute>().FirstOrDefault(a => (ushort)a.Parameter == pid && a.Command == commandType && checkManufacturer((ushort)a.Manufacturer));

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
                var objectAttribute = definedDataTreeObjectType.GetCustomAttributes<DataTreeObjectAttribute>().FirstOrDefault(a => (ushort)a.Parameter == pid && a.Command == commandType && checkManufacturer((ushort)a.Manufacturer));


                var flatDataTree = flateningDateTree(Children);
                //var children = getChildrenUsingPath(objectAttribute, Children);
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
                        DataTreeObjectParameterAttribute getOPA(ParameterInfo parameterInfo)
                        {
                            var attributes = parameterInfo.GetCustomAttributes<DataTreeObjectParameterAttribute>();
                            if (attributes.Count() == 1)
                                return attributes.Single();

                            return attributes.Single(a => (ushort)a.Parameter == pid);
                        }
                        #region Fast path for non-compound objects with matching parameter attributes
                        var cParameters = constructor.GetParameters();
                        var pAttributes = cParameters.Select(p => getOPA(p)).ToArray();

                        if (pAttributes.Select(p => p.Name).SequenceEqual(flatDataTree.Keys))
                        {
                            List<object> parameters = new List<object>();
                            bool _break = false;
                            for (int i = 0; i < cParameters.Length; i++)
                            {
                                var constructorParameter = cParameters[i];
                                var parameterAttribute = pAttributes[i];
                                if (flatDataTree.TryGetValue(parameterAttribute.Name, out object value))
                                {
                                    Type expectedType = Nullable.GetUnderlyingType(constructorParameter.ParameterType) ?? constructorParameter.ParameterType;

                                    if (value is null)
                                        if (expectedType.IsValueType)
                                            value = Activator.CreateInstance(expectedType); // set to default value if null so it can be assigned to value types, if the parameter is nullable this will be overwritten to null in the constructor invoke and if not it will be set to default value which is the best we can do in this case

                                    if (expectedType == value?.GetType() || value is null)
                                        parameters.Add(value);
                                    else if (value is object[] compoundObjectArray)
                                    {
                                        ConstructorInfo constructorCompound = constructorParameter.ParameterType.GetElementType().GetConstructors().FirstOrDefault(c => c.GetCustomAttribute<DataTreeObjectConstructorAttribute>() is DataTreeObjectConstructorAttribute);
                                        DataTreeObjectConstructorAttribute cAttributeCompound = constructorCompound.GetCustomAttribute<DataTreeObjectConstructorAttribute>();
                                        var pAttributesCompound = constructorCompound.GetParameters().Select(p => p.GetCustomAttribute<DataTreeObjectParameterAttribute>()).ToArray();
                                        value = getCompoundObject(compoundObjectArray, pAttributesCompound, constructorCompound);
                                        parameters.Add(value);
                                    }
                                    else
                                    {
                                        Logger?.LogInformation($"Can't find propperty for {parameterAttribute.Name}, Expected Type: {constructorParameter.ParameterType}, Actual Type: {value?.GetType()}");
                                        _break = true;
                                    }
                                }
                            }
                            if (!_break && cParameters.Count() == parameters.Count)
                                return constructor.Invoke(parameters.ToArray());
                        }
                        //If the Command is only a List with one compound object, the Path can be the root key
                        else if (flatDataTree.Count == 1 &&
                            flatDataTree.TryGetValue(objectAttribute.Path, out object compoundObjects))
                            if (compoundObjects is object[] compoundObjectArray)
                                return getCompoundObject(compoundObjectArray, pAttributes, constructor);
                            else if (compoundObjects is null)
                                return Array.CreateInstance(definedDataTreeObjectType, 0);
                        #endregion
                        static object getCompoundObject(object[] compoundObjectArray, DataTreeObjectParameterAttribute[] pAttributes, ConstructorInfo constructor)
                        {
                            List<object> parameters = new List<object>();
                            List<object> results = new List<object>();
                            foreach (IReadOnlyDictionary<string, object> compoundDict in compoundObjectArray)
                            {
                                foreach (var parameterAttribute in pAttributes)
                                {
                                    if (compoundDict.TryGetValue(parameterAttribute.Name, out object value))
                                        parameters.Add(value);
                                }
                                results.Add(constructor.Invoke(parameters.ToArray()));
                                parameters.Clear();
                            }
                            Type targetType = results.First().GetType();
                            var array = Array.CreateInstance(targetType, results.Count);
                            Array.Copy(results.ToArray(), array, results.Count);
                            return array;
                        }
                        #region Replaced by code above - keeping in case i need to revert
                        //if (!children.All(c => c.IsCompound))
                        //    return createObjectFromDataTree(objectAttribute, children);
                        //else
                        //{
                        //    var array = Array.CreateInstance(definedDataTreeObjectType, children.Length);
                        //    foreach (var comp in children)
                        //        array.SetValue(createObjectFromDataTree(objectAttribute, comp.Children), comp.Index);
                        //    return array;
                        //}


                        //object createObjectFromDataTree(DataTreeObjectAttribute objectAttribute, DataTree[] children)
                        //{
                        //    var parameters = new List<object>();
                        //    foreach (var param in constructor.GetParameters())
                        //        if (param.GetCustomAttribute<DataTreeObjectParameterAttribute>() is DataTreeObjectParameterAttribute pAttribute)
                        //        {
                        //            var children2 = children;

                        //            string name = pAttribute.Name;
                        //            string[] path = name.Split('/');
                        //            while (path.Length > 1)
                        //            {
                        //                children2 = children2.FirstOrDefault(c => string.Equals(c.Name, path[0])).Children;
                        //                path = path.Skip(1).ToArray();
                        //                if (path.Length == 1)
                        //                    name = path[0];
                        //            }
                        //            if (!pAttribute.IsArray && children2.FirstOrDefault(c => string.Equals(c.Name, name)) is DataTree child)
                        //                parameters.Add(child.Value);
                        //            else if (pAttribute.IsArray && children2.Where(c => string.Equals(c.Name, pAttribute.Name)).OfType<DataTree>() is IEnumerable<DataTree> childenum)
                        //            {
                        //                Type targetType = children2.First().Value.GetType();
                        //                var array = Array.CreateInstance(targetType, children2.Length);
                        //                Array.Copy(children2.Select(c => c.Value).ToArray(), array, children2.Length);
                        //                parameters.Add(array);
                        //            }
                        //            else
                        //                throw new ArgumentException($"No matching Value found for '{pAttribute.Name}'");
                        //        }

                        //    var instance = constructor.Invoke(parameters.ToArray());
                        //    return instance;
                        //}
                        #endregion
                    }
                }
            }

            #region Replaced by code above - keeping in case i need to revert
            if (Children.Length == 1)
            {
                var flatDataTree = flateningDateTree(Children);
                DataTree dataTree = Children[0];

                if (flatDataTree.Count == 1 && flatDataTree.Values.FirstOrDefault() is object value)
                    return value;

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
            #endregion
        }
        catch (Exception e)
        {
            Logger?.LogError(e);
#pragma warning disable CA2200
            throw e;
#pragma warning restore CA2200
        }
        throw new NotImplementedException();
    }
    private Dictionary<string, object> flateningDateTree(DataTree[] children)
    {
        Dictionary<string, object> result = new Dictionary<string, object>();
        foreach (var child in children)
        {
            if (child.Value != null)
                result.TryAdd(child.Name, child.Value);
            else if (child.Children?.Length == 0)
                result.TryAdd(child.Name, null);
            else if (child.Children != null)
            {
                if (child.IsCompound)
                    result.TryAdd(child.Name, flateningDateTree(child.Children));
                else if (child.IsList)
                {
                    List<object> list = new List<object>();
                    foreach (var grandChild in child.Children)
                    {
                        if (grandChild.Value != null)
                            list.Add(grandChild.Value);
                        else if (grandChild.Children != null)
                            list.Add(flateningDateTree(grandChild.Children));
                    }
                    Type targetType = list.First().GetType();
                    var array = Array.CreateInstance(targetType, list.Count);
                    Array.Copy(list.ToArray(), array, list.Count);
                    result.Add(child.Name, array);
                }
                else //Is BitField
                {
                    foreach (var grandChild in child.Children)
                    {
                        if (grandChild.Value != null)
                            result.TryAdd(string.Join('/', child.Name, grandChild.Name), grandChild.Value);
                    }
                }
            }
        }
        return result;
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
            if (!cmd.HasValue)
                return result;
            List<DataTree> children = new List<DataTree>();

            switch (cmd.Value.EnumValue)
            {
                case Command.ECommandDublicate.GetRequest:
                    cmd = define.GetRequest;
                    break;
                case Command.ECommandDublicate.GetResponse:
                    cmd = define.GetResponse;
                    break;
                case Command.ECommandDublicate.SetRequest:
                    cmd = define.SetRequest;
                    break;
                case Command.ECommandDublicate.SetResponse:
                    cmd = define.SetResponse;
                    break;
            }
            if (cmd.Value.SingleField.HasValue)
                children.Add(getChildren(cmd.Value.SingleField.Value, obj));
            if ((cmd.Value.ListOfFields?.Length ?? 0) > 0)
            {
                if (cmd.Value.ListOfFields.Length > 1)
                    throw new NotImplementedException();


                children.Add(getChildren(cmd.Value.ListOfFields[0], obj));

            }
            DataTree getChildren(OneOfTypes oneOf, object o)
            {
                var oneofOt = oneOf.ObjectType;
                if (oneofOt == null && oneOf.ReferenceType.HasValue)
                    oneofOt = oneOf.ReferenceType.Value.ReferencedObject;

                if (oneofOt != null)
                    return new DataTree(oneofOt.Name, 0, o);

                throw new NotImplementedException();
            }
            if (children.Count != 0)
                return new DataTreeBranch(obj, children: children.ToArray());
            if (cmd.Value.GetIsEmpty())
                return DataTreeBranch.Empty;
        }
        return result;
    }

    internal static DataTreeBranch FromObject(object obj, object key, ERDM_Command command, ERDM_Parameter parameter)
    {
        if (obj == null)
            return DataTreeBranch.Empty;

        Type type = obj.GetType();

        if (type.IsGenericType && typeof(IDictionary).IsAssignableFrom(type.GetGenericTypeDefinition()))
        {
            Type[] genericArguments = type.GetGenericArguments();
            Type keyType = genericArguments[0];
            Type valueType = genericArguments[1];

            var tryGetValueMethod = type.GetMethod("TryGetValue");
            object[] parameters = { key, null };
            bool found = false;
            try
            {
                found = (bool)tryGetValueMethod.Invoke(obj, parameters);
            }
            catch (Exception e)
            {
                Logger?.LogError(e);
            }

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

        var dataTreeObjectAttributes = type.GetCustomAttributes<DataTreeObjectAttribute>();
        if (dataTreeObjectAttributes.FirstOrDefault(a => a.Parameter == parameter && a.Command == Tools.ConvertCommandDublicateToCommand(command)) is not DataTreeObjectAttribute dataTreeObjectAttribute)
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
            List<DataTree> innerChildren = new List<DataTree>();
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
                        if (!val.GetType().IsArray)
                        {
                            if (!deeperChildren.TryGetValue(path[0], out List<DataTree> ddc))
                            {
                                ddc = new List<DataTree>();
                                deeperChildren.TryAdd(path[0], ddc);
                            }
                            ddc.Add(new DataTree(path[1], attribute.Index, val));
                        }
                        else
                        {
                            List<DataTree> _children = new List<DataTree>();
                            uint _index = 0;
                            foreach (var cv in (Array)val)
                            {
                                string _name = path[1];
                                if (string.IsNullOrWhiteSpace(_name))
                                    _name = null;
                                if (!cv.GetType().GetConstructors().Any(c => c.GetCustomAttribute<DataTreeObjectConstructorAttribute>() is not null))
                                    _children.Add(new DataTree(_name, _index, cv));
                                else
                                {
                                    var _props = cv.GetType().GetProperties().Where(p => p.GetCustomAttributes<DataTreeObjectPropertyAttribute>() is not null);
                                    _children.Add(new DataTree(_name, _index, convertToDataTree(cv, _props.ToArray(), parameter)));
                                }
                                _index++;
                            }

                            innerChildren.Add(new DataTree(path[0], attribute.Index, _children.ToArray()));
                        }
                    }
                    else
                        innerChildren.Add(new DataTree(attribute.Name, attribute.Index, val ?? attribute.NullValue));
                }
            }
            foreach (var dC in deeperChildren)
            {
                var index = FindMissingNumbers(innerChildren.Select(ic => (int)ic.Index)).FirstOrDefault();
                innerChildren.Add(new DataTree(dC.Key, (uint)index, children: dC.Value.OrderBy(c => c.Index).ToArray()));
            }
            return innerChildren.OrderBy(iC => iC.Index).ToArray();

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
        if (this.IsUnset != other.IsUnset || this.IsEmpty != other.IsEmpty)
            return false;

        if ((this.Children is null) && (other.Children is null))
            return true;

        if ((this.Children is null) || (other.Children is null))
            return false;

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

    public override string ToString()
    {
        if (Children is not null)
        {
            return "DTB:" + Environment.NewLine + String.Join(Environment.NewLine, Children.Select(c => $"{c.Name} = {c.Value}"));
        }
        return base.ToString();
    }
}