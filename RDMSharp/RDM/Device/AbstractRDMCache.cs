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
        protected bool IsDisposed { get; private set; }
        protected bool IsDisposing { get; private set; }
        protected ConcurrentDictionary<ParameterDataCacheBag, DataTreeBranch> parameterValuesDataTreeBranch { get; private set; } = new ConcurrentDictionary<ParameterDataCacheBag, DataTreeBranch>();
        protected ConcurrentDictionary<DataTreeObjectDependeciePropertyBag, object> parameterValuesDependeciePropertyBag { get; private set; } = new ConcurrentDictionary<DataTreeObjectDependeciePropertyBag, object>();

        protected ConcurrentDictionary<ERDM_Parameter, object> parameterValues { get; private set; } = new ConcurrentDictionary<ERDM_Parameter, object>();
        public virtual IReadOnlyDictionary<ERDM_Parameter, object> ParameterValues
        {
            get { return this.parameterValues?.AsReadOnly(); }
        }

        protected AsyncRDMRequestHelper asyncRDMRequestHelper;

        public AbstractRDMCache()
        {

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
                this.parameterValues.AddOrUpdate(bag.Parameter, valueToStore, (o1, o2) => valueToStore);
            else
            {
                this.parameterValues.AddOrUpdate(bag.Parameter,
                    (o1) =>
                    //Add
                    {
                        ConcurrentDictionary<object, object> dict = new ConcurrentDictionary<object, object>();
                        dict.AddOrUpdate(bag.Index, valueToStore, (o1, o2) => valueToStore);
                        return dict;
                    },
                    (o1, o2) =>
                    // Update
                    {
                        ConcurrentDictionary<object, object> dict = (ConcurrentDictionary<object, object>)o2;
                        dict.AddOrUpdate(bag.Index, valueToStore, (o1, o2) => valueToStore);
                        return dict;
                    });


            }
        }

        protected async Task runPeerToPeerProcess(PeerToPeerProcess ptpProcess)
        {
            await ptpProcess?.Run(asyncRDMRequestHelper);
        }

        protected async Task requestParameterWithEmptyPayload(ParameterBag parameterBag, MetadataJSONObjectDefine define, UID uid, SubDevice subDevice)
        {
            PeerToPeerProcess ptpProcess = new PeerToPeerProcess(ERDM_Command.GET_COMMAND, uid, subDevice, parameterBag);
            await runPeerToPeerProcess(ptpProcess);
            if (!ptpProcess.ResponsePayloadObject.IsUnset)
            {
                updateParameterValuesDependeciePropertyBag(parameterBag.PID, ptpProcess.ResponsePayloadObject);
                updateParameterValuesDataTreeBranch(new ParameterDataCacheBag(parameterBag.PID), ptpProcess.ResponsePayloadObject);
            }
        }
        protected async Task requestParameterWithPayload(ParameterBag parameterBag, MetadataJSONObjectDefine define, UID uid, SubDevice subDevice)
        {
            var req = define.GetRequest.Value.GetRequiredProperties();
            if (req.Length == 1 && req[0] is Metadata.JSON.OneOfTypes.IIntegerType intType)
            {
                try
                {
                    string name = intType.Name;

                    IComparable dependecyValue = (IComparable)parameterValuesDependeciePropertyBag.FirstOrDefault(bag => bag.Key.Parameter == parameterBag.PID && bag.Key.Command == Metadata.JSON.Command.ECommandDublicte.GetRequest && string.Equals(bag.Key.Name, name)).Value;

                    object i = intType.GetMinimum();
                    object max = intType.GetMaximum();
                    object count = Convert.ChangeType(0, i.GetType());
                    while (dependecyValue.CompareTo(count) > 0)
                    {
                        if (!intType.IsInRange(i))
                            continue;

                        if (((IComparable)max).CompareTo(i) == -1)
                            return;

                        DataTreeBranch dataTreeBranch = new DataTreeBranch(new DataTree(name, 0, i));
                        PeerToPeerProcess ptpProcess = new PeerToPeerProcess(ERDM_Command.GET_COMMAND, uid, subDevice, parameterBag, dataTreeBranch);
                        await runPeerToPeerProcess(ptpProcess);
                        if (!ptpProcess.ResponsePayloadObject.IsUnset)
                            updateParameterValuesDataTreeBranch(new ParameterDataCacheBag(parameterBag.PID, i), ptpProcess.ResponsePayloadObject);

                        i = intType.IncrementJumpRange(i);
                        count = intType.Increment(count);
                    }
                }
                catch (Exception e)
                {

                }
            }
        }


        public void Dispose()
        {
            if (this.IsDisposed || this.IsDisposing)
                return;

            this.IsDisposing = true;

            this.asyncRDMRequestHelper?.Dispose();
            this.asyncRDMRequestHelper = null;

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