using RDMSharp.Metadata.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

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
        }
        public DataTreeBranch(MetadataJSONObjectDefine define, Command.ECommandDublicte commandType, params DataTree[] children): this(children)
        {
            if (define == null)
                throw new ArgumentNullException();

            ParsedObject = this.getParsedObject(define, commandType);
        }

        private object getParsedObject(MetadataJSONObjectDefine define, Command.ECommandDublicte commandType)
        {
            if (IsEmpty || IsUnset)
                return null;
            try
            {
                var definedDataTreeObjectType = MetadataFactory.GetDefinedDataTreeObjectType(define, commandType);
                if (definedDataTreeObjectType != null)
                {
                    if (definedDataTreeObjectType.IsEnum)
                        return Enum.ToObject(definedDataTreeObjectType, Children.Single().Value);

                    ConstructorInfo[] constructors = definedDataTreeObjectType.GetConstructors();
                    var objectAttribute = definedDataTreeObjectType.GetCustomAttributes<DataTreeObjectAttribute>().FirstOrDefault(a => (ushort)a.Parameter == define.PID && a.Command == commandType);


                    var children = Children;

                    if (!string.IsNullOrWhiteSpace(objectAttribute.Path))
                    {
                        string[] path = objectAttribute.Path.Split('/');
                        while (path.Length >= 1)
                        {
                            children = children.FirstOrDefault(c => string.Equals(c.Name, path[0])).Children;
                            path = path.Skip(1).ToArray();
                        }
                    }


                    foreach (var constructor in constructors)
                    {
                        if (constructor.GetCustomAttribute<DataTreeObjectConstructorAttribute>() is DataTreeObjectConstructorAttribute cAttribute)
                        {
                            var parameters = new List<object>();
                            foreach (var param in constructor.GetParameters())
                                if (param.GetCustomAttribute<DataTreeObjectParameterAttribute>() is DataTreeObjectParameterAttribute pAttribute)
                                {
                                    string name = pAttribute.Name;
                                    string[] path = name.Split('/');
                                    while (path.Length > 1)
                                    {
                                        children = children.FirstOrDefault(c => string.Equals(c.Name, path[0])).Children;
                                        path = path.Skip(1).ToArray();
                                        if (path.Length == 1)
                                            name = path[0];
                                    }
                                    if (!pAttribute.IsArray && children.FirstOrDefault(c => string.Equals(c.Name, pAttribute.Name)) is DataTree child)
                                        parameters.Add(child.Value);
                                    else if (pAttribute.IsArray && children.Where(c => string.Equals(c.Name, pAttribute.Name)).OfType<DataTree>() is IEnumerable<DataTree> childenum)
                                    {
                                        Type targetType = children.First().Value.GetType();
                                        var array = Array.CreateInstance(targetType, children.Length);
                                        Array.Copy(children.Select(c => c.Value).ToArray(), array, children.Length);
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

        public override bool Equals(object obj)
        {
            return obj is DataTreeBranch branch && Equals(branch);
        }

        public bool Equals(DataTreeBranch other)
        {
            return EqualityComparer<DataTree[]>.Default.Equals(Children, other.Children);
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