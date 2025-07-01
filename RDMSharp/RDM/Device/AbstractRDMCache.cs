using Microsoft.Extensions.Logging;
using RDMSharp.Metadata;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RDMSharp
{
    public abstract class AbstractRDMCache : IDisposable
    {
        protected static ILogger Logger = Logging.CreateLogger<AbstractRDMCache>();
        protected bool IsDisposed { get; private set; }
        protected bool IsDisposing { get; private set; }
        internal ConcurrentDictionary<ParameterDataCacheBag, DataTreeBranch> parameterValuesDataTreeBranch { get; private set; } = new ConcurrentDictionary<ParameterDataCacheBag, DataTreeBranch>();
        protected ConcurrentDictionary<DataTreeObjectDependeciePropertyBag, object> parameterValuesDependeciePropertyBag { get; private set; } = new ConcurrentDictionary<DataTreeObjectDependeciePropertyBag, object>();

        protected ConcurrentDictionary<ERDM_Parameter, object> parameterValues { get; private set; } = new ConcurrentDictionary<ERDM_Parameter, object>();
        public virtual IReadOnlyDictionary<ERDM_Parameter, object> ParameterValues
        {
            get { return this.parameterValues?.AsReadOnly(); }
        }
        protected event EventHandler<ParameterValueAddedEventArgs> ParameterValueAdded;
        protected event EventHandler<ParameterValueChangedEventArgs> ParameterValueChanged;
        protected event EventHandler<ParameterRequestedEventArgs> ParameterRequested;

        public class ParameterValueAddedEventArgs : EventArgs
        {
            public readonly ERDM_Parameter Parameter;
            public readonly object Index;
            public readonly object Value;

            public ParameterValueAddedEventArgs(ERDM_Parameter parameter, object value, object index = null)
            {
                Parameter = parameter;
                Index = index;
                Value = value;
            }
        }
        public class ParameterValueChangedEventArgs : EventArgs
        {
            public readonly ERDM_Parameter Parameter;
            public readonly object Index;
            public readonly object NewValue;
            public readonly object OldValue;

            public ParameterValueChangedEventArgs(ERDM_Parameter parameter, object newValue, object oldValue, object index = null)
            {
                Parameter = parameter;
                Index = index;
                NewValue = newValue;
                OldValue = oldValue;
            }
        }
        public class ParameterRequestedEventArgs : EventArgs
        {
            public readonly ERDM_Parameter Parameter;
            public readonly object Index;

            public ParameterRequestedEventArgs(ERDM_Parameter parameter, object index = null)
            {
                Parameter = parameter;
                Index = index;
            }
        }

        public AbstractRDMCache()
        {

        }

        protected void InvokeParameterValueAdded(ParameterValueAddedEventArgs e)
        {
            this.ParameterValueAdded?.InvokeFailSafe(this, e);
        }


        protected void updateParameterValuesDependeciePropertyBag(ERDM_Parameter parameter, DataTreeBranch dataTreeBranch)
        {
            object obj = dataTreeBranch.ParsedObject;
            if (obj == null)
                return;

            foreach (var p in obj.GetType().GetProperties().Where(p => p.GetCustomAttributes<DataTreeObjectDependeciePropertyAttribute>().Any()).ToList())
            {
                object value = p.GetValue(obj);
                foreach (var item in p.GetCustomAttributes<DataTreeObjectDependeciePropertyAttribute>())
                    parameterValuesDependeciePropertyBag.AddOrUpdate(item.Bag, value, (o1, o2) => value);
            }
        }

        protected void updateParameterValuesDataTreeBranch(ParameterDataCacheBag bag, DataTreeBranch dataTreeBranch)
        {
            parameterValuesDataTreeBranch.AddOrUpdate(bag, dataTreeBranch, (o1, o2) => dataTreeBranch);

            object valueToStore = dataTreeBranch.ParsedObject ?? dataTreeBranch;
            if (bag.Index == null)
                this.parameterValues.AddOrUpdate(bag.Parameter, (o1) =>
                {
                    try
                    {
                        return valueToStore;
                    }
                    finally
                    {
                        ParameterValueAdded?.InvokeFailSafe(this, new ParameterValueAddedEventArgs(o1, valueToStore));
                    }
                }, (o1, o2) =>
                {
                    try
                    {
                        if(o2.GetType() != valueToStore.GetType())
                        {
                            if(o2 is IRDMPayloadObjectOneOf oneOf)
                            {
                                oneOf = (IRDMPayloadObjectOneOf)Activator.CreateInstance(oneOf.GetType(), valueToStore, oneOf.Count);
                                valueToStore = oneOf;
                            }
                        }
                        return valueToStore;
                    }
                    finally
                    {
                        if (o2 != valueToStore)
                            ParameterValueChanged?.InvokeFailSafe(this, new ParameterValueChangedEventArgs(o1, valueToStore, o2));
                    }
                });

            else
            {
                this.parameterValues.AddOrUpdate(bag.Parameter,
                    (pid) =>
                    //Add
                    {
                        try
                        {
                            ConcurrentDictionary<object, object> dict = new ConcurrentDictionary<object, object>();
                            dict.AddOrUpdate(bag.Index, valueToStore, (o1, o2) => valueToStore);
                            return dict;
                        }
                        finally
                        {
                            ParameterValueAdded?.InvokeFailSafe(this, new ParameterValueAddedEventArgs(pid, valueToStore, bag.Index));
                        }
                    },
                    (pid, cd) =>
                    // Update
                    {
                        object old = null;
                        bool changed = false;
                        try
                        {
                            ConcurrentDictionary<object, object> dict = (ConcurrentDictionary<object, object>)cd;
                            dict.AddOrUpdate(bag.Index, valueToStore, (_, o2) =>
                            {
                                if (o2 == valueToStore)
                                    return valueToStore;

                                old = o2;
                                changed = true;
                                return valueToStore;
                            });
                            return dict;
                        }
                        finally
                        {
                            if (changed)
                                ParameterValueChanged?.InvokeFailSafe(this, new ParameterValueChangedEventArgs(pid, valueToStore, old, bag.Index));
                            else
                                ParameterValueAdded?.InvokeFailSafe(this, new ParameterValueAddedEventArgs(pid, valueToStore, bag.Index));
                        }
                    });
            }

            ParameterRequested?.InvokeFailSafe(this, new ParameterRequestedEventArgs(bag.Parameter, bag.Index));
        }
        protected virtual async Task OnSendRDMMessage(RDMMessage rdmMessage)
        {
            await Task.CompletedTask;
        }
        protected virtual async Task OnResponseMessage(RDMMessage rdmMessage)
        {
            await Task.CompletedTask;
        }

        protected async Task runPeerToPeerProcess(PeerToPeerProcess ptpProcess)
        {
            ptpProcess.BeforeSendMessage = OnSendRDMMessage;
            ptpProcess.ResponseMessage = OnResponseMessage;
            await ptpProcess?.Run();
        }
        protected async Task<bool> requestSetParameterWithEmptyPayload(ParameterBag parameterBag, MetadataJSONObjectDefine define, UID uid, SubDevice subDevice)
        {
            PeerToPeerProcess ptpProcess = new PeerToPeerProcess(ERDM_Command.SET_COMMAND, uid, subDevice, parameterBag);
            await runPeerToPeerProcess(ptpProcess);
            if (!ptpProcess.ResponsePayloadObject.IsUnset)
            {
                updateParameterValuesDependeciePropertyBag(parameterBag.PID, ptpProcess.ResponsePayloadObject);
                updateParameterValuesDataTreeBranch(new ParameterDataCacheBag(parameterBag.PID), ptpProcess.ResponsePayloadObject);
                return ptpProcess.State == PeerToPeerProcess.EPeerToPeerProcessState.Finished;
            }
            return false;
        }
        protected async Task<bool> requestSetParameterWithPayload(ParameterBag parameterBag, MetadataJSONObjectDefine define, UID uid, SubDevice subDevice, object value)
        {
            define.GetCommand(Metadata.JSON.Command.ECommandDublicate.SetRequest, out var cmd);
            var req = cmd.Value.GetRequiredProperties();
            if (req.Length == 1)
            {
                DataTreeBranch dataTreeBranch = DataTreeBranch.FromObject(value, null, parameterBag, ERDM_Command.SET_COMMAND);
                PeerToPeerProcess ptpProcess = new PeerToPeerProcess(ERDM_Command.SET_COMMAND, uid, subDevice, parameterBag, dataTreeBranch);
                await runPeerToPeerProcess(ptpProcess);
                if (ptpProcess.State == PeerToPeerProcess.EPeerToPeerProcessState.Failed)
                    throw new Exception($"Failed to set parameter {parameterBag.PID} with value {value}");
                if (ptpProcess.State == PeerToPeerProcess.EPeerToPeerProcessState.Finished)
                {
                    if (ptpProcess.ResponsePayloadObject.IsEmpty && define.GetResponse.HasValue)
                    {
                        updateParameterValuesDataTreeBranch(new ParameterDataCacheBag(ptpProcess.ParameterBag.PID), dataTreeBranch);
                        if (this.ParameterValues.TryGetValue(parameterBag.PID, out object cacheValue))
                        {
                            if (value != cacheValue)
                                throw new Exception($"Failed to set parameter {parameterBag.PID} with value {value}, cache value is {cacheValue}");
                        }
                    }
                    else if (!ptpProcess.ResponsePayloadObject.IsUnset && define.GetResponse.HasValue)
                        updateParameterValuesDataTreeBranch(new ParameterDataCacheBag(ptpProcess.ParameterBag.PID), ptpProcess.ResponsePayloadObject);
                    return true;
                }
            }
            return false;
        }

        protected async Task<byte> requestGetParameterWithEmptyPayload(ParameterBag parameterBag, MetadataJSONObjectDefine define, UID uid, SubDevice subDevice)
        {
            try
            {
                PeerToPeerProcess ptpProcess = new PeerToPeerProcess(ERDM_Command.GET_COMMAND, uid, subDevice, parameterBag);
                await runPeerToPeerProcess(ptpProcess);
                if (!ptpProcess.ResponsePayloadObject.IsUnset)
                {
                    updateParameterValuesDependeciePropertyBag(ptpProcess.ParameterBag.PID, ptpProcess.ResponsePayloadObject);
                    updateParameterValuesDataTreeBranch(new ParameterDataCacheBag(ptpProcess.ParameterBag.PID), ptpProcess.ResponsePayloadObject);
                }
                return ptpProcess.MessageCounter;
            }
            catch(Exception e)
            {
                Logger?.LogError(e, $"Failed to get parameter {parameterBag.PID} with empty payload");
            }
            return 0;
        }
        protected async Task<byte> requestGetParameterWithPayload(ParameterBag parameterBag, MetadataJSONObjectDefine define, UID uid, SubDevice subDevice, object i=null)
        {
            define.GetCommand(Metadata.JSON.Command.ECommandDublicate.GetRequest, out var cmd);
            var req = cmd.Value.GetRequiredProperties();
            if (req.Length == 1 && req[0] is Metadata.JSON.OneOfTypes.IIntegerType intType)
            {
                try
                {
                    string name = intType.Name;
                    var depBag = parameterValuesDependeciePropertyBag.FirstOrDefault(bag => bag.Key.Parameter == parameterBag.PID && bag.Key.Command == Metadata.JSON.Command.ECommandDublicate.GetRequest && string.Equals(bag.Key.Name, name));
                    IComparable dependecyValue = (IComparable)depBag.Value;

                    if (i == null)
                    {
                        if(dependecyValue == null)
                        {

                        }
                        if (dependecyValue != null)
                        {
                            if (!dependecyValue.GetType().IsArray)
                            {
                                i = intType.GetMinimum();
                                object max = intType.GetMaximum();
                                object count = Convert.ChangeType(0, i.GetType());
                                while (dependecyValue.CompareTo(count) > 0)
                                {
                                    if (!intType.IsInRange(i))
                                        continue;

                                    if (((IComparable)max).CompareTo(i) == -1)
                                        return 0;

                                    DataTreeBranch dataTreeBranch = new DataTreeBranch(new DataTree(name, 0, i));
                                    PeerToPeerProcess ptpProcess = new PeerToPeerProcess(ERDM_Command.GET_COMMAND, uid, subDevice, parameterBag, dataTreeBranch);
                                    await runPeerToPeerProcess(ptpProcess);
                                    if (!ptpProcess.ResponsePayloadObject.IsUnset)
                                        updateParameterValuesDataTreeBranch(new ParameterDataCacheBag(ptpProcess.ParameterBag.PID, i), ptpProcess.ResponsePayloadObject);

                                    i = intType.IncrementJumpRange(i);
                                    count = intType.Increment(count);
                                }
                            }
                            else
                            {
                                foreach (var item in (Array)dependecyValue)
                                {
                                    var type = item.GetType();
                                    i = type.GetProperty(depBag.Key.Property);
                                    DataTreeBranch dataTreeBranch = new DataTreeBranch(new DataTree(name, 0, i));
                                    PeerToPeerProcess ptpProcess = new PeerToPeerProcess(ERDM_Command.GET_COMMAND, uid, subDevice, parameterBag, dataTreeBranch);
                                    await runPeerToPeerProcess(ptpProcess);
                                    if (!ptpProcess.ResponsePayloadObject.IsUnset)
                                        updateParameterValuesDataTreeBranch(new ParameterDataCacheBag(ptpProcess.ParameterBag.PID, i), ptpProcess.ResponsePayloadObject);
                                }
                            }
                            return 0;
                        }
                    }
                    else
                    {
                        DataTreeBranch dataTreeBranch = new DataTreeBranch(new DataTree(name, 0, i));
                        PeerToPeerProcess ptpProcess = new PeerToPeerProcess(ERDM_Command.GET_COMMAND, uid, subDevice, parameterBag, dataTreeBranch);
                        await runPeerToPeerProcess(ptpProcess);
                        if (!ptpProcess.ResponsePayloadObject.IsUnset)
                            updateParameterValuesDataTreeBranch(new ParameterDataCacheBag(ptpProcess.ParameterBag.PID, parameterBag.PID == ERDM_Parameter.QUEUED_MESSAGE || parameterBag.PID == ERDM_Parameter.STATUS_MESSAGES ? null : i), ptpProcess.ResponsePayloadObject);
                        return ptpProcess.MessageCounter;
                    }
                }
                catch (Exception e)
                {
                    Logger?.LogError(e, $"Failed to get parameter {parameterBag.PID} with Bag: {parameterBag}");
                }
            }
            return 0;
        }


        public void Dispose()
        {
            if (this.IsDisposed || this.IsDisposing)
                return;

            this.IsDisposing = true;

            this.parameterValues.Clear();
            this.parameterValues = null;
            this.parameterValuesDataTreeBranch.Clear();
            this.parameterValuesDataTreeBranch = null;
            this.parameterValuesDependeciePropertyBag.Clear();
            this.parameterValuesDependeciePropertyBag = null;
            this.IsDisposed = true;
            this.IsDisposing = false;
        }
    }
}